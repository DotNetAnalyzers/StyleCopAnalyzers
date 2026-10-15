<style>
#breadcrumb { display: none; }
</style>

# Обзор

StyleCop.Analyzers выдаёт предупреждения и предлагает исправления кода при нарушении правил стиля и единообразия в коде C#.
Эти анализаторы распространяются в виде пакета NuGet.

[![Пакет NuGet](https://img.shields.io/nuget/vpre/StyleCop.Analyzers?label=StyleCop.Analyzers)](https://nuget.org/packages/StyleCop.Analyzers)

Нажмите на значок выше и следуйте инструкциям для вашей среды, чтобы установить пакет анализаторов.
Это активирует анализаторы для вашего проекта.

Для этих анализаторов доступно множество [параметров настройки](Configuration.md).

## Включение обработки XML-документации

Анализаторы XML-документации могут запускаться только при включённой обработке XML-комментариев документации.
Дополнительные сведения приведены в [SA0001](SA0001.md).

Добавьте в файл проекта следующее:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
