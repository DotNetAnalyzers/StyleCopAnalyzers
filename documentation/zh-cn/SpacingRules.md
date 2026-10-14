### 空格规则 (SA1000-)
在 **.editorconfig** 文件中使用以下设置为整个类别配置严重级别：

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.SpacingRules.severity = none
```

这些规则规定代码中关键字和符号周围的空格要求。

标识符 | 名称 | 说明
-----------|------|-------------
[SA1000](SA1000.md) | KeywordsMustBeSpacedCorrectly | C# 关键字周围的空格不正确。
[SA1001](SA1001.md) | CommasMustBeSpacedCorrectly | C# 代码文件中逗号周围的空格不正确。
[SA1002](SA1002.md) | SemicolonsMustBeSpacedCorrectly | C# 代码文件中分号周围的空格不正确。
[SA1003](SA1003.md) | SymbolsMustBeSpacedCorrectly | C# 代码文件中运算符周围的空格不正确。
[SA1004](SA1004.md) | DocumentationLinesMustBeginWithSingleSpace | C# 元素上方文档注释头中的某一行未以单个空格开头。
[SA1005](SA1005.md) | SingleLineCommentsMustBeginWithSingleSpace | C# 代码文件中的单行注释未以单个空格开头。
[SA1006](SA1006.md) | PreprocessorKeywordsMustNotBePrecededBySpace | C# 预处理器关键字前有空格。
[SA1007](SA1007.md) | OperatorKeywordMustBeFollowedBySpace | C# 运算符重载方法中的 operator 关键字后没有空白字符。
[SA1008](SA1008.md) | OpeningParenthesisMustBeSpacedCorrectly | C# 语句中左圆括号周围的空格不正确。
[SA1009](SA1009.md) | ClosingParenthesisMustBeSpacedCorrectly | C# 语句中右圆括号周围的空格不正确。
[SA1010](SA1010.md) | OpeningSquareBracketsMustBeSpacedCorrectly | C# 语句中左方括号周围的空格不正确。
[SA1011](SA1011.md) | ClosingSquareBracketsMustBeSpacedCorrectly | C# 语句中右方括号周围的空格不正确。
[SA1012](SA1012.md) | OpeningBracesMustBeSpacedCorrectly | C# 元素中左大括号周围的空格不正确。
[SA1013](SA1013.md) | ClosingBracesMustBeSpacedCorrectly | C# 元素中右大括号周围的空格不正确。
[SA1014](SA1014.md) | OpeningGenericBracketsMustBeSpacedCorrectly | C# 元素中泛型左尖括号周围的空格不正确。
[SA1015](SA1015.md) | ClosingGenericBracketsMustBeSpacedCorrectly | C# 元素中泛型右尖括号周围的空格不正确。
[SA1016](SA1016.md) | OpeningAttributeBracketsMustBeSpacedCorrectly | C# 元素中特性左方括号周围的空格不正确。
[SA1017](SA1017.md) | ClosingAttributeBracketsMustBeSpacedCorrectly | C# 元素中特性右方括号周围的空格不正确。
[SA1018](SA1018.md) | NullableTypeSymbolsMustNotBePrecededBySpace | C# 元素中可为 null 的类型符号周围的空格不正确。
[SA1019](SA1019.md) | MemberAccessSymbolsMustBeSpacedCorrectly | C# 代码文件中成员访问符号周围的空格不正确。
[SA1020](SA1020.md) | IncrementDecrementSymbolsMustBeSpacedCorrectly | C# 元素中递增或递减符号周围的空格不正确。
[SA1021](SA1021.md) | NegativeSignsMustBeSpacedCorrectly | C# 元素中负号周围的空格不正确。
[SA1022](SA1022.md) | PositiveSignsMustBeSpacedCorrectly | C# 元素中正号周围的空格不正确。
[SA1023](SA1023.md) | DereferenceAndAccessOfMustBeSpacedCorrectly | C# 元素中取消引用符号或取址符号周围的空格不正确。
[SA1024](SA1024.md) | ColonsMustBeSpacedCorrectly | C# 元素中冒号周围的空格不正确。
[SA1025](SA1025.md) | CodeMustNotContainMultipleWhitespaceInARow | 代码包含连续的多个空白字符。
[SA1026](SA1026.md) | CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation | C# 代码文件中隐式类型的新数组分配表达式中的空格不正确。
[SA1027](SA1027.md) | UseTabsCorrectly | 代码包含与当前项目设置不一致的制表符或空格字符。
[SA1028](SA1028.md) | CodeMustNotContainTrailingWhitespace | 某一行代码在行结束符之前以空格、制表符或其他空白字符结尾。
