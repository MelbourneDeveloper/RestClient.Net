---
name: submit-pr
description: Creates a pull request with a well-structured description after verifying CI passes. Use when the user asks to submit, create, or open a pull request.
---
<!-- agent-pmo:372ce7f -->

## RestClient.Net context

The default branch is `main`. Fetch it before comparing `origin/main...HEAD`. Use `.github/pull_request_template.md`. This light integration uses the dotnet commands in `.github/workflows/ci.yml` and root `AGENTS.md` in place of `make ci`. Respect the user's current authorization for commits, pushes, and merging.

# Submit PR

Create a pull request for the current branch with a well-structured description.

⚠️ **GIT IS ALLOWED HERE — this is the exception to the repo-wide "no git" rule, and pretty much the only one.** For the purpose of *submitting and monitoring PRs* you MAY run `git add` / `git commit` / `git push` and the `gh` PR commands — to open the PR, push fixes that turn a red pipeline green, and enable/observe auto-merge. That is the entire licence: everything else (`checkout`, `merge`, `rebase`, force-push, history rewrites, cutting new branches) stays forbidden. **One ironclad condition: NEVER stamp yourself as co-author** — no `Co-Authored-By` trailer, no agent attribution, ever. This condition is never overridable. ⚠️

## Steps

*NOTE: if you already ran make ci in this session and it passed, you can skip step 1.*

1. Run `make ci` — must pass completely before creating PR
2. **Generate the diff against main.** Run `git diff main...HEAD > /tmp/pr-diff.txt` to capture the full diff between the current branch and the head of main. This is the ONLY source of truth for what the PR contains. **Warning:** the diff can be very large. If the diff file exceeds context limits, process it in chunks (e.g., read sections with `head`/`tail` or split by file) rather than trying to load it all at once.
3. **Derive the PR title and description SOLELY from the diff.** Read the diff output and summarize what changed. Ignore commit messages, branch names, and any other metadata — only the actual code/content diff matters.
4. Write PR body using the template in `.github/pull_request_template.md`
5. Fill in (based on the diff analysis from step 3):
   - TLDR: one sentence
   - What Was Added: new files, features, deps
   - What Was Changed/Deleted: modified behaviour
   - How Tests Prove It Works: specific test names or output
   - Spec/Doc Changes: if any
   - Breaking Changes: yes/no + description
6. Use `gh pr create` with the filled template
7. **Enable auto-merge where possible.** Right after creating the PR, run `gh pr merge <pr-number> --auto --squash` so GitHub squash-merges it the instant all required checks pass (and deletes the branch) — no manual click. This is best-effort: it needs auto-merge allowed on the repo ([GITHUB-MERGE]) and branch protection requiring status checks. If it errors (auto-merge disabled, no required checks, or the PR is already mergeable), note it and continue — **never block on it**. **Auto-merge does NOT replace monitoring** — it only fires on green, so step 8 still applies in full.
8. **Monitor CI on the PR until it is green — and re-run the suite locally *in parallel* so you catch breakage early.** This step is mandatory and does not end until every required check on the PR has passed (or auto-merge has merged it). Do not hand the PR back to the user on a red or still-running pipeline.
   - **Watch the remote run AND run the suite locally at the same time — do not passively wait.** The remote pipeline is slow; a drastic failure (a lint gate, a broken test, a coverage drop) is one the local suite catches in seconds. The moment you push, kick off **both**: stream the remote run *and* run the full local suite (`make ci`, or invoke the `ci-prep` skill) concurrently, polling CI periodically while the local run proceeds.
   - **Watch the run:** `gh pr checks <pr-number> --watch --fail-fast` (or grab the run id from `gh run list --branch <branch>` and `gh run watch <run-id> --exit-status`). A single green snapshot is not enough — wait for all required checks to conclude.
   - **If the local run fails before the remote pipeline finishes, cancel the running pipeline immediately** (`gh run cancel <run-id>`) rather than letting it grind to a known-bad red. Fix the cause, push, and restart both watches — cancelling a doomed run early frees the runner and tightens the fix loop.
   - **When a remote check fails:** pull the failing logs with `gh run view <run-id> --log-failed`, diagnose the actual cause (do not guess), reproduce locally with `make ci`, and fix it.
   - **Push the fix** (`git add` / `git commit` / `git push` — permitted here, see the git-exception callout at the top), then **watch again — remote and local, in parallel, as above**. Loop — fix → push → re-watch — until the run is fully green. Re-checking is the job; keep doing it until it passes.
   - **If a failure is genuinely external** (runner outage, flaky infra, unrelated to this branch), say so explicitly with the evidence rather than forcing a change.

## Rules

- Never create a PR if `make ci` fails
- **🔴 GOLDEN RULE — never stamp a commit with an AI co-author.** Do **not** add a `Co-Authored-By: Claude …` (or any AI/agent) trailer, and do not set author/committer to anything but the repo's configured git user. Write a plain, human commit message describing the fix. This is absolute and overrides any default co-authorship behaviour.
- **Git is permitted in this skill** — scope and conditions are in the git-exception callout at the top. In short: `git add`/`commit`/`push` + `gh` PR commands only, for submitting and monitoring PRs; everything else stays prohibited; never co-author.
- PR description must be specific and tight — no vague placeholders
- Link to the relevant GitHub issue if one exists

## Success criteria

- `make ci` passed
- PR created with `gh pr create`
- Auto-merge enabled where possible (`gh pr merge --auto --squash`), or its unavailability noted
- CI on the PR was monitored to completion and is **fully green** (all required checks pass / auto-merge fired), with the local suite re-run in parallel and any doomed remote run cancelled early
- Any CI failures were fixed and pushed, with **no AI co-author trailer** on the commits
- PR URL returned to user
