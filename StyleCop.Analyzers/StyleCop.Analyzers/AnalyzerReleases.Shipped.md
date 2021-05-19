; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA0000 | StyleCop.CSharp.SpecialRules | Info | SA0000Roslyn7446Workaround
SA1000 | StyleCop.CSharp.SpacingRules | Warning | SA1000KeywordsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1000.md)
SA1001 | StyleCop.CSharp.SpacingRules | Warning | SA1001CommasMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1001.md)
SA1002 | StyleCop.CSharp.SpacingRules | Warning | SA1002SemicolonsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1002.md)
SA1004 | StyleCop.CSharp.SpacingRules | Warning | SA1004DocumentationLinesMustBeginWithSingleSpace
SA1005 | StyleCop.CSharp.SpacingRules | Warning | SA1005SingleLineCommentsMustBeginWithSingleSpace
SA1006 | StyleCop.CSharp.SpacingRules | Warning | SA1006PreprocessorKeywordsMustNotBePrecededBySpace
SA1007 | StyleCop.CSharp.SpacingRules | Warning | SA1007OperatorKeywordMustBeFollowedBySpace
SA1009 | StyleCop.CSharp.SpacingRules | Warning | SA1009ClosingParenthesisMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1009.md)
SA1010 | StyleCop.CSharp.SpacingRules | Warning | SA1010OpeningSquareBracketsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1010.md)
SA1011 | StyleCop.CSharp.SpacingRules | Warning | SA1011ClosingSquareBracketsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1011.md)
SA1012 | StyleCop.CSharp.SpacingRules | Warning | SA1012OpeningBracesMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1012.md)
SA1013 | StyleCop.CSharp.SpacingRules | Warning | SA1013ClosingBracesMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1013.md)
SA1014 | StyleCop.CSharp.SpacingRules | Warning | SA1014OpeningGenericBracketsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1014.md)
SA1015 | StyleCop.CSharp.SpacingRules | Warning | SA1015ClosingGenericBracketsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1015.md)
SA1016 | StyleCop.CSharp.SpacingRules | Warning | SA1016OpeningAttributeBracketsMustBeSpacedCorrectly
SA1017 | StyleCop.CSharp.SpacingRules | Warning | SA1017ClosingAttributeBracketsMustBeSpacedCorrectly
SA1018 | StyleCop.CSharp.SpacingRules | Warning | SA1018NullableTypeSymbolsMustNotBePrecededBySpace
SA1019 | StyleCop.CSharp.SpacingRules | Warning | SA1019MemberAccessSymbolsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1019.md)
SA1020 | StyleCop.CSharp.SpacingRules | Warning | SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1020.md)
SA1021 | StyleCop.CSharp.SpacingRules | Warning | SA1021NegativeSignsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1021.md)
SA1022 | StyleCop.CSharp.SpacingRules | Warning | SA1022PositiveSignsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1022.md)
SA1023 | StyleCop.CSharp.SpacingRules | Warning | SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1023.md)
SA1024 | StyleCop.CSharp.SpacingRules | Warning | SA1024ColonsMustBeSpacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1024.md)
SA1025 | StyleCop.CSharp.SpacingRules | Warning | SA1025CodeMustNotContainMultipleWhitespaceInARow
SA1026 | StyleCop.CSharp.SpacingRules | Warning | SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation
SA1027 | StyleCop.CSharp.SpacingRules | Warning | SA1027TabsMustNotBeUsed
SA1028 | StyleCop.CSharp.SpacingRules | Warning | SA1028CodeMustNotContainTrailingWhitespace
SA1100 | StyleCop.CSharp.ReadabilityRules | Warning | SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists
SA1101 | StyleCop.CSharp.ReadabilityRules | Warning | SA1101PrefixLocalCallsWithThis
SA1106 | StyleCop.CSharp.ReadabilityRules | Warning | SA1106CodeMustNotContainEmptyStatements
SA1107 | StyleCop.CSharp.ReadabilityRules | Warning | SA1107CodeMustNotContainMultipleStatementsOnOneLine
SA1108 | StyleCop.CSharp.ReadabilityRules | Warning | SA1108BlockStatementsMustNotContainEmbeddedComments
SA1110 | StyleCop.CSharp.ReadabilityRules | Warning | SA1110OpeningParenthesisMustBeOnDeclarationLine
SA1111 | StyleCop.CSharp.ReadabilityRules | Warning | SA1111ClosingParenthesisMustBeOnLineOfLastParameter
SA1112 | StyleCop.CSharp.ReadabilityRules | Warning | SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis
SA1113 | StyleCop.CSharp.ReadabilityRules | Warning | SA1113CommaMustBeOnSameLineAsPreviousParameter
SA1114 | StyleCop.CSharp.ReadabilityRules | Warning | SA1114ParameterListMustFollowDeclaration
SA1115 | StyleCop.CSharp.ReadabilityRules | Warning | SA1115ParameterMustFollowComma
SA1116 | StyleCop.CSharp.ReadabilityRules | Warning | SA1116SplitParametersMustStartOnLineAfterDeclaration
SA1117 | StyleCop.CSharp.ReadabilityRules | Warning | SA1117ParametersMustBeOnSameLineOrSeparateLines
SA1118 | StyleCop.CSharp.ReadabilityRules | Warning | SA1118ParameterMustNotSpanMultipleLines
SA1119 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1119StatementMustNotUseUnnecessaryParenthesis, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1119.md)
SA1120 | StyleCop.CSharp.ReadabilityRules | Warning | SA1120CommentsMustContainText
SA1121 | StyleCop.CSharp.ReadabilityRules | Warning | SA1121UseBuiltInTypeAlias
SA1122 | StyleCop.CSharp.ReadabilityRules | Warning | SA1122UseStringEmptyForEmptyStrings
SA1123 | StyleCop.CSharp.ReadabilityRules | Warning | SA1123DoNotPlaceRegionsWithinElements
SA1124 | StyleCop.CSharp.ReadabilityRules | Warning | SA1124DoNotUseRegions
SA1125 | StyleCop.CSharp.ReadabilityRules | Warning | SA1125UseShorthandForNullableTypes
SA1127 | StyleCop.CSharp.ReadabilityRules | Warning | SA1127GenericTypeConstraintsMustBeOnOwnLine
SA1128 | StyleCop.CSharp.ReadabilityRules | Warning | SA1128ConstructorInitializerMustBeOnOwnLine
SA1129 | StyleCop.CSharp.ReadabilityRules | Warning | SA1129DoNotUseDefaultValueTypeConstructor
SA1130 | StyleCop.CSharp.ReadabilityRules | Warning | SA1130UseLambdaSyntax
SA1131 | StyleCop.CSharp.ReadabilityRules | Warning | SA1131UseReadableConditions
SA1132 | StyleCop.CSharp.ReadabilityRules | Warning | SA1132DoNotCombineFields
SA1133 | StyleCop.CSharp.ReadabilityRules | Warning | SA1133DoNotCombineAttributes
SA1134 | StyleCop.CSharp.ReadabilityRules | Warning | SA1134AttributesMustNotShareLine
SA1200 | StyleCop.CSharp.OrderingRules | Warning | SA1200UsingDirectivesMustBePlacedCorrectly, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1200.md)
SA1201 | StyleCop.CSharp.OrderingRules | Warning | SA1201ElementsMustAppearInTheCorrectOrder
SA1202 | StyleCop.CSharp.OrderingRules | Warning | SA1202ElementsMustBeOrderedByAccess
SA1203 | StyleCop.CSharp.OrderingRules | Warning | SA1203ConstantsMustAppearBeforeFields
SA1204 | StyleCop.CSharp.OrderingRules | Warning | SA1204StaticElementsMustAppearBeforeInstanceElements
SA1205 | StyleCop.CSharp.OrderingRules | Warning | SA1205PartialElementsMustDeclareAccess, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1205.md)
SA1206 | StyleCop.CSharp.OrderingRules | Warning | SA1206DeclarationKeywordsMustFollowOrder, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1206.md)
SA1207 | StyleCop.CSharp.OrderingRules | Warning | SA1207ProtectedMustComeBeforeInternal, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1207.md)
SA1208 | StyleCop.CSharp.OrderingRules | Warning | SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1208.md)
SA1209 | StyleCop.CSharp.OrderingRules | Warning | SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1209.md)
SA1210 | StyleCop.CSharp.OrderingRules | Warning | SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1210.md)
SA1211 | StyleCop.CSharp.OrderingRules | Warning | SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1211.md)
SA1212 | StyleCop.CSharp.OrderingRules | Warning | SA1212PropertyAccessorsMustFollowOrder, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1212.md)
SA1213 | StyleCop.CSharp.OrderingRules | Warning | SA1213EventAccessorsMustFollowOrder, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1213.md)
SA1214 | StyleCop.CSharp.OrderingRules | Warning | SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements
SA1216 | StyleCop.CSharp.OrderingRules | Warning | SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1216.md)
SA1217 | StyleCop.CSharp.OrderingRules | Warning | SA1217UsingStaticDirectivesMustBeOrderedAlphabetically, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1217.md)
SA1300 | StyleCop.CSharp.NamingRules | Warning | SA1300ElementMustBeginWithUpperCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1300.md)
SA1302 | StyleCop.CSharp.NamingRules | Warning | SA1302InterfaceNamesMustBeginWithI, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1302.md)
SA1303 | StyleCop.CSharp.NamingRules | Warning | SA1303ConstFieldNamesMustBeginWithUpperCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1303.md)
SA1304 | StyleCop.CSharp.NamingRules | Warning | SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1304.md)
SA1305 | StyleCop.CSharp.NamingRules | Warning | SA1305FieldNamesMustNotUseHungarianNotation, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1305.md)
SA1306 | StyleCop.CSharp.NamingRules | Warning | SA1306FieldNamesMustBeginWithLowerCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1306.md)
SA1307 | StyleCop.CSharp.NamingRules | Warning | SA1307AccessibleFieldsMustBeginWithUpperCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1307.md)
SA1308 | StyleCop.CSharp.NamingRules | Warning | SA1308VariableNamesMustNotBePrefixed, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1308.md)
SA1309 | StyleCop.CSharp.NamingRules | Warning | SA1309FieldNamesMustNotBeginWithUnderscore, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1309.md)
SA1310 | StyleCop.CSharp.NamingRules | Warning | SA1310FieldNamesMustNotContainUnderscore, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1310.md)
SA1311 | StyleCop.CSharp.NamingRules | Warning | SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1311.md)
SA1312 | StyleCop.CSharp.NamingRules | Warning | SA1312VariableNamesMustBeginWithLowerCaseLetter
SA1313 | StyleCop.CSharp.NamingRules | Warning | SA1313ParameterNamesMustBeginWithLowerCaseLetter
SA1400 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1400AccessModifierMustBeDeclared, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1400.md)
SA1401 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1401FieldsMustBePrivate, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1401.md)
SA1402 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1402FileMayOnlyContainASingleClass, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1402.md)
SA1403 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1403FileMayOnlyContainASingleNamespace, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1403.md)
SA1404 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1404CodeAnalysisSuppressionMustHaveJustification, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1404.md)
SA1405 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1405DebugAssertMustProvideMessageText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1405.md)
SA1406 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1406DebugFailMustProvideMessageText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1406.md)
SA1407 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1407ArithmeticExpressionsMustDeclarePrecedence, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1407.md)
SA1408 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1408ConditionalExpressionsMustDeclarePrecedence, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1408.md)
SA1410 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1410RemoveDelegateParenthesisWhenPossible, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1410.md)
SA1411 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1411.md)
SA1412 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1412StoreFilesAsUtf8, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1412.md)
SA1500 | StyleCop.CSharp.LayoutRules | Warning | SA1500BracesForMultiLineStatementsMustNotShareLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1500.md)
SA1501 | StyleCop.CSharp.LayoutRules | Warning | SA1501StatementMustNotBeOnASingleLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1501.md)
SA1502 | StyleCop.CSharp.LayoutRules | Warning | SA1502ElementMustNotBeOnASingleLine
SA1503 | StyleCop.CSharp.LayoutRules | Warning | SA1503BracesMustNotBeOmitted, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1503.md)
SA1504 | StyleCop.CSharp.LayoutRules | Warning | SA1504AllAccessorsMustBeSingleLineOrMultiLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1504.md)
SA1505 | StyleCop.CSharp.LayoutRules | Warning | SA1505OpeningBracesMustNotBeFollowedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1505.md)
SA1506 | StyleCop.CSharp.LayoutRules | Warning | SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1506.md)
SA1507 | StyleCop.CSharp.LayoutRules | Warning | SA1507CodeMustNotContainMultipleBlankLinesInARow, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1507.md)
SA1508 | StyleCop.CSharp.LayoutRules | Warning | SA1508ClosingBracesMustNotBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1508.md)
SA1509 | StyleCop.CSharp.LayoutRules | Warning | SA1509OpeningBracesMustNotBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1509.md)
SA1510 | StyleCop.CSharp.LayoutRules | Warning | SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1510.md)
SA1511 | StyleCop.CSharp.LayoutRules | Warning | SA1511WhileDoFooterMustNotBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1511.md)
SA1512 | StyleCop.CSharp.LayoutRules | Warning | SA1512SingleLineCommentsMustNotBeFollowedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1512.md)
SA1513 | StyleCop.CSharp.LayoutRules | Warning | SA1513ClosingBraceMustBeFollowedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1513.md)
SA1514 | StyleCop.CSharp.LayoutRules | Warning | SA1514ElementDocumentationHeaderMustBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1514.md)
SA1515 | StyleCop.CSharp.LayoutRules | Warning | SA1515SingleLineCommentMustBePrecededByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1515.md)
SA1516 | StyleCop.CSharp.LayoutRules | Warning | SA1516ElementsMustBeSeparatedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1516.md)
SA1517 | StyleCop.CSharp.LayoutRules | Warning | SA1517CodeMustNotContainBlankLinesAtStartOfFile, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1517.md)
SA1519 | StyleCop.CSharp.LayoutRules | Warning | SA1519BracesMustNotBeOmittedFromMultiLineChildStatement, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1519.md)
SA1520 | StyleCop.CSharp.LayoutRules | Warning | SA1520UseBracesConsistently, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1520.md)
SA1600 | StyleCop.CSharp.DocumentationRules | Warning | SA1600ElementsMustBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1600.md)
SA1601 | StyleCop.CSharp.DocumentationRules | Warning | SA1601PartialElementsMustBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1601.md)
SA1602 | StyleCop.CSharp.DocumentationRules | Warning | SA1602EnumerationItemsMustBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1602.md)
SA1604 | StyleCop.CSharp.DocumentationRules | Warning | SA1604ElementDocumentationMustHaveSummary, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1604.md)
SA1605 | StyleCop.CSharp.DocumentationRules | Warning | SA1605PartialElementDocumentationMustHaveSummary, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1605.md)
SA1606 | StyleCop.CSharp.DocumentationRules | Warning | SA1606ElementDocumentationMustHaveSummaryText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1606.md)
SA1607 | StyleCop.CSharp.DocumentationRules | Warning | SA1607PartialElementDocumentationMustHaveSummaryText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1607.md)
SA1608 | StyleCop.CSharp.DocumentationRules | Warning | SA1608ElementDocumentationMustNotHaveDefaultSummary, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1608.md)
SA1609 | StyleCop.CSharp.DocumentationRules | Warning | SA1609PropertyDocumentationMustHaveValue, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1609.md)
SA1610 | StyleCop.CSharp.DocumentationRules | Warning | SA1610PropertyDocumentationMustHaveValueText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1610.md)
SA1611 | StyleCop.CSharp.DocumentationRules | Warning | SA1611ElementParametersMustBeDocumented
SA1612 | StyleCop.CSharp.DocumentationRules | Warning | SA1612ElementParameterDocumentationMustMatchElementParameters, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1612.md)
SA1613 | StyleCop.CSharp.DocumentationRules | Warning | SA1613ElementParameterDocumentationMustDeclareParameterName, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1613.md)
SA1614 | StyleCop.CSharp.DocumentationRules | Warning | SA1614ElementParameterDocumentationMustHaveText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1614.md)
SA1615 | StyleCop.CSharp.DocumentationRules | Warning | SA1615ElementReturnValueMustBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1615.md)
SA1616 | StyleCop.CSharp.DocumentationRules | Warning | SA1616ElementReturnValueDocumentationMustHaveText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1616.md)
SA1617 | StyleCop.CSharp.DocumentationRules | Warning | SA1617VoidReturnValueMustNotBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1617.md)
SA1618 | StyleCop.CSharp.DocumentationRules | Warning | SA1618GenericTypeParametersMustBeDocumented, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1618.md)
SA1619 | StyleCop.CSharp.DocumentationRules | Warning | SA1619GenericTypeParametersMustBeDocumentedPartialClass, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1619.md)
SA1620 | StyleCop.CSharp.DocumentationRules | Warning | SA1620GenericTypeParameterDocumentationMustMatchTypeParameters, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1620.md)
SA1621 | StyleCop.CSharp.DocumentationRules | Warning | SA1621GenericTypeParameterDocumentationMustDeclareParameterName, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1621.md)
SA1622 | StyleCop.CSharp.DocumentationRules | Warning | SA1622GenericTypeParameterDocumentationMustHaveText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1622.md)
SA1625 | StyleCop.CSharp.DocumentationRules | Warning | SA1625ElementDocumentationMustNotBeCopiedAndPasted, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1625.md)
SA1626 | StyleCop.CSharp.DocumentationRules | Warning | SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1626.md)
SA1627 | StyleCop.CSharp.DocumentationRules | Warning | SA1627DocumentationTextMustNotBeEmpty
SA1642 | StyleCop.CSharp.DocumentationRules | Warning | SA1642ConstructorSummaryDocumentationMustBeginWithStandardText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1642.md)
SA1643 | StyleCop.CSharp.DocumentationRules | Warning | SA1643DestructorSummaryDocumentationMustBeginWithStandardText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1643.md)
SA1648 | StyleCop.CSharp.DocumentationRules | Warning | SA1648InheritDocMustBeUsedWithInheritingClass, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1648.md)
SA1649 | StyleCop.CSharp.DocumentationRules | Warning | SA1649FileNameMustMatchTypeName
SA1651 | StyleCop.CSharp.DocumentationRules | Warning | SA1651DoNotUsePlaceholderElements, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1651.md)
SA1652 | StyleCop.CSharp.DocumentationRules | Warning | SA1652EnableXmlDocumentationOutput, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1652.md)
SX1101 | StyleCop.CSharp.ReadabilityRules | Warning | SX1101DoNotPrefixLocalMembersWithThis
SX1309 | StyleCop.CSharp.NamingRules | Warning | SX1309FieldNamesMustBeginWithUnderscore, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1309.md)
SX1309S | StyleCop.CSharp.NamingRules | Warning | SX1309SStaticFieldNamesMustBeginWithUnderscore, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1309S.md)

