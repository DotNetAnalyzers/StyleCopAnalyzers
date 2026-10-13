# 在 .NET Core 中使用 StyleCop Analyzers

StyleCop Analyzers 可与 **dotnet** 工具一起使用，包括 ASP.NET Core。

## .NET SDK 项目 (*.csproj)

编辑项目文件，添加对 **StyleCop.Analyzers** 的包引用。
请务必设置 **PrivateAssets**，以便计算项目引用之间的传递依赖关系时不包含此引用：

```xml
<ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.1.0-beta004" PrivateAssets="All" />
</ItemGroup>
```

此时还原并生成项目即可运行分析器。
若要配置分析器，还需要完成一些额外步骤。

### 规则集与 stylecop.json

按如下方式更新项目文件，以应用设置和自定义规则：

```xml
<PropertyGroup>
    ...
    <CodeAnalysisRuleSet>stylecop.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
<ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

### 启用 XML 文档处理

只有启用 XML 文档注释处理后，XML 文档分析器才能运行。
有关详细信息，请参阅 [SA0001](SA0001.md)。

更新项目文件，加入以下内容：

```xml
<PropertyGroup>
    ...
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## 旧式项目 (*.xproj)

旧式项目使用 **project.json** 配置分析器和其他生成选项。
首先在 **project.json** 的 `dependencies` 节中添加以下内容：

```json
"StyleCop.Analyzers": {
  "version": "1.0.2",
  "type": "build"
}
```

在 `buildOptions` 节中添加以下内容，即可配置规则集和配置文件：

```json
"additionalArguments": [
  "/ruleset:path/to/ruleset.ruleset",
  "/additionalfile:path/to/stylecop.json"
],
"xmlDoc": "true"
```
