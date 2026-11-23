# Release package regressions

Run against the actual packages that will be published:

```sh
python3 scripts/tests/test_release_packages.py \
  --packages artifacts/packages \
  --restclient-version 7.3.1 \
  --exhaustion-version 1.0.1
```

Requires Python 3, the .NET SDK, and NuGet access for supporting dependencies.
Both `.nupkg` files must exist in the specified directory. These checks inspect
the packed dependency metadata and analyzer asset, then restore an independent
consumer outside this repository with a fresh NuGet cache. Its only package
reference is RestClient.Net; source mapping forces both release packages to come
from the supplied directory.

After publishing, repeat with `--public-nuget` instead of `--packages <directory>`.
That inspects the downloaded public artifacts and restores the consumer solely
from nuget.org into a fresh cache, proving the published dependency chain works.

The consumer builds an incomplete switch, fixes it, and reintroduces the missing
case to prove the analyzer runs throughout normal edits. Both switch expressions
and statements exercise the 25-leaf recursive hierarchy from issue #146 and must
produce the bounded EXHAUSTION002 warning. Compiler invocations have a 512 MiB
managed-heap limit, a 90-second timeout, and disabled shared compiler/build servers.
Timeouts terminate the complete process tree. Temporary projects and packages are
cleaned up after the run.
