---
name: pull-request-workflow
description: How to prepare, open, update and hand off pull requests in DotNetAnalyzers/StyleCopAnalyzers. Use when creating a branch or PR, deciding whether to stack PRs, building on a contributor's PR or fork commits, resolving merge conflicts in test classes, checking CI before marking a PR ready, responding to review comments, or editing a PR description.
---

# Pull request workflow

Agents prepare PRs; humans review and merge. **Never merge a PR, never close or comment on other people's
PRs or issues, and never announce a decision publicly** unless the human you work for asked for that specific
action.

## 1. Scope and branches

- **One PR per independent change**, each based on current `master`. Thirteen quick-win fixes were 13 PRs
  (#4195–#4207), not one.
- **Stack only when the work really builds on earlier unmerged work** (for example, #4193's union rules needed
  #4192's preview default; #4149 needed #4148). Base the upper PR on the lower PR's branch and say so in the
  body ("Stacked on #N; merge that first"). When the lower PR merges, update the upper one: rebasing within your
  own stack is fine; otherwise merge `master` in.
- No unrelated edits, formatting churn, or package bumps. If you notice another problem, report it or open a
  separate PR.
- Commit subjects: imperative and specific, one rule per commit for multi-rule work
  (`Update SA1201 for extension blocks`). The body says why.

## 2. Building on someone else's work

Contributors' work must stay in history with their authorship.

- **Their PR branch:** merge `master` into it (a merge commit, conflicts resolved in the merge) and push your
  commits on top, if the PR allows maintainer edits (`maintainerCanModify`). Never rebase, squash, or
  force-push a contributor's branch.
- **Branch you can't push to:** push their original commits (same SHAs) to a new branch, merge `master`, add
  your commits, and open a replacement PR that says "Supersedes #N" and credits them. Leave their PR alone; the
  maintainer decides whether to close it (GitHub marks it merged automatically when its commits land, as with
  #4071 → #4121).
- **Commits tangled with unrelated fork history** (a fork's master with header rewrites, unsigning, etc.):
  `git cherry-pick -x <sha>` only the relevant commits; the author is kept and `-x` records the source.
- **Re-implementing an idea from a fork commit:** your own commit with
  `Co-authored-by: Name <email>` and the original commit named in the body.
- Credit everyone in the PR body ("This builds on @bjornhellander's original commit 2e31c40a5 ...").
- Don't delete contributors' branches.

## 3. Before pushing

- [ ] Tests written first and failing on master for bug fixes (`fixing-a-bug`), coverage per
      `testing-and-coverage`.
- [ ] Solution builds with **zero warnings** (warnings are errors here).
- [ ] Generated files are current if you touched a `.resx`, `Syntax.xml`, `OperationInterfaces.xml`, a generator
      or the compiler version. Run the "Check generated files" commands from `lightup-and-roslyn-versions`.
      Forgetting this is the most common CI failure (#4208 added a code fix title to `NamingResources.resx` and
      failed until `NamingResources.Designer.cs` was committed).
- [ ] `documentation/SA####.md` updated for visible behavior changes.
- [ ] Only intended files staged. Never `git add -A` after a build (`Lightup/.generated` line-ending noise).

## 4. Open as a draft

```bash
git push -u origin <branch>
gh pr create --draft --base master --title "SA1013: don't report a closing interpolation brace that starts a line" --body-file body.md
```

Body: `Fixes #N` (user issue) and `Part of #N` / `Closes #N` (audit sub-issue) on their own lines; what was
wrong; what changed; tests and the fact they fail on master; perf impact; behavior changes and design choices
flagged ⚑ for the reviewer; anything left out and why; credit.

**Editing a PR body or title:** `gh pr edit` fails on this repo (Projects classic GraphQL deprecation). Use REST:

```bash
gh api -X PATCH repos/DotNetAnalyzers/StyleCopAnalyzers/pulls/<N> -F body=@body.md
gh api -X PATCH repos/DotNetAnalyzers/StyleCopAnalyzers/pulls/<N> -f title="New title"
```

## 5. CI must be fully green before "ready for review"

`.github/workflows/build.yml` produces **25 checks** today: Build Debug/Release (2), Check generated files on
windows and ubuntu (2), Test C# 6–15 × Debug/Release (20), and Merge code coverage (1). Adding a test project
adds 2. Mark ready only when every check of the run **for the current head SHA** concluded `SUCCESS`:

```bash
gh pr view <N> --json headRefOid,statusCheckRollup -q '.headRefOid, ([.statusCheckRollup[] | .conclusion] | group_by(.) | map("\(.[0]) \(length)"))'
# want: the SHA you pushed, then ["SUCCESS 25"]
gh pr ready <N>
```

- `IN_PROGRESS`/empty conclusions mean wait. Don't mark ready on a partial run.
- The workflow cancels in-progress PR runs on a new push (`concurrency: cancel-in-progress`). Two pushes close
  together can leave `CANCELLED` jobs from a superseded run on the same SHA, which the PR shows as failing.
  Confirm the newer run passed, then rerun the cancelled one so the PR is clean: `gh run rerun <run-id>`.
- A real failure: read the job log (`gh run view <run-id> --log-failed`), fix, push, wait again. Don't rerun
  real failures hoping they pass. Test jobs time out at 45 minutes; a hang (for example a binding-redirect
  `FileLoadException` while downloading reference packages) is a real failure.
- Codecov status checks are disabled; read its PR comment for uncovered patch lines anyway.

## 6. Review feedback

- Move the PR back to draft while you work (`gh pr ready <N> --undo`), push new commits (no force-push), and
  mark it ready again only after CI is fully green again.
- Reply on each review thread with what changed and the commit SHA. If the reviewer asked a question that is a
  design decision (a new setting, wider scope, a behavior change), ask the human you work for instead of
  deciding.
- A "changes requested" review stays until that reviewer re-reviews; don't dismiss reviews.

## 7. Merge conflicts after other PRs land

- Prefer merging `master` into the branch over rebasing once a PR has been reviewed (review comments point at
  SHAs).
- Most conflicts here are add/add in the same `SA####CSharpNUnitTests` class. Keep **both** sides' test
  methods, then verify: every test method from both sides is present verbatim, no duplicate method names, no
  conflict markers, members still ordered for SA1202 (public tests before protected helpers). Text-based merge
  helpers miscount braces inside interpolated strings like `$@"...{(x ? "a" : "b")}..."`; check their output.
- Rebuild and rerun the affected tests after resolving.

## 8. Hand-off

Tell the human the PR URL, what it does, CI status (with the run), behavior changes and open decisions. The
maintainers merge with merge commits (not squash), so keep commits meaningful.