## Release 1.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA0001 | StyleCop.CSharp.SpecialRules | Warning | SA0001XmlCommentAnalysisDisabled, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0001.md)
SA0002 | StyleCop.CSharp.SpecialRules | Warning | SA0002InvalidSettingsFile, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA0002.md)
SA1136 | StyleCop.CSharp.ReadabilityRules | Warning | SA1136EnumValuesShouldBeOnSeparateLines
SA1137 | StyleCop.CSharp.ReadabilityRules | Warning | SA1137ElementsShouldHaveTheSameIndentation
SA1139 | StyleCop.CSharp.ReadabilityRules | Warning | SA1139UseLiteralSuffixNotationInsteadOfCasting
SA1314 | StyleCop.CSharp.NamingRules | Warning | SA1314TypeParameterNamesMustBeginWithT
SA1413 | StyleCop.CSharp.ReadabilityRules | Warning | SA1413UseTrailingCommasInMultiLineInitializers
SA1629 | StyleCop.CSharp.DocumentationRules | Warning | SA1629DocumentationTextMustEndWithAPeriod

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA0000 | StyleCop.CSharp.SpecialRules | Info | SA0000Roslyn7446Workaround
SA1516 | StyleCop.CSharp.LayoutRules | Warning | SA1516ElementsMustBeSeparatedByBlankLine, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1516.md)
SA1620 | StyleCop.CSharp.DocumentationRules | Warning | SA1620GenericTypeParameterDocumentationMustMatchTypeParameters, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1620.md)
SA1621 | StyleCop.CSharp.DocumentationRules | Warning | SA1621GenericTypeParameterDocumentationMustDeclareParameterName, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1621.md)
SA1622 | StyleCop.CSharp.DocumentationRules | Warning | SA1622GenericTypeParameterDocumentationMustHaveText, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1622.md)
SA1652 | StyleCop.CSharp.DocumentationRules | Warning | SA1652EnableXmlDocumentationOutput, [Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1652.md)
