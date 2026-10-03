# Source API exporter

`npm run generate-api` runs this .NET 9 command-line tool using Roslyn 4.8.0,
matching the analyzer project's Roslyn version. Only this small project needs
restoring; the application, sample projects, and their analyzers are not built.

The exporter reads the actual C# files in RestClient.Net, Outcome, both generator
projects, and Exhaustion. Roslyn syntax and semantic symbols supply public types,
nested types, partial declarations, members, delegate signatures, positional record
properties, parameters, constraints, defaults, and overload identities. Private and
internal declarations (including public children of internal types) stay hidden.
Signatures preserve source aliases and expressions; compiler-generated record
helpers and inherited members are not duplicated as declared API.

XML summaries, remarks, parameter/type parameter descriptions, and return prose
become both JSON data and Markdown. Every declaration carries its repository file,
line, and source URL. The output has no timestamp, absolute local paths, or machine
metadata; repeating an export with identical inputs is byte-for-byte deterministic.

Generated files live under `src/api/reference/`. `api.json` follows the versioned
`schema.json` contract. Type pages use `layouts/api.njk`, the site's shared prose
styles, and `templateEngineOverride: md` so code and XML comments containing
`{{ expressions }}` cannot be evaluated as Nunjucks templates. Member anchors derive
from Roslyn documentation IDs, keeping overloads distinct and documentation-only
edits stable. Curated `/api/` guides are ordinary Markdown and are not overwritten.

```sh
node scripts/generate-api-docs.js
node scripts/generate-api-docs.js --source-ref COMMIT_SHA
node --test tests-node/api-export.test.js
```

For isolated fixtures, pass `--root PATH --projects ProjectOne,ProjectTwo --output
PATH`. The output directory is exporter-owned: stale generated Markdown is removed
on each run. Source syntax errors fail generation. The small tool restores matching
metadata packages for Urls, Microsoft.Extensions.Http, and Microsoft.OpenApi so
public signature symbols resolve correctly, and supplies the standard SDK implicit
global imports enabled by the repository. No product build or execution is needed.
Displayed signatures retain the exact source-level type names and aliases.

Exports use C# 12 parsing with the supplied source files; project-specific conditional
compilation and source generators are not evaluated. When a new external type enters
a public signature, add its matching metadata package to this tool as well.
