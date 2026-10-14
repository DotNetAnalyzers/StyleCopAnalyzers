### 排序规则 (SA1200-)
在 **.editorconfig** 文件中使用以下设置为整个类别配置严重级别：

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.OrderingRules.severity = none
```

这些规则规定代码内容的标准排列顺序。

标识符 | 名称 | 说明
-----------|------|-------------
[SA1200](SA1200.md) | UsingDirectivesMustBePlacedCorrectly | C# using 指令位于命名空间元素之外。
[SA1201](SA1201.md) | ElementsMustAppearInTheCorrectOrder | C# 代码文件中的某个元素相对于其他代码元素的排列顺序不正确。
[SA1202](SA1202.md) | ElementsMustBeOrderedByAccess | C# 代码文件中的某个元素相对于其他代码元素未按访问级别正确排序。
[SA1203](SA1203.md) | ConstantsMustAppearBeforeFields | 常量字段位于非常量字段之后。
[SA1204](SA1204.md) | StaticElementsMustAppearBeforeInstanceElements | 静态元素位于同类型的实例元素之后。
[SA1205](SA1205.md) | PartialElementsMustDeclareAccess | 分部元素未定义访问修饰符。
[SA1206](SA1206.md) | DeclarationKeywordsMustFollowOrder | 元素声明中的关键字未遵循标准排列顺序。
[SA1207](SA1207.md) | ProtectedMustComeBeforeInternal | C# protected internal 元素的声明中，关键字 *protected* 位于关键字 *internal* 之后。
[SA1208](SA1208.md) | SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives | C# 代码文件中声明 *System* 命名空间成员的 using 指令，位于声明其他命名空间成员的 using 指令之后。
[SA1209](SA1209.md) | UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives | using 别名指令位于普通 using 指令之前。
[SA1210](SA1210.md) | UsingDirectivesMustBeOrderedAlphabeticallyByNamespace | C# 代码文件中的 using 指令未按命名空间的字母顺序排列。
[SA1211](SA1211.md) | UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName | C# 代码文件中的 using 别名指令未按别名的字母顺序排列。
[SA1212](SA1212.md) | PropertyAccessorsMustFollowOrder | 属性或索引器中的 get 访问器位于 set 访问器之后。
[SA1213](SA1213.md) | EventAccessorsMustFollowOrder | 事件中的 add 访问器位于 remove 访问器之后。
[SA1214](SA1214.md) | ReadonlyElementsMustAppearBeforeNonReadonlyElements | 只读字段位于非只读字段之后。
[SA1215](SA1215.md) | InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements | 实例只读元素位于同类型的实例非只读元素之后。
[SA1216](SA1216.md) | UsingStaticDirectivesMustBePlacedAtTheCorrectLocation | `using static` 指令位于错误的位置（在普通 using 指令之前或别名 using 指令之后）。
[SA1217](SA1217.md) | UsingStaticDirectivesMustBeOrderedAlphabetically | C# 代码文件中的 `using static` 指令未按完整类型名称的字母顺序排列。
