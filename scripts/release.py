#!/usr/bin/env python3
"""Build and publish a verified RestClient.Net / Exhaustion release."""

import argparse
import io
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
import time
import urllib.error
import urllib.request
import xml.etree.ElementTree as ET
import zipfile


ROOT = Path(__file__).resolve().parents[1]
NUGET_SOURCE = "https://api.nuget.org/v3/index.json"
NUGET_PACKAGES = "https://api.nuget.org/v3-flatcontainer"


def normalize_version(value: str, tag_prefix: str = "") -> str:
    if tag_prefix and value.startswith(tag_prefix):
        value = value[len(tag_prefix):]
    number = r"(?:0|[1-9][0-9]*)"
    if not re.fullmatch(rf"{number}\.{number}\.{number}(?:-[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?", value):
        raise ValueError(f"Invalid release version: {value!r}")
    if "-" in value:
        for identifier in value.split("-", 1)[1].split("."):
            if identifier.isdigit() and len(identifier) > 1 and identifier.startswith("0"):
                raise ValueError(f"Invalid release version: {value!r}")
    return value


def project_versions(client_version=None, analyzer_version=None):
    client = ET.parse(ROOT / "RestClient.Net/RestClient.Net.csproj").findtext(".//Version") if client_version is None else client_version
    analyzer = ET.parse(ROOT / "Directory.Build.props").findtext(".//ExhaustionVersion") if analyzer_version is None else analyzer_version
    return normalize_version(client, "restclient-v"), normalize_version(analyzer, "exhaustion-v")


def _run(command, env=None):
    print("Running:", " ".join(map(str, command)), flush=True)
    subprocess.run(list(map(str, command)), cwd=ROOT, env=env, check=True, timeout=600)


def pack_analyzer(output: Path, analyzer_version: str, env=None, artifacts=None):
    output.mkdir(parents=True, exist_ok=True)
    command = ["dotnet", "pack", "Exhaustion/Exhaustion.csproj", "--configuration", "Release",
               "--output", output, f"-p:ExhaustionVersion={analyzer_version}",
               f"-p:Version={analyzer_version}", "-p:GeneratePackageOnBuild=false"]
    if artifacts:
        command.extend(["--artifacts-path", artifacts,
                        f"-p:PathMap={artifacts.parent}=/_/work%2C{ROOT}=/_/src"])
    _run(command, env)


def build_release(output: Path, client_version: str, analyzer_version: str):
    # A new cache and build directory prevent an older same-version package or
    # previous build output from masquerading as the analyzer being released.
    with tempfile.TemporaryDirectory(prefix="restclient-release-") as temporary:
        temporary = Path(temporary)
        env = dict(os.environ, NUGET_PACKAGES=str(temporary / "nuget"))
        artifacts = temporary / "build"
        pack_analyzer(output, analyzer_version, env, artifacts)
        properties = [f"-p:Version={client_version}", f"-p:ExhaustionVersion={analyzer_version}",
                      f"-p:ArtifactsPath={artifacts}", "-p:UseArtifactsOutput=true",
                      f"-p:PathMap={temporary}=/_/work%2C{ROOT}=/_/src"]
        _run(["dotnet", "restore", "RestClient.Net/RestClient.Net.csproj",
              "--source", output, "--source", NUGET_SOURCE, *properties], env)
        _run(["dotnet", "pack", "RestClient.Net/RestClient.Net.csproj",
              "--configuration", "Release", "--no-restore", "--output", output,
              *properties], env)


def _package_metadata(package):
    with zipfile.ZipFile(package) as archive:
        specs = [name for name in archive.namelist() if name.endswith(".nuspec")]
        if len(specs) != 1:
            raise ValueError("Package must contain exactly one nuspec")
        metadata = ET.fromstring(archive.read(specs[0])).find("{*}metadata")
        if metadata is None:
            raise ValueError("Package is missing metadata")
        identity = metadata.findtext("{*}id")
        version = metadata.findtext("{*}version")
        dependencies = metadata.find("{*}dependencies")
        dependency_groups = []
        if dependencies is not None:
            for group in dependencies:
                dependency_groups.append((tuple(sorted(group.attrib.items())),
                                          tuple(sorted(tuple(sorted(item.attrib.items())) for item in group))))
        libraries = {name: archive.read(name) for name in archive.namelist() if name.lower().endswith(".dll")}
        return identity, version, tuple(sorted(dependency_groups)), libraries


