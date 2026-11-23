---
name: fix-bug
description: Fix a bug using test-driven development. Use when the user reports a bug, describes unexpected behavior, wants to fix a defect, or says something is broken. Enforces a strict test-first workflow where a failing test must be written and verified before any fix is attempted.
---
<!-- agent-pmo:372ce7f -->

## RestClient.Net context

Use the MSTest or F# test project that owns the defect. Run a focused regression with `dotnet test <project> --configuration Release --filter FullyQualifiedName~<test>`. Follow the user's existing authorization when moving from the demonstrated failure to the fix. See the test and Docker notes in the root `AGENTS.md`.

# Bug Fix Skill — Test-First Workflow

You MUST follow this exact workflow. Do NOT skip steps. Do NOT fix the bug before writing a failing test.

## Step 1: Understand the Bug

- Read the bug description: $ARGUMENTS
- Investigate the codebase to understand the relevant code
- Identify the root cause (or narrow down candidates)
- Summarize your understanding of the bug to the user before proceeding

## Step 2: Write a Failing Test

- Write a test that **directly exercises the buggy behavior**
- The test must assert the **correct/expected** behavior — so it FAILS against the current broken code
- The test name should clearly describe the bug (e.g., `test_orange_color_not_applied_to_head`)
- Use the project's existing test framework and conventions

## Step 3: Run the Test — Confirm It FAILS

- Run ONLY the new test (not the full suite)
- **Verify the test FAILS** and that it fails **because of the bug**, not for some other reason (typo, import error, wrong selector, etc.)
- If the test passes: your test does not capture the bug. Go back to Step 2
- If the test fails for the wrong reason: fix the test, not the code. Go back to Step 2
- **Repeat until the test fails specifically because of the bug**

## Step 4: Show Failure to User

- Show the user the test code and the failure output
- Explicitly ask: "This test fails because of the bug. Can you confirm this captures the issue before I fix it?"
- **STOP and WAIT for user acknowledgment before proceeding**
- Do NOT continue to Step 5 until the user confirms

## Step 5: Fix the Bug

- Make the **minimum change** needed to fix the bug
- Do not refactor, clean up, or "improve" surrounding code
- Do not change the test

## Step 6: Run the Test — Confirm It PASSES

- Run the new test again
- **Verify it PASSES**
- If it fails: go back to Step 5 and adjust the fix
- **Repeat until the test passes**

## Step 7: Run the Full Test Suite

- Run ALL tests to make sure nothing else broke
- If other tests fail: fix the regression without breaking the new test
- Report the final result to the user

## Rules

- NEVER fix the bug before the failing test is written and confirmed
- NEVER skip asking the user to acknowledge the test failure
- NEVER modify the test to make it pass — modify the source code
- If you cannot write a test for the bug, explain why and ask the user how to proceed
- Keep the fix minimal — one bug, one fix, one test
