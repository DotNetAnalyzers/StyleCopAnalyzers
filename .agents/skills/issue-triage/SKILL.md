---
name: issue-triage
description: Procedure for triaging the StyleCop.Analyzers issue backlog into likely fixed, duplicate, quick-win bug, and stale or won't-fix groups, with evidence and confidence levels, and for closing issues with evidence-citing comments. Use when asked to review open issues, check whether an issue still reproduces, find duplicates, pick quick wins, or close issues.
---

# Issue triage

Triage is read-only until a maintainer has seen the report. **Report first, close later**, and only the
issues the maintainer approved. Closing, commenting and labeling are visible to reporters and can't be quietly
undone.

## 1. Collect

```bash
R=DotNetAnalyzers/StyleCopAnalyzers
gh issue list -R $R --state open --limit 1000 \
  --json number,title,labels,createdAt,updatedAt,author,comments,body > issues.json
```

For each issue, also collect:

- **Linked PRs** (merged fixes are the strongest evidence):

  ```bash
  gh api graphql -F n=3898 -f query='query($n:Int!){repository(owner:"DotNetAnalyzers",name:"StyleCopAnalyzers"){
    issue(number:$n){timelineItems(itemTypes:[CROSS_REFERENCED_EVENT,CONNECTED_EVENT],first:50){nodes{
      ... on CrossReferencedEvent{source{... on PullRequest{number state mergedAt title} ... on Issue{number state title}}}
      ... on ConnectedEvent{subject{... on PullRequest{number state mergedAt title}}}}}}}}'
  ```

- **Commits that mention it**: `git log origin/master --oneline --grep "#3898"`, and `rg "WorkItem\(3898"` in the
  test projects.
- **Maintainer comments** (sharwell, other maintainers and regular reviewers): "by design", "would accept a
  PR", "duplicate of", "not planned" decide categories more than the reporter's text does.

Read every issue and its latest comments yourself. Titles mislead.

## 2. Probe

For each bug report that might be fixed, run the reporter's snippet against current master with the probe
harness (`.agents/skills/csharp-language-version-audit/scripts/probe`, `--fix` for code fix reports, the
reporter's `--lang` and `stylecop.json`). Save the snippet and output; the closing comment quotes what you ran.
Record the master commit you probed.

## 3. Categorize

| Category | Meaning | Evidence required |
| --- | --- | --- |
| **Likely fixed** | No longer reproduces, or a merged PR addresses it | A merged PR/commit that fixes it, **or** a probe on current master showing the reported code no longer misbehaves. Both is best. |
| **Duplicate** | Same root cause or request as another issue | Name the canonical issue and say why they're the same. The canonical one is usually the one with the discussion or the older one. If the canonical issue is closed as fixed, list the issue under **Likely fixed** instead. |
| **Quick win** | Still reproduces, the fix is small and safe | Probe output confirming it, the code location, a size estimate (lines), perf risk (`analyzer-performance`), and value. Note any design decision needed. |
| **Stale / won't fix** | Answered, by design, superseded, or belongs elsewhere | A maintainer comment saying so, a reporter saying it's resolved, an existing `wontfix` label, or a built-in replacement (IDE0130, CA1507, IDE0055). |
| (everything else) | Real, larger, or unclear | Leave it alone. Note still-real bugs found while probing separately. |

Confidence for each row: **High** (merged fix plus probe, or explicit maintainer/reporter statement),
**Medium-High** (probe only, or a clear duplicate with the same root cause), **Medium** (partial evidence:
some bullets of a multi-part issue unverified, or a similar but not identical trigger). Only High and
Medium-High are good candidates to close. Re-check Medium ones before closing; in the Oct 2026 pass, 8 of 63
approved issues were held open after a second look (for example, a duplicate with a different trigger, or a
multi-part issue where only one part was verified).

## 4. Report

Write a report with one table per category: issue, title, evidence (PR numbers, commit, probe result with the
master SHA), confidence. Rank quick wins by value with size and perf risk. List "still real, not quick"
findings at the end. Share it with the maintainer and wait for their decision per group.

## 5. Close (only what was approved)

Immediately before acting, re-check that each issue is still open and has no new comments since triage. Post
the comment first, then close with the right reason.

```bash
gh issue comment 3550 -R $R --body "This is fixed by #3940, which ... I confirmed it on current master. Thanks for the report!"
gh issue close 3550 -R $R --reason completed        # likely fixed
gh issue close 2113 -R $R --reason "not planned"    # stale / won't fix
# duplicates: use GraphQL so GitHub records the duplicate link
i=$(gh api repos/$R/issues/3877 -q .node_id); d=$(gh api repos/$R/issues/3349 -q .node_id)
gh api graphql -f i=$i -f d=$d -f query='mutation($i:ID!,$d:ID!){closeIssue(input:{issueId:$i,stateReason:DUPLICATE,duplicateIssueId:$d}){issue{state stateReason}}}'
```

Comment templates (keep them short, specific, and friendly; cite the evidence):

- Fixed by a PR: `This is fixed by #4187, which adds safe and closed to SA1206's modifier ordering, with C# 15
  tests. Thanks for reporting it!`
- Fixed, verified by probe: `I checked this on current master: <exact code> no longer reports SA1515 (see
  #3940). Please reopen if you still see it.`
- Can't reproduce: `I couldn't reproduce this on current master: <code> reports no SA1100. Please reopen with a
  repro if you still see it on a current build.`
- Duplicate: `Duplicate of #3349. Closing in favor of #3349; please follow and add to the discussion there.`
- Stale, built-in replacement: `The .NET SDK now covers this with IDE0130 (namespace must match folder
  structure). Closing for now; feel free to reopen if IDE0130 misses a case you need.`
- Stale, by design: `As Bjorn explained, this is the intended SA1516 behavior. Closing for now; feel free to
  reopen with new information.`

Log every action (issue, comment URL, close reason). If anything fails midway, report exactly which issues were
done.

## Quick wins become PRs

Each quick win is its own master-based PR following `fixing-a-bug`. If the issue was closed as a duplicate,
point `Fixes #N` at the open canonical issue (a PR for #3057 said "Fixes #3057" after #3057 had been closed as a
duplicate of #2631, so it closed nothing). If a fix turns out bigger or riskier than estimated, stop and report
it instead of growing the PR.
