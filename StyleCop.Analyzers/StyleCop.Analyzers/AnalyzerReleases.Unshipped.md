; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA1141 | StyleCop.CSharp.ReadabilityRules | Warning | SA1141UseTupleSyntax, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1141.html)
SA1142 | StyleCop.CSharp.ReadabilityRules | Warning | SA1142ReferToTupleElementsByName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1142.html)
SA1316 | StyleCop.CSharp.NamingRules | Warning | SA1316TupleElementNamesShouldUseCorrectCasing, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1316.html)
SA1414 | StyleCop.CSharp.ReadabilityRules | Warning | SA1414TupleTypesInSignaturesShouldHaveElementNames, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1414.html)
SA1518 | StyleCop.CSharp.LayoutRules | Warning | SA1518UseLineEndingsCorrectlyAtEndOfFile, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1518.html)

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
SA1413 | StyleCop.CSharp.MaintainabilityRules | Warning | StyleCop.CSharp.ReadabilityRules | Warning | SA1413UseTrailingCommasInMultiLineInitializers, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1413.html)
