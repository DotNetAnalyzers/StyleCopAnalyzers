---
name: fixing-a-bug
description: Test-first procedure for fixing a StyleCop.Analyzers bug report (false positive, false negative, wrong message or location, AD0001 crash, or a code fix that produces bad code or deletes comments). Use whenever you fix a reported issue or an audit finding. It requires reproducing the exact reported behavior in a failing test on master before changing product code.
---

# Fixing a bug

The rule: **no product change until a test reproduces exactly what the reporter described and fails on
master in that way.** A test that fails for some other reason (a compile error in the snippet, a wrong
expected location, a line-ending difference) proves nothing about the bug.

Related skills: `testing-and-coverage` (where tests live, how to run them), `lightup-and-roslyn-versions`,
`analyzer-performance`, `pull-request-workflow`.

## 1. Understand the report

- Read the whole issue and every comment. Note the reporter's StyleCop version, compiler/SDK, language
  version, settings (`stylecop.json`, `.editorconfig`, rule set), and the exact snippet.
- Write down the **observed** behavior (diagnostic ID, message text, location; or the AD0001 exception; or
  the code fix output) and the **expected** behavior.
- Check maintainer comments for design intent. A "bug" that a maintainer called by-design needs a decision,
  not a fix (for example sharwell's preference for discards in #3057/#2631 led to a narrow SA1312 exemption
  plus a separate discard code fix in #4208).
- Search for duplicates and earlier PRs (see `issue-triage`). Contributors' forks often have a fix already
  (Björn's draft commit for #3906 became #4198); build on it, credited (see `pull-request-workflow`).

## 2. Reproduce on master

Quick check: run the reporter's snippet through the probe harness
(`.agents/skills/csharp-language-version-audit/scripts/probe`, `--fix` for code fix bugs) built from
current master. If it doesn't reproduce, try the reporter's language version (`--lang 10`) and settings
(a sibling `<snippet>.json` is used as `stylecop.json`). If it still doesn't reproduce, the issue may already
be fixed: go to `issue-triage`, don't write a fix.

## 3. Write the failing test

Where it goes:

- Pick the **lowest** test project whose compiler can parse the snippet, so every later project inherits it:
  `StyleCop.Analyzers.Test` (C# 6) for plain syntax, `Test.CSharp11` for raw strings or newlines in
  interpolation holes, `Test.CSharp15` for unions. #4196 added the verbatim-string case to the C# 6 class
  and the C# 11-only `$"..."` case to the C# 11 class.
- Add it to the existing `SA####UnitTests` / `SA####CSharpNUnitTests` class for the rule.
- If the expected result legitimately differs by compiler, use a `protected virtual` expectation in the base
  class with an override in the later class, or `LightupHelpers.SupportsCSharpN` in the test.

How it's written:

- Reproduce the reporter's code as closely as possible, trimmed to the minimum. Keep the parts that matter
  (trivia, comments, line breaks, attributes, the exact construct).
- Mark the location with markup (`{|#0:}|}` plus `Diagnostic().WithLocation(0)`, or `[|...|]`) and assert
  the message arguments with `.WithArguments(...)`. Location and message must match what the reporter saw.
- If the rule has a code fix, use `VerifyCSharpFixAsync(testCode, expected, fixedCode, ...)` instead of a
  separate diagnostic-only test (reviewers ask for this; #4200). The testing library also runs Fix All in
  document, project and solution scope and checks that no diagnostic remains. When nested fixes need more than
  one pass, set `NumberOfFixAllIterations`/`NumberOfIncrementalIterations` explicitly rather than splitting the
  test.
- For a crash, assert the expected (non-crashing) result: any AD0001 fails the verifier with the exception.
- For a false positive, include the near-miss that must **still** be reported (#4196 kept `{x }` on the same
  line reported). For a false negative, include the valid form that must stay quiet.
- Add `[WorkItem(<issue>, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/<issue>")]` and a
  `<summary>` like the neighboring tests. Use `ConfigureAwait(false)`.

Run it on master and read the failure:

```bash
dotnet build StyleCop.Analyzers/StyleCop.Analyzers.Test.CSharp11 -c Debug
dotnet test StyleCop.Analyzers/StyleCop.Analyzers.Test.CSharp11 --no-build -f net6.0 \
  --filter "FullyQualifiedName~SA1013"
```

The failure message must show the reported behavior: for a false positive, a diagnostic count mismatch that
lists the reported ID at the reported line and column; for a code fix bug, a diff showing the bad fixed text. If it fails differently, fix the test
first. Record "fails on master" in the PR body (#4196: "Both fail on master"; #4198: "All 7 `file` cases fail
on master").

## 4. Fix

- Smallest change in the analyzer or code fix that makes the test pass. Product code stays on Roslyn 1.2.1
  (light-up only).
- Match the rule's documented behavior in `documentation/SA####.md`. If the docs already describe the
  correct behavior (SA1013 exempts a brace that starts a line), the fix aligns code with docs.
- No new semantic-model work in the analyzer when a syntax check suffices; put expensive or uncertain checks in
  the code fix (#4208). See `analyzer-performance`.
- Code fixes must keep comments and other trivia (#4199, #4200), and should not be offered when they can't
  preserve meaning or comments (#4200 doesn't offer SA1102's fix when it would delete a comment; #4208 skips
  cases where removing the type changes overload resolution or conversions).
- Fix the root cause in shared helpers when several rules share it, and test each affected rule.

## 5. Prove the fix

- The new tests pass, on every test project that inherits them.
- Run the rule's whole suite in the lowest and highest projects (C# 6 and the newest), and compare the failure
  sets with master (on Linux some code-fix tests fail only because of CRLF; see `testing-and-coverage`).
  Don't dismiss a failure as "the known CRLF one" without checking the message: #4148's C# 6 failures were a
  real Roslyn 1.x difference that looked like the line-ending noise.
- Every new branch in the product change is covered by a test, including the success path (#4197 review).
- If you changed a `.resx` (message, code fix title), regenerate the Designer files (`lightup-and-roslyn-versions`).

## 6. Docs and PR

- Update `documentation/SA####.md` when visible behavior changes (new exemption, new cases reported, new
  setting interaction). Keep docs-only clarifications in their own PR when there's no code change (#4205).
- PR title: `SA####: <what changed>`. Body: `Fixes #<issue>` on its own line, what was wrong, what changed,
  the tests and that they fail on master, the perf impact (usually "one extra syntax check, no measurable
  cost"), and any behavior change flagged clearly (#4202: SA1316 now reports explicit tuple names by default).
- If the issue was closed as a duplicate, point `Fixes` at the open canonical issue instead.

## Known Roslyn quirks in old test projects

- C# 6 project (Roslyn 1.3.2): syntax node actions don't run on partial method **implementation** parts, so
  expectations for partial methods differ from C# 7+ (`LightupHelpers.SupportsCSharp7 ? both : one`).
- Roslyn 4.0 (C# 10/11 projects): some diagnostics on positional records and record structs are reported twice
  (dotnet/roslyn#53136); tests use non-positional records or `virtual` expectations overridden in later projects.
- Roslyn 5.x: syntax node actions don't run on the defining part of a partial constructor.
- Roslyn 5.9: nodes inside a union are visited up to three times (fixed in 5.10; the C# 15 test project uses a
  5.10 preview build).
- A `$$"""` raw string can't be written inside test markup, because `$$` is the markup caret marker; use
  `MarkupMode.None` or build the string differently.
