# Использование StyleCop Analyzers с .NET Core

StyleCop Analyzers можно использовать с инструментами **dotnet**, в том числе с ASP.NET Core.

## Проекты .NET SDK (*.csproj)

Отредактируйте файл проекта и добавьте ссылку на пакет **StyleCop.Analyzers**. Обязательно задайте **PrivateAssets**, чтобы
эта ссылка не включалась при вычислении транзитивных зависимостей через ссылки на проекты:

```xml
<ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.1.0-beta004" PrivateAssets="All" />
</ItemGroup>
```

Если сейчас восстановить зависимости и собрать проект, анализаторы уже будут запускаться. Для их настройки нужны ещё несколько действий.

### Наборы правил и stylecop.json

Измените файл проекта следующим образом, чтобы применить настройки и пользовательские правила:

```xml
<PropertyGroup>
    ...
    <CodeAnalysisRuleSet>stylecop.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
<ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

### Включение обработки XML-документации

Анализаторы XML-документации могут запускаться только при включённой обработке XML-комментариев документации. Дополнительные сведения приведены в [SA0001](SA0001.md).

Добавьте в файл проекта следующее:

```xml
<PropertyGroup>
    ...
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## Устаревшие проекты (*.xproj)

Устаревшие проекты используют **project.json** для настройки анализаторов и других параметров сборки. Сначала добавьте в
раздел `dependencies` файла **project.json** следующее:

```json
"StyleCop.Analyzers": {
  "version": "1.0.2",
  "type": "build"
}
```

Набор правил и файл конфигурации можно задать, добавив в раздел `buildOptions` следующее:

```json
"additionalArguments": [
  "/ruleset:path/to/ruleset.ruleset",
  "/additionalfile:path/to/stylecop.json"
],
"xmlDoc": "true"
```
