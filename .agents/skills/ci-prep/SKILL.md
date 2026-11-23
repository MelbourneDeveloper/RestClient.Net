---
name: ci-prep
description: Prepares the current branch for CI by running the exact same steps locally and fixing issues. If CI is already failing, fetches the GH Actions logs first to diagnose. Use before pushing, when CI is red, or when the user says "fix ci".
---
<!-- agent-pmo:372ce7f -->

## RestClient.Net context

The CI entry point is `.github/workflows/ci.yml`; inspect its actual dotnet commands. Root `AGENTS.md` lists the local equivalents. Get Actions run IDs from `gh pr view <number> --json statusCheckRollup` when a branch-filtered run lookup returns nothing.

# CI Prep

Prepare the current state for CI. If CI is already failing, fetch and analyze the logs first.

## Arguments

- `--failing` — Indicates a GitHub Actions run is already failing. When present, you MUST execute **Step 1** before doing anything else.
- Any other argument is treated as a job name to focus on (but all failures are still reported).

If `--failing` is NOT passed, skip directly to **Step 2**.

## Step 1 — Fetch failed CI logs (only when `--failing`)

You MUST do this before any other work.

```bash
BRANCH=$(git branch --show-current)
PR_JSON=$(gh pr list --head "$BRANCH" --state open --json number,title,url --limit 1)
```

If the JSON array is empty, **stop immediately**:
> No open PR found for branch `$BRANCH`. Create a PR first.

Otherwise fetch the logs:

```bash
PR_NUMBER=$(echo "$PR_JSON" | jq -r '.[0].number')
gh pr checks "$PR_NUMBER"
RUN_ID=$(gh run list --branch "$BRANCH" --limit 1 --json databaseId --jq '.[0].databaseId')
gh run view "$RUN_ID"
gh run view "$RUN_ID" --log-failed
```

Read **every line** of `--log-failed` output. For each failure note the exact file, line, and error message. If a job name argument was provided, prioritize that job but still report all failures.

## Step 2 — Analyze the CI workflow

1. Find the CI workflow file. Look in `.github/workflows/` for `ci.yml`, `build.yml`, `test.yml`, `checks.yml`, `main.yml`, `pull_request.yml`, or any workflow triggered on `pull_request` or `push`.
2. Read the workflow file completely. Parse every job and every step.
3. Extract the ordered list of commands the CI actually runs. In a spec-compliant repo this is `make lint → make test → make build` (REPO-STANDARDS-SPEC [MAKE-TARGETS]), but the actual CI may use `npm`, `cargo`, `dotnet`, raw shell commands, or anything else. Extract what is *actually there*.
4. Note any environment variables, matrix strategies, or conditional steps that affect execution.

**Do NOT assume the steps are `make lint`, `make test`, `make build`.** The actual CI may run different commands, in a different order. Extract what the CI *actually does*. If you find extra targets beyond the 7 in [MAKE-TARGETS] (e.g. `make fmt-check`, `make coverage-check`), flag them in your final report — they should be consolidated by the agent-pmo skill.

### Release workflow blocker scan

If `.github/workflows/release.yml` exists, scan it before broad local CI. These are critical blockers
and must be fixed before release work is considered CI-ready:

- Tag-triggered jobs checking out `ref: main` instead of the tagged SHA.
- Any `git commit`, `git push`, branch mutation, or tag mutation during release.
- Version bump commits after the tag already exists.
- Ad hoc `sed` version stamping of structured files instead of a first-class stamper/build input.
- Missing tests that pass a test version into the same stamper used by release.
- Native VSIX releases without Node `22.x`, `npx vsce package --target <vsceTarget>`, one VSIX per
  target, target-suffixed filenames, and package-content verification.
- VS Code native-binary activation that reads or mutates PATH, uses package-manager/global installs
  as normal startup sources, or copies bundled VSIX binaries after install.

## Step 3 — Run each CI step locally, in order

Work through failures in this priority order:

1. **Formatting** — run auto-formatters first to clear noise
2. **Compilation errors** — must compile before lint/test
3. **Lint violations** — fix the code pattern
4. **Runtime / test failures** — fix source code to satisfy the test

For each command extracted from the CI workflow:

1. Run the command exactly as CI would run it (adjusting only for local environment differences like not needing `actions/checkout`).
2. If the step fails, **stop and fix the issues** before continuing to the next step.
3. After fixing, re-run the same step to confirm it passes.
4. Move to the next step only after the current one succeeds.

### Hard constraints

- **NEVER modify test files** — fix the source code, not the tests
- **NEVER add suppressions** (`#[allow(...)]`, `// eslint-disable`, `#pragma warning disable`)
- **NEVER use `any` in TypeScript** to silence type errors
- **NEVER delete or ignore failing tests**
- **NEVER remove assertions**

If stuck on the same failure after 5 attempts, ask the user for help.

## Step 4 — Report

- List every step that was run and its result (pass/fail/fixed).
- If any step could not be fixed, report what failed and why.
- Confirm whether the branch is ready to push.

## Step 5 — Remote CI follow-up (only when `--failing`)

Once all CI steps pass locally:

1. Report the local fixes and exact commands that now pass.
2. Do not commit or push. The user owns source-control writes.
3. If the user pushes, monitor the new run until completion or failure.
4. Upon failure, go back to Step 1.

## Rules

- **Always read the CI workflow first.** Never assume what commands CI runs.
- Do not commit or push from this skill.
- Fix issues found in each step before moving to the next
- Never skip steps or suppress errors
- If the CI workflow has multiple jobs, run all of them (respecting dependency order)
- Skip steps that are CI-infrastructure-only (checkout, setup-node/python/rust actions, cache steps, artifact uploads) — focus on the actual build/test/lint commands

## Success criteria

- Every command that CI runs has been executed locally and passed
- All fixes are applied to the working tree
- The CI passes successfully (if you are correcting and existing failure)