def verify_published_package(expected: Path, downloaded: bytes) -> None:
    expected_id, expected_version, expected_dependencies, expected_libraries = _package_metadata(expected)
    actual_id, actual_version, actual_dependencies, actual_libraries = _package_metadata(io.BytesIO(downloaded))
    if (actual_id, actual_version) != (expected_id, expected_version):
        raise ValueError("Published package identity or version does not match the release")
    if actual_dependencies != expected_dependencies:
        raise ValueError("Published package dependencies do not match the release")
    if not expected_libraries or actual_libraries != expected_libraries:
        raise ValueError("Published package DLLs differ from this build; bump the package version before publishing")


def push_package(package: Path) -> None:
    api_key = os.environ.get("NUGET_API_KEY")
    if not api_key:
        raise RuntimeError("NUGET_API_KEY is required for publication")
    if not package.is_file():
        raise ValueError(f"Release package does not exist: {package}")
    # Do not print the command or propagate a CalledProcessError containing the key.
    print(f"Publishing {package.name}", flush=True)
    try:
        subprocess.run(["dotnet", "nuget", "push", str(package), "--api-key", api_key,
                        "--source", NUGET_SOURCE, "--skip-duplicate"],
                       cwd=ROOT, check=True, timeout=180)
    except (subprocess.CalledProcessError, subprocess.TimeoutExpired):
        raise RuntimeError(f"NuGet publication failed for {package.name}") from None


def wait_for_public_package(package: Path, timeout: float = 600, poll_interval: float = 15) -> None:
    identity, version, _, _ = _package_metadata(package)
    identifier = identity.lower()
    normalized = version.lower()
    base = f"{NUGET_PACKAGES}/{identifier}"
    url = f"{base}/{normalized}/{identifier}.{normalized}.nupkg"
    deadline = time.monotonic() + timeout
    while True:
        remaining = deadline - time.monotonic()
        if remaining <= 0:
            raise TimeoutError(f"NuGet did not make {identity} {version} available within {timeout}s")
        try:
            with urllib.request.urlopen(url, timeout=min(30, remaining)) as response:
                verify_published_package(package, response.read())
            remaining = deadline - time.monotonic()
            if remaining <= 0:
                raise TimeoutError("NuGet indexing verification exceeded its deadline")
            with urllib.request.urlopen(f"{base}/index.json", timeout=min(30, remaining)) as response:
                versions = json.load(response)["versions"]
            if normalized in [item.lower() for item in versions]:
                print(f"Verified published {identity} {version}", flush=True)
                return
        except urllib.error.HTTPError as error:
            if error.code not in (404, 429, 500, 502, 503, 504):
                raise
        except (urllib.error.URLError, TimeoutError):
            pass
        if time.monotonic() >= deadline:
            raise TimeoutError(f"NuGet did not make {identity} {version} available within {timeout}s")
        print(f"Waiting for NuGet to index {identity} {version}", flush=True)
        time.sleep(min(poll_interval, max(0, deadline - time.monotonic())))


def publish_release(packages: Path, client_version: str, analyzer_version: str) -> None:
    # Client publication cannot start until the exact fixed analyzer is public.
    for package in (packages / f"Exhaustion.{analyzer_version}.nupkg",
                    packages / f"RestClient.Net.{client_version}.nupkg"):
        push_package(package)
        wait_for_public_package(package)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("command", choices=["versions", "pack-analyzer", "pack", "publish-analyzer", "publish"])
    parser.add_argument("--client-version")
    parser.add_argument("--analyzer-version")
    parser.add_argument("--output", type=Path, default=ROOT / ".artifacts/packages")
    args = parser.parse_args()
    client_version, analyzer_version = project_versions(args.client_version, args.analyzer_version)
    output = args.output.resolve()
    if args.command == "versions":
        print(f"client_version={client_version}\nanalyzer_version={analyzer_version}")
    elif args.command == "pack-analyzer":
        pack_analyzer(output, analyzer_version)
    elif args.command == "pack":
        build_release(output, client_version, analyzer_version)
    elif args.command == "publish-analyzer":
        package = output / f"Exhaustion.{analyzer_version}.nupkg"
        push_package(package)
        wait_for_public_package(package)
    elif args.command == "publish":
        publish_release(output, client_version, analyzer_version)


if __name__ == "__main__":
    main()
