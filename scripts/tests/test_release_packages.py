#!/usr/bin/env python3
"""Exercise the actual release packages from an isolated, ordinary NuGet consumer."""

import argparse
from contextlib import ExitStack
import json
import os
from pathlib import Path
import re
import signal
import subprocess
import tempfile
import unittest
import urllib.request
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape
import zipfile


MINIMUM_FIXED_VERSION = (1, 0, 1)
OPTIONS = None


def version_tuple(value):
    match = re.match(r"^[\[(]?(\d+)\.(\d+)\.(\d+)", value)
    if not match:
        raise AssertionError(f"Expected a semantic version or lower bound, got {value!r}")
    return tuple(map(int, match.groups()))


def read_package(package_id, version):
    path = OPTIONS.packages / f"{package_id}.{version}.nupkg"
    if not path.is_file():
        matches = [p for p in OPTIONS.packages.glob("*.nupkg") if p.name.lower() == path.name.lower()]
        if len(matches) != 1:
            raise AssertionError(f"Release package does not exist: {path}")
        path = matches[0]
    with zipfile.ZipFile(path) as archive:
        manifests = [name for name in archive.namelist() if name.endswith(".nuspec")]
        if len(manifests) != 1:
            raise AssertionError(f"Expected one manifest in {path}, got {manifests}")
        return ET.fromstring(archive.read(manifests[0])), archive.namelist()


def run_dotnet(arguments, directory, timeout):
    """Contain regressions in a 512 MiB heap and terminate the entire process tree."""
    environment = dict(os.environ)
    environment.update({
        "DOTNET_GCHeapHardLimit": "0x20000000",
        "DOTNET_PROCESSOR_COUNT": "2",
        "DOTNET_CLI_TELEMETRY_OPTOUT": "1",
        "DOTNET_NOLOGO": "1",
        "MSBUILDDISABLENODEREUSE": "1",
        "NUGET_PACKAGES": str(directory / "packages"),
    })
    command = ["dotnet", *arguments]
    # A file keeps an accidentally enormous legacy diagnostic out of Python's heap.
    with tempfile.TemporaryFile(mode="w+b") as output_file:
        process = subprocess.Popen(
            command, cwd=directory, env=environment,
            stdout=output_file, stderr=subprocess.STDOUT,
            start_new_session=os.name != "nt",
        )
        try:
            process.wait(timeout=timeout)
        except subprocess.TimeoutExpired:
            if os.name == "nt":
                subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"],
                               check=False, timeout=10, capture_output=True)
            else:
                os.killpg(process.pid, signal.SIGKILL)
            process.wait(timeout=10)
            raise AssertionError(f"Timed out after {timeout}s; killed bounded process tree: {command}")
        output_size = output_file.tell()
        output_file.seek(max(0, output_size - 65536))
        output = output_file.read().decode("utf-8", errors="replace")
    return process.returncode, output, output_size


class ReleasePackagesTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.restclient_manifest, _ = read_package("RestClient.Net", OPTIONS.restclient_version)
        cls.analyzer_manifest, cls.analyzer_files = read_package("Exhaustion", OPTIONS.exhaustion_version)
        cls.temporary = tempfile.TemporaryDirectory(prefix="restclient-package-consumer-")
        cls.addClassCleanup(cls.temporary.cleanup)
        cls.directory = Path(cls.temporary.name)
        cls.project = cls.directory / "Consumer.csproj"
        cls.project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>{escape(OPTIONS.framework)}</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <EnableNETAnalyzers>false</EnableNETAnalyzers>
    <!-- C# does not know that the hierarchy is closed; Exhaustion supplies that check. -->
    <NoWarn>CS8509</NoWarn>
    <WarningsAsErrors>EXHAUSTION001</WarningsAsErrors>
    <UseSharedCompilation>false</UseSharedCompilation>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="RestClient.Net" Version="[{escape(OPTIONS.restclient_version)}]" />
  </ItemGroup>
</Project>
''', encoding="utf-8")
        if OPTIONS.public_nuget:
            configuration = '''<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
'''
        else:
            # Exact source mapping prevents a fallback from hiding a broken local package.
            configuration = f'''<configuration>
  <packageSources>
    <clear />
    <add key="release" value="{escape(str(OPTIONS.packages), {'"': '&quot;'})}" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="release">
      <package pattern="RestClient.Net" />
      <package pattern="Exhaustion" />
    </packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
