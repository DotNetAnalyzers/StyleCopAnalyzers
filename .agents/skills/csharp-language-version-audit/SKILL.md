---
name: csharp-language-version-audit
description: Procedure for auditing StyleCop.Analyzers against a new C# language version (C# 10 through 15 were done this way). Use when a new C# version or preview compiler ships, when creating the "C# N language feature audit" tracking issue and its "Review impact of <feature> on existing rules" sub-issues, when adding the StyleCop.Analyzers.Test.CSharpN project, when probing rules against new syntax, when closing audit sub-issues, or when a new feature exposes a Roslyn compiler or analyzer-driver bug.
---

# C# language version audit

An audit answers one question for every new language feature: does each existing SA rule (and its code fix)
handle the new syntax correctly? The output is a tracking issue, one sub-issue per feature, a test project for
the version, and a set of small PRs that each close (or partly address) a sub-issue.

Related skills: `lightup-and-roslyn-versions` (product code can't reference new Roslyn APIs),
`fixing-a-bug` (every finding is fixed test-first), `testing-and-coverage`, `pull-request-workflow`.

## 1. Enumerate the features

Use all three sources and reconcile them. Each one has missed something the others had.

1. What's new: `https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-N` (each heading has an
   anchor; the sub-issue links to it).
2. Roslyn's [Language Feature Status](https://raw.githubusercontent.com/dotnet/roslyn/main/docs/Language%20Feature%20Status.md)
   (features merged into the compiler for the version, including ones the docs don't cover yet).
3. csharplang proposals: `https://github.com/dotnet/csharplang/tree/main/proposals/csharp-N.0` (exact syntax and
   grammar; read these to write probe snippets).

Include features without new syntax (for example C# 14 span conversions, C# 13 `OverloadResolutionPriority`).
They still get a sub-issue; it can be closed after a probe shows no impact.

## 2. Create the label, tracking issue and sub-issues

Match the existing audits exactly (see #4046 for C# 14 and #4180 for C# 15).

- Label: `c# N`, color `5319e7` (same as `c# 7` through `c# 15`).
- Tracking issue: title `C# N language feature audit`, label `c# N`, empty body.
- Sub-issues: title `Review impact of <feature> on existing rules`, label `c# N`, body is only the what's-new
  link, for example `https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#union-types`.
- Attach each sub-issue to the tracker with GitHub's sub-issues API (not a task list in the body).

```bash
R=DotNetAnalyzers/StyleCopAnalyzers
gh label create "c# 16" --color 5319e7 -R $R
gh issue create -R $R --title "C# 16 language feature audit" --label "c# 16" --body ""
# for each feature:
url=$(gh issue create -R $R --label "c# 16" --title "Review impact of <feature> on existing rules" \
      --body "https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-16#<anchor>")
id=$(gh api "repos/$R/issues/${url##*/}" -q .id)          # the numeric id, not the issue number
gh api -X POST "repos/$R/issues/<tracker-number>/sub_issues" -F sub_issue_id=$id
```

Creating issues is visible to everyone; agree on the feature list with a maintainer first.

## 3. Add the test project for the version

Each `StyleCop.Analyzers.Test.CSharpN` project references a Roslyn that supports C# N and contains one
`partial` class per test class of the previous project, inheriting it, so every older test also runs under the
newer compiler and language version. Model the change on the C# 15 project (commits `fa4dc8c9d`, `e77b122a8`).

Checklist:

- [ ] `StyleCop.Analyzers/StyleCop.Analyzers.Test.CSharpN/StyleCop.Analyzers.Test.CSharpN.csproj`: copy N-1,
      bump `Microsoft.CodeAnalysis.CSharp.Workspaces`, add a `ProjectReference` to the N-1 test project (each
      project references every earlier one). Keep `net6.0` plus `net472` on Windows.
- [ ] `Properties/AssemblyInfo.cs` for the new project, and `InternalsVisibleTo("StyleCop.Analyzers.Test.CSharpN, PublicKey=...")`
      in all three of `StyleCop.Analyzers`, `StyleCop.Analyzers.CodeFixes` and `StyleCop.Analyzers.Test`.
- [ ] One stub per test class of the previous project. Stubs must keep the UTF-8 BOM (SA1412) and the file
      header. The private analyzer `SP0001` ("Expected test class '{0}' was not found") lists every missing
      stub when you build, and the `GeneratePartCodeRefactoringProvider` refactoring creates one in the IDE.
      A bash equivalent:

  ```bash
  N=16; M=$((N-1)); P=StyleCop.Analyzers
  for f in $(cd $P/StyleCop.Analyzers.Test.CSharp$M && find . -name "*CSharp${M}UnitTests.cs"); do
    dir=$(dirname "$f" | sed 's#^\./##'); [ "$dir" = . ] && dir=
    old=$(basename "$f" .cs); new=${old/CSharp$M/CSharp$N}
    ns=StyleCop.Analyzers.Test.CSharp$N${dir:+.${dir//\//.}}; base=StyleCop.Analyzers.Test.CSharp$M${dir:+.${dir//\//.}}
    mkdir -p "$P/StyleCop.Analyzers.Test.CSharp$N/$dir"
    printf '\xEF\xBB\xBF// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.\n// Licensed under the MIT License. See LICENSE in the project root for license information.\n\nnamespace %s\n{\n    using %s;\n\n    public partial class %s : %s\n    {\n    }\n}\n' \
      "$ns" "$base" "$new" "$old" > "$P/StyleCop.Analyzers.Test.CSharp$N/$dir/$new.cs"
  done
  ```

  Don't put `CSharpM` in the name of a non-test helper type in the previous project: the derived-test
  generator subclasses every type whose name contains that string.
- [ ] `StyleCopAnalyzers.sln` entry.
- [ ] `Lightup/LanguageVersionEx.cs`: `CSharpN = (LanguageVersion)N00`. `Lightup/LightupHelpers.cs`:
      `SupportsCSharpN` (checks `Enum.GetNames(typeof(LanguageVersion))`).
- [ ] `GenericAnalyzerTest.CreateDefaultReferenceAssemblies`: if the Roslyn major version is new, add it to the
      `codeAnalysisTestVersion` switch, and add an `if (LightupHelpers.SupportsCSharpN)` branch that picks the
      .NET reference assemblies shipped with C# N (support matrix:
      https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version). This is
      the precedent from 8eb1db36b ("Match default reference assemblies to default language version") and
      ed996672f (C# 12 → `Net80`); #4189/#4190 added C# 13 → `Net90`, C# 14+ → `Net100`. Keep a few tests
      pinned to `ReferenceAssemblies.NetFramework.Net472` where a feature can also target .NET Framework.
- [ ] `.github/workflows/build.yml`: an "Upload test binaries (C# N)" step in the build job and a matrix
      `include` entry in the test job. This adds two checks (Debug and Release); update the expected check
      count in `pull-request-workflow`.
- [ ] New syntax kinds as `SyntaxKindEx` constants with Roslyn's exact name and value (see
      `lightup-and-roslyn-versions`).
- [ ] Build the whole solution with zero warnings, and run the new project. Its failures must equal the N-1
      project's failures (compare sets; see `testing-and-coverage`). Some inherited expectations change on a
      newer compiler; make those base expectations `protected virtual` and override them in the new class
      (as `deef9d8c1` did for SA1600 CS9348).

### Preview versions: the default language version

While C# N is in preview, Roslyn has no `LanguageVersion.CSharpN` value, so `LightupHelpers.SupportsCSharpN`
is `false` even in the C# N test project, and tests silently run as C# N-1. #4192 fixed this for C# 15 with the
test-only `StyleCop.Analyzers.Test/Helpers/TestLanguageVersion.cs`: it detects the compiler by one of its
preview syntax kinds (`SyntaxKindEx.UnionDeclaration`) and returns `LanguageVersionEx.Preview` as the default
for both verifiers. For a new preview version, extend that helper with a feature that exists only in the new
compiler, and confirm it works: temporarily return `null` and check that the new-feature tests fail. When the
version ships, Roslyn adds the enum value and the detection can be removed.

## 4. Probe every rule against every feature

Use the probe harness in `scripts/probe` (this skill's folder). It loads the built StyleCop analyzers, runs all
of them (and optionally the first code fix for each diagnostic) on each snippet file, and prints diagnostics,
compiler errors, analyzer exceptions (AD0001), and fix results.

```bash
dotnet build StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/StyleCop.Analyzers.CodeFixes.csproj -c Debug
dotnet run --project .agents/skills/csharp-language-version-audit/scripts/probe -- --fix --lang preview snippets/
# probe with another compiler, for example a dotnet-tools feed build:
dotnet run --project .agents/skills/csharp-language-version-audit/scripts/probe -p:RoslynVersion=5.10.0-1.26477.9 -- snippets/
```

Keep snippet folders out of the repo (or in a scratch folder you don't commit).

For each feature write at least two snippets:

- **well-formatted** (`a-union.cs`): idiomatic, documented, correctly ordered code. Any diagnostic here is a
  possible false positive.
- **badly formatted** (`a2-union-bad.cs`): wrong spacing around every new token, bad ordering, missing docs,
  lower-case names, missing access modifiers. A missing diagnostic here is a false negative.

Put the feature in every position the grammar allows: top level, nested in a class, inside a namespace, with
attributes, generic, partial, with doc comments, with trivia and comments around new tokens.

Look for, per rule category: spacing SA1000–SA1028 (new tokens and keywords), readability SA1100–SA1142,
ordering SA1200–SA1217 (`<unknown>` in a message means a declaration kind is missing from `MemberNames` or the
ordering helpers), naming SA1300–SA1316, maintainability SA1400–SA1414, layout SA1500–SA1520, documentation
SA1600–SA1651 (are the new members treated as public?), and code fixes (`BAD-FIX` = new compiler errors, crash,
or the diagnostic still there after the fix).

The harness ignores noisy rules by default (SA1633, SA1600, SA1101, SA1413, ...); add `--keep SA1600,SA1101`
or a `// probe-keep: SA1600` line when the feature touches them. Compiler errors are normal for preview
features whose runtime types aren't in the reference set (for example unions report CS0518/CS0656); the
analyzers still run.

New declaration kinds: most type-level rules share `Helpers/SyntaxKinds.cs` (`BaseTypeDeclaration`,
`TypeDeclaration`). Adding the kind there (the way records and unions were added in #4193) covers ~30 rules
at once. Remove any per-rule registration of the same kind, or that rule runs twice.

Record each finding in the sub-issue before fixing (see the #4182 comment: "Fixed in #N" / "Still open,
blocked on Roslyn" lists).

## 5. Fix and test

- One PR per independent finding, test-first (`fixing-a-bug`). Product code uses light-up only.
- Tests go in `Test.CSharpN/<Category>/SA####CSharpNUnitTests.cs` when the code needs C# N; if the same bug
  reproduces with older syntax, put that test in the oldest project where it compiles.
- One commit per rule: `Update SA#### for <feature>` (or `Update SA#### tests for <feature>` when only tests).
- PR body: `Closes #<sub-issue>` only when the PR sweeps every rule for that feature; otherwise
  `Part of #<sub-issue>`. Add "Only changes tests and documentation." when no product code changed.
- Flag design decisions (for example, where a new member kind sorts in SA1201, or wording like SA1642's
  "union") with a ⚑ in the body and let a maintainer decide.
- A test-only PR that locks in current behavior is a valid outcome (#4151, #4164).

## 6. Close sub-issues with evidence

Close only after the evidence is merged or verified on current master, with a one-line comment:

- `Fixed by #4152 (merged).`
- `Covered by the tests in #4151 (merged); no product change was needed.`
- `Probed with the C# 15 test project: extension indexers in extension blocks are handled like other indexers
  by the ordering (SA1201/SA1202), spacing, layout and documentation rules. No changes needed.`

Close the tracker when every sub-issue is closed: `All sub-issues are closed, so this audit is complete.`
A PR that says `Part of #N` never closes anything automatically; check the remaining sub-issues after merges.

## 7. When the compiler is the bug

C# 15 unions showed the pattern (#4182, #4191). Roslyn 5.9.0 ran syntax node actions on every node inside a
union two or three times, so diagnostics were duplicated in the IDE and the testing library.

1. **Prove it outside StyleCop.** Write a minimal analyzer that counts callbacks per node and kind (and logs
   `context.ContainingSymbol`), run it with `CompilationWithAnalyzers` on a tiny snippet, and compare with a
   normal declaration (class or record). Note what is and isn't affected (node actions vs symbol, operation,
   syntax tree actions; command-line `csc` hides exact duplicates, the IDE does not).
2. **Find the upstream issue and fix commit.** Search dotnet/roslyn issues and PRs first; file a new issue only
   with maintainer approval. Record the fixing PR and commit.
3. **Find a build with the fix.** Roslyn CI builds are on the `dotnet-tools` feed (already in `NuGet.config`):
   `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json`. Re-run the minimal
   harness against candidate versions; don't trust version numbers (an SDK's 5.9 compiler can be older than a
   5.10 preview, and a newer SDK can still have the bug).
4. **Use that build only in the test project that needs it**, with a csproj comment naming the Roslyn issue and
   "move to <version> on nuget.org once it ships". Fix any NU1605 downgrade with a matching
   `PackageReference Update` in the same project.
5. **Write exactly-once tests** (a single expected diagnostic per location, which the verifier enforces), and
   check them against the buggy compiler: they must fail there with the duplicate counts (17 of the #4191 tests
   did). That proves the tests would catch a regression.
6. **Don't add analyzer version checks** or diagnostic de-duplication to product code. Version numbers don't
   reliably tell you whether a compiler has the fix, and the workaround costs every user for a bug that only
   some compilers have. If customers on the buggy compiler need a mitigation, propose it to the maintainers
   with its cost (see `analyzer-performance`) rather than adding it in a feature PR.

Other compiler quirks we hit: Roslyn 1.x doesn't run syntax node actions on partial method implementation
parts; Roslyn 5.x doesn't run them on the defining part of a partial constructor; Roslyn 4.0 reports some
positional-record diagnostics twice (dotnet/roslyn#53136). Expect version-dependent results and model them with
`virtual` expectations or `LightupHelpers.SupportsCSharpN` in tests, never in product logic.
