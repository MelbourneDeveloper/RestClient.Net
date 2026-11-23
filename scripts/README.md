# Verified package releases

The `Release RestClient.Net and Exhaustion` GitHub Action accepts a RestClient.Net
version or a `restclient-v*` tag. It publishes both packages from the checked-out
commit. It uses the requested version directly, never a repository version variable.

`ExhaustionVersion` in `Directory.Build.props` controls the analyzer package version
and the dependency in every RestClient.Net target framework. Increment it when
changing the analyzer. RestClient.Net's project version records the next client
release; a manual release input overrides it consistently during restore, build,
and pack.

Before publishing, the action runs analyzer regressions and installs the packed
RestClient.Net into separate consumers with fresh NuGet caches. These consumers
reference only RestClient.Net: incomplete switches must report `EXHAUSTION001`,
complete switches must compile, and the issue #146 hierarchy must finish with the
bounded `EXHAUSTION002` diagnostic. Consumer processes have time and heap limits.

The action publishes Exhaustion first and waits until its exact analyzer DLL is
downloadable and its version is indexed. Only then can it publish RestClient.Net.
It verifies both published packages and repeats the consumer checks using only
nuget.org. The standalone Exhaustion action uses the same packaging and verification
helpers. Both publishers share a concurrency group.

NuGet versions are immutable. A retry can reuse an existing version only if its
DLLs, package identity, and dependencies match this build. A mismatch fails the
release; increment the affected package version instead of accepting an older DLL.
NuGet's signing changes are excluded from the binary comparison.

## Local verification

```sh
python3 scripts/release.py pack --output .artifacts/release
python3 -m unittest discover -s scripts/tests -p test_release_orchestration.py -v
python3 scripts/tests/test_release_packages.py --packages .artifacts/release \
  --restclient-version 7.3.1 --exhaustion-version 1.0.1
```

Packaging uses a new restore cache and separate build output so an existing local
package cannot hide a stale analyzer. CI bootstraps the unpublished analyzer into
`.artifacts/packages` before restoring the solution, allowing the complete suite to
run before a new analyzer version exists on NuGet. These verification commands do
not publish packages.
