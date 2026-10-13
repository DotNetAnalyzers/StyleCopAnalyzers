<a id="configuring-stylecop-analyzers"></a>

# 配置 StyleCop Analyzers

StyleCop Analyzers 有两类配置：**运行哪些规则以及规则的严重级别**，和用于微调各个规则的**设置**（例如文件头中使用的公司名称）。

**建议：** 两类配置都使用 **.editorconfig** 文件或 **.globalconfig** 文件。
所有设置都可在其中指定，因此只需一个文件。
**stylecop.json** 和规则集文件在现有项目中仍然有效，下面会介绍它们及其迁移方法。

| 配置机制 | 规则严重级别 | 设置 |
| --- | --- | --- |
| **.editorconfig** / **.globalconfig**（推荐） | 支持（`dotnet_diagnostic.<ID>.severity`） | 全部（`variables` 需要 Roslyn 4.4 或更新版本） |
| **stylecop.json** | 不支持 | 全部 |
| 规则集文件（`.ruleset`） | 支持 | 不支持 |

如果同一设置同时出现在 **stylecop.json** 和 **.editorconfig**/**.globalconfig** 文件中，则使用 **stylecop.json** 中的值。

<a id="unified-configuration-with-editorconfig-or-globalconfig"></a>

## 使用 .editorconfig 或 .globalconfig 统一配置

```ini
# .editorconfig (or .globalconfig, without the [*.cs] section header and with `is_global = true` at the top)
root = true

[*.cs]
# Rule severities
dotnet_diagnostic.SA1633.severity = none
dotnet_diagnostic.SA1309.severity = none

# Settings
stylecop.documentation.companyName = Contoso
csharp_using_directive_placement = outside_namespace
```

* 规则严重级别使用标准的 `dotnet_diagnostic.<rule ID>.severity` 属性（`error`、`warning`、`suggestion`、`silent`、`none`）。
  请参阅[配置代码分析器](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/configuration-files)。
* **.editorconfig** 文件根据节标题（例如 `[*.cs]`）和文件夹层次结构，分别应用于各个源文件。
* 当 **.globalconfig** 文件的名称恰好为 **.globalconfig**，且位于项目文件夹或其上级文件夹中时，会自动被加载。
  其他文件名需要将文件添加到 `GlobalAnalyzerConfigFiles` MSBuild 项中。
  全局配置文件没有节标题：在顶部写入 `is_global = true`，然后写入各属性。
* 无需复制默认模板。
  只选择要更改的严重级别和设置；各默认值在下面的章节中列出。
  全部规则及其默认严重级别的列表可在[规则实现状态](RuleStatus.md)和各规则页面中找到。
* SA0001 无法通过 **.editorconfig** 禁用（这是 Roslyn 的限制）。
  请使用规则集文件或 `NoWarn`。
* 属性名称不区分大小写。
  布尔值为 `true` 或 `false`，`unset` 可忽略父级文件中设置的值。

<a id="settings-reference"></a>

## 设置参考

下表列出所有设置、各配置机制中的设置方法、允许的值以及默认值。
各设置的详细信息见后续章节。

| 类别 | **stylecop.json**（`settings` 对象） | **.editorconfig** / **.globalconfig** | 值 | 默认值 |
| --- | --- | --- | --- | --- |
| 缩进 | `indentation.indentationSize` | `indent_size` | 整数 | `4` |
| 缩进 | `indentation.tabSize` | `tab_width` | 整数 | `4` |
| 缩进 | `indentation.useTabs` | `indent_style` | `true`/`false` ⇄ `tab`/`space` | `false` / `space` |
| 可读性 | `readabilityRules.allowBuiltInTypeAliases` | `stylecop.readability.allowBuiltInTypeAliases` | 布尔值 | `false` |
| 排序 | `orderingRules.elementOrder` | `stylecop.ordering.elementOrder` | `kind`、`accessibility`、`constant`、`static`、`readonly` 的列表 | `kind, accessibility, constant, static, readonly` |
| 排序 | `orderingRules.systemUsingDirectivesFirst` | `dotnet_sort_system_directives_first` | 布尔值 | `true` |
| 排序 | `orderingRules.usingDirectivesPlacement` | `stylecop.ordering.usingDirectivesPlacement`（或 `csharp_using_directive_placement`） | `insideNamespace`、`outsideNamespace`、`preserve` ⇄ `inside_namespace`、`outside_namespace` | `insideNamespace` |
| 排序 | `orderingRules.blankLinesBetweenUsingGroups` | `stylecop.ordering.blankLinesBetweenUsingGroups`（或 `dotnet_separate_import_directive_groups`） | `allow`、`require`、`omit` ⇄ `false`、`true` | `allow` |
| 命名 | `namingRules.allowCommonHungarianPrefixes` | `stylecop.naming.allowCommonHungarianPrefixes` | 布尔值 | `true` |
| 命名 | `namingRules.allowedHungarianPrefixes` | `stylecop.naming.allowedHungarianPrefixes` | 以逗号分隔的列表 | 空 |
| 命名 | `namingRules.allowedNamespaceComponents` | `stylecop.naming.allowedNamespaceComponents` | 以逗号分隔的列表 | 空 |
| 命名 | `namingRules.includeInferredTupleElementNames` | `stylecop.naming.includeInferredTupleElementNames` | 布尔值 | `false` |
| 命名 | `namingRules.tupleElementNameCasing` | `stylecop.naming.tupleElementNameCasing` | `camelCase`、`pascalCase` | `pascalCase` |
| 可维护性 | `maintainabilityRules.topLevelTypes` | `stylecop.maintainability.topLevelTypes` | `class`、`interface`、`struct`、`enum`、`delegate` 的列表 | `class` |
| 布局 | `layoutRules.newlineAtEndOfFile` | `stylecop.layout.newlineAtEndOfFile`（或 `insert_final_newline`） | `allow`、`require`、`omit` ⇄ `true`、`false` | `allow` |
| 布局 | `layoutRules.allowConsecutiveUsings` | `stylecop.layout.allowConsecutiveUsings` | 布尔值 | `true` |
| 布局 | `layoutRules.allowDoWhileOnClosingBrace` | `stylecop.layout.allowDoWhileOnClosingBrace` | 布尔值 | `false` |
| 文档 | `documentationRules.documentExposedElements` | `stylecop.documentation.documentExposedElements` | 布尔值 | `true` |
| 文档 | `documentationRules.documentInternalElements` | `stylecop.documentation.documentInternalElements` | 布尔值 | `true` |
| 文档 | `documentationRules.documentPrivateElements` | `stylecop.documentation.documentPrivateElements` | 布尔值 | `false` |
| 文档 | `documentationRules.documentPrivateFields` | `stylecop.documentation.documentPrivateFields` | 布尔值 | `false` |
| 文档 | `documentationRules.documentInterfaces` | `stylecop.documentation.documentInterfaces` | `all`、`exposed`、`none`（或布尔值） | `all` |
| 文档 | `documentationRules.companyName` | `stylecop.documentation.companyName` | 文本 | `PlaceholderCompany` |
| 文档 | `documentationRules.copyrightText` | `stylecop.documentation.copyrightText`（或 `file_header_template`） | 文本；允许 `\n` 和 `\r` | `Copyright (c) {companyName}. All rights reserved.` |
| 文档 | `documentationRules.variables.<name>` | `stylecop.documentation.variables.<name>`（需要 Roslyn 4.4 或更新版本） | 文本 | 无 |
| 文档 | `documentationRules.headerDecoration` | `stylecop.documentation.headerDecoration` | 文本 | 无 |
| 文档 | `documentationRules.xmlHeader` | `stylecop.documentation.xmlHeader` | 布尔值 | `true` |
| 文档 | `documentationRules.fileNamingConvention` | `stylecop.documentation.fileNamingConvention` | `stylecop`、`metadata` | `stylecop` |
| 文档 | `documentationRules.documentationCulture` | `stylecop.documentation.documentationCulture` | 区域性名称 | `en-US` |
| 文档 | `documentationRules.excludeFromPunctuationCheck` | `stylecop.documentation.excludeFromPunctuationCheck` | 以逗号分隔的列表 | `seealso` |

> :memo: 如果某行在括号中列出通用属性，同时设置两者时，StyleCop 专用属性优先。
> 通用属性与 .NET SDK 和 IDE 共用，因此如果已经在使用它们，直接沿用会很方便。

<a id="annotated-template"></a>

### 带注释的模板

编辑器不提供 `stylecop.*` 键的自动补全，因此此模板列出了每个设置及其默认值。
只需复制要更改的行；不必保留未更改的默认值。

```ini
[*.cs]
# Indentation
indent_size = 4
tab_width = 4
indent_style = space                      # tab | space

# Readability
stylecop.readability.allowBuiltInTypeAliases = false

# Ordering
stylecop.ordering.elementOrder = kind, accessibility, constant, static, readonly
dotnet_sort_system_directives_first = true
stylecop.ordering.usingDirectivesPlacement = insideNamespace   # insideNamespace | outsideNamespace | preserve
stylecop.ordering.blankLinesBetweenUsingGroups = allow         # allow | require | omit

# Naming
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes =                     # comma-separated
stylecop.naming.allowedNamespaceComponents =                   # comma-separated
stylecop.naming.includeInferredTupleElementNames = false
stylecop.naming.tupleElementNameCasing = pascalCase            # camelCase | pascalCase

# Maintainability
stylecop.maintainability.topLevelTypes = class                 # class, interface, struct, enum, delegate

# Layout
stylecop.layout.newlineAtEndOfFile = allow                     # allow | require | omit
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowDoWhileOnClosingBrace = false

# Documentation
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
stylecop.documentation.documentInterfaces = all                # all | exposed | none
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.variables.myvariable = value             # requires Roslyn 4.4 or newer
stylecop.documentation.headerDecoration =
stylecop.documentation.xmlHeader = true
stylecop.documentation.fileNamingConvention = stylecop         # stylecop | metadata
stylecop.documentation.documentationCulture = en-US
stylecop.documentation.excludeFromPunctuationCheck = seealso   # comma-separated
```

> :warning: 值不能包含 `#` 或 `;`：编译器会将任一字符到行尾的所有内容视为注释。
> 这会影响 `copyrightText`、`headerDecoration`（例如 `#####`）和变量等值；如果需要这些字符，请使用 **stylecop.json** 设置该项。
> 因此，此模板中的行尾注释可以安全地保留。

<a id="migrating-from-stylecopjson"></a>

## 从 stylecop.json 迁移

1. 创建 **.editorconfig**（或 **.globalconfig**），并使用上面的*设置参考*表添加各项设置。
   JSON 路径 `documentationRules.companyName` 对应 `stylecop.documentation.companyName`；通常 JSON 的 `<section>Rules.<name>` 对应 `stylecop.<section>.<name>`，例外是 `indentation`（使用 `indent_size`、`tab_width`、`indent_style`）和上面列出的通用属性。
2. 将数组转换为以逗号分隔的列表，例如 `["a", "b"]` 转换为 `a, b`。
3. 将 `variables` 中的各个条目转换为 `stylecop.documentation.variables.<name>` 属性。
   **.editorconfig** 中的变量名称在比较时按小写处理，因此 `copyrightText` 中的 `{name}` 引用不区分大小写。
4. 变量不会合并：如果 **stylecop.json** 定义了任何 `variables`，则会忽略 `stylecop.documentation.variables.*` 属性。
5. 删除 **stylecop.json**（以及对应的 `AdditionalFiles` 条目，如果存在）。
   两者同时存在时，**stylecop.json** 的值优先。

> :warning: 从 **.editorconfig** 读取 `variables` 需要能够提供已配置键列表的编译器（Roslyn 4.4 / Visual Studio 2022 17.4 或更新版本）。
> 如果必须支持旧版编译器，请将 `variables` 保留在 **stylecop.json** 中。

<a id="migrating-from-rule-set-files"></a>

## 从规则集文件迁移

每个规则集条目都可转换为严重级别属性：

| 规则集操作 | .editorconfig |
| --- | --- |
| `Action="None"` | `dotnet_diagnostic.SA1000.severity = none` |
| `Action="Hidden"` | `dotnet_diagnostic.SA1000.severity = silent` |
| `Action="Info"` | `dotnet_diagnostic.SA1000.severity = suggestion` |
| `Action="Warning"` | `dotnet_diagnostic.SA1000.severity = warning` |
| `Action="Error"` | `dotnet_diagnostic.SA1000.severity = error` |

`dotnet format` 和 Visual Studio 等工具也可以根据规则集自动生成这些条目。

本文档的其余部分介绍旧式配置机制。

<a id="rule-sets"></a>

## 规则集

代码分析规则集一直是 Visual Studio 中配置大多数诊断分析器的标准方式。
有关创建和自定义这些文件的信息，可参阅 docs.microsoft.com 上的[使用规则集对代码分析规则进行分组](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules)文档。

包含 StyleCop Analyzers 默认配置的规则集示例文件位于 <https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/src/StyleCop.Analyzers.CodeFixes/rulesets/StyleCopAnalyzersDefault.ruleset>。

<a id="getting-started-with-stylecopjson"></a>

## **stylecop.json** 入门

向新项目添加 **stylecop.json** 配置文件最简单的方法，是使用项目提供的代码修复。
要调用此代码修复，请打开任意报告 SA1633 的文件¹，然后按 Ctrl+. 打开“快速修复”菜单。
在菜单中选择**将 StyleCop 设置文件添加到项目**。

也支持以点开头的文件命名约定，因此可以将配置文件命名为 **.stylecop.json**。

<a id="json-schema-for-intellisense"></a>

### 用于 IntelliSense 的 JSON 架构

**stylecop.json** 提供了 JSON 架构。
在 **stylecop.json** 中添加对该架构的引用后，Visual Studio 在编辑文件时会提供 IntelliSense 功能（代码补全、快速信息等）。
可通过在 **stylecop.json** 中添加以下顶级属性来配置架构：

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"
}
```

> :bulb: 上文介绍的代码修复会自动配置 **stylecop.json**，使其引用此架构。
> 如果 Visual Studio 中的架构似乎不是最新的，请在 **stylecop.json** 文档中的任意位置右键单击，然后选择**重新加载架构**。

<a id="source-control"></a>

### 源代码管理

为获得最佳效果，应将 **stylecop.json** 纳入源代码管理。
这样可将预期设置自动传递给项目中的所有团队成员。

> :warning: 如果使用 Git，请确保 **.gitignore** 文件*不包含*以下行。
> 如果存在，应将其删除。
>
> ```
> [Ss]tyle[Cc]op.*
> ```

<a id="indentation"></a>

## 缩进

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的缩进规则。
各属性都可在 **stylecop.json** 文件的 `indentation` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "indentation": {
    }
  }
}
```

<a id="basic-indentation"></a>

### 基本缩进

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的基本缩进。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `indentationSize` | **4** | 1.1.0 | 每级代码缩进占用的列数。根据 `useTabs` 和 `tabSize` 设置，使用制表符和/或空格填充。 |
| `tabSize` | **4** | 1.1.0 | 源代码中实际制表符的宽度。在制表符和空格之间转换时使用此值。 |
| `useTabs` | **false** | 1.1.0 | **true** 表示使用实际制表符缩进；**false** 表示使用空格缩进。 |

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用 editorconfig.org 介绍的基本缩进设置（`indent_size`、`tab_width` 和 `indent_style`）。
> :bulb: 使用 Visual Studio 时，IDE 不会根据 **stylecop.json** 中的值自动调整编辑器设置。
> 建议改用 **.editorconfig** 文件以提供此功能。
> 使用 Visual Studio [EditorConfig](https://marketplace.visualstudio.com/items?itemName=EditorConfigTeam.EditorConfig) 扩展的用户，无需手动更新 C# 缩进设置即可匹配项目风格。

<a id="spacing-rules"></a>

## 空格规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的空格规则功能。
各属性均在 **stylecop.json** 文件的 `spacingRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "spacingRules": {
    }
  }
}
```

> 目前空格规则没有可配置的设置。

<a id="readability-rules"></a>

## 可读性规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的可读性规则功能。
各属性均在 **stylecop.json** 文件的 `readabilityRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "readabilityRules": {
    }
  }
}
```

<a id="aliases-for-built-in-types"></a>

### 内置类型的别名

以下属性用于在 **stylecop.json** 中配置内置类型的别名。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `allowBuiltInTypeAliases` | **false** | 1.1.0-beta007 | 指定是否允许内置类型使用别名。 |

默认情况下，SA1121 会对使用内置类型的命名别名报告诊断：

```csharp
using HRESULT = System.Int32;

HRESULT hr = SomeNativeOperation(); // SA1121
```

可将 `allowBuiltInTypeAliases` 配置属性设置为 `true`，以允许这种情况，同时继续对直接引用元数据类型名称 `Int32` 报告诊断。

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.readability.allowBuiltInTypeAliases = true
```

<a id="ordering-rules"></a>

## 排序规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的排序规则功能。
各属性均在 **stylecop.json** 文件的 `orderingRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "orderingRules": {
    }
  }
}
```

<a id="element-order"></a>

### 元素顺序

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的元素排序。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `elementOrder` | `[ "kind", "accessibility", "constant", "static", "readonly" ]` | 1.0.0 | 指定文档内元素排序所依据的特征及其优先顺序。 |

`elementOrder` 属性是元素特征的数组。
排序规则（SA1201、SA1202、SA1203、SA1204、SA1214 和 SA1215）按定义顺序评估这些特征以识别排序问题，代码修复在重新排列代码元素时也使用此属性。
数组中省略的特征会被忽略。
支持以下特征：

* `kind`：按元素种类排序（预定义顺序请参阅 [SA1201](SA1201.md)）。
* `accessibility`：按元素声明的可访问性排序（预定义顺序请参阅 [SA1202](SA1202.md)）。
* `constant`：常量元素位于非常量元素之前。
* `static`：静态元素位于非静态元素之前。
* `readonly`：只读元素位于非只读元素之前。

此配置属性支持多种排序配置，如以下示例所示。

<a id="example-all-constants-first"></a>

#### 示例：所有常量优先

以下示例展示自定义元素顺序，其中*所有*常量字段都位于非常量字段之前，而不考虑可访问性。

```json
{
  "settings": {
    "orderingRules": {
      "elementOrder": [
        "kind",
        "constant",
        "accessibility",
        "static",
        "readonly"
      ]
    }
  }
}
```

<a id="example-ignore-accessibility"></a>

#### 示例：忽略可访问性

以下示例展示自定义元素顺序，其中忽略元素的可访问性，但仍强制执行其他排序规则。

```json
{
  "settings": {
    "orderingRules": {
      "elementOrder": [
        "kind",
        "constant",
        "static",
        "readonly"
      ]
    }
  }
}
```

> :bulb: 也可在 **.editorconfig** 文件中设置此属性：
>
> ```ini
> stylecop.ordering.elementOrder = kind, accessibility, constant, static, readonly
> ```

<a id="using-directives"></a>

### using 指令

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的 using 指令。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `systemUsingDirectivesFirst` | true | 1.0.0 | 指定 `System` using 指令是否位于其他 using 指令之前。 |
| `usingDirectivesPlacement` | `"insideNamespace"` | 1.0.0 | 指定 using 指令的期望位置。 |
| `blankLinesBetweenUsingGroups` | `"allow"` | 1.1.0 | 指定是否要求使用空行分隔 using 语句组。 |

`systemUsingDirectivesFirst` 影响以下规则及其代码修复：

* [SA1208](SA1208.md) 仅在此属性为 `true` 时，报告位于其他 using 指令之后的 `System` using 指令。
* [SA1210](SA1210.md) 在此属性为 `true` 时，将 `System` 命名空间排在其他命名空间之前；为 `false` 时，将所有命名空间一起按字母顺序排序。
* [SA1217](SA1217.md) 在此属性为 `true` 时，将 `System` 类型的 `using static` 指令排在其他 `using static` 指令之前；为 `false` 时，将所有指令一起按字母顺序排序。

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用 [.NET/C# 格式设置](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules)中的对应属性：
```ini
dotnet_sort_system_directives_first = true
csharp_using_directive_placement = inside_namespace
dotnet_separate_import_directive_groups = true
```

> :memo: 通用属性无法表示所有值：`csharp_using_directive_placement` 没有 `preserve`，`dotnet_separate_import_directive_groups` 没有 `omit`。
> 这些值应使用 StyleCop 专用属性；它们优先于通用属性：
>
> ```ini
> stylecop.ordering.usingDirectivesPlacement = preserve
> stylecop.ordering.blankLinesBetweenUsingGroups = omit
> ```

<a id="using-directives-placement"></a>

#### using 指令的位置

`usingDirectivesPlacement` 属性影响以下报告 using 指令位置不正确的规则的行为。

* [SA1200 using 指令应放在正确的位置](SA1200.md)

> :warning: 使用某些功能（包括但不限于预处理器指令）可能导致 using 指令代码修复无法自动移动 using 指令。
> 如果对 using 指令执行“全部修复”操作后仍报告 SA1200，则需要手动解决剩余情况。

此属性有三个允许的值，说明如下。

<a id="insidenamespace"></a>

##### `"insideNamespace"`

此模式下，using 指令应位于命名空间声明的*内部*。
这是默认模式，遵循 StyleCop Classic 中 SA1200 的原始行为。

* SA1200 报告位于命名空间声明之外的 using 指令（在必须如此的少数情况下有例外）。
* using 指令代码修复会尽可能将 using 指令移到命名空间声明内部。

<a id="outsidenamespace"></a>

##### `"outsideNamespace"`

此模式下，using 指令应位于命名空间声明的*外部*。

* SA1200 报告位于命名空间声明内部的 using 指令。
* using 指令代码修复会尽可能将 using 指令移到命名空间声明外部。

<a id="preserve"></a>

##### `"preserve"`

此模式下，using 指令可以位于命名空间内部或外部。

* SA1200 不报告任何违规。
* using 指令代码修复可能重新排序 using 指令，但不会改变其所在位置。

<a id="blank-lines-between-groups"></a>

#### 组之间的空行

`blankLinesBetweenUsingGroups` 属性影响以下报告 using 指令组之间存在或缺少空行的规则的行为。

* [SA1516 元素之间应使用空行分隔](SA1516.md)

using 指令可按其用途分组。
StyleCop Analyzers 识别以下 using 指令组类型：

- System using 指令（仅当 `systemUsingDirectivesFirst` 为 true 时）
- 普通 using 指令
- 静态 using 指令
- 别名 using 指令

此属性有三个允许的值，说明如下。

<a id="allow"></a>

##### `"allow"`

此模式下，using 指令组之间的空行是*可选的*。

* 不会产生诊断。
* using 指令代码修复不会插入空行。

<a id="require"></a>

##### `"require"`

此模式下，using 指令组之间的空行是*必需的*。

* SA1516 报告 using 指令组之间缺少空行的情况。
* using 指令代码修复会插入空行。
* SA1516 代码修复会添加缺少的空行。

<a id="omit"></a>

##### `"omit"`

此模式下，using 指令组之间*不允许*有空行。

* SA1516 报告 using 指令组之间存在空行的情况。
* using 指令代码修复不会插入空行。
* SA1516 代码修复会删除 using 指令组之间的空行。

<a id="naming-rules"></a>

## 命名规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的命名规则功能。
各属性均在 **stylecop.json** 文件的 `namingRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "namingRules": {
    }
  }
}
```

<a id="hungarian-notation"></a>

### 匈牙利命名法

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 允许的匈牙利命名法前缀。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `allowCommonHungarianPrefixes` | **true** | 1.0.0 | 指定是否允许常见的非匈牙利命名法前缀。为 true 时，允许双字母单词 'as'、'at'、'by'、'do'、'go'、'if'、'in'、'is'、'it'、'no'、'of'、'on'、'or' 和 'to' 作为变量名称的前缀。 |
| `allowedHungarianPrefixes` | `[ ]` | 1.0.0 | 指定允许在变量名称中使用的其他前缀。有关详细信息，请参阅下面的示例。 |

以下示例展示允许常见前缀以及自定义前缀 'md' 和 'cd' 的设置文件。

```json
{
  "settings": {
    "namingRules": {
      "allowedHungarianPrefixes": [
        "cd",
        "md"
      ]
    }
  }
}
```

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes = cd, md
```

<a id="namespace-components"></a>

### 命名空间组成部分

以下属性用于在 **stylecop.json** 中配置允许的命名空间组成部分（例如以小写字母开头的部分）。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `allowedNamespaceComponents` | `[ ]` | 1.2.0 | 指定允许使用的命名空间组成部分。有关详细信息，请参阅下面的示例。 |

以下示例展示允许 `eBay` 或 `Apple.iPod` 等命名空间组成部分的设置文件。

```json
{
  "settings": {
    "namingRules": {
      "allowedNamespaceComponents": [
        "eBay",
        "iPod"
      ]
    }
  }
}
```

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.naming.allowedNamespaceComponents = eBay, iPod
```

<a id="tuple-element-names"></a>

### 元组元素名称

以下属性用于在 **stylecop.json** 中配置元组元素名称分析器的行为。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `includeInferredTupleElementNames` | false | 1.2.0 | 指定是否也分析推断的元组元素名称。显式元素名称始终会被分析，包括元组表达式中的显式名称。 |
| `tupleElementNameCasing` | "PascalCase" | 1.2.0 | 指定元组元素名称使用的大小写约定。 |

以下示例展示要求所有元组元素（包括推断的元素名称）使用驼峰大小写的设置文件。

```json
{
  "settings": {
    "namingRules": {
      "includeInferredTupleElementNames": true,
      "tupleElementNameCasing" : "camelCase"
    }
  }
}
```

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.naming.includeInferredTupleElementNames = true
stylecop.naming.tupleElementNameCasing = camelCase
```

<a id="tuple-element-name-casing"></a>

#### 元组元素名称的大小写

`tupleElementNameCasing` 属性影响 [SA1316 元组元素名称应使用正确的大小写](SA1316.md)分析器的行为。

此属性有两个允许的值，说明如下。

<a id="camelcase"></a>

##### `"camelCase"`

此模式下，元组元素名称必须以小写字母开头。

<a id="pascalcase"></a>

##### `"PascalCase"`

此模式下，元组元素名称必须以大写字母开头。

<a id="maintainability-rules"></a>

## 可维护性规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的可维护性规则功能。
各属性均在 **stylecop.json** 文件的 `maintainabilityRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "maintainabilityRules": {
    }
  }
}
```

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的可维护性规则。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `topLevelTypes` | `[ "class" ]` | 1.1.0 | 指定应放在单独文件中的类型种类。 |

`topLevelTypes` 属性是一个数组，指定根据 SA1402 规则应放在单独文件中的类型种类。
支持以下类型：
* `class`
* `interface`
* `struct`
* `enum`
* `delegate`

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性（以逗号分隔，名称与上面相同）：
```ini
stylecop.maintainability.topLevelTypes = class
```

<a id="layout-rules"></a>

## 布局规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的布局规则功能。
各属性均在 **stylecop.json** 文件的 `layoutRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "layoutRules": {
    }
  }
}
```

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的布局规则。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `newlineAtEndOfFile` | `"allow"` | 1.0.0 | 指定如何处理文件末尾的换行符。 |
| `allowConsecutiveUsings` | `true` | 1.1.0 | 指定 SA1519 是否允许不带大括号的连续 using 语句。 |
| `allowDoWhileOnClosingBrace` | `false` | >1.2.0 | 指定 SA1500 是否允许 `do`/`while` 循环的 `while` 表达式与右大括号位于同一行，即 Visual Studio 默认代码片段生成的形式。 |

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用 editorconfig.org 介绍的换行设置（`insert_final_newline`），以及以下附加属性：
```ini
stylecop.layout.newlineAtEndOfFile = allow
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowDoWhileOnClosingBrace = false
```

> :memo: `insert_final_newline = true` 对应 `"require"`，`false` 对应 `"omit"`。
> 两者都无法表示 `"allow"`，因此此值应使用 `stylecop.layout.newlineAtEndOfFile`。
> StyleCop 专用属性优先。

<a id="lines-at-end-of-file"></a>

### 文件末尾的换行

可自定义 [SA1518](SA1518.md) 处理文件末尾换行符的行为。
`newlineAtEndOfFile` 属性支持以下值：

* `"allow"`：允许文件以单个换行符结尾，但不作要求。
* `"require"`：要求文件以单个换行符结尾。
* `"omit"`：文件不得以换行符结尾。

<a id="consecutive-using-statements-without-braces"></a>

### 不带大括号的连续 using 语句

可自定义 [SA1519](SA1519.md) 处理不带大括号的连续 using 语句的行为。
`allowConsecutiveUsings` 属性指定以下行为：

* `true`：不带大括号的连续 using 语句不会产生诊断。
* `false`：不带大括号的连续 using 语句会产生 SA1519 诊断。

这仅允许在 using 语句后紧跟另一个 using 语句时省略大括号。
using 语句后跟任何其他类型的语句时，仍然必须使用大括号。

<a id="do-while-loop-placement"></a>

### do-while 循环的位置

可自定义 [SA1500](SA1500.md) 对 `do`/`while` 循环中 `while` 表达式位置的要求。
`allowDoWhileOnClosingBrace` 属性指定以下行为：

* `true`：`do`/`while` 循环的 `while` 表达式可以与右大括号位于同一行，也可以单独占一行。
* `false`：`do`/`while` 循环的 `while` 表达式必须与右大括号分行。

<a id="documentation-rules"></a>

## 文档规则

本节介绍可在 **stylecop.json** 或 **.editorconfig** 中配置的文档规则功能。
各属性均在 **stylecop.json** 文件的 `documentationRules` 对象中配置，如以下示例所示。

```json
{
  "settings": {
    "documentationRules": {
    }
  }
}
```

<a id="copyright-headers"></a>

### 版权文件头

以下属性用于在 **stylecop.json** 中配置 StyleCop Analyzers 的版权文件头。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `companyName` | `"PlaceholderCompany"` | 1.0.0 | 指定版权声明中应出现的公司名称。 |
| `copyrightText` | `"Copyright (c) {companyName}. All rights reserved."` | 1.0.0 | 指定版权文件头中应出现的默认版权文本。 |
| `xmlHeader` | **true** | 1.0.0 | 指定文件头是否应使用标准 StyleCop XML 格式，即将版权声明包裹在 `<copyright>` 元素中。 |
| `variables` | 不适用 | 1.0.0 | 指定可在 `copyrightText` 值中引用的替换变量。 |
| `headerDecoration` | 不适用 | 1.1.0 | 设置此值可为文件头注释添加装饰，使文件头类似于 StyleCop Classic ReSharper 修复生成的文件头。 |

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.xmlHeader = true
stylecop.documentation.headerDecoration = -----------------
```

> :memo: 可使用 [IDE0073（要求文件头）](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0073)中介绍的 `file_header_template` 属性，代替 `stylecop.documentation.copyrightText`。
> 但 StyleCop 专用属性优先。

> :bulb: 可在 **.editorconfig** 文件中使用 `stylecop.documentation.variables.<name> = <value>` 设置变量。
> EditorConfig 会将属性名称转换为小写，因此 `<name>` 的匹配不区分大小写。
> 这需要能够提供已配置键列表的编译器版本（Roslyn 4.4 / Visual Studio 2022 17.4 或更新版本）。

<a id="configuring-copyright-text"></a>

#### 配置版权文本

为成功使用经过 StyleCop 检查的文件头，大多数项目需要配置 `companyName` 属性。

> `companyName` 属性经常需要自定义，因此代码修复生成的默认 **stylecop.json** 文件中包含此属性。

`copyrightText` 属性是可包含占位符的字符串。
每个占位符的格式为 `{variable}`，其中 `variable` 是内置变量（见下文），或 `variables` 属性中某个属性的名称。
以下示例展示自定义 **stylecop.json** 文件，其中 `copyrightText` 同时引用 `companyName` 和两个自定义变量。

```json
{
  "settings": {
    "documentationRules": {
      "companyName": "FooCorp",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information.",
      "variables": {
        "licenseName": "MIT",
        "licenseFile": "LICENSE"
      }
    }
  }
}
```

使用上述配置时，文件 **TypeName.cs** 应包含以下文件头。

```csharp
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
```

<a id="built-in-variables"></a>

##### 内置变量

| 变量 | 含义 |
| --- | --- |
| `companyName` | **stylecop.json** 中 `companyName` 配置属性的值。 |
| `fileName` | 当前源文件的文件名。 |

> :memo: 如果 **stylecop.json** 的 `variables` 属性中显式包含 `fileName` 变量，则使用该值，而不是当前源文件的名称。

<a id="configuring-xml-headers"></a>

#### 配置 XML 文件头

当 `xmlHeader` 属性为 **true**（默认值）时，StyleCop Analyzers 要求文件头符合以下标准 StyleCop 格式。

```csharp
// <copyright file="{fileName}" company="{companyName}">
// {copyrightText}
// </copyright>
```

当 `xmlHeader` 属性被显式设置为 **false** 时，StyleCop Analyzers 要求文件头符合以下可自定义格式。

```csharp
// {copyrightText}
```

<a id="configuring-copyright-text-header-decoration"></a>

#### 配置版权文本文件头装饰

`headerDecoration` 属性是一个字符串，可包含用于装饰生成的文件头的文本，使其类似于 StyleCop Classic ReSharper 修复生成的文件头。

`headerDecoration` 属性的默认值为空，因此不会添加装饰。

> :memo: 文件头装饰不会被检查，仅用于修复文件头。

```json
{
  "settings": {
    "documentationRules": {
      "companyName": "FooCorp",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "headerDecoration": "-----------------------------------------------------------------------"
    }
  }
}
```

使用上述配置时，文件 **TypeName.cs** 的修复结果如下。

```csharp
// -----------------------------------------------------------------------
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
```

<a id="documentation-requirements"></a>

### 文档要求

StyleCop Analyzers 包含默认要求开发人员为代码库的大部分内容编写文档的规则。
对于未在整个开发过程中使用 StyleCop 的团队，这种要求很容易造成负担。
为引导开发人员逐步建立文档完善的代码库，**stylecop.json** 提供了若干属性，可逐渐提高文档要求。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `documentInterfaces` | **true** | 1.0.0 | 指定是否需要为接口成员编写文档。为 true 时，无论可访问性如何，所有接口成员都需要文档。 |
| `documentExposedElements` | **true** | 1.0.0 | 指定是否需要为公开的元素编写文档。为 true 时，所有公开可访问的类型和成员都需要文档。 |
| `documentInternalElements` | **true** | 1.0.0 | 指定是否需要为内部元素编写文档。为 true 时，所有内部可访问的类型和成员都需要文档。 |
| `documentPrivateElements` | **false** | 1.0.0 | 指定是否需要为私有元素编写文档。为 true 时，除声明为私有的字段外，所有类型和成员都需要文档。 |
| `documentPrivateFields` | **false** | 1.0.0 | 指定是否需要为私有字段编写文档。为 true 时，无论可访问性如何，所有字段都需要文档。 |

这些属性影响以下报告缺少文档的规则的行为。
报告文档不正确或不完整的规则仍然适用于代码中的所有文档注释。

* [SA1600 元素应具有文档](SA1600.md)
* [SA1601 分部元素应具有文档](SA1601.md)
* [SA1602 枚举项应具有文档](SA1602.md)

以下示例展示要求开发人员为所有公共可访问的成员以及所有接口（无论可访问性如何）编写文档，但不要求为其他内部或私有成员编写文档的配置文件。

> :memo: 与为整个代码库编写文档相比，为接口编写文档所需的工作量较小，但价值很高，因为它涵盖了最可能影响跨团队使用场景的代码部分。

```json
{
  "settings": {
    "documentationRules": {
      "documentInterfaces": true,
      "documentInternalElements": false
    }
  }
}
```

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.documentation.documentInterfaces = true
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
```

<a id="documentation-culture"></a>

### 文档区域性

某些文档规则要求摘要文本以特定字符串开头。
为了让团队能够使用母语编写代码文档，**stylecop.json** 包含 `documentationCulture` 属性。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `documentationCulture` | `"en-US"` |  1.1.0 | 指定某些文档文本所使用的区域性或语言。 |

此属性影响以下报告文档不正确的规则的行为。

* [SA1623 属性摘要文档应与访问器匹配](SA1623.md)
* [SA1624 属性摘要文档应省略访问权限受限的 set 访问器](SA1624.md)
* [SA1642 构造函数摘要文档应以标准文本开头](SA1642.md)
* [SA1643 析构函数摘要文档应以标准文本开头](SA1643.md)

> :memo: `documentationCulture` 的默认值是固定的，不随用户的系统语言变化。
> 这样可确保在同一项目中工作的不同开发人员始终使用相同的值。

目前支持以下值。
不支持的值会自动回退到默认值。

* `"cs-CZ"`
* `"de-DE"`
* `"en-GB"`
* `"en-US"`
* `"es-MX"`
* `"fr-FR"`
* `"nl-NL"`
* `"pl-PL"`
* `"pt-BR"`
* `"ru-RU"`
* `"zh-CN"`

```json
{
  "settings": {
    "documentationRules": {
      "documentationCulture": "de-DE"
    }
  }
}
```

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.documentation.documentationCulture = de-DE
```

<a id="file-naming-conventions"></a>

### 文件命名约定

**stylecop.json** 中的 `fileNamingConvention` 属性决定 [SA1649 文件名应与类型名称匹配](SA1649.md)分析器如何检查文件名。

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `fileNamingConvention` | `"stylecop"` |  1.0.0 | 指定泛型的文件命名约定。 |

对于以下代码：

```csharp
public class Class1<T1, T2, T3>
{
}
```

分析器要求文件名符合下表。
未设置 `fileNamingConvention` 属性时，默认使用 `stylecop` 约定。

文件命名约定 | 期望的文件名
-----------------------| ------------------
stylecop               | Class1{T1,T2,T3}.cs
metadata               | Class1`3.cs

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.documentation.fileNamingConvention = stylecop
```

<a id="text-ending-with-a-period"></a>

### 以句号结尾的文本

[SA1629 文档文本必须以句号结尾](SA1629.md)分析器检查 XML 文档的各个部分是否以句号结尾。
可在 **stylecop.json** 中使用以下属性来控制分析器行为：

| 属性 | 默认值 | 最低版本 | 摘要 |
| --- | --- | --- | --- |
| `excludeFromPunctuationCheck` | `[ "seealso" ]` |  1.1.0 | 指定 XML 文档中不参与分析的顶级标记。 |

使用 **.editorconfig** 文件配置 StyleCop Analyzers 时，可使用以下属性：
```ini
stylecop.documentation.excludeFromPunctuationCheck = seealso
```

<a id="sharing-configuration-among-solutions"></a>

## 在解决方案之间共享配置

可只定义一次首选配置，并在多个独立项目中重复使用。
具体方法是发布自己的 NuGet 包，其中包含 `stylecop.json` 配置，还可以包含自定义规则集文件。
自定义 `.props` 文件将该配置关联到使用此 NuGet 包的项目。

`.nuspec` 文件示例：

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <id>acme.stylecop</id>
    <version>1.0.0</version>
    <dependencies>
      <dependency id="StyleCop.Analyzers" version="1.0.2" />
    </dependencies>
  </metadata>
  <files>
    <file src="stylecop.json" target="" />
    <file src="acme.stylecop.ruleset" target="" />
    <file src="acme.stylecop.props" target="build" />
  </files>
</package>
```

`.props` 文件示例：

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
      <CodeAnalysisRuleSet>$(MSBuildThisFileDirectory)..\acme.stylecop.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
  <ItemGroup>
    <AdditionalFiles Include="$(MSBuildThisFileDirectory)..\stylecop.json" Link="stylecop.json" />
  </ItemGroup>
</Project>
```
