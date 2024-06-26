; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA1141 | StyleCop.CSharp.ReadabilityRules | Warning | SA1141UseTupleSyntax, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1141.md)
SA1142 | StyleCop.CSharp.ReadabilityRules | Warning | SA1142ReferToTupleElementsByName, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1142.md)
SA1316 | StyleCop.CSharp.NamingRules | Warning | SA1316TupleElementNamesShouldUseCorrectCasing, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1316.md)
SA1414 | StyleCop.CSharp.ReadabilityRules | Warning | SA1414TupleTypesInSignaturesShouldHaveElementNames, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1414.md)
SA1518 | StyleCop.CSharp.LayoutRules | Warning | SA1518UseLineEndingsCorrectlyAtEndOfFile, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1518.md)
SX1116 | StyleCop.CSharp.ReadabilityRules | Warning | SX1116SplitParametersMustStartOnLineAfterDeclaration, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1116.md)
SX1117 | StyleCop.CSharp.ReadabilityRules | Warning | SX1117ParametersMustBeOnSameLineOrSeparateLines, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1117.md)

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
SA1413 | StyleCop.CSharp.MaintainabilityRules | Warning | StyleCop.CSharp.ReadabilityRules | Warning | SA1413UseTrailingCommasInMultiLineInitializers, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1413.md)
