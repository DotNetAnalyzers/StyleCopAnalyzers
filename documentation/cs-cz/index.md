<style>
#breadcrumb { display: none; }
</style>

# Přehled

StyleCop.Analyzers poskytuje varování a opravy kódu pro pravidla stylu a konzistence v kódu C#.
Tyto analyzátory jsou distribuovány prostřednictvím balíčku NuGet.

[![Balíček NuGet](https://img.shields.io/nuget/vpre/StyleCop.Analyzers?label=StyleCop.Analyzers)](https://nuget.org/packages/StyleCop.Analyzers)

Klikněte na odznak výše a podle pokynů pro vaše prostředí nainstalujte balíček analyzátorů.
Tím se analyzátory aktivují pro váš projekt.

Pro tyto analyzátory je k dispozici mnoho [možností konfigurace](Configuration.md).

## Povolení zpracování XML dokumentace

Analyzátory pro dokumentaci XML mohou běžet pouze tehdy, pokud je povoleno zpracování komentářů XML dokumentace.
Více informací najdete v [SA0001](SA0001.md).

Upravte soubor projektu tak, aby obsahoval následující:

```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
