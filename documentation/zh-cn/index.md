<style>
#breadcrumb { display: none; }
</style>

# 概览

StyleCop.Analyzers 为 C# 代码中违反代码风格和一致性规则的情况提供警告和代码修复。
这些分析器通过 NuGet 包分发。

[![NuGet 包](https://img.shields.io/nuget/vpre/StyleCop.Analyzers?label=StyleCop.Analyzers)](https://nuget.org/packages/StyleCop.Analyzers)

单击上方徽章，按照适用于你的环境的说明安装分析器包。
这将为你的项目启用分析器。

这些分析器提供了许多[配置选项](Configuration.md)。

## 启用 XML 文档处理

只有启用 XML 文档注释处理后，XML 文档分析器才能运行。
有关详细信息，请参阅 [SA0001](SA0001.md)。

更新项目文件，加入以下内容：

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
