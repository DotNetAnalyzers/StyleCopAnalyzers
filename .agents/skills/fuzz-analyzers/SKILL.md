---
name: fuzz-analyzers
description: Run the local StyleCop.Analyzers fuzz harness, triage its findings, and fix real crashes or code fixes that break valid code. Use when asked to fuzz analyzers or code fixes, rerun a campaign, reproduce a saved finding, or turn a fuzz result into a tested fix. Do not add this harness to CI.
---

# Fuzz analyzers

The harness in `tools/StyleCop.Analyzers.Fuzz` mutates C# and runs every StyleCop analyzer and code fix in a
worker process. It is a local campaign tool. It is not in `StyleCopAnalyzers.slnx`, and CI must not build or
run it. Do not add a workflow job, a solution project, or a required check for `mutate` or `repro`.

It is deterministic, not coverage-guided. The same corpus, seed, and iteration count replay the same
candidates. A new seed, more iterations, or a larger corpus can find new bugs, but most reruns repeat known
ones. Record the seed in the PR. Do not file issues automatically.

Related skills: `fixing-a-bug`, `testing-and-coverage`, `pull-request-workflow`, `analyzer-performance`.

## 1. Build and self-test

Use the SDK in `global.json`. The tool uses the central `Microsoft.CodeAnalysis.CSharp.Workspaces` version,
not the Roslyn 1.2.1 reference the shipping analyzers compile against.

```bash
dotnet build tools/StyleCop.Analyzers.Fuzz/StyleCop.Analyzers.Fuzz.csproj -c Debug -maxcpucount:1 -warnaserror
dotnet run --project tools/StyleCop.Analyzers.Fuzz -c Debug --no-build -- self-test
```

`-maxcpucount:1` is required because the build also compiles the analyzers, and the reference-assembly
annotator does not support parallel builds.

## 2. Run a campaign

Write output outside the repo. Never commit campaign directories, `summary.json`, or saved `input.json`
files.

```bash
# optional: distinct string literals from the test tree, as extra seeds
dotnet run --project tools/StyleCop.Analyzers.Fuzz -c Debug --no-build -- \
  extract test /tmp/stylecop-fuzz-corpus

# built-in seeds. iterations is the number of mutations after the seeds are replayed.
# random-seed defaults to 4338. Pick a new seed when looking for new bugs.
dotnet run --project tools/StyleCop.Analyzers.Fuzz -c Debug --no-build -- \
  mutate tools/StyleCop.Analyzers.Fuzz/Seeds /tmp/stylecop-fuzz-out 200 4338 30

# one saved finding, or a directory of them
dotnet run --project tools/StyleCop.Analyzers.Fuzz -c Debug --no-build -- \
  repro /tmp/stylecop-fuzz-out/<signature> /tmp/stylecop-fuzz-repro
```

A sibling `.json` next to a `.cs` seed is used as `stylecop.json`. `mutate` exits 1 when it saved any
finding. That is a triage list, not a harness failure. `repro` exits 1 when the saved input still reproduces.

Each signature directory contains `input.cs`, `input.json`, `finding.json`, and `fixed.cs` when a fix ran.
The harness keeps the shortest input for each signature.

## 3. Triage before changing product code

| Kind | Treat it as |
| --- | --- |
| `AnalyzerException`, `CodeFixException`, `Crash`, `HostException` | A real defect if the minimized snippet still throws. Incomplete code that crashes an analyzer is still a bug. |
| `Timeout` | Investigate only if it reproduces. A slow incomplete tree is not automatically a product bug. |
| `CodeFixBreaksCode` | A real bug. The harness reports this only when the input compiled and the fix introduced an error. |
| `CodeFixDoesNotChangeSolution` | Noise unless the input is valid, the diagnostic is real, and the offered fix should have changed that code. |
| `UnsupportedOperation` | Usually a harness limit. Do not change product code unless the action should have been a normal text edit. |

Ignore findings whose only problem is a diagnostic on incomplete or already-invalid code. Ignore format-string
seeds such as `class Foo{0}` that came from test-literal extraction. Do not fix a finding you cannot reproduce
with `repro` on current `master`.

Minimize `input.cs` until deleting any further token makes the finding disappear. Keep the trivia, settings,
and construct that matter.

## 4. Fix only confirmed bugs

Follow `fixing-a-bug`: a failing test on `master` comes before any product change. Put the test in the lowest
`SA####UnitTests` project whose compiler parses the snippet. For a crash, assert the non-crashing result. For
a code fix that breaks valid code, `VerifyCSharpFixAsync` must show the bad fixed text on `master`.

One bug per commit. Do not use `Fixes #4338`; that issue asks for the harness, not for an individual crash.
State the analyzer cost in the PR body. A code-fix-only change has no analyzer cost.

Do not weaken a test, suppress a finding, or change the harness so the signature disappears. If the harness
itself is wrong, fix the harness and say so separately from the product fix.

## 5. What not to automate

- Do not add this project to `StyleCopAnalyzers.slnx`.
- Do not add a CI job, scheduled workflow, or required check that runs `mutate` or `repro`.
- Do not open or close issues from findings unless the person you work for asked for that specific action.
- Do not commit campaign output.
