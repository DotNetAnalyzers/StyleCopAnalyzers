; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA1141 | StyleCop.CSharp.ReadabilityRules | Warning | SA1141UseTupleSyntax, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1141.html)
SA1142 | StyleCop.CSharp.ReadabilityRules | Warning | SA1142ReferToTupleElementsByName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1142.html)
SA1143 | StyleCop.CSharp.ReadabilityRules | Warning | SA1143ConditionalExpressionsMustBeFormattedConsistently, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1143.html)
SA1315 | StyleCop.CSharp.NamingRules | Warning | SA1315ParametersShouldMatchInheritedNames, [Documentation](../../documentation/SA1315.md)
SA1316 | StyleCop.CSharp.NamingRules | Warning | SA1316TupleElementNamesShouldUseCorrectCasing, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1316.html)
SA1317 | StyleCop.CSharp.NamingRules | Warning | SA1317IdentifierShouldBeNamedOnlyWithLatinLetters, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1317.html)
SA1414 | StyleCop.CSharp.ReadabilityRules | Warning | SA1414TupleTypesInSignaturesShouldHaveElementNames, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1414.html)
SX1412 | StyleCop.CSharp.MaintainabilityRules | Warning | SX1412StoreFilesAsUtf8WithoutByteOrderMark, [Documentation](../../documentation/SX1412.md)
SA1518 | StyleCop.CSharp.LayoutRules | Warning | SA1518UseLineEndingsCorrectlyAtEndOfFile, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1518.html)
SA1653 | StyleCop.CSharp.DocumentationRules | Warning | SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1653.html)
SA1654 | StyleCop.CSharp.DocumentationRules | Warning | SA1654ProperlyEncodeGenericBrackets, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1654.html)

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
SA1413 | StyleCop.CSharp.MaintainabilityRules | Warning | StyleCop.CSharp.ReadabilityRules | Warning | SA1413UseTrailingCommasInMultiLineInitializers, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1413.html)
