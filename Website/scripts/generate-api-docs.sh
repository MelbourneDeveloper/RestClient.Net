#!/usr/bin/env bash
# Export source-backed API reference with the same tool used by npm run build.
set -euo pipefail
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
node "$script_dir/generate-api-docs.js" "$@"
