# Contributing

If you want to contribute code you can get started by looking for issues marked as
[up for grabs](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/up%20for%20grabs).
We also have the [easy](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/easy) tag
for issues suitable if you are unfamiliar with roslyn.

You can also help by filing issues, participating in discussions and doing code review.

## Building prerequisites

* The latest version of Visual Studio 2026 (Community Edition or higher) is required for building this repository. Version 18.9 is the minimum, because the build uses the Roslyn 5.9 compiler and analyzers.
* The version of the [.NET SDK](https://dotnet.microsoft.com/download/dotnet) as specified in the global.json file at the root of this repo.
  Use the init script at the root of the repo to conveniently acquire and install the right version.

## Building documentation

The documentation site is built from the Markdown files in `documentation` using
[DocFX](https://dotnet.github.io/docfx/). From the repository root, run:

```powershell
dotnet tool restore
.\build\documentation.ps1
```

The generated site is written to `_site` (ignored by git). To preview it locally, run
`dotnet docfx serve _site` and open the URL printed by DocFX.
The English edition retains its existing URLs; the Russian edition is under `ru-ru/`.
The language dropdown in the header shows the current language and switches to the same page
in another edition. Languages are listed by their native names.
Each edition has its own navigation and search index. Russian search supports both Russian
and English terms, so rule identifiers and code terminology remain searchable.

Localized Markdown sources belong in `documentation/<locale>` (for example, `documentation/ru-ru`),
with the same filenames as the English sources. Keep all editions up to date when changing a page;
there is no automatic English fallback.
Each localized sidebar belongs in `documentation/<locale>/toc.yml` and its top navigation in
`documentation/<locale>/navbar/toc.yml`. Shared header behavior lives in
`build/docfx/common/public/main.js`, whose shared `locales` list defines each edition's native name,
URL prefix, language-picker label, and search languages; localized interface labels belong in
`build/docfx/<locale>/token.json`. All DocFX editions are deployed together in one Pages artifact.
When adding a language, add its DocFX configuration, wire its build and status-report translation into
`build/documentation.ps1`, and extend the shared locale list and localization tests.
The build script also translates the generated status report's rule titles using the Russian
resources and translates code-fix explanations, while preserving the implementation data and commit metadata.
It fails if a report entry lacks a translation.
Localization regression tests run with `node --test build/documentation-localization.test.cjs`
after building both editions.

The GitHub Build workflow also checks links in all repository Markdown and HTML files using
[Markup Link Checker (mlc)](https://github.com/becheran/mlc). Install version 1.2.2 from its
[releases](https://github.com/becheran/mlc/releases/tag/v1.2.2), then run `mlc` from the repository root.
The shared `.mlc.toml` configuration excludes git-ignored build output and allows Microsoft's
localized redirects without skipping URL validation. Runtime template URLs and links to generated
DocFX pages have targeted `mlc-disable-next-line` comments; do not exclude whole documentation directories.

The rule implementation status is a DocFX page at
<https://dotnetanalyzers.github.io/StyleCopAnalyzers/RuleStatus.html>.
`documentation/RuleStatus.md` includes the unframed HTML and script fragment in `docs/status.md`
using DocFX's `[!INCLUDE]` syntax, so it shares the documentation navigation and theme.
This page uses DocFX's `landing` layout to give the wide table the full content area while retaining
the shared header and footer; ordinary documentation pages retain their sidebars.
Keep the fragment's HTML in a continuous block without blank lines; otherwise Markdown may render
indented HTML as a code sample. The status table uses DocFX's Bootstrap styles, not a separate theme.
DocFX copies non-Markdown resources under `docs` to `_site/status`, including the JSON report
and a redirect from the old `status/index.html` URL.

The status page loads `status/StyleCop.Analyzers.Status.json` rather than AppVeyor.
CI generates this report from the Release build and downloads it into `docs` before the DocFX build.
To preview the status page locally, build the code fixes and generator in the same configuration, then
generate the report before compiling the documentation:

```powershell
dotnet build .\StyleCop.Analyzers\StyleCop.Analyzers.CodeFixes -c Release
dotnet build .\StyleCop.Analyzers\StyleCop.Analyzers.Status.Generator -c Release
dotnet .\StyleCop.Analyzers\StyleCop.Analyzers.Status.Generator\bin\Release\net10.0\StyleCop.Analyzers.Status.Generator.dll .\StyleCopAnalyzers.sln Release > .\docs\StyleCop.Analyzers.Status.json
```

The JSON report is an ignored build output. Status page regression tests run with
`node --test build\status-page.test.cjs`. Pages deployment waits for the documentation, generated-file checks,
and analyzer tests to succeed, so the displayed status describes a validated build.

When adding a page, link it from `documentation/toc.yml` or an existing rule-area page so it is discoverable.
The root `toc.yml` defines the short top navigation, while `documentation/toc.yml` defines the left sidebar.
The sidebar TOC is emitted under `_site/navigation` to keep it separate from the top navigation without
changing documentation page URLs.
The GitHub Build workflow compiles the site for pull requests and publishes it to
<https://dotnetanalyzers.github.io/StyleCopAnalyzers/> only on pushes to `master`.
In the repository's **Settings > Pages**, the publishing source must be set to **GitHub Actions**
instead of deployment from a branch. Deployment uses the `github-pages` environment; its protection
rules must allow the `master` branch.

## Test performance experiments

`build\test-performance.ps1` measures Release test execution separately from builds and coverage.
It records repeated wall-clock measurements, xUnit process CPU time and peak working set, test counts,
the revision, and machine configuration. Alternate repetitions reverse the candidate order.
For example, after building the C# 15 test project:

```powershell
.\build\test-performance.ps1 -Runner .\packages\xunit.runner.console.2.4.1\tools\net472\xunit.console.x86.exe `
    -LanguageVersions 15 -ThreadCounts default,4,16,64 -Repetitions 3 -OutputDirectory <artifact-directory>
```

xUnit 2 already parallelizes test classes by default, but not methods or theory rows within a class.
`-LanguageVersions 6,7,8,9,10,11,12,13,14,15 -Parallel all` also parallelizes compiler test assemblies.
The thread limit applies **per assembly**, not to the entire process. Use the 64-bit console runner
for the grouped experiment, since all assemblies and their Roslyn dependencies share a process.
Do not run builds or competing test experiments during a timing run.

The manual **Test performance experiment** workflow measures the single-assembly x86/x64 and grouped
x64 scenarios on `windows-latest`. Compare both wall time and total runner-minutes; fewer jobs can
consume less infrastructure while extending the critical path. These measurements omit coverage;
the regular Build workflow remains unchanged, including every compiler, configuration, and coverage job.
A proposed CI replacement also needs a coverage-enabled comparison before adoption. The manual
**Bounded test process experiment** compares one versus two separate x86 processes for the oldest and
newest suites. `build\test-performance-groups.ps1` accepts all ten language versions and a
`-MaxProcesses` limit to reproduce this scheduling strategy locally. Separate processes preserve
compiler dependency isolation and avoid loading every compiler into one large process.

The separate **TUnit performance experiment** generates TUnit entry points for the complete C# 15
suite without editing any original test body or xUnit assertion. To reproduce it:

```powershell
dotnet build .\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp15 -c Release -f net472 -m:1
dotnet build .\build\test-performance\Generate -c Release -m:1 -p:BuildProjectReferences=false
.\build\test-performance\Generate\bin\Release\net472\Generate.exe .\build\test-performance\TUnit\GeneratedTests.cs
dotnet build .\build\test-performance\TUnit -c Release -m:1 -p:BuildProjectReferences=false
.\build\test-performance.ps1 -Framework TUnit `
    -Runner .\build\test-performance\TUnit\bin\Release\net472\TestPerformance.TUnit.exe `
    -ThreadCounts default,4,16,64 -ExpectedTests 9752 -OutputDirectory <artifact-directory>
```

The pilot preserves data providers, optional parameters, inherited protected tests, and xUnit lifecycle
hooks. Culture-changing and explicitly sequential classes run exclusively. Most calls to original test
methods are direct; protected methods use reflection. It measures a scheduling experiment, not a completed
framework migration: source-generation build time is additional wrapper-build cost, not the net build-time
change of rewriting the original projects. TUnit's platform starts a worker process, so the script leaves
its CPU and memory columns unavailable rather than reporting the launcher's misleadingly low values.
Do not remove tests, change assertions, or infer hosted-runner performance from a many-core workstation.

### Initial measurements (October 10, 2026)

Release, .NET Framework 4.7.2, server GC, background GC disabled, no coverage. Values below are
median end-to-end seconds of three runs, with minimum and maximum in parentheses. The workstation
has 192 logical processors; the hosted Windows runners reported four. Every completed C# 15 run
passed all 9,752 cases, with matching per-method case counts in the TUnit pilot.

| Scenario | Workstation | Hosted Windows |
| --- | ---: | ---: |
| xUnit x86, default concurrency, C# 15 | 34.9 (33.9-37.9) | 76.3 (76.3-98.7) |
| xUnit x86, two threads, C# 15 | Not measured | 74.5 (74.1-75.7) |
| xUnit x64, default concurrency, C# 15 | 65.5 (63.5-78.7) | 121.2 (116.1-140.3) |
| TUnit x86, default concurrency, C# 15 | 22.7 (19.5-24.0) | 103.5 (102.3-138.1) |
| C# 6 + 15, separate x86 processes, serial | 66.5 (64.7-84.8) | 197.7 (193.7-228.8) |
| C# 6 + 15, separate x86 processes, two concurrent | 40.4 (32.6-52.0) | 183.8 (166.6-198.9) |

The local ten-process run passed all 88,004 cases in median 128.9 seconds (123.2-130.7).
Running all ten assemblies inside one x64 process took 325.1 seconds locally at four threads
per assembly (one completed sample; remaining repetitions stopped). The hosted default-concurrency
equivalent exhausted memory and produced incomplete results, so it is **rejected**, not a speedup.
The local xUnit class timings identify SA1121's 893 sequential cases as a critical path:
28.3 seconds of a 29.0-second test-execution baseline. More collection threads do not eliminate that tail.

The hosted TUnit result is slower than xUnit x86 despite its local improvement. Its additional
wrapper build took 29.6 seconds on that runner (12.6 seconds for the initial local build).
These are separate machines/runs, not a randomized same-runner framework comparison; initial
cold-cache outliers and local baseline drift are visible in the ranges. TUnit CPU/allocation savings
were not established. Do not adopt a migration or change normal CI based on the local timing alone.
The bounded-process experiment shows useful local scaling but only a modest hosted improvement
over serial execution; it does not demonstrate the same latency as two separate hosted machines.
Keep the current CI matrix and x86 runner until a coverage-enabled, same-runner comparison supports
a specific resource/latency tradeoff.

Raw XML/TRX, environment manifests, and CSVs are uploaded by the manual workflows:
[xUnit and all-assembly experiments](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/actions/runs/38087512468),
[TUnit pilot](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/actions/runs/38088281517), and
[bounded processes](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/actions/runs/38089391031).
Download the artifacts before their retention period expires if preserving raw evidence is required.

## Generated files

Everything under `StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated` is written by source generators during the
build and checked in so that changes to it can be reviewed:

* the syntax and operation lightup wrappers, generated by StyleCop.Analyzers.CodeGeneration from `Lightup/Syntax.xml` and
  `Lightup/OperationInterfaces.xml`
* the `*Resources.Designer.cs` accessors, generated from the `.resx` files

CI fails if these files are stale. After changing any of those inputs, the generators or the compiler version,
regenerate them from the root of the repo and commit the result:

```
git rm -r -q StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
dotnet build StyleCop.Analyzers/StyleCop.Analyzers/StyleCop.Analyzers.csproj --no-incremental
git add -A StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
```

Use `--no-incremental`: an incremental build that finds the project up to date does not run the generators, so the
deleted files would not be written again.

## Implementing a diagnostic

1. To start working on a diagnostic, add a comment to the issue indicating you are working on implementing it.

2. Add a new issue for a code fix for the diagnostic. For example, I added #171 when I worked on #6. Even if no code fix
   is possible, the issue is a place for discussions regarding possible corrections. Code fixes may, but do not have to
   be implemented alongside the diagnostic.

3. If a diagnostic or code fix is submitted without tests, it might be rejected. However, it may be accepted provided
   all of the following are true:

   1. The code is disabled by default, by passing `AnalyzerConstants.DisabledNoTests` for the `isEnabledByDefault`
      parameter when creating the `DiagnosticDescriptor`. It will be enabled by default only after tests are in place.
   2. A new issue was created for implementing tests for the item (e.g. #176).
   3. Evidence was given that the feature is currently operational, and the code appears to be a solid starting point
      for other contributors to continue the implementation effort.
