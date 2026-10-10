---
name: analyzer-performance
description: Performance rules for StyleCop.Analyzers analyzers and code fixes. Use when writing or reviewing any analyzer change (new registrations, new syntax kinds, semantic model calls, tree walks, caches), when choosing between doing work in the analyzer or the code fix, when tempted to add a compiler version check or diagnostic de-duplication, or when a PR claims a speed-up.
---

# Analyzer performance

StyleCop's analyzers run on every keystroke in the IDE and on every build, for every file. The maintainers do
not accept performance risk for correctness polish: a change that makes code without the affected construct
do more work needs a strong reason and numbers. State the cost in every PR body ("one extra `Kind()` check on
interpolation braces only, no measurable cost"; "the analyzer is unchanged, so there's no cost").

## Rules

1. **Register for the narrowest syntax kinds.** `context.RegisterSyntaxNodeAction(handler, SyntaxKind.X, ...)`
   only calls you for those nodes. Supporting a new construct usually means adding its kind to an existing
   registration (unions in #4191 added `SyntaxKindEx.UnionDeclaration`); code that doesn't use the construct
   pays nothing.
2. **Use the shared kind lists** in `Helpers/SyntaxKinds.cs` (`BaseTypeDeclaration`, `TypeDeclaration`) for
   type declarations, and don't also register the same kind per rule: a duplicate registration runs the rule
   twice per node (#4193 removed #4191's per-rule union lines for this reason).
3. **Use the settings-aware overloads** in `AnalyzerExtensions.cs`
   (`RegisterSyntaxNodeAction(Action<SyntaxNodeAnalysisContext, StyleCopSettings>, kinds)` on a
   `CompilationStartAnalysisContext`, `RegisterSyntaxTreeAction`), which read `stylecop.json` once and cache
   the settings per tree. Don't parse settings in a node callback.
4. **Token-based rules share one token walk.** Spacing and layout rules use `RegisterSyntaxTreeTokensAction`,
   which collects a tree's tokens once for all of them (#4106). Don't add another full-tree walk; plug into the
   shared one.
5. **Syntax before semantics.** Decide from syntax whenever possible. If a semantic check is needed, do the
   cheap syntactic filter first and call `SemanticModel` only for the few nodes that pass it. Prefer
   `GetDeclaredSymbol`/`GetSymbolInfo` on one node over walking symbols. Pass `context.CancellationToken`.
6. **Expensive or uncertain analysis goes in the code fix.** Fixes run only when the user asks. #4208's SA1312
   discard fix does data-flow and overload-resolution checks to decide whether a fix is safe; the analyzer
   didn't change.
7. **No compiler version checks and no de-duplication filters in product code** to work around compiler bugs.
   They run for every user on every node and can't reliably tell fixed and buggy compilers apart. Use a fixed
   compiler in tests instead (`csharp-language-version-audit`, section 7). If a mitigation for customers on a
   buggy compiler is really needed, propose it separately with its measured cost; #4191 shipped without one.
8. **Light-up accessors are created once.** `LightupHelpers.Create*Accessor` delegates are stored in static
   fields; never create them (or use reflection) inside a callback.
9. **Allocations matter.** Avoid LINQ, closures and `ToString()` of nodes or tokens in hot callbacks; compare
   `SyntaxKind`s, `ValueText`, and spans. Don't cache per-compilation data in static fields (memory leaks in
   the IDE); scope it to the `CompilationStartAnalysisContext`, as `SettingsHelper.GetOrCreateSettingsStorage`
   and `SyntaxTreeTokens.GetOrCreate` do.
10. **Keep `EnableConcurrentExecution()` and `ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)`**
    as the existing analyzers do (StyleCop doesn't analyze generated code).

## Reviewing a change for perf risk

- [ ] Which callbacks run more often than before? (New kinds in a registration, a new registration, a broader
      kind such as `IdentifierName` or every token.)
- [ ] For code that does **not** use the new construct, is the work identical to before?
- [ ] Any new `SemanticModel` call, symbol walk, or full tree walk in the analyzer? Can it move to the code fix?
- [ ] Any new per-callback allocation or reflection?
- [ ] Any shared state, cache or static field? What is its lifetime (compilation, tree, process)?

If any answer is "yes, more work", measure before proposing it.

## Measuring

- **StyleCopTester** (`StyleCop.Analyzers/StyleCopTester`, `net46`, Windows) loads a solution with
  MSBuildWorkspace and runs the analyzers, printing the elapsed time:

  ```
  StyleCopTester.exe /id:SA1513 path\to\Large.sln          # one rule
  StyleCopTester.exe /all /stats path\to\Large.sln         # all rules, plus node/token/trivia counts
  StyleCopTester.exe /editperf:Program /edititer:20 path\to\Large.sln   # incremental "editing" simulation
  ```

  Compare master and your branch on the same large solution (this repo's own `StyleCopAnalyzers.sln` works),
  several runs each, same machine, nothing else running.
- **Memory**: for caches or shared arrays, report allocation and working-set numbers, not only time. That was
  the follow-up requested for #4106's token-walk change ("the shared token arrays ... live for the
  compilation's lifetime, so allocation and working-set numbers ... would help").
- Put the numbers and the exact repro steps in the PR body so others can re-run them.

## Package size is a cost too

Changes to what ships in the package need numbers as well: #4173 (embedded PDBs) reported per-DLL growth
(StyleCop.Analyzers.dll +32.1%, total +399,872 B, +27%) and nupkg size before and after, and was accepted against
an explicit threshold set by the maintainer.
