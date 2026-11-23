---
name: upgrade-packages
description: Upgrade all dependencies/packages to their latest versions for the detected language(s). Use when the user says "upgrade packages", "update dependencies", "bump versions", "update packages", or "upgrade deps".
---
<!-- agent-pmo:372ce7f -->

## RestClient.Net context

NuGet manifests are the solution's `*.csproj`, `RestClient.Net.FsTest/RestClient.Net.FsTest.fsproj`, and `Directory.Build.props`. The website uses npm in `Website/`. Keep shared OpenAPI package versions aligned across both generators and their tests. Dependabot batches updates on `dependabot-upgrades`; bring a reviewed batch to `main` through CI. Use root `AGENTS.md` for validation commands.

# Upgrade Packages

Upgrade all project dependencies to their latest compatible (or latest major, if `--major`) versions.

## Arguments

- `--check-only` — List outdated packages without upgrading. Stop after Step 2.
- `--major` — Include major version bumps (breaking changes). Without this flag, stay within semver-compatible ranges.
- Any other argument is treated as a specific package name to upgrade (instead of all packages).

## Step 1 — Detect language and package manager

Inspect the repo root and subdirectories for manifest files. Identify ALL that apply:

| Manifest file | Language | Package manager |
|---|---|---|
| `package.json` | Node.js / TypeScript | npm / yarn / pnpm (check lockfile) |
| `*.csproj` / `*.fsproj` / `*.sln` | C# / F# | NuGet (dotnet) |
| `Directory.Build.props` | C# / F# | NuGet (dotnet) |

If multiple languages are present, process each one in order.

**If you cannot detect any manifest file, stop and tell the user.**

## Step 2 — List outdated packages

Run the appropriate command to list what's outdated BEFORE upgrading anything. Show the user what will change.

### Node.js (npm)
```bash
npm outdated
```
If using yarn: `yarn outdated`. If using pnpm: `pnpm outdated`.

**Read the docs:**
- npm: https://docs.npmjs.com/cli/v10/commands/npm-update
- yarn: https://yarnpkg.com/cli/up
- pnpm: https://pnpm.io/cli/update

### C# / F# (NuGet)
```bash
dotnet list package --outdated
```
For transitive dependencies too: `dotnet list package --outdated --include-transitive`

**Read the docs:** https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-list-package

## Step 3 — Read the official upgrade docs

**Before running any upgrade command, you MUST fetch and read the official documentation URL listed above for the detected package manager.** Use WebFetch to retrieve the page. This ensures you use the correct flags and understand the behavior. Do not guess at flags or options from memory.

## Step 4 — Upgrade packages

Run the upgrade. If a specific package name was given as an argument, upgrade only that package.

### Node.js (npm)
```bash
npm update                            # semver-compatible (within package.json ranges)
# --major flag:
npx npm-check-updates -u && npm install   # bump package.json to latest majors
```
If using yarn: `yarn up` / `yarn up -R '**'`. If using pnpm: `pnpm update` / `pnpm update --latest`.

### C# / F# (NuGet)
There is NO single `dotnet upgrade-all` command. You must upgrade each package individually:
```bash
# For each outdated package from Step 2:
dotnet add <project.csproj> package <PackageName>    # upgrades to latest
# Or with specific version:
dotnet add <project.csproj> package <PackageName> --version <version>
```
For `Directory.Build.props`, edit the version numbers directly in the XML.

**Read the docs:** https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-package

Alternatively, use the dotnet-outdated global tool:
```bash
dotnet tool install --global dotnet-outdated-tool
dotnet outdated --upgrade
```
**Read the docs:** https://github.com/dotnet-outdated/dotnet-outdated

## Step 5 — Verify the upgrade

After upgrading, run the project's build and test suite to confirm nothing broke:

```bash
make ci
```

If `make ci` is not available, run whatever build/test commands the project uses (check the Makefile, CI workflow, or CLAUDE.md).

If tests fail:
1. Read the failure output carefully
2. Check the changelog / migration guide for the upgraded packages (fetch the release notes URL if available)
3. Fix breaking changes in the code
4. Re-run tests
5. If stuck after 3 attempts on the same failure, report it to the user with the error details and the package that caused it

## Step 6 — Report

Provide a summary:

- Packages upgraded (old version -> new version)
- Packages skipped (and why, e.g., major version bump without `--major` flag)
- Build/test result after upgrade
- Any breaking changes that were fixed
- Any packages that could not be upgraded (with error details)

## Rules

- **Always list outdated packages first** before upgrading anything
- **Always read the official docs** for the package manager before running upgrade commands
- **Always run tests after upgrading** to catch breakage immediately
- **Never remove packages** unless they were explicitly deprecated and replaced
- **Never downgrade packages** unless rolling back a broken upgrade
- **Never modify lockfiles manually** (package-lock.json, yarn.lock, Cargo.lock, etc.) — let the package manager regenerate them
- **Commit nothing** — leave changes in the working tree for the user to review

## Success criteria

- All outdated packages upgraded to latest compatible (or latest major if `--major`)
- Build passes
- Tests pass
- User has a clear summary of what changed
