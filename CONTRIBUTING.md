# Contributing

If you want to contribute code you can get started by looking for issues marked as
[up for grabs](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/up%20for%20grabs).
We also have the [easy](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/labels/easy) tag
for issues suitable if you are unfamiliar with roslyn.

You can also help by filing issues, participating in discussions and doing code review.

## Building prerequisites

* Visual Studio is optional. For IDE builds on Windows, use Visual Studio 2026 (Community Edition or higher),
  version 18.9 or newer, because the build uses the Roslyn 5.9 compiler and analyzers.
* The version of the [.NET SDK](https://dotnet.microsoft.com/download/dotnet) as specified in the global.json file at the root of this repo.
  Use the init script at the root of the repo to conveniently acquire and install the right version.

The full solution builds with the .NET SDK on Windows and Linux:

```powershell
dotnet restore StyleCopAnalyzers.sln
dotnet build StyleCopAnalyzers.sln --no-restore -c Release @Directory.Build.rsp
```

`Directory.Build.rsp` sets `/m:1` because the reference assembly annotator task does not support
parallel builds. MSBuild discovers this file automatically, but `dotnet build` overrides it
with its own parallel-build switch. Pass `@Directory.Build.rsp` explicitly as shown above to
keep SDK builds serial too.
The GitHub workflow builds Debug and Release on Linux. The legacy `StyleCopTester` utility
can be compiled on Linux using reference assemblies, but running it still requires Windows
and .NET Framework because it uses WPF.

The shipped analyzers and code fixes still target `netstandard1.1` on both build hosts.
The package contains compiler-loaded assemblies under `analyzers/dotnet/cs`, not separate
`lib/net472` or `lib/net10.0` assets. Test framework selection does not change its target
frameworks, dependencies, or supported analyzer hosts.

`.gitattributes` fixes the line endings of packaged text assets to CRLF, preserving the existing
Windows-built payload even when the rest of the source checkout uses LF. It also fixes resource
and shipped C# source line endings so multiline localized values and verbatim string literals
remain identical across hosts.

## Running analyzer tests

The GitHub Build workflow builds and runs all C# 6 through C# 15 test assemblies
in Debug and Release on Linux using their `net10.0` target. Local Windows builds also compile
the `net472` target, but the Linux workflow does not build or execute .NET Framework tests.
To run tests locally on either platform, install the SDK from `global.json` and the
.NET 10 runtime, then run (replace the project and configuration as needed):

```powershell
dotnet build .\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp15 -c Debug
dotnet test .\StyleCop.Analyzers\StyleCop.Analyzers.Test.CSharp15 --no-build -c Debug -f net10.0
```

Use forward slashes in these paths when running from a Linux shell. Add
`--filter "FullyQualifiedName~SA1201"` to select one rule.
Each newer compiler's test project also runs the tests inherited from earlier projects.

`.gitattributes` checks out test C# sources with CRLF on every platform so multiline
source/expected-output literals match the verifier's explicit CRLF formatting option. Assertions still compare
the exact output; tests for other newline sequences supply them explicitly.
After changing these attributes in an existing checkout, refresh unchanged test sources
(or use a fresh checkout) before rebuilding. Preserve any local edits.

CI instruments tests with the manifest-pinned `coverlet.console` tool and merges the
Debug Cobertura reports using `reportgenerator`. `dotnet tool restore` restores these
tools along with DocFX; ReportGenerator also needs the .NET 8 runtime. Test-result
artifacts contain VSTest TRX reports.

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
