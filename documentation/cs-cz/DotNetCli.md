# Použití StyleCop.Analyzers s .NET CLI

StyleCop Analyzers lze používat s nástroji **dotnet**, včetně ASP.NET Core.

## Projekty .NET SDK (*.csproj)

Upravte soubor projektu a přidejte odkaz na balíček **StyleCop.Analyzers**. Ujistěte se, že nastavíte **PrivateAssets**, aby se odkaz nepočítal do přenosných závislostí při přepočtu odkazů mezi projekty:

```xml
<ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" Version="1.1.0-beta004" PrivateAssets="All" />
</ItemGroup>
```

Pokud je projekt nyní obnoven a sestaven, analyzátory se spouští už teď. Pokud mají být nakonfigurovány, je potřeba provést několik dalších kroků.

### Pravidla a stylecop.json

Upravte soubor projektu následovně, aby se použila nastavení a vlastní pravidla:

```xml
<PropertyGroup>
    ...
    <CodeAnalysisRuleSet>stylecop.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
<ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

### Povolení zpracování XML dokumentace

Analyzátory pro dokumentaci XML mohou běžet pouze tehdy, pokud je povoleno zpracování komentářů XML dokumentace. Více informací najdete v [SA0001](SA0001.md).

Upravte soubor projektu tak, aby obsahoval následující:

```xml
<PropertyGroup>
    ...
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## Starší projekty (*.xproj)

Starší projekty používají pro konfiguraci analyzátorů a dalších možností sestavení **project.json**. Začněte přidáním následujícího do sekce `dependencies` souboru **project.json**:

```json
"StyleCop.Analyzers": {
  "version": "1.0.2",
  "type": "build"
}
```

Sadu pravidel a konfigurační soubor lze nastavit přidáním následujícího do sekce `buildOptions`:

```json
"additionalArguments": [
  "/ruleset:path/to/ruleset.ruleset",
  "/additionalfile:path/to/stylecop.json"
],
"xmlDoc": "true"
```