'''
        (cls.directory / "NuGet.Config").write_text(configuration, encoding="utf-8")
        cls.restored = False

    def test_restclient_dependency_requires_fixed_analyzer_and_allows_analyzers(self):
        self.assertEqual(OPTIONS.restclient_version,
                         self.restclient_manifest.findtext("{*}metadata/{*}version"))
        groups = self.restclient_manifest.findall("{*}metadata/{*}dependencies/{*}group")
        self.assertTrue(groups, "RestClient.Net must declare dependencies for its target frameworks")
        for group in groups:
            with self.subTest(framework=group.attrib.get("targetFramework")):
                dependencies = [item for item in group if item.attrib.get("id", "").lower() == "exhaustion"]
                self.assertEqual(1, len(dependencies), "Each framework must receive Exhaustion")
                dependency = dependencies[0]
                self.assertGreaterEqual(
                    version_tuple(dependency.attrib["version"]), MINIMUM_FIXED_VERSION,
                    "RestClient.Net still permits the unfixed Exhaustion 1.0.0 package",
                )
                self.assertEqual(version_tuple(OPTIONS.exhaustion_version),
                                 version_tuple(dependency.attrib["version"]))
                excluded = {part.strip().lower() for part in re.split(r"[;,]", dependency.attrib.get("exclude", ""))}
                self.assertFalse({"all", "analyzers", "buildtransitive"} & excluded,
                                 f"RestClient.Net must propagate analyzer assets: {dependency.attrib}")

    def test_analyzer_package_contains_fixed_version_and_compiler_asset(self):
        self.assertEqual(OPTIONS.exhaustion_version,
                         self.analyzer_manifest.findtext("{*}metadata/{*}version"))
        self.assertGreaterEqual(version_tuple(OPTIONS.exhaustion_version), MINIMUM_FIXED_VERSION,
                                "A release must publish the analyzer containing the issue #146 fix")
        self.assertIn("analyzers/dotnet/cs/Exhaustion.dll", self.analyzer_files)
        self.assertFalse(any(name.startswith("lib/") and name.endswith("Exhaustion.dll")
                             for name in self.analyzer_files), "The analyzer must not become a runtime dependency")

    def restore_consumer(self):
        if not self.__class__.restored:
            code, output, _ = run_dotnet([
                "restore", str(self.project), "--configfile", "NuGet.Config",
                "--disable-parallel", "-p:NuGetAudit=false", "-p:RestoreIgnoreFailedSources=false",
            ], self.directory, 120)
            self.assertEqual(0, code, "The independent consumer must restore successfully:\n" + output)
            self.__class__.restored = True
        assets = json.loads((self.directory / "obj/project.assets.json").read_text())
        self.assertIn(f"RestClient.Net/{OPTIONS.restclient_version}", assets["libraries"])
        self.assertIn(f"Exhaustion/{OPTIONS.exhaustion_version}", assets["libraries"])
        framework = assets["project"]["frameworks"][OPTIONS.framework]
        self.assertEqual(["RestClient.Net"], list(framework["dependencies"]),
                         "The consumer must enable Exhaustion solely by referencing RestClient.Net")

    def build_consumer(self, source):
        self.restore_consumer()
        (self.directory / "Consumer.cs").write_text(source, encoding="utf-8")
        diagnostics_file = self.directory / "diagnostics.sarif"
        diagnostics_file.unlink(missing_ok=True)
        code, output, size = run_dotnet([
            "build", str(self.project), "--no-restore", "--disable-build-servers",
            "--configuration", "Release", "--verbosity", "minimal", "--nologo",
            "-t:Rebuild", "-m:1", "-nodeReuse:false", "-p:UseSharedCompilation=false",
            f"-p:ErrorLog={diagnostics_file}",
        ], self.directory, 90)
        self.assertLess(size, 65536, "Compiler output must remain bounded; final excerpt:\n" + output[-4096:])
        for unexpected in ("AD0001", "CS8032", "CS8785", "OutOfMemoryException", "Stack overflow"):
            self.assertNotIn(unexpected, output, "The packaged analyzer must load and finish normally:\n" + output)
        self.assertTrue(diagnostics_file.is_file(), "The compiler did not produce diagnostics:\n" + output)
        diagnostics = json.loads(diagnostics_file.read_text())
        results = [result for run in diagnostics["runs"] for result in run.get("results", [])]
        return code, output, results

    def test_consumer_incomplete_then_complete_then_incomplete_switch(self):
        hierarchy = '''public abstract record Choice
{
    private Choice() { }
    public sealed record One : Choice;
    public sealed record Two : Choice;
}
public static class Consumer
{
    public static int Evaluate(Choice choice) => choice switch
    {
        Choice.One => 1,
        REPLACEMENT
    };
}
'''
        for complete in (False, True, False):
            with self.subTest(complete=complete):
                source = hierarchy.replace("REPLACEMENT", "Choice.Two => 2," if complete else "_ => 0,")
                code, output, diagnostics = self.build_consumer(source)
                if complete:
                    self.assertEqual(0, code, output)
                    self.assertEqual([], diagnostics, "A complete switch must compile without diagnostics")
                else:
                    self.assertNotEqual(0, code, "An incomplete hierarchy compiled successfully: the packaged analyzer never ran.\n" + output)
                    self.assertEqual(["EXHAUSTION001"], [item["ruleId"] for item in diagnostics], output)
                    self.assertEqual("error", diagnostics[0]["level"])
                    self.assertIn("Missing: Two", str(diagnostics[0]["message"]))
                    self.assertIn("Consumer.cs", str(diagnostics[0]["locations"]))

    def test_consumer_issue146_recursive_hierarchy_reports_bounded_diagnostic(self):
        # Same 25-leaf, 25^3 constructor-product reproducer as BoundedAnalysisRegressionTests.
        leaves = "\n".join(f"public sealed record Leaf{index} : Expression;" for index in range(24))
        hierarchy = f'''public abstract record Expression
{{
    private Expression() {{ }}
    {leaves}
    public sealed record Branch(Expression Left, Expression Middle, Expression Right) : Expression;
}}
'''
        expression = '''public static class Consumer
{
    public static Expression StripAlias(Expression value) => value switch
    {
        Expression.Branch branch => branch.Left,
        _ => value,
    };
}
'''
        statement = '''public static class Consumer
{
    public static Expression StripAlias(Expression value)
    {
        switch (value)
        {
            case Expression.Branch branch: return branch.Left;
            default: return value;
        }
    }
}
'''
        for source in (expression, statement):
            with self.subTest(switch="expression" if source == expression else "statement"):
                code, output, diagnostics = self.build_consumer(hierarchy + source)
                self.assertEqual(0, code, "Issue #146 must produce a controlled warning, without crashing:\n" + output)
                self.assertEqual(["EXHAUSTION002"], [item["ruleId"] for item in diagnostics],
                                 "The installed analyzer must execute the bounded issue #146 analysis:\n" + output)
                self.assertEqual("warning", diagnostics[0]["level"])
                self.assertIn("could not be determined", str(diagnostics[0]["message"]))
                self.assertLess(len(str(diagnostics[0]["message"])), 4096)
                self.assertIn("Consumer.cs", str(diagnostics[0]["locations"]))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument("--packages", type=Path, help="Directory containing both release .nupkg files")
    source.add_argument("--public-nuget", action="store_true", help="Verify published packages and restore solely from nuget.org")
    parser.add_argument("--restclient-version", required=True)
    parser.add_argument("--exhaustion-version", required=True)
    parser.add_argument("--framework", default="net8.0")
    OPTIONS, remaining = parser.parse_known_args()
    with ExitStack() as cleanup:
        if OPTIONS.public_nuget:
            OPTIONS.packages = Path(cleanup.enter_context(tempfile.TemporaryDirectory(prefix="published-release-packages-")))
            for package_id, version in (("RestClient.Net", OPTIONS.restclient_version),
                                        ("Exhaustion", OPTIONS.exhaustion_version)):
                filename = f"{package_id}.{version}.nupkg"
                url = f"https://api.nuget.org/v3-flatcontainer/{package_id.lower()}/{version.lower()}/{filename.lower()}"
                with urllib.request.urlopen(url, timeout=60) as response:
                    content = response.read(32 * 1024 * 1024 + 1)
                    if len(content) > 32 * 1024 * 1024:
                        raise AssertionError(f"Unexpectedly large release package: {url}")
                (OPTIONS.packages / filename).write_bytes(content)
        else:
            OPTIONS.packages = OPTIONS.packages.resolve(strict=True)
        unittest.main(argv=[__file__, *remaining], verbosity=2)
