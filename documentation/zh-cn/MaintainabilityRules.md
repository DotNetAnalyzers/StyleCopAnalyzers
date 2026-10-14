### 可维护性规则 (SA1400-)
在 **.editorconfig** 文件中使用以下设置为整个类别配置严重级别：

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.MaintainabilityRules.severity = none
```

这些规则提高代码的可维护性。

标识符 | 名称 | 说明
-----------|------|------------
[SA1119](SA1119.md) | StatementMustNotUseUnnecessaryParenthesis | C# 语句包含应当删除的不必要的圆括号。
[SA1400](SA1400.md) | AccessModifierMustBeDeclared | C# 元素的访问修饰符未显式定义。
[SA1401](SA1401.md) | FieldsMustBePrivate | C# 类中的字段使用了 private 以外的访问修饰符。
[SA1402](SA1402.md) | FileMayOnlyContainASingleType | C# 代码文件包含多个不同的类型。
[SA1403](SA1403.md) | FileMayOnlyContainASingleNamespace | C# 代码文件包含多个命名空间。
[SA1404](SA1404.md) | CodeAnalysisSuppressionMustHaveJustification | 代码分析 SuppressMessage 特性未包含理由说明。
[SA1405](SA1405.md) | DebugAssertMustProvideMessageText | C# 代码中的 Debug.Assert 调用未包含描述性消息。
[SA1406](SA1406.md) | DebugFailMustProvideMessageText | C# 代码中的 Debug.Fail 调用未包含描述性消息。
[SA1407](SA1407.md) | ArithmeticExpressionsMustDeclarePrecedence | C# 语句中的复杂表达式包含算术、位运算或移位运算符，但未使用圆括号明确运算优先级。
[SA1408](SA1408.md) | ConditionalExpressionsMustDeclarePrecedence | C# 语句包含复杂的条件表达式，但省略了运算符周围的圆括号。
[SA1409](SA1409.md) | RemoveUnnecessaryCode | C# 文件包含可以删除而不会改变整体代码逻辑的不必要代码。
[SA1410](SA1410.md) | RemoveDelegateParenthesisWhenPossible | 对 C# 匿名方法的调用没有任何方法参数，但语句仍包含圆括号。
[SA1411](SA1411.md) | AttributeConstructorMustNotUseUnnecessaryParenthesis | 特性声明不包含任何参数，但仍包含圆括号。
[SA1412](SA1412.md) | StoreFilesAsUtf8 | 文件编码不是带字节顺序标记的 UTF-8。
[SA1413](SA1413.md) | UseTrailingCommasInMultiLineInitializers | 多行初始值设定项、枚举、switch 表达式或属性模式的最后一项应使用逗号。
[SA1414](SA1414.md) | TupleTypesInSignaturesShouldHaveElementNames | 成员声明中出现的元组类型应显式指定元组元素名称。
