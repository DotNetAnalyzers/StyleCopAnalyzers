---
name: lightup-and-roslyn-versions
description: How StyleCop.Analyzers product code supports new C# syntax while compiling against Roslyn 1.2.1 ("light-up"). Use when product code needs a new SyntaxKind, syntax node, operation, symbol API or language version check; when refreshing Lightup/Syntax.xml; when the "Check generated files" CI job fails; or when deciding between light-up and a compiler version check.
---

# Light-up and Roslyn versions

The shipped analyzers (`StyleCop.Analyzers`, `StyleCop.Analyzers.CodeFixes`) build against
**Microsoft.CodeAnalysis 1.2.1** and target `netstandard1.1`, so they load in every compiler from VS 2015 on.
Product code can never reference a newer Roslyn API directly. Tests use newer compilers (see
`testing-and-coverage`). "Light-up" is how product code reaches newer APIs at run time when they exist.

## The tools, in order of preference

1. **`Lightup/SyntaxKindEx.cs`**: new syntax kinds as constants with Roslyn's exact name and numeric value, for
   example `public const SyntaxKind UnionDeclaration = (SyntaxKind)9082;`. Get the value from Roslyn's
   `SyntaxKind.cs` at the commit of the compiler you test with. `SyntaxKindExUnitTests` (inherited by every test
   project) checks each constant against the real enum in that project's Roslyn, so a wrong value fails CI.
   Same pattern: `SymbolKindEx`, `MethodKindEx`, `LanguageVersionEx`, `SymbolDisplay*OptionsEx`.
2. **A cast to an existing base type.** Many new nodes derive from types Roslyn 1.2.1 already has: an extension
   block or union is a `TypeDeclarationSyntax`, so `Modifiers`, `Identifier`, `Members` and `OpenBraceToken`
   work after checking `Kind()`. Prefer this to new wrappers.
3. **Generated wrappers** (`*SyntaxWrapper`, `I*OperationWrapper`) produced by `StyleCop.Analyzers.CodeGeneration`
   from `Lightup/Syntax.xml` and `Lightup/OperationInterfaces.xml`. Use `XSyntaxWrapper.IsInstance(node)` and
   `(XSyntaxWrapper)node`; missing properties return defaults on old compilers.
4. **Hand-written extension helpers** in `Lightup/*Extensions.cs` (for example
   `ParenthesizedLambdaExpressionSyntaxExtensions`, `IPropertySymbolExtensions`) built on
   `LightupHelpers.CreateSyntaxPropertyAccessor` (also used for symbol properties) and its siblings. Name them
   exactly like the
   Roslyn API, so a generated wrapper can replace them later.

Never hand-edit anything under `StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated`.

## Language version checks

- `LightupHelpers.SupportsCSharpN` says whether the *running compiler* knows C# N, not whether the code being
  analyzed uses it. Use it in **tests** to pick expectations (for example `LightupHelpers.SupportsCSharp7 ? ... : ...`
  in `SA1601UnitTests`, `SA1130UnitTests`).
- In product code, decide from the syntax itself (a kind or token is present). When the meaning of the same
  code depends on the language version of the code being analyzed, add a method to
  `Helpers/LanguageFeatureHelpers.cs` that compares the parse options' `LanguageVersion` with
  `LanguageVersionEx.CSharpN` (for example `SupportsUsingAliasToAnyType`, `SupportsNativeSizedIntegers`).
- Don't gate behavior on compiler *version numbers* (assembly versions, `typeof(Compilation).Assembly`
  versions) to work around compiler bugs. Builds with and without a fix share version prefixes; the C# 15 union
  duplicate-callback bug was present in SDK 10.0.401's 5.9 compiler and fixed in a 5.10 preview
  (see `csharp-language-version-audit`, section 7).

## Refreshing Syntax.xml (new wrappers)

Needed when a feature adds node types or properties that a rule must read and a base-type cast can't reach.

1. Take `src/Compilers/CSharp/Portable/Syntax/Syntax.xml` from dotnet/roslyn **at the commit of the
   Microsoft.CodeAnalysis.CSharp package you test with** (the commit is in the package's nuspec `repository`
   element), and copy it verbatim over `StyleCop.Analyzers/StyleCop.Analyzers/Lightup/Syntax.xml`.
   #4124 did this for 5.9.0 (dotnet/roslyn@35d9211b8).
2. Regenerate (below), build with zero warnings, and run the Lightup tests in every test project
   (`--filter "FullyQualifiedName~Lightup"`). Each new wrapper needs `VerifyThatWrapperClassIsPresent` coverage.
3. Do this in its own PR, before feature PRs that need the wrappers. Watch the generator for recursion between
   overridden members and wrapper types (Björn's `43a238963` fix; covered by tests in #4124).

## Generated files and the "Check generated files" CI job

`Lightup/.generated` holds the generated wrappers plus the `*Resources.Designer.cs` files generated from every
`.resx`. They're committed for review, and CI fails when they're stale. Typical causes: an edited `.resx` (a new
message, code fix title or localized string), an edited `Syntax.xml`/`OperationInterfaces.xml`, or a compiler or
generator update. Regenerate from the repo root:

```bash
git rm -r -q StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
dotnet build StyleCop.Analyzers/StyleCop.Analyzers/StyleCop.Analyzers.csproj --no-incremental
git add -A StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
git diff --cached --stat   # only the files you expected
```

`--no-incremental` is required: an up-to-date incremental build doesn't run the generators. On Linux, many
generated files show as modified (` M`) in `git status` only because of line endings; `git diff` with the `text`
attribute is clean and those files must not be committed. Never `git add -A` the whole repo after a build.

Run the same check CI runs before pushing:

```bash
git rm -r -q StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
dotnet build StyleCop.Analyzers/StyleCop.Analyzers/StyleCop.Analyzers.csproj -c Debug --no-incremental -maxcpucount:1
git -c core.safecrlf=false add --all --intent-to-add -- StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
git -c core.safecrlf=false diff HEAD --exit-code --stat -- StyleCop.Analyzers/StyleCop.Analyzers/Lightup/.generated
```

Exit code 0 means up to date. Then restore your index (`git reset -q`) if you don't intend to commit.

## Checklist for a product change that touches new syntax

- [ ] Builds against Roslyn 1.2.1 (the normal solution build proves this; CI builds Debug and Release).
- [ ] New kinds are `SyntaxKindEx` constants with exact Roslyn names and values.
- [ ] No new direct references to Roslyn APIs newer than 1.2.1, no reflection outside `LightupHelpers`.
- [ ] Shared kind lists (`Helpers/SyntaxKinds.cs`) updated instead of per-rule copies when the new kind is a
      type declaration (#4193).
- [ ] Regenerated `.generated` files committed when `.resx` or XML inputs changed.
- [ ] Light-up helpers have tests in `StyleCop.Analyzers.Test/Lightup` (they run on every Roslyn).
