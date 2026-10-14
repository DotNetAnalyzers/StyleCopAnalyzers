### 可读性规则 (SA1100-)
在 **.editorconfig** 文件中使用以下设置为整个类别配置严重级别：

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.ReadabilityRules.severity = none
```

这些规则确保代码格式良好、易于阅读。

标识符 | 名称 | 说明
-----------|------|-------------
[SA1100](SA1100.md) | DoNotPrefixCallsWithBaseUnlessLocalImplementationExists | 对继承类成员的调用以 `base.` 开头，但当前类不包含该成员的重写或实现。
[SA1101](SA1101.md) | PrefixLocalCallsWithThis | C# 代码文件中对当前类或基类的实例成员的调用未带有 'this.' 前缀。
[SA1102](SA1102.md) | QueryClauses | C# 查询子句未在前一个子句所在行或下一行开始。
[SA1103](SA1103.md) | QueryClauses | C# 查询表达式中的子句既未全部位于同一行，也未各自独占一行。
[SA1104](SA1104.md) | QueryClauses | C# 查询表达式中的某个子句与前一个跨越多行的子句在同一行开始。
[SA1105](SA1105.md) | QueryClauses | C# 查询表达式中的某个子句跨越多行，但未另起一行。
[SA1106](SA1106.md) | CodeMustNotContainEmptyStatements | C# 代码包含多余的分号。
[SA1107](SA1107.md) | CodeMustNotContainMultipleStatementsOnOneLine | C# 代码在同一行中包含多个语句。
[SA1108](SA1108.md) | BlockStatementsMustNotContainEmbeddedComments | C# 语句在语句声明与其左大括号之间包含注释。
[SA1109](SA1109.md) | BlockStatementsMustNotContainEmbeddedRegions | C# 语句在语句声明与其左大括号之间包含 region 标记。
[SA1110](SA1110.md) | OpeningParenthesisMustBeOnDeclarationLine | C# 方法或索引器的调用或声明中的左圆括号或左方括号，与方法或索引器名称不在同一行。
[SA1111](SA1111.md) | ClosingParenthesisMustBeOnLineOfLastParameter | C# 方法或索引器的调用或声明中的右圆括号或右方括号，与最后一个参数不在同一行。
[SA1112](SA1112.md) | ClosingParenthesisMustBeOnLineOfOpeningParenthesis | C# 方法或索引器的调用或声明不接受任何参数，但其中的右圆括号或右方括号与左括号不在同一行。
[SA1113](SA1113.md) | CommaMustBeOnSameLineAsPreviousParameter | C# 方法或索引器的调用或声明中两个参数之间的逗号，与前一个参数不在同一行。
[SA1114](SA1114.md) | ParameterListMustFollowDeclaration | 方法或索引器的调用或声明中的参数列表，未在左括号所在行或下一行开始。
[SA1115](SA1115.md) | ParameterMustFollowComma | C# 方法或索引器的调用或声明中的某个参数，未在前一个参数所在行或下一行开始。
[SA1116](SA1116.md) | SplitParametersMustStartOnLineAfterDeclaration | C# 方法或索引器的调用或声明中的参数跨越多行，但第一个参数未在左括号的下一行开始。
[SA1117](SA1117.md) | ParametersMustBeOnSameLineOrSeparateLines | C# 方法或索引器的调用或声明中的参数既未全部位于同一行，也未各自独占一行。
[SA1118](SA1118.md) | ParameterMustNotSpanMultipleLines | C# 方法或索引器中除第一个参数以外的某个参数跨越多行。
[SA1120](SA1120.md) | CommentsMustContainText | C# 注释不包含任何注释文本。
[SA1121](SA1121.md) | UseBuiltInTypeAlias | 代码使用了某个 C# 基本类型，但未使用其内置别名。
[SA1122](SA1122.md) | UseStringEmptyForEmptyStrings | C# 代码包含以 `""` 形式编写的空字符串。
[SA1123](SA1123.md) | DoNotPlaceRegionsWithinElements | C# 代码在代码元素的主体中包含 region。
[SA1124](SA1124.md) | DoNotUseRegions | C# 代码包含 region。
[SA1125](SA1125.md) | UseShorthandForNullableTypes | 可为 null 的类型未使用 C# 简写形式定义。
[SA1126](SA1126.md) | PrefixCallsCorrectly | C# 代码文件中对成员的调用未使用 'this.'、'base.'、'object.' 或 'typename.' 前缀来表明预期的方法调用。
[SA1127](SA1127.md) | GenericTypeConstraintsMustBeOnOwnLine | C# 代码文件中类型或方法声明的泛型约束与声明位于同一行。
[SA1128](SA1128.md) | ConstructorInitializerMustBeOnOwnLine | C# 代码文件中的构造函数初始值设定项与构造函数声明位于同一行。
[SA1129](SA1129.md) | DoNotUseDefaultValueTypeConstructor | 使用 `new T()` 语法构造了值类型。
[SA1130](SA1130.md) | UseLambdaSyntax | 匿名方法采用 `delegate (parameters) { }` 形式声明，而使用 `(parameters) => { }` 语法的 lambda 表达式可提供等效行为。
[SA1131](SA1131.md) | UseReadableConditions | 变量与字面量或常量值进行比较时，变量位于表达式的右侧。
[SA1132](SA1132.md) | DoNotCombineFields | 同一字段声明语法中声明了两个或更多字段。
[SA1133](SA1133.md) | DoNotCombineAttributes | 同一组方括号中出现了两个或更多特性。
[SA1134](SA1134.md) | AttributesMustNotShareLine | 某个特性与其他特性或元素位于同一行代码中。
[SA1135](SA1135.md) | UsingDirectivesMustBeQualified | using 指令未使用限定名称。
[SA1136](SA1136.md) | EnumValuesShouldBeOnSeparateLines | 多个枚举值位于同一行代码中。
[SA1137](SA1137.md) | ElementsShouldHaveTheSameIndentation | 两个各自另起一行的同级元素具有不同的缩进级别。
[SA1139](SA1139.md) | UseLiteralsSuffixNotationInsteadOfCasting | 应使用字面量后缀表示法，而不是强制转换。
[SA1141](SA1141.md) | UseTupleSyntax | 应使用元组语法，而不是底层的 ValueTuple 实现类型。
[SA1142](SA1142.md) | ReferToTupleElementsByName | 某个元组元素具有可用的元素名称，但引用时使用了其元数据名称。
