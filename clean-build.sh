#!/usr/bin/env bash
set -e
set -o pipefail

git clean -x -f -d
dotnet msbuild -m:1 -p:StopOnFirstFailure=true -t:Build -restore

dotnet test
