# 已知差异

本文档介绍 StyleCop Analyzers 相对于 StyleCop Classic 的已知行为差异。
此处列出的差异满足以下所有条件：

1. 行为变化会影响 C# 5 或更早版本的代码。
   StyleCop Classic 不支持 C# 6，因此此处不考虑仅为支持新语言功能而进行的必要更改。
2. 行为变化目前属于有意设计。
   意外的行为变化一经发现就会作为缺陷报告。
3. 变化影响的是 StyleCop Classic 中已有的规则。
   可禁用 StyleCop Analyzers 新增的规则，以更接近 StyleCop Classic 的行为。

许多情况下，行为变化只是文档的更改，因为 StyleCop Classic 的实现偏离了其自身文档中规定的规则。
为完整起见，下面也列出了我们发现的这类差异。
以下符号用于标记并非单纯文档错误的情况：

> :warning: 对于同一段代码，StyleCop Analyzers 的实现会产生与 StyleCop Classic 不同的警告时，会用此符号标记。

## 未启用的规则

StyleCop Classic 中的若干规则被有意排除在 StyleCop Analyzers 之外。
下表列出这些规则，并提供作出排除决定的问题链接。

| ID | 标题 | 问题 |
| --- | --- | --- |
| SA1109 | 块语句不应包含嵌入的 region | [#998](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/998) |
| SA1126 | 应正确添加调用前缀 | [#59](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/59) |
| SA1215 | 实例只读元素应位于实例非只读元素之前 | [#1812](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1812) |
| SA1409 | 应删除不必要的代码 | [#1058](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1058) |
| SA1603 | 文档应包含有效的 XML | [#1291](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1291) |
| SA1628 | 文档文本应以大写字母开头 | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1630 | 文档文本应包含空白字符 | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1631 | 文档应满足字母字符比例要求 | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1632 | 文档文本应满足最小字符长度要求 | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1644 | 文档注释头不得包含空行 | [#164](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/164) |
| SA1645 | 包含的文档文件不存在 | [#165](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/165) |
| SA1646 | 包含的文档 XPath 不存在 | [#166](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/166) |
| SA1647 | include 节点不包含有效的 file 和 path | [#167](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/167) |
| SA1650 | 元素文档应正确拼写 | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |

## 空格规则

### SA1000: 关键字周围的空格应正确

SA1000 有以下变化：

1. :warning: 现在要求 `await` 和 `case` 后有空格。

2. 当 `throw` 关键字出现在重新引发异常的语句中时，不再要求其后有空格。
   StyleCop Classic 的文档中未提及此例外。

   ```csharp
   throw;
   ```

3. :warning: 当 `new` 关键字出现在泛型类型约束中时，不再要求其后有空格。
   StyleCop Classic 未检查泛型类型约束中 `new` 关键字周围的空格。

   ```csharp
   public void Foo<T>() where T : IInterface, new()
   {
       // ...
   }
   ```

### SA1001

:warning: 开放泛型类型中逗号周围的空格要求略有变化。
下表展示此变化。

| StyleCop Analyzers | StyleCop Classic |
| --- | --- |
| `typeof(Func<,>)` | `typeof(Func<, >)` |

### SA1002

:warning: StyleCop Classic 要求无限 `for` 循环写成如下形式：

```csharp
for (;;)
{
}
```

StyleCop Analyzers 会对相同代码报告 SA1002，而要求代码写成如下形式：

```csharp
for (; ;)
{
}
```

:bulb: 遇到此类代码时，建议改写为以下形式，以提高可读性：

```csharp
while (true)
{
}
```

### SA1003

:warning: StyleCop Classic 允许类型强制转换出现在行尾，例如：

```csharp
uint value = (uint)
    3;
```

StyleCop Analyzers 不允许这种情况，而要求写成如下形式：

```csharp
uint value = (uint)3;
```

### SA1025

:warning: StyleCop Classic 允许行尾注释前出现多个空格，例如：

```csharp
int x;    // comment
```

它还允许符号前有多个空格，例如以下代码的第二行：

```csharp
int xyz = 1;
int w   = 1;
```

StyleCop Analyzers 目前不会为这些情况设置 SA1025 规则的例外，包括标志枚举中对齐的 `=` 等运算符（[#3684](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3684)）。

## 可读性规则

### SA1110, SA1111, SA1113, SA1114

StyleCop Analyzers 会检查这些规则的原始文档中未提及的几种结构：

* 委托声明
* 匿名方法表达式
* lambda 表达式

### SA1119

:warning: StyleCop Classic 不会对以下代码报告 SA1119：

```csharp
var a = (new[] { 1, 2, 3 }).ToArray();
```

StyleCop Analyzers 会对相同代码报告 SA1119，要求按如下方式更改以修正它：

```csharp
var a = new[] { 1, 2, 3 }.ToArray();
```

## 排序规则

### SA1208

StyleCop Analyzers 仅将不带别名限定符的 using 指令视为“System” using 指令，而 StyleCop Classic 会忽略别名。
例如，StyleCop Analyzers 不会将 `using global::System;` 视为 System using 指令，但 StyleCop Classic 会。

### SA1210

StyleCop Analyzers 在对 using 指令排序时会考虑别名限定符，以匹配 Visual Studio 2015 的默认行为。
StyleCop Classic 在对 using 指令排序时会忽略别名限定符。

StyleCop Analyzers 排序顺序示例：
```csharp
using Beer;
using global::Wine;
using Tea;
```

StyleCop Classic 排序顺序示例：
```csharp
using Beer;
using Tea;
using global::Wine;
```

### SA1214

StyleCop Classic 仅对涉及静态字段的违规报告 SA1214。
StyleCop Analyzers 将 SA1214 和 SA1215 合并为一个规则，以便用户更好地自定义涉及类型成员的若干排序规则的行为。

:warning: StyleCop Classic 中报告为 SA1215 的违规，在 StyleCop Analyzers 中报告为 SA1214。

## 命名规则

### SA1300

StyleCop Analyzers 将枚举成员加入应以大写字母开头的元素列表，并对违规报告 SA1300。
StyleCop Classic 不会对未以大写字母开头的枚举成员报告任何消息。

### SA1303

:warning: StyleCop Classic 会对以小写字母开头的局部常量报告 SA1303，尽管它们不是字段。
StyleCop Analyzers 将 SA1303 限定于字段，因此局部常量遵循局部变量命名规则，以下代码不会产生警告（[#2082](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2082)）：

```csharp
public void SomeMethod()
{
    const string url = "some constant value";
}
```

### SA1305

此规则在 StyleCop Analyzers 中默认禁用，但用户可通过规则集文件启用它。

:warning: StyleCop Analyzers 不会对重写方法和实现接口的方法中的参数报告 SA1305。
StyleCop Classic 会对所有方法报告 SA1305。

### SA1313

:warning: StyleCop Classic 允许仅由下划线组成的 lambda 参数（例如 `_`、`__`、`___`、…），而不报告 SA1313。
StyleCop Analyzers 仅对 `_` 和 `__` 作特殊处理；更长的纯下划线名称仍会报告 SA1313（[#2759](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2759)）。

## 可维护性规则

目前没有已知差异。

## 布局规则

### SA1514

:warning: 虽然 StyleCop Classic 通常不允许代码中出现 `#region` 指令（SA1124），但它可能间接允许元素文档注释头紧跟在 `#region` 指令之后而不加空行（即不报告 SA1514）。
StyleCop Analyzers 将 `#region` 视为前面的内容，因此除非在指令后添加空行，否则会报告 SA1514（[#1280](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1280)）。

### SA1515

:warning: 以下代码在 StyleCop Classic 中不会报告警告：

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]
// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

StyleCop Analyzers 会对此代码报告 SA1515，而要求写成如下形式：

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]

// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

## 文档规则

### SA1642

StyleCop Analyzers 要求在标准构造函数文本中引用类名时使用 `<see>` 元素。
StyleCop Classic 中此元素是可选的。

StyleCop Classic 为 `private` 构造函数使用的构造函数文本设置了特殊情况。
StyleCop Analyzers 仍允许用户在私有构造函数的文档中使用这种措辞，但更推荐标准构造函数文本。
对私有构造函数应用 SA1642 代码修复时，插入的文本符合 StyleCop Analyzers 的要求，但会在 StyleCop Classic 中产生警告。
例如：

以下代码在 StyleCop Classic 和 StyleCop Analyzers 中均不会产生警告：

```csharp
public class ApiStatus
{
    /// <summary>
    /// Prevents a default instance of the <see cref="ApiStatus"/> class from being created.
    /// </summary>
    private ApiStatus()
    {
    }
}
```

:warning: 以下代码在 StyleCop Classic 中会产生警告，但在 StyleCop Analyzers 中不会产生警告：

```csharp
public class ApiStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiStatus"/> class.
    /// </summary>
    private ApiStatus()
    {
    }
}
```

### SA1648

此规则已修改，以更贴近 Sandcastle Help File Builder 对 `<inheritdoc>` 的使用方式。
因此，某些在 StyleCop Classic 中会报告 SA1648 的代码，在 StyleCop Analyzers 中不再报告此警告。

### SA1649

StyleCop Analyzers 修改了 SA1649，将第一个类型与实际文件名进行比较，而不是与文件头中的值比较。
对于同时启用 SA1638 和 SA1649 的 StyleCop Classic 用户，此更改不会使之前符合 StyleCop 要求的代码产生任何新警告。
