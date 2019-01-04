### Readability Rules (SA1100-)
Rules which ensure that the code is well-formatted and readable.

Identifier | Name | Description
-----------|------|-------------
[SA1100](SA1100.md) | DoNotPrefixCallsWithBaseUnlessLocalImplementationExists | A call to a member from an inherited class begins with `base.`, and the local class does not contain an override or implementation of the member. 
[SA1101](SA1101.md) | PrefixLocalCallsWithThis | A call to an instance member of the local class or a base class is not prefixed with 'this.', within a C# code file. 
[SA1102](SA1102.md) | QueryClauses | A C# query clause does not begin on the same line as the previous clause, or on the next line. 
[SA1103](SA1103.md) | QueryClauses | The clauses within a C# query expression are not all placed on the same line, and each clause is not placed on its own line. 
[SA1104](SA1104.md) | QueryClauses | A clause within a C# query expression begins on the same line as the previous clause, when the previous clause spans across multiple lines. 
[SA1105](SA1105.md) | QueryClauses | A clause within a C# query expression spans across multiple lines, and does not begin on its own line. 
[SA1106](SA1106.md) | CodeMustNotContainEmptyStatements | The C# code contains an extra semicolon. 
[SA1107](SA1107.md) | CodeMustNotContainMultipleStatementsOnOneLine | The C# code contains more than one statement on a single line. 
[SA1108](SA1108.md) | BlockStatementsMustNotContainEmbeddedComments | A C# statement contains a comment between the declaration of the statement and the opening brace of the statement. 
[SA1109](SA1109.md) | BlockStatementsMustNotContainEmbeddedRegions | A C# statement contains a region tag between the declaration of the statement and the opening brace of the statement. 
[SA1110](SA1110.md) | OpeningParenthesisMustBeOnDeclarationLine | The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.
[SA1111](SA1111.md) | ClosingParenthesisMustBeOnLineOfLastParameter | The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter. 
[SA1112](SA1112.md) | ClosingParenthesisMustBeOnLineOfOpeningParenthesis | The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the opening bracket when the element does not take any parameters. 
[SA1113](SA1113.md) | CommaMustBeOnSameLineAsPreviousParameter | A comma between two parameters in a call to a C# method or indexer, or in the declaration of a method or indexer, is not placed on the same line as the previous parameter. 
[SA1114](SA1114.md) | ParameterListMustFollowDeclaration | The start of the parameter list for a method or indexer call or declaration does not begin on the same line as the opening bracket, or on the line after the opening bracket. 
[SA1115](SA1115.md) | ParameterMustFollowComma | A parameter within a C# method or indexer call or declaration does not begin on the same line as the previous parameter, or on the next line. 
[SA1116](SA1116.md) | SplitParametersMustStartOnLineAfterDeclaration | The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter does not start on the line after the opening bracket. 
[SA1117](SA1117.md) | ParametersMustBeOnSameLineOrSeparateLines | The parameters to a C# method or indexer call or declaration are not all on the same line or each on a separate line. 
[SA1118](SA1118.md) | ParameterMustNotSpanMultipleLines | A parameter to a C# method or indexer, other than the first parameter, spans across multiple lines. 
[SA1120](SA1120.md) | CommentsMustContainText | The C# comment does not contain any comment text. 
[SA1121](SA1121.md) | UseBuiltInTypeAlias | The code uses one of the basic C# types, but does not use the built-in alias for the type. 
[SA1122](SA1122.md) | UseStringEmptyForEmptyStrings | The C# code includes an empty string, written as `""`. 
[SA1123](SA1123.md) | DoNotPlaceRegionsWithinElements | The C# code contains a region within the body of a code element. 
[SA1124](SA1124.md) | DoNotUseRegions | The C# code contains a region. 
[SA1125](SA1125.md) | UseShorthandForNullableTypes | The Nullable type has been defined not using the C# shorthand.
[SA1126](SA1126.md) | PrefixCallsCorrectly | A call to a member is not prefixed with the 'this.', 'base.', 'object.' or 'typename.' prefix to indicate the intended method call, within a C# code file. 
[SA1127](SA1127.md) | GenericTypeConstraintsMustBeOnOwnLine | A generic constraint on a type or method declaration is on the same line as the declaration, within a C# code file. 
[SA1128](SA1128.md) | ConstructorInitializerMustBeOnOwnLine | A constructor initializer is on the same line as the constructor declaration, within a C# code file. 
[SA1129](SA1129.md) | DoNotUseDefaultValueTypeConstructor | A value type was constructed using the syntax `new T()`. 
[SA1130](SA1130.md) | UseLambdaSyntax | An anonymous method was declared using the form `delegate (parameters) { }`, when a lambda expression would provide equivalent behavior with the syntax `(parameters) => { }`.
[SA1131](SA1131.md) | UseReadableConditions | A comparison was made between a variable and a literal or constant value, and the variable appeared on the right-hand side of the expression.
[SA1132](SA1132.md) | DoNotCombineFields | Two or more fields were declared in the same field declaration syntax. 
[SA1133](SA1133.md) | DoNotCombineAttributes | Two or more attributes appeared within the same set of square brackets. 
[SA1134](SA1134.md) | AttributesMustNotShareLine | An attribute is placed on the same line of code as another attribute or element. 
[SA1135](SA1135.md) | UsingDirectivesMustBeQualified | A using directive is not qualified.
[SA1136](SA1136.md) | EnumValuesShouldBeOnSeparateLines | Multiple enum values are placed on the same line of code. 
[SA1137](SA1137.md) | ElementsShouldHaveTheSameIndentation | Two sibling elements which each start on their own line have different levels of indentation.
[SA1139](SA1139.md) | UseLiteralsSuffixNotationInsteadOfCasting | Use literal suffix notation instead of casting. 
[SA1141](SA1141.md) | UseTupleSyntax | Use tuple syntax instead of the underlying ValueTuple implementation type.
