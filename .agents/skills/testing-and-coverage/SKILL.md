---
name: testing-and-coverage
description: How StyleCop.Analyzers tests are organized and run, and what "adequately tested" means here. Use when adding or changing tests, deciding which StyleCop.Analyzers.Test.CSharpN project a test belongs in, running one rule's tests locally, interpreting local failures on Linux versus Windows CI, writing exactly-once diagnostic tests, or meeting code coverage expectations from reviewers and Codecov.
---

# Testing and coverage

## Layout

| Project | Compiler (Microsoft.CodeAnalysis) | Default language version |
| --- | --- | --- |
| `StyleCop.Analyzers.Test` | 1.3.2 | C# 6 |
| `Test.CSharp7` / `8` / `9` | 2.8.2 / 3.6.0 / 3.8.0 | C# 7.x / 8 / 9 |
| `Test.CSharp10` / `11` / `12` | 4.0.1 / 4.4.0 / 4.8.0 | C# 10 / 11 / 12 |
| `Test.CSharp13` / `14` | 4.12.0 / 5.0.0 | C# 13 / 14 |
| `Test.CSharp15` | 5.10 preview (dotnet-tools feed) | preview (`TestLanguageVersion.Default`) |

Check the csproj files for current versions; this table is from Oct 2026.

- Every class in `Test.CSharpN` is `public partial class SA####CSharpNUnitTests : SA####CSharp(N-1)UnitTests`.
  All tests of earlier projects run again under each later compiler and language version. A test belongs in the
  **lowest** project whose compiler parses it.
- Version-dependent expectations: `protected virtual` members in the base class, overridden later (examples:
  `SA1504CSharp13/14`, `SA1130CSharp13`), or `LightupHelpers.SupportsCSharpN` inside the test.
- Reference assemblies follow the language version (`GenericAnalyzerTest.CreateDefaultReferenceAssemblies`:
  C# 13 → .NET 9, C# 14+ → .NET 10, older versions older frameworks). A test that must also cover .NET Framework
  sets `ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472` explicitly.
- Verifiers live in `StyleCop.Analyzers.Test/Verifiers` (`StyleCopDiagnosticVerifier<TAnalyzer>`,
  `StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>`, `CustomDiagnosticVerifier<TAnalyzer>`) on top of
  Microsoft.CodeAnalysis.Testing 1.1.4 with the framework-neutral `DefaultVerifier`. Don't add test-framework
  specific verifier packages (the xUnit flavor was discontinued after 1.1.2). Assertion failures surface as
  `InvalidOperationException` with the library's message.
- Tests are xUnit 2 `[Fact]`/`[Theory]`. Shared data: `Helpers/CommonMemberData.cs`. Settings: pass a
  `stylecop.json` string through the verifier overloads that take `settings`.

## Running tests locally

Both target frameworks build on Windows (`net6.0` and `net472`); on Linux and macOS only `net6.0` builds, and
running it needs the .NET 6 runtime in addition to the SDK in `global.json`.

```bash
# one rule, one project
dotnet build StyleCop.Analyzers/StyleCop.Analyzers.Test.CSharp15 -c Debug
dotnet test StyleCop.Analyzers/StyleCop.Analyzers.Test.CSharp15 --no-build -f net6.0 --filter "FullyQualifiedName~SA1201"
# lowest project for the same rule
dotnet test StyleCop.Analyzers/StyleCop.Analyzers.Test --no-build -f net6.0 --filter "FullyQualifiedName~SA1201"
```

- A test project build compiles every earlier project too. Building the C# 6 project and the newest one is
  usually enough locally; CI runs all ten in Debug and Release.
- Builds are CPU- and memory-heavy; avoid running several builds or test runs in parallel.
- A build rewrites files under `Lightup/.generated`; on Linux they then look modified only because of line
  endings. Don't commit them unless you meant to regenerate (`lightup-and-roslyn-versions`).

## Linux results versus Windows CI

On Linux, hundreds of code-fix tests fail in every project only because a fix inserts CRLF where the test
source has LF (`Iterative code fix application` and similar diffs; SA1516, SA1127, SA1500/SA1501, SA1514, SA1027
...). Windows CI (with `core.autocrlf true`) is the authority. Locally, **compare failing test sets**, never
totals:

```bash
dotnet test <project> --no-build -f net6.0 --filter "$FILTER" 2>&1 | sed -nE 's/^\s+Failed ([^ ]+).*/\1/p' | sort > after.txt
# same command on a master worktree -> before.txt
comm -13 before.txt after.txt      # new failures: each one must be explained
```

Read every new failure's message before calling it line-ending noise. If it isn't purely `\r\n` vs `\n`, it's
real (a C# 6 partial-method difference was misread this way once and broke CI).

## What adequate coverage means

Reviewers read the Codecov PR comment (patch coverage) and ask about uncovered lines (#4197). Codecov status
checks are disabled, so nothing turns red; you have to look. For every product change:

- [ ] A failing-then-passing test for the reported case (`fixing-a-bug`).
- [ ] **Both outcomes of every new condition**: the case that is now reported and the case that is now
      exempt. #4197 had only the failing `<include>` case; the review asked for the passing one, and 19 cases
      were added (included docs passing and failing, text that is only quotes or parentheses).
- [ ] Near misses: the closest code that must still be reported, and the closest that must stay quiet.
- [ ] Code fix: `VerifyCSharpFixAsync` with the fixed source (covers single fix and Fix All). Cases where the
      fix must **not** be offered: expect the diagnostic and pass the unchanged source as fixed source.
- [ ] Each syntactic position the change touches (top level, nested, namespace, generic, partial, with
      attributes or comments).
- [ ] Settings that change behavior, with `stylecop.json` in the test.
- [ ] Localized resources: if a message or code-fix title changed, the culture tests still pass
      (`SA1642` has per-culture tests, for example).
- [ ] New light-up helpers: tests in `StyleCop.Analyzers.Test/Lightup` (they run on every compiler).

## Exactly-once diagnostics

The verifier fails when a diagnostic is reported more times than expected, so a test that lists one expected
diagnostic per location also asserts "no duplicates". When a compiler bug duplicates callbacks (unions on Roslyn
5.9), write the expectation exactly once and confirm the test fails on the buggy compiler (see
`csharp-language-version-audit`, section 7). Never loosen an expectation to accept duplicates.

## Compiler diagnostics in test code

Snippets should compile. When a preview feature needs runtime types the reference assemblies lack (C# 15 unions
need `IUnion`/`UnionAttribute`, giving CS0518/CS0656), set `CompilerDiagnostics = CompilerDiagnostics.None` (or
expect the specific compiler diagnostic) and say why in a comment, as the C# 15 union tests do.

## Never

- Never skip, delete, `[Skip]`, or weaken an existing test to make a change pass. If an existing expectation is
  wrong, change it in its own commit and explain why in the PR (#4071's SA1208/SA1217 expectations were wrong
  and were corrected with an explanation).
- Never mark tests as Windows-only or Linux-only to hide line-ending failures.
- Never rely on local Linux totals as proof; CI's 10 Windows test jobs (Debug and Release) are the gate.
