"""Exercise immutable package validation and dependency-first publication."""

from __future__ import annotations

import importlib.util
import io
import json
from pathlib import Path
import tempfile
import unittest
from unittest import mock
import urllib.error
from xml.sax.saxutils import escape
import zipfile


SCRIPT_PATH = Path(__file__).resolve().parents[1] / "release.py"
SPEC = importlib.util.spec_from_file_location("release_under_test", SCRIPT_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Cannot load release helper: {SCRIPT_PATH}")
release = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(release)


ANALYZER_DLL = "analyzers/dotnet/cs/Exhaustion.dll"
CLIENT_DLLS = {
    "lib/net8.0/RestClient.Net.dll": b"net8-client-fixed-analyzer-dependency",
    "lib/net9.0/RestClient.Net.dll": b"net9-client-fixed-analyzer-dependency",
    "lib/netstandard2.1/RestClient.Net.dll": b"netstandard-client-fixed-analyzer-dependency",
}
FRAMEWORKS = ("net8.0", "net9.0", ".NETStandard2.1")


def package_bytes(
    package_id: str = "Exhaustion",
    version: str = "1.0.1",
    *,
    dlls: dict[str, bytes] | None = None,
    dependencies: tuple[tuple[str, str, str, str], ...] = (),
    signed: bool = False,
) -> bytes:
    """Create actual NuGet ZIPs; DLL payload differences remain observable."""
    if dlls is None:
        dlls = {ANALYZER_DLL: b"bounded-analyzer-source-146"}
    dependency_groups = "".join(
        '<group targetFramework="{}"><dependency id="{}" version="{}"{} /></group>'.format(
            escape(framework),
            escape(dependency),
            escape(required_version),
            f' exclude="{escape(excluded)}"' if excluded else "",
        )
        for framework, dependency, required_version, excluded in dependencies
    )
    nuspec = (
        '<?xml version="1.0" encoding="utf-8"?>'
        '<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">'
        "<metadata>"
        f"<id>{escape(package_id)}</id><version>{escape(version)}</version>"
        "<authors>RestClient.Net</authors><description>Release regression fixture</description>"
        f"<dependencies>{dependency_groups}</dependencies>"
        "</metadata></package>"
    )
    archive = io.BytesIO()
    with zipfile.ZipFile(archive, "w", compression=zipfile.ZIP_DEFLATED) as package:
        package.writestr(f"{package_id}.nuspec", nuspec)
        for path, payload in dlls.items():
            package.writestr(path, payload)
        package.writestr("README.md", "Release regression fixture")
        if signed:
            package.writestr(".signature.p7s", b"nuget-added-repository-signature")
            package.comment = b"NuGet may repackage and sign uploaded artifacts"
    return archive.getvalue()


def analyzer_dependencies(
    version: str = "1.0.1", excluded: str = ""
) -> tuple[tuple[str, str, str, str], ...]:
    return tuple((framework, "Exhaustion", version, excluded) for framework in FRAMEWORKS)


class VersionValidationTests(unittest.TestCase):
    def test_accepts_releasable_versions_and_removes_only_the_requested_tag_prefix(self):
        cases = (
            ("1.0.1", "", "1.0.1"),
            ("7.3.1", "restclient-v", "7.3.1"),
            ("restclient-v7.3.1", "restclient-v", "7.3.1"),
            ("exhaustion-v1.0.1", "exhaustion-v", "1.0.1"),
            ("1.1.0-rc.1", "", "1.1.0-rc.1"),
            ("0.0.0", "", "0.0.0"),
        )
        for supplied, prefix, expected in cases:
            with self.subTest(supplied=supplied, prefix=prefix):
                actual = release.normalize_version(supplied, prefix)
                self.assertEqual(expected, actual)
                self.assertIsInstance(actual, str)
                self.assertNotIn("/", actual)
                self.assertNotIn("\n", actual)

    def test_rejects_malformed_versions_before_any_external_command_can_run(self):
        invalid_versions = (
            "",
            "1",
            "1.0",
            "1.0.1.0",
            "-1.0.1",
            "01.0.1",
            "1.00.1",
            "1.0.01",
            "1.0.1-",
            "1.0.1-rc..1",
            "1.0.1-01",
            "1.0.1-rc.01",
            "v1.0.1",
            "1.0.1/../../different-package",
            "1.0.1\nOTHER_VERSION=1.0.0",
            "1.0.1\x00",
            "1.0.1; echo unsafe",
            "$(echo 1.0.1)",
            "１.0.1",
        )
        for supplied in invalid_versions:
            with self.subTest(supplied=supplied):
                with self.assertRaises((ValueError, RuntimeError)):
                    release.normalize_version(supplied)

    def test_cannot_reinterpret_another_packages_tag_as_a_client_release(self):
        for supplied in ("exhaustion-v1.0.1", "prefix-restclient-v7.3.1", "restclient-v"):
            with self.subTest(supplied=supplied):
                with self.assertRaises((ValueError, RuntimeError)):
                    release.normalize_version(supplied, "restclient-v")

    def test_explicit_empty_overrides_cannot_silently_release_the_project_default(self):
        for client, analyzer in (("", None), (None, ""), ("", "")):
            with self.subTest(client=client, analyzer=analyzer):
                with self.assertRaises(ValueError):
                    release.project_versions(client, analyzer)


class PublishedArtifactTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix="restclient-release-tests-")
        self.addCleanup(self.directory.cleanup)
        self.packages = Path(self.directory.name)

    def expected_package(self, payload: bytes, filename: str = "Exhaustion.1.0.1.nupkg") -> Path:
        path = self.packages / filename
        path.write_bytes(payload)
        return path

    def test_matching_analyzer_can_be_reused_after_nuget_adds_its_signature(self):
        expected_bytes = package_bytes()
        actual_bytes = package_bytes(signed=True)
        expected = self.expected_package(expected_bytes)

        self.assertNotEqual(expected_bytes, actual_bytes)
        self.assertIsNone(release.verify_published_package(expected, actual_bytes))
        self.assertEqual(expected_bytes, expected.read_bytes())

    def test_same_analyzer_version_with_old_dll_is_rejected(self):
        expected_bytes = package_bytes()
        expected = self.expected_package(expected_bytes)
        stale = package_bytes(dlls={ANALYZER_DLL: b"old-unbounded-analyzer-146"}, signed=True)

        with self.assertRaises((ValueError, RuntimeError)):
            release.verify_published_package(expected, stale)
        self.assertEqual(expected_bytes, expected.read_bytes())

    def test_missing_analyzer_asset_is_rejected_even_when_nuspec_matches(self):
        expected = self.expected_package(package_bytes())
        empty_analyzer = package_bytes(dlls={}, signed=True)

        with self.assertRaises((ValueError, RuntimeError)):
            release.verify_published_package(expected, empty_analyzer)

    def test_analyzer_in_wrong_directory_is_not_a_valid_replacement(self):
        expected = self.expected_package(package_bytes())
        wrong_layout = package_bytes(
            dlls={"lib/netstandard2.0/Exhaustion.dll": b"bounded-analyzer-source-146"}
        )

        with self.assertRaises((ValueError, RuntimeError)):
            release.verify_published_package(expected, wrong_layout)

    def test_wrong_package_identity_or_version_is_rejected_despite_identical_dll(self):
        expected = self.expected_package(package_bytes())
        for package_id, version in (("Other.Analyzer", "1.0.1"), ("Exhaustion", "1.0.0")):
            with self.subTest(package_id=package_id, version=version):
                downloaded = package_bytes(package_id, version, signed=True)
                with self.assertRaises((ValueError, RuntimeError)):
                    release.verify_published_package(expected, downloaded)

    def test_matching_client_preserves_every_framework_and_fixed_dependency(self):
        dependencies = analyzer_dependencies()
        expected = self.expected_package(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=dependencies),
            "RestClient.Net.7.3.1.nupkg",
        )
        downloaded = package_bytes(
            "RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=dependencies, signed=True
        )

        self.assertIsNone(release.verify_published_package(expected, downloaded))

    def test_stale_client_analyzer_dependency_is_rejected_for_every_framework(self):
        dependencies = analyzer_dependencies()
        expected = self.expected_package(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=dependencies),
            "RestClient.Net.7.3.1.nupkg",
        )
        for affected_framework in FRAMEWORKS:
            with self.subTest(affected_framework=affected_framework):
                stale_dependencies = tuple(
                    (framework, package_id, "1.0.0" if framework == affected_framework else version, excluded)
                    for framework, package_id, version, excluded in dependencies
                )
                downloaded = package_bytes(
                    "RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=stale_dependencies
                )
                with self.assertRaises((ValueError, RuntimeError)):
                    release.verify_published_package(expected, downloaded)

    def test_excluding_transitive_analyzers_is_rejected_for_every_framework(self):
        dependencies = analyzer_dependencies()
        expected = self.expected_package(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=dependencies),
            "RestClient.Net.7.3.1.nupkg",
        )
        for affected_framework in FRAMEWORKS:
            with self.subTest(affected_framework=affected_framework):
                excluded_dependencies = tuple(
                    (framework, package_id, version, "Build,Analyzers" if framework == affected_framework else excluded)
                    for framework, package_id, version, excluded in dependencies
                )
                downloaded = package_bytes(
                    "RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=excluded_dependencies
                )
                with self.assertRaises((ValueError, RuntimeError)):
                    release.verify_published_package(expected, downloaded)

    def test_missing_dependency_group_cannot_silently_disable_one_framework(self):
        expected = self.expected_package(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=analyzer_dependencies()),
            "RestClient.Net.7.3.1.nupkg",
        )
        downloaded = package_bytes(
            "RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=analyzer_dependencies()[:-1]
        )

        with self.assertRaises((ValueError, RuntimeError)):
            release.verify_published_package(expected, downloaded)

    def test_every_client_dll_must_match_the_built_package(self):
        expected = self.expected_package(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=analyzer_dependencies()),
            "RestClient.Net.7.3.1.nupkg",
        )
        for changed_path in CLIENT_DLLS:
            with self.subTest(changed_path=changed_path):
                stale_dlls = {**CLIENT_DLLS, changed_path: b"old-client-build"}
                downloaded = package_bytes(
                    "RestClient.Net", "7.3.1", dlls=stale_dlls, dependencies=analyzer_dependencies()
                )
                with self.assertRaises((ValueError, RuntimeError)):
                    release.verify_published_package(expected, downloaded)


class PublicationOrderTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix="restclient-publication-tests-")
        self.addCleanup(self.directory.cleanup)
        self.packages = Path(self.directory.name)
        self.analyzer = self.packages / "Exhaustion.1.0.1.nupkg"
        self.client = self.packages / "RestClient.Net.7.3.1.nupkg"
        self.analyzer.write_bytes(package_bytes())
        self.client.write_bytes(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=analyzer_dependencies())
        )
        self.calls = mock.Mock()
        self.push = mock.Mock()
        self.wait = mock.Mock()
        self.calls.attach_mock(self.push, "push")
        self.calls.attach_mock(self.wait, "wait")
        self.addCleanup(mock.patch.stopall)
        mock.patch.object(release, "push_package", self.push).start()
        mock.patch.object(release, "wait_for_public_package", self.wait).start()

    def publish(self):
        release.publish_release(self.packages, "7.3.1", "1.0.1")

    def test_analyzer_is_published_and_verified_before_client_is_published(self):
        self.publish()

        self.assertEqual(
            [
                mock.call.push(self.analyzer),
                mock.call.wait(self.analyzer),
                mock.call.push(self.client),
                mock.call.wait(self.client),
            ],
            self.calls.mock_calls,
        )
        self.assertEqual(2, self.push.call_count)
        self.assertEqual(2, self.wait.call_count)

    def test_analyzer_push_failure_cannot_publish_client(self):
        self.push.side_effect = RuntimeError("analyzer upload failed")

        with self.assertRaisesRegex(RuntimeError, "analyzer upload failed"):
            self.publish()
        self.push.assert_called_once_with(self.analyzer)
        self.wait.assert_not_called()
        self.assertEqual([mock.call.push(self.analyzer)], self.calls.mock_calls)

    def test_stale_duplicate_analyzer_prevents_client_publication(self):
        self.wait.side_effect = RuntimeError("published analyzer DLL differs; bump version")

        with self.assertRaisesRegex(RuntimeError, "published analyzer DLL differs"):
            self.publish()
        self.push.assert_called_once_with(self.analyzer)
        self.wait.assert_called_once_with(self.analyzer)
        self.assertEqual(
            [mock.call.push(self.analyzer), mock.call.wait(self.analyzer)], self.calls.mock_calls
        )

    def test_analyzer_availability_timeout_prevents_client_publication(self):
        self.wait.side_effect = TimeoutError("NuGet index did not expose analyzer")

        with self.assertRaisesRegex(TimeoutError, "NuGet index"):
            self.publish()
        self.push.assert_called_once_with(self.analyzer)
        self.wait.assert_called_once_with(self.analyzer)
        self.assertEqual(2, len(self.calls.mock_calls))

    def test_matching_already_published_analyzer_allows_idempotent_retry(self):
        def verify_download(package: Path):
            if package == self.analyzer:
                release.verify_published_package(package, package_bytes(signed=True))
            else:
                release.verify_published_package(
                    package,
                    package_bytes(
                        "RestClient.Net", "7.3.1", dlls=CLIENT_DLLS,
                        dependencies=analyzer_dependencies(), signed=True,
                    ),
                )

        self.wait.side_effect = verify_download
        self.publish()

        self.assertEqual([mock.call(self.analyzer), mock.call(self.client)], self.push.call_args_list)
        self.assertEqual([mock.call(self.analyzer), mock.call(self.client)], self.wait.call_args_list)

    def test_client_push_failure_is_reported_without_claiming_verification(self):
        self.push.side_effect = (None, RuntimeError("client upload failed"))

        with self.assertRaisesRegex(RuntimeError, "client upload failed"):
            self.publish()
        self.assertEqual([mock.call(self.analyzer), mock.call(self.client)], self.push.call_args_list)
        self.wait.assert_called_once_with(self.analyzer)

    def test_stale_duplicate_client_is_reported_as_failure(self):
        self.wait.side_effect = (None, RuntimeError("published client DLL differs"))

        with self.assertRaisesRegex(RuntimeError, "published client DLL differs"):
            self.publish()
        self.assertEqual([mock.call(self.analyzer), mock.call(self.client)], self.push.call_args_list)
        self.assertEqual([mock.call(self.analyzer), mock.call(self.client)], self.wait.call_args_list)


class PublicNuGetVerificationTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix="restclient-public-download-tests-")
        self.addCleanup(self.directory.cleanup)
        self.packages = Path(self.directory.name)
        self.analyzer = self.packages / "Exhaustion.1.0.1.nupkg"
        self.analyzer.write_bytes(package_bytes())
        self.download_url = (
            "https://api.nuget.org/v3-flatcontainer/exhaustion/1.0.1/exhaustion.1.0.1.nupkg"
        )
        self.index_url = "https://api.nuget.org/v3-flatcontainer/exhaustion/index.json"
        self.elapsed = 0.0
        self.monotonic = self.patch(release.time, "monotonic", side_effect=lambda: self.elapsed)
        self.sleep = self.patch(release.time, "sleep", side_effect=self.advance_time)

    def patch(self, target, attribute, **options):
        patcher = mock.patch.object(target, attribute, **options)
        value = patcher.start()
        self.addCleanup(patcher.stop)
        return value

    def advance_time(self, seconds):
        self.assertGreaterEqual(seconds, 0)
        self.elapsed += seconds

    @staticmethod
    def package_response():
        return io.BytesIO(package_bytes(signed=True))

    @staticmethod
    def index_response(*versions):
        return io.BytesIO(json.dumps({"versions": versions}).encode())

    def test_success_requires_both_matching_download_and_indexed_version(self):
        requests = self.patch(
            release.urllib.request, "urlopen",
            side_effect=[self.package_response(), self.index_response("1.0.0", "1.0.1")],
        )

        self.assertIsNone(release.wait_for_public_package(self.analyzer, timeout=10, poll_interval=3))

        self.assertEqual([self.download_url, self.index_url], [call.args[0] for call in requests.call_args_list])
        self.assertTrue(all(0 < call.kwargs["timeout"] <= 10 for call in requests.call_args_list))
        self.sleep.assert_not_called()
        self.assertEqual(0, self.elapsed)

    def test_expired_deadline_does_not_start_another_download(self):
        requests = self.patch(release.urllib.request, "urlopen")

        with self.assertRaises(TimeoutError):
            release.wait_for_public_package(self.analyzer, timeout=0, poll_interval=3)

        requests.assert_not_called()
        self.sleep.assert_not_called()
        self.assertEqual(0, self.elapsed)

    def test_unpublished_package_retries_404_then_checks_actual_download_and_index(self):
        missing = urllib.error.HTTPError(self.download_url, 404, "Not yet published", None, None)
        self.addCleanup(missing.close)
        requests = self.patch(
            release.urllib.request, "urlopen",
            side_effect=[missing, self.package_response(), self.index_response("1.0.1")],
        )

        release.wait_for_public_package(self.analyzer, timeout=10, poll_interval=3)

        self.assertEqual(
            [self.download_url, self.download_url, self.index_url],
            [call.args[0] for call in requests.call_args_list],
        )
        self.sleep.assert_called_once_with(3)
        self.assertEqual(3, self.elapsed)
        self.assertTrue(all(0 < call.kwargs["timeout"] <= 7 for call in requests.call_args_list[1:]))

    def test_download_without_index_entry_cannot_be_reported_as_available(self):
        requests = self.patch(
            release.urllib.request, "urlopen",
            side_effect=[
                self.package_response(), self.index_response("1.0.0"),
                self.package_response(), self.index_response("1.0.0", "1.0.1"),
            ],
        )

        release.wait_for_public_package(self.analyzer, timeout=10, poll_interval=3)

        self.assertEqual(
            [self.download_url, self.index_url, self.download_url, self.index_url],
            [call.args[0] for call in requests.call_args_list],
        )
        self.sleep.assert_called_once_with(3)
        self.assertEqual(3, self.elapsed)

    def test_indexing_timeout_bounds_requests_and_sleep_by_the_remaining_deadline(self):
        def unavailable_index(url, **_):
            return self.package_response() if url == self.download_url else self.index_response("1.0.0")

        requests = self.patch(release.urllib.request, "urlopen", side_effect=unavailable_index)

        with self.assertRaises(TimeoutError):
            release.wait_for_public_package(self.analyzer, timeout=5, poll_interval=2)

        self.assertEqual(5, self.elapsed)
        self.assertEqual([mock.call(2), mock.call(2), mock.call(1)], self.sleep.call_args_list)
        self.assertEqual([5, 5, 3, 3, 1, 1], [call.kwargs["timeout"] for call in requests.call_args_list])
        self.assertEqual(3, sum(call.args[0] == self.index_url for call in requests.call_args_list))

    def test_interrupted_download_retries_before_attempting_index_verification(self):
        interrupted = mock.MagicMock()
        interrupted.__enter__.return_value.read.side_effect = TimeoutError("download interrupted")
        requests = self.patch(
            release.urllib.request, "urlopen",
            side_effect=[interrupted, self.package_response(), self.index_response("1.0.1")],
        )

        release.wait_for_public_package(self.analyzer, timeout=10, poll_interval=3)

        self.assertEqual(
            [self.download_url, self.download_url, self.index_url],
            [call.args[0] for call in requests.call_args_list],
        )
        interrupted.__enter__.return_value.read.assert_called_once_with()
        self.sleep.assert_called_once_with(3)

    def test_permanent_http_failure_is_reported_without_retrying(self):
        forbidden = urllib.error.HTTPError(self.download_url, 403, "Forbidden", None, None)
        self.addCleanup(forbidden.close)
        requests = self.patch(release.urllib.request, "urlopen", side_effect=forbidden)

        with self.assertRaises(urllib.error.HTTPError) as error:
            release.wait_for_public_package(self.analyzer, timeout=10, poll_interval=3)

        self.assertIs(forbidden, error.exception)
        requests.assert_called_once()
        self.sleep.assert_not_called()

    def test_stale_download_after_404_prevents_client_publication_without_retrying_mismatch(self):
        missing = urllib.error.HTTPError(self.download_url, 404, "Not yet published", None, None)
        self.addCleanup(missing.close)
        stale = io.BytesIO(package_bytes(dlls={ANALYZER_DLL: b"unfixed-analyzer"}, signed=True))
        requests = self.patch(release.urllib.request, "urlopen", side_effect=[missing, stale])
        push = self.patch(release, "push_package")
        client = self.packages / "RestClient.Net.7.3.1.nupkg"
        client.write_bytes(
            package_bytes("RestClient.Net", "7.3.1", dlls=CLIENT_DLLS, dependencies=analyzer_dependencies())
        )

        with self.assertRaisesRegex(ValueError, "DLLs differ"):
            release.publish_release(self.packages, "7.3.1", "1.0.1")

        push.assert_called_once_with(self.analyzer)
        self.assertEqual([self.download_url, self.download_url], [call.args[0] for call in requests.call_args_list])
        self.sleep.assert_called_once_with(15)
        self.assertEqual(15, self.elapsed)


if __name__ == "__main__":
    unittest.main()
