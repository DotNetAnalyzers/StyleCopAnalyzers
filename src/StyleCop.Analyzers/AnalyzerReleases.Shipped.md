; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA0000 | StyleCop.CSharp.SpecialRules | Info | SA0000Roslyn7446Workaround
SA1000 | StyleCop.CSharp.SpacingRules | Warning | SA1000KeywordsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1000.html)
SA1001 | StyleCop.CSharp.SpacingRules | Warning | SA1001CommasMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1001.html)
SA1002 | StyleCop.CSharp.SpacingRules | Warning | SA1002SemicolonsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1002.html)
SA1004 | StyleCop.CSharp.SpacingRules | Warning | SA1004DocumentationLinesMustBeginWithSingleSpace
SA1005 | StyleCop.CSharp.SpacingRules | Warning | SA1005SingleLineCommentsMustBeginWithSingleSpace
SA1006 | StyleCop.CSharp.SpacingRules | Warning | SA1006PreprocessorKeywordsMustNotBePrecededBySpace
SA1007 | StyleCop.CSharp.SpacingRules | Warning | SA1007OperatorKeywordMustBeFollowedBySpace
SA1009 | StyleCop.CSharp.SpacingRules | Warning | SA1009ClosingParenthesisMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1009.html)
SA1010 | StyleCop.CSharp.SpacingRules | Warning | SA1010OpeningSquareBracketsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1010.html)
SA1011 | StyleCop.CSharp.SpacingRules | Warning | SA1011ClosingSquareBracketsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1011.html)
SA1012 | StyleCop.CSharp.SpacingRules | Warning | SA1012OpeningBracesMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1012.html)
SA1013 | StyleCop.CSharp.SpacingRules | Warning | SA1013ClosingBracesMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1013.html)
SA1014 | StyleCop.CSharp.SpacingRules | Warning | SA1014OpeningGenericBracketsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1014.html)
SA1015 | StyleCop.CSharp.SpacingRules | Warning | SA1015ClosingGenericBracketsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1015.html)
SA1016 | StyleCop.CSharp.SpacingRules | Warning | SA1016OpeningAttributeBracketsMustBeSpacedCorrectly
SA1017 | StyleCop.CSharp.SpacingRules | Warning | SA1017ClosingAttributeBracketsMustBeSpacedCorrectly
SA1018 | StyleCop.CSharp.SpacingRules | Warning | SA1018NullableTypeSymbolsMustNotBePrecededBySpace
SA1019 | StyleCop.CSharp.SpacingRules | Warning | SA1019MemberAccessSymbolsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1019.html)
SA1020 | StyleCop.CSharp.SpacingRules | Warning | SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1020.html)
SA1021 | StyleCop.CSharp.SpacingRules | Warning | SA1021NegativeSignsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1021.html)
SA1022 | StyleCop.CSharp.SpacingRules | Warning | SA1022PositiveSignsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1022.html)
SA1023 | StyleCop.CSharp.SpacingRules | Warning | SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1023.html)
SA1024 | StyleCop.CSharp.SpacingRules | Warning | SA1024ColonsMustBeSpacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1024.html)
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
SA1119 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1119StatementMustNotUseUnnecessaryParenthesis, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1119.html)
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
SA1200 | StyleCop.CSharp.OrderingRules | Warning | SA1200UsingDirectivesMustBePlacedCorrectly, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1200.html)
SA1201 | StyleCop.CSharp.OrderingRules | Warning | SA1201ElementsMustAppearInTheCorrectOrder
SA1202 | StyleCop.CSharp.OrderingRules | Warning | SA1202ElementsMustBeOrderedByAccess
SA1203 | StyleCop.CSharp.OrderingRules | Warning | SA1203ConstantsMustAppearBeforeFields
SA1204 | StyleCop.CSharp.OrderingRules | Warning | SA1204StaticElementsMustAppearBeforeInstanceElements
SA1205 | StyleCop.CSharp.OrderingRules | Warning | SA1205PartialElementsMustDeclareAccess, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1205.html)
SA1206 | StyleCop.CSharp.OrderingRules | Warning | SA1206DeclarationKeywordsMustFollowOrder, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1206.html)
SA1207 | StyleCop.CSharp.OrderingRules | Warning | SA1207ProtectedMustComeBeforeInternal, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1207.html)
SA1208 | StyleCop.CSharp.OrderingRules | Warning | SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1208.html)
SA1209 | StyleCop.CSharp.OrderingRules | Warning | SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1209.html)
SA1210 | StyleCop.CSharp.OrderingRules | Warning | SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1210.html)
SA1211 | StyleCop.CSharp.OrderingRules | Warning | SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1211.html)
SA1212 | StyleCop.CSharp.OrderingRules | Warning | SA1212PropertyAccessorsMustFollowOrder, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1212.html)
SA1213 | StyleCop.CSharp.OrderingRules | Warning | SA1213EventAccessorsMustFollowOrder, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1213.html)
SA1214 | StyleCop.CSharp.OrderingRules | Warning | SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements
SA1216 | StyleCop.CSharp.OrderingRules | Warning | SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1216.html)
SA1217 | StyleCop.CSharp.OrderingRules | Warning | SA1217UsingStaticDirectivesMustBeOrderedAlphabetically, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1217.html)
SA1300 | StyleCop.CSharp.NamingRules | Warning | SA1300ElementMustBeginWithUpperCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1300.html)
SA1302 | StyleCop.CSharp.NamingRules | Warning | SA1302InterfaceNamesMustBeginWithI, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1302.html)
SA1303 | StyleCop.CSharp.NamingRules | Warning | SA1303ConstFieldNamesMustBeginWithUpperCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1303.html)
SA1304 | StyleCop.CSharp.NamingRules | Warning | SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1304.html)
SA1305 | StyleCop.CSharp.NamingRules | Warning | SA1305FieldNamesMustNotUseHungarianNotation, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1305.html)
SA1306 | StyleCop.CSharp.NamingRules | Warning | SA1306FieldNamesMustBeginWithLowerCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1306.html)
SA1307 | StyleCop.CSharp.NamingRules | Warning | SA1307AccessibleFieldsMustBeginWithUpperCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1307.html)
SA1308 | StyleCop.CSharp.NamingRules | Warning | SA1308VariableNamesMustNotBePrefixed, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1308.html)
SA1309 | StyleCop.CSharp.NamingRules | Warning | SA1309FieldNamesMustNotBeginWithUnderscore, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1309.html)
SA1310 | StyleCop.CSharp.NamingRules | Warning | SA1310FieldNamesMustNotContainUnderscore, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1310.html)
SA1311 | StyleCop.CSharp.NamingRules | Warning | SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1311.html)
SA1312 | StyleCop.CSharp.NamingRules | Warning | SA1312VariableNamesMustBeginWithLowerCaseLetter
SA1313 | StyleCop.CSharp.NamingRules | Warning | SA1313ParameterNamesMustBeginWithLowerCaseLetter
SA1400 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1400AccessModifierMustBeDeclared, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1400.html)
SA1401 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1401FieldsMustBePrivate, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1401.html)
SA1402 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1402FileMayOnlyContainASingleClass, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1402.html)
SA1403 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1403FileMayOnlyContainASingleNamespace, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1403.html)
SA1404 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1404CodeAnalysisSuppressionMustHaveJustification, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1404.html)
SA1405 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1405DebugAssertMustProvideMessageText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1405.html)
SA1406 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1406DebugFailMustProvideMessageText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1406.html)
SA1407 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1407ArithmeticExpressionsMustDeclarePrecedence, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1407.html)
SA1408 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1408ConditionalExpressionsMustDeclarePrecedence, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1408.html)
SA1410 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1410RemoveDelegateParenthesisWhenPossible, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1410.html)
SA1411 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1411.html)
SA1412 | StyleCop.CSharp.MaintainabilityRules | Warning | SA1412StoreFilesAsUtf8, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1412.html)
SA1500 | StyleCop.CSharp.LayoutRules | Warning | SA1500BracesForMultiLineStatementsMustNotShareLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1500.html)
SA1501 | StyleCop.CSharp.LayoutRules | Warning | SA1501StatementMustNotBeOnASingleLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1501.html)
SA1502 | StyleCop.CSharp.LayoutRules | Warning | SA1502ElementMustNotBeOnASingleLine
SA1503 | StyleCop.CSharp.LayoutRules | Warning | SA1503BracesMustNotBeOmitted, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1503.html)
SA1504 | StyleCop.CSharp.LayoutRules | Warning | SA1504AllAccessorsMustBeSingleLineOrMultiLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1504.html)
SA1505 | StyleCop.CSharp.LayoutRules | Warning | SA1505OpeningBracesMustNotBeFollowedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1505.html)
SA1506 | StyleCop.CSharp.LayoutRules | Warning | SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1506.html)
SA1507 | StyleCop.CSharp.LayoutRules | Warning | SA1507CodeMustNotContainMultipleBlankLinesInARow, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1507.html)
SA1508 | StyleCop.CSharp.LayoutRules | Warning | SA1508ClosingBracesMustNotBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1508.html)
SA1509 | StyleCop.CSharp.LayoutRules | Warning | SA1509OpeningBracesMustNotBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1509.html)
SA1510 | StyleCop.CSharp.LayoutRules | Warning | SA1510ChainedStatementBlocksMustNotBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1510.html)
SA1511 | StyleCop.CSharp.LayoutRules | Warning | SA1511WhileDoFooterMustNotBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1511.html)
SA1512 | StyleCop.CSharp.LayoutRules | Warning | SA1512SingleLineCommentsMustNotBeFollowedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1512.html)
SA1513 | StyleCop.CSharp.LayoutRules | Warning | SA1513ClosingBraceMustBeFollowedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1513.html)
SA1514 | StyleCop.CSharp.LayoutRules | Warning | SA1514ElementDocumentationHeaderMustBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1514.html)
SA1515 | StyleCop.CSharp.LayoutRules | Warning | SA1515SingleLineCommentMustBePrecededByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1515.html)
SA1516 | StyleCop.CSharp.LayoutRules | Warning | SA1516ElementsMustBeSeparatedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1516.html)
SA1517 | StyleCop.CSharp.LayoutRules | Warning | SA1517CodeMustNotContainBlankLinesAtStartOfFile, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1517.html)
SA1519 | StyleCop.CSharp.LayoutRules | Warning | SA1519BracesMustNotBeOmittedFromMultiLineChildStatement, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1519.html)
SA1520 | StyleCop.CSharp.LayoutRules | Warning | SA1520UseBracesConsistently, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1520.html)
SA1600 | StyleCop.CSharp.DocumentationRules | Warning | SA1600ElementsMustBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1600.html)
SA1601 | StyleCop.CSharp.DocumentationRules | Warning | SA1601PartialElementsMustBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1601.html)
SA1602 | StyleCop.CSharp.DocumentationRules | Warning | SA1602EnumerationItemsMustBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1602.html)
SA1604 | StyleCop.CSharp.DocumentationRules | Warning | SA1604ElementDocumentationMustHaveSummary, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1604.html)
SA1605 | StyleCop.CSharp.DocumentationRules | Warning | SA1605PartialElementDocumentationMustHaveSummary, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1605.html)
SA1606 | StyleCop.CSharp.DocumentationRules | Warning | SA1606ElementDocumentationMustHaveSummaryText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1606.html)
SA1607 | StyleCop.CSharp.DocumentationRules | Warning | SA1607PartialElementDocumentationMustHaveSummaryText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1607.html)
SA1608 | StyleCop.CSharp.DocumentationRules | Warning | SA1608ElementDocumentationMustNotHaveDefaultSummary, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1608.html)
SA1609 | StyleCop.CSharp.DocumentationRules | Warning | SA1609PropertyDocumentationMustHaveValue, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1609.html)
SA1610 | StyleCop.CSharp.DocumentationRules | Warning | SA1610PropertyDocumentationMustHaveValueText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1610.html)
SA1611 | StyleCop.CSharp.DocumentationRules | Warning | SA1611ElementParametersMustBeDocumented
SA1612 | StyleCop.CSharp.DocumentationRules | Warning | SA1612ElementParameterDocumentationMustMatchElementParameters, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1612.html)
SA1613 | StyleCop.CSharp.DocumentationRules | Warning | SA1613ElementParameterDocumentationMustDeclareParameterName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1613.html)
SA1614 | StyleCop.CSharp.DocumentationRules | Warning | SA1614ElementParameterDocumentationMustHaveText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1614.html)
SA1615 | StyleCop.CSharp.DocumentationRules | Warning | SA1615ElementReturnValueMustBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1615.html)
SA1616 | StyleCop.CSharp.DocumentationRules | Warning | SA1616ElementReturnValueDocumentationMustHaveText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1616.html)
SA1617 | StyleCop.CSharp.DocumentationRules | Warning | SA1617VoidReturnValueMustNotBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1617.html)
SA1618 | StyleCop.CSharp.DocumentationRules | Warning | SA1618GenericTypeParametersMustBeDocumented, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1618.html)
SA1619 | StyleCop.CSharp.DocumentationRules | Warning | SA1619GenericTypeParametersMustBeDocumentedPartialClass, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1619.html)
SA1620 | StyleCop.CSharp.DocumentationRules | Warning | SA1620GenericTypeParameterDocumentationMustMatchTypeParameters, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1620.html)
SA1621 | StyleCop.CSharp.DocumentationRules | Warning | SA1621GenericTypeParameterDocumentationMustDeclareParameterName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1621.html)
SA1622 | StyleCop.CSharp.DocumentationRules | Warning | SA1622GenericTypeParameterDocumentationMustHaveText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1622.html)
SA1625 | StyleCop.CSharp.DocumentationRules | Warning | SA1625ElementDocumentationMustNotBeCopiedAndPasted, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1625.html)
SA1626 | StyleCop.CSharp.DocumentationRules | Warning | SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1626.html)
SA1627 | StyleCop.CSharp.DocumentationRules | Warning | SA1627DocumentationTextMustNotBeEmpty
SA1642 | StyleCop.CSharp.DocumentationRules | Warning | SA1642ConstructorSummaryDocumentationMustBeginWithStandardText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1642.html)
SA1643 | StyleCop.CSharp.DocumentationRules | Warning | SA1643DestructorSummaryDocumentationMustBeginWithStandardText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1643.html)
SA1648 | StyleCop.CSharp.DocumentationRules | Warning | SA1648InheritDocMustBeUsedWithInheritingClass, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1648.html)
SA1649 | StyleCop.CSharp.DocumentationRules | Warning | SA1649FileNameMustMatchTypeName
SA1651 | StyleCop.CSharp.DocumentationRules | Warning | SA1651DoNotUsePlaceholderElements, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1651.html)
SA1652 | StyleCop.CSharp.DocumentationRules | Warning | SA1652EnableXmlDocumentationOutput, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1652.html)
SX1101 | StyleCop.CSharp.ReadabilityRules | Warning | SX1101DoNotPrefixLocalMembersWithThis
SX1309 | StyleCop.CSharp.NamingRules | Warning | SX1309FieldNamesMustBeginWithUnderscore, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SX1309.html)
SX1309S | StyleCop.CSharp.NamingRules | Warning | SX1309SStaticFieldNamesMustBeginWithUnderscore, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SX1309S.html)

## Release 1.1

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA0001 | StyleCop.CSharp.SpecialRules | Warning | SA0001XmlCommentAnalysisDisabled, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA0001.html)
SA0002 | StyleCop.CSharp.SpecialRules | Warning | SA0002InvalidSettingsFile, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA0002.html)
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
SA1516 | StyleCop.CSharp.LayoutRules | Warning | SA1516ElementsMustBeSeparatedByBlankLine, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1516.html)
SA1620 | StyleCop.CSharp.DocumentationRules | Warning | SA1620GenericTypeParameterDocumentationMustMatchTypeParameters, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1620.html)
SA1621 | StyleCop.CSharp.DocumentationRules | Warning | SA1621GenericTypeParameterDocumentationMustDeclareParameterName, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1621.html)
SA1622 | StyleCop.CSharp.DocumentationRules | Warning | SA1622GenericTypeParameterDocumentationMustHaveText, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1622.html)
SA1652 | StyleCop.CSharp.DocumentationRules | Warning | SA1652EnableXmlDocumentationOutput, [Documentation](https://dotnetanalyzers.github.io/StyleCopAnalyzers/SA1652.html)
