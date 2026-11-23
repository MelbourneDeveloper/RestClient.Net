# AgentPMO integration

AgentPMO is cloned separately at `../AgentPMOWorkflow`. The installed templates come from [Nimblesite/AgentPMOWorkflow](https://github.com/Nimblesite/AgentPMOWorkflow/tree/372ce7fafc82305d60be901892d965c7cbe0ce3c), revision `372ce7fafc82305d60be901892d965c7cbe0ce3c`. The upstream MIT notice is retained in `skills/AGENTPMO-LICENSE`.

All seven template skills are in `skills/`: `fix-bug`, `ci-prep`, `submit-pr`, `upgrade-packages`, `code-dedup`, `spec-check`, and `website-audit`. Their workflows are preserved with repository context added; package-upgrade examples are limited to NuGet and npm. The root `AGENTS.md` maps upstream Makefile commands to this repository's existing commands.

The light setup adds CodeQL for C#, website JavaScript, and Actions, and AgentPMO's grouped Dependabot staging model. The existing CI pipeline is retained as `.github/workflows/ci.yml`, with cancellation of superseded runs and an explicit F# test step.

Dependabot's sweep uses the trusted base workflow and only handles same-repository PRs authored and triggered by Dependabot. It checks out the staging branch, merges Git data, and never executes files from the incoming PR. Updates reach `main` through a separately reviewed consolidation PR.

Full standards enforcement, dashboard scheduling, extra developer tools, coverage-policy changes, repository restructuring, and release-workflow changes are outside this light integration. Existing application code and tests are unchanged.

To update the integration later, compare these files with the pinned upstream templates and retain this scope; do not run the full setup or standards-enforcement workflow automatically.
