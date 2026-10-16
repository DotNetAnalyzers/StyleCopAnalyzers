# Nastavení analyzátorů StyleCop

StyleCop Analyzers má dva druhy konfigurace: **která pravidla se spouštějí a jak jsou důležitá** a **nastavení**, která upřesňují jednotlivá pravidla (například název společnosti používaný v záhlaví souborů).

**Doporučení:** pro obojí používejte soubor **.editorconfig** nebo **.globalconfig**.
Všechna nastavení lze zadat právě tam, takže stačí jeden soubor.
**stylecop.json** a soubory sady pravidel nadále fungují v existujících projektech a níže je také popsána migrace z nich.

| Mechanismus | Závažnosti pravidel | Nastavení |
| --- | --- | --- |
| **.editorconfig** / **.globalconfig** (doporučeno) | Ano (`dotnet_diagnostic.<ID>.severity`) | Vše (`variables` vyžaduje Roslyn 4.4 nebo novější) |
| **stylecop.json** | Ne | Vše |
| Soubory sad pravidel (`.ruleset`) | Ano | Ne |

Pokud se stejné nastavení nachází v souborech **stylecop.json** a **.editorconfig**/**.globalconfig**, použije se
hodnota z **stylecop.json**.

## Jednotná konfigurace pomocí .editorconfig nebo .globalconfig

```ini
# .editorconfig (nebo .globalconfig bez záhlaví oddílu [*.cs] a s `is_global = true` na začátku)
root = true

[*.cs]
# Závažnosti pravidel
dotnet_diagnostic.SA1633.severity = none
dotnet_diagnostic.SA1309.severity = none

dotnet_analyzer_diagnostic.category-StyleCop.CSharp.DocumentationRules.severity = none

# Nastavení
stylecop.documentation.companyName = Contoso
csharp_using_directive_placement = outside_namespace
```

* Závažnost pravidla používá standardní vlastnost `dotnet_diagnostic.<rule ID>.severity` (`error`, `warning`, `suggestion`,
  `silent`, `none`). Viz [konfigurace analyzátorů kódu](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/configuration-files).
* Závažnost celé kategorie nastavíte pomocí prefixu `dotnet_analyzer_diagnostic.category-` a úplného názvu kategorie, například `StyleCop.CSharp.DocumentationRules`. Zobrazované názvy jako `Pravidla dokumentace` nejsou platné identifikátory kategorií.
* Soubory **.editorconfig** se uplatňují pro jednotlivé zdrojové soubory podle záhlaví oddílů (například `[*.cs]`) a hierarchie složek.
* Soubor **.globalconfig** se automaticky použije, pokud se jmenuje přesně **.globalconfig** a je umístěn ve složce
  projektu nebo nad ní. Jiné názvy souborů vyžadují přidání souboru do položky MSBuild `GlobalAnalyzerConfigFiles`.
  Globální konfigurační soubory nemají záhlaví oddílů: na začátek napište `is_global = true` a potom vlastnosti.
* Neexistuje žádná výchozí šablona ke zkopírování. Vyberte jen závažnosti a nastavení, která chcete změnit; výchozí hodnoty
  jsou zdokumentované v následujících oddílech. Seznam všech pravidel a jejich výchozích závažností najdete ve
  [stavu implementace pravidel](RuleStatus.md) a na stránce jednotlivých pravidel.
* SA0001 nelze zakázat pomocí **.editorconfig** (omezení Roslynu). Použijte soubor sady pravidel nebo `NoWarn`.
* Názvy vlastností nerozlišují velká a malá písmena. Logické hodnoty jsou `true` nebo `false` a `unset` ignoruje hodnotu nastavenou v nadřazeném souboru.

## Použití stylecop.json

Pokud se rozhodnete konfigurovat analyzátory pomocí souboru stylecop.json, musíte jej do projektu zahrnout jako dodatečný soubor.

Upravte soubor projektu následovně, aby se použila nastavení a vlastní pravidla:

```xml
<ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
</ItemGroup>
```

Soubor `stylecop.json` můžete vytvořit podle následující šablony:

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
  }
}
```

Mnoho editorů JSON nabízí ověřování a automatické doplňování na základě schématu uvedeného ve vlastnosti `$schema`.

## Reference nastavení

Tato tabulka uvádí všechna nastavení, způsob jejich nastavení pro každý mechanismus, povolené hodnoty a výchozí hodnotu. Podrobnosti ke každému
nastavení jsou v následujících oddílech.

| Oddíl | **stylecop.json** (objekt `settings`) | **.editorconfig** / **.globalconfig** | Hodnoty | Výchozí hodnota |
| --- | --- | --- | --- | --- |
| Odsazení | `indentation.indentationSize` | `indent_size` | celé číslo | `4` |
| Odsazení | `indentation.tabSize` | `tab_width` | celé číslo | `4` |
| Odsazení | `indentation.useTabs` | `indent_style` | `true`/`false` ⇄ `tab`/`space` | `false` / `space` |
| Čitelnost | `readabilityRules.allowBuiltInTypeAliases` | `stylecop.readability.allowBuiltInTypeAliases` | logická hodnota | `false` |
| Řazení | `orderingRules.elementOrder` | `stylecop.ordering.elementOrder` | seznam položek `kind`, `accessibility`, `constant`, `static`, `readonly` | `kind, accessibility, constant, static, readonly` |
| Řazení | `orderingRules.memberKindOrder` | `stylecop.ordering.memberKindOrder` | seznam druhů členů (viz [Pořadí druhů členů](#member-kind-order)) | předdefinované pořadí SA1201 |
| Řazení | `orderingRules.systemUsingDirectivesFirst` | `dotnet_sort_system_directives_first` | logická hodnota | `true` |
| Řazení | `orderingRules.usingDirectivesPlacement` | `stylecop.ordering.usingDirectivesPlacement` (nebo `csharp_using_directive_placement`) | `insideNamespace`, `outsideNamespace`, `preserve` ⇄ `inside_namespace`, `outside_namespace` | `insideNamespace` |
| Řazení | `orderingRules.blankLinesBetweenUsingGroups` | `stylecop.ordering.blankLinesBetweenUsingGroups` (nebo `dotnet_separate_import_directive_groups`) | `allow`, `require`, `omit` ⇄ `false`, `true` | `allow` |
| Pojmenování | `namingRules.allowCommonHungarianPrefixes` | `stylecop.naming.allowCommonHungarianPrefixes` | logická hodnota | `true` |
| Pojmenování | `namingRules.allowedHungarianPrefixes` | `stylecop.naming.allowedHungarianPrefixes` | seznam oddělený čárkami | prázdný |
| Pojmenování | `namingRules.allowedNamespaceComponents` | `stylecop.naming.allowedNamespaceComponents` | seznam oddělený čárkami | prázdný |
| Pojmenování | `namingRules.includeInferredTupleElementNames` | `stylecop.naming.includeInferredTupleElementNames` | logická hodnota | `false` |
| Pojmenování | `namingRules.tupleElementNameCasing` | `stylecop.naming.tupleElementNameCasing` | `camelCase`, `pascalCase` | `pascalCase` |
| Udržovatelnost | `maintainabilityRules.topLevelTypes` | `stylecop.maintainability.topLevelTypes` | seznam typů `class`, `interface`, `struct`, `enum`, `delegate` | `class` |
| Rozložení | `layoutRules.newlineAtEndOfFile` | `stylecop.layout.newlineAtEndOfFile` (nebo `insert_final_newline`) | `allow`, `require`, `omit` ⇄ `true`, `false` | `allow` |
| Rozložení | `layoutRules.allowConsecutiveUsings` | `stylecop.layout.allowConsecutiveUsings` | logická hodnota | `true` |
| Rozložení | `layoutRules.allowBracelessIfStatements` | `stylecop.layout.allowBracelessIfStatements` | `off`, `sameLine`, `nextLine` | `off` |
| Rozložení | `layoutRules.allowDoWhileOnClosingBrace` | `stylecop.layout.allowDoWhileOnClosingBrace` | logická hodnota | `false` |
| Dokumentace | `documentationRules.documentExposedElements` | `stylecop.documentation.documentExposedElements` | logická hodnota | `true` |
| Dokumentace | `documentationRules.documentInternalElements` | `stylecop.documentation.documentInternalElements` | logická hodnota | `true` |
| Dokumentace | `documentationRules.documentPrivateElements` | `stylecop.documentation.documentPrivateElements` | logická hodnota | `false` |
| Dokumentace | `documentationRules.documentPrivateFields` | `stylecop.documentation.documentPrivateFields` | logická hodnota | `false` |
| Dokumentace | `documentationRules.documentInterfaces` | `stylecop.documentation.documentInterfaces` | `all`, `exposed`, `none` (nebo logická hodnota) | `all` |
| Dokumentace | `documentationRules.companyName` | `stylecop.documentation.companyName` | text | `PlaceholderCompany` |
| Dokumentace | `documentationRules.copyrightText` | `stylecop.documentation.copyrightText` (nebo `file_header_template`) | text; povoleno `\n` a `\r` | `Copyright (c) {companyName}. All rights reserved.` |
| Dokumentace | `documentationRules.variables.<name>` | `stylecop.documentation.variables.<name>` (vyžaduje Roslyn 4.4 nebo novější) | text | žádná |
| Dokumentace | `documentationRules.headerDecoration` | `stylecop.documentation.headerDecoration` | text | žádná |
| Dokumentace | `documentationRules.xmlHeader` | `stylecop.documentation.xmlHeader` | logická hodnota | `true` |
| Dokumentace | `documentationRules.fileNamingConvention` | `stylecop.documentation.fileNamingConvention` | `stylecop`, `metadata` | `stylecop` |
| Dokumentace | `documentationRules.documentationCulture` | `stylecop.documentation.documentationCulture` | název jazykové verze | `en-US` |
| Dokumentace | `documentationRules.excludeFromPunctuationCheck` | `stylecop.documentation.excludeFromPunctuationCheck` | seznam oddělený čárkami | `seealso` |

> :memo: Pokud řádek uvádí v závorkách obecnou vlastnost, má při současném nastavení obou vlastností přednost vlastnost
> specifická pro StyleCop. Obecné vlastnosti sdílí .NET SDK i IDE, proto jsou praktické, pokud je již používáte.

### Komentovaná šablona

Editory nenabízejí doplňování klíčů `stylecop.*`, proto tato šablona uvádí všechna nastavení s výchozími hodnotami. Zkopírujte
pouze řádky, které chcete změnit; nezměněné výchozí hodnoty není nutné uvádět.

```ini
[*.cs]
# Odsazení
indent_size = 4
tab_width = 4
indent_style = space                      # tab | space

# Čitelnost
stylecop.readability.allowBuiltInTypeAliases = false

# Řazení
stylecop.ordering.elementOrder = kind, accessibility, constant, static, readonly
dotnet_sort_system_directives_first = true
stylecop.ordering.usingDirectivesPlacement = insideNamespace   # insideNamespace | outsideNamespace | preserve
stylecop.ordering.blankLinesBetweenUsingGroups = allow         # allow | require | omit

# Pojmenování
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes =                     # seznam oddělený čárkami
stylecop.naming.allowedNamespaceComponents =                   # seznam oddělený čárkami
stylecop.naming.includeInferredTupleElementNames = false
stylecop.naming.tupleElementNameCasing = pascalCase            # camelCase | pascalCase

# Udržovatelnost
stylecop.maintainability.topLevelTypes = class                 # class, interface, struct, enum, delegate

# Rozložení
stylecop.layout.newlineAtEndOfFile = allow                     # allow | require | omit
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowBracelessIfStatements = off
stylecop.layout.allowDoWhileOnClosingBrace = false

# Dokumentace
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
stylecop.documentation.documentInterfaces = all                # all | exposed | none
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.variables.myvariable = value             # vyžaduje Roslyn 4.4 nebo novější
stylecop.documentation.headerDecoration =
stylecop.documentation.xmlHeader = true
stylecop.documentation.fileNamingConvention = stylecop         # stylecop | metadata
stylecop.documentation.documentationCulture = en-US
stylecop.documentation.excludeFromPunctuationCheck = seealso   # seznam oddělený čárkami
```

> :warning: Hodnota nesmí obsahovat `#` ani `;`: kompilátor považuje vše od kteréhokoli z těchto znaků do konce řádku
> za komentář. To je důležité u hodnot, jako jsou `copyrightText`, `headerDecoration` (například `#####`) a
> proměnné. Pokud tyto znaky potřebujete, použijte pro dané nastavení **stylecop.json**. Koncové komentáře v této šabloně
> proto můžete bezpečně ponechat.

## Přechod ze stylecop.json

1. Vytvořte soubor **.editorconfig** (nebo **.globalconfig**) a přidejte do něj každé nastavení podle výše uvedené tabulky
   *Reference nastavení*. Cesta JSON `documentationRules.companyName` se převede na `stylecop.documentation.companyName`;
   obecně se `<section>Rules.<name>` převede na `stylecop.<section>.<name>`, s výjimkou `indentation` (používá
   `indent_size`, `tab_width`, `indent_style`) a výše uvedených obecných vlastností.
2. Převeďte pole na seznamy oddělené čárkami, například `["a", "b"]` na `a, b`.
3. Převeďte každou položku `variables` na vlastnost `stylecop.documentation.variables.<name>`. Názvy proměnných se
   v **.editorconfig** porovnávají jako malá písmena, takže odkaz `{name}` v `copyrightText` se shoduje bez ohledu na
   velikost písmen.
4. Proměnné se neslučují: pokud **stylecop.json** definuje nějaké položky `variables`, vlastnosti
   `stylecop.documentation.variables.*` se ignorují.
5. Odstraňte **stylecop.json** (a případnou položku `AdditionalFiles`, která na něj odkazuje). Pokud jsou přítomny oba
   soubory, mají přednost hodnoty z **stylecop.json**.

> :warning: Čtení položek `variables` ze souboru **.editorconfig** vyžaduje kompilátor, který zpřístupňuje nakonfigurované
> klíče (Roslyn 4.4 / Visual Studio 2022 17.4 nebo novější). Pokud potřebujete podporovat starší kompilátory, ponechte
> položky `variables` v souboru **stylecop.json**.

## Přechod ze souborů sad pravidel

Každá položka sady pravidel se převede na vlastnost závažnosti:

| Akce sady pravidel | .editorconfig |
| --- | --- |
| `Action="None"` | `dotnet_diagnostic.SA1000.severity = none` |
| `Action="Hidden"` | `dotnet_diagnostic.SA1000.severity = silent` |
| `Action="Info"` | `dotnet_diagnostic.SA1000.severity = suggestion` |
| `Action="Warning"` | `dotnet_diagnostic.SA1000.severity = warning` |
| `Action="Error"` | `dotnet_diagnostic.SA1000.severity = error` |

Nástroje jako `dotnet format` a Visual Studio mohou tyto položky ze sady pravidel také vytvořit.

Starší mechanismy jsou popsané ve zbývající části dokumentu.

## Sady pravidel

Sady pravidel pro analýzu kódu jsou standardním způsobem konfigurace většiny diagnostických analyzátorů v prostředí Visual Studio. Informace o vytváření a přizpůsobování těchto souborů najdete v dokumentaci [Použití sad pravidel ke sdružování pravidel analýzy kódu](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) na webu docs.microsoft.com.

Ukázkový soubor sady pravidel obsahující výchozí konfiguraci StyleCop Analyzers je dostupný na adrese <https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/src/StyleCop.Analyzers.CodeFixes/rulesets/StyleCopAnalyzersDefault.ruleset>.

## Začínáme se souborem **stylecop.json**

Nejjednodušší způsob, jak přidat do nového projektu konfigurační soubor **stylecop.json**, je použít opravu kódu poskytovanou projektem. Spusťte ji otevřením libovolného souboru, ve kterém se hlásí SA1633¹, a stisknutím kláves Ctrl+. otevřete nabídku Rychlé opravy. V nabídce vyberte **Přidat do projektu soubor nastavení StyleCop**.

Podporována je také konvence názvů souborů začínajících tečkou, takže konfigurační soubor lze pojmenovat **.stylecop.json**.

### Schéma JSON pro IntelliSense

Pro soubor **stylecop.json** je k dispozici schéma JSON. Pokud do souboru **stylecop.json** přidáte odkaz na toto schéma, Visual Studio při jeho úpravách nabídne funkce IntelliSense (doplňování kódu, rychlé informace apod.). Schéma lze nastavit přidáním následující vlastnosti nejvyšší úrovně do souboru **stylecop.json**:

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"
}
```

> :bulb: Výše popsaná oprava kódu automaticky nastaví odkaz na schéma v souboru **stylecop.json**.
> Pokud se schéma ve Visual Studiu jeví jako zastaralé, klikněte pravým tlačítkem kamkoli do dokumentu **stylecop.json**
> a vyberte **Znovu načíst schémata**.

### Správa zdrojového kódu

Nejlepších výsledků dosáhnete, když do správy zdrojového kódu zahrnete soubor **stylecop.json**. Očekávaná nastavení se tak automaticky zpřístupní všem členům týmu pracujícím na projektu.

> :warning: Pokud používáte Git, ujistěte se, že soubor **.gitignore** *neobsahuje* následující řádek. Pokud jej obsahuje,
> odstraňte jej.
>
> ```
> [Ss]tyle[Cc]op.*
> ```

## Odsazení

Tato část popisuje pravidla odsazení, která lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Každou popsanou vlastnost lze nastavit v objektu `indentation` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "indentation": {
    }
  }
}
```

### Základní odsazení

Následující vlastnosti slouží ke konfiguraci základního odsazení StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `indentationSize` | **4** | 1.1.0 | Počet sloupců použitý pro každou úroveň odsazení kódu. Podle nastavení `useTabs` a `tabSize` se každá úroveň odsazení vytvoří pomocí tabulátorů nebo mezer. |
| `tabSize` | **4** | 1.1.0 | Šířka znaku tabulátoru ve zdrojovém kódu. Tato hodnota se používá při převodu mezi tabulátory a mezerami. |
| `useTabs` | **false** | 1.1.0 | **true** pro odsazování tabulátory; jinak **false** pro odsazování mezerami |

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít základní nastavení odsazení
(`indent_size`, `tab_width` a `indent_style`) popsaná na webu editorconfig.org.
> :bulb: Při práci ve Visual Studiu IDE automaticky neupraví nastavení editoru podle hodnot v souboru **stylecop.json**.
> Tuto funkci zajistíte použitím souboru **.editorconfig**. Uživatelé rozšíření [EditorConfig](https://marketplace.visualstudio.com/items?itemName=EditorConfigTeam.EditorConfig)
> pro Visual Studio pak nebudou muset měnit nastavení odsazení C#, aby odpovídalo stylu projektu.

## Pravidla mezer

Tato část popisuje funkce pravidel mezer, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `spacingRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "spacingRules": {
    }
  }
}
```

> Pro pravidla mezer nejsou v současnosti k dispozici žádná konfigurovatelná nastavení.

## Pravidla čitelnosti

Tato část popisuje funkce pravidel čitelnosti, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `readabilityRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "readabilityRules": {
    }
  }
}
```

### Aliasy předdefinovaných typů

Následující vlastnost slouží ke konfiguraci aliasů předdefinovaných typů v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `allowBuiltInTypeAliases` | **false** | 1.1.0-beta007 | Určuje, zda jsou povoleny aliasy předdefinovaných typů. |

Ve výchozím nastavení SA1121 hlásí diagnostiku u deklarace pojmenovaného aliasu předdefinovaného typu:

```csharp
using HRESULT = System.Int32; // SA1121

HRESULT hr = SomeNativeOperation();
```

Konfigurační vlastnost `allowBuiltInTypeAliases` lze nastavit na `true`, aby se povolily pojmenované aliasy předdefinovaných typů a současně se nadále hlásily diagnostiky pro přímé odkazy na názvy typů metadat, například `Int32`. Diagnostika se nadále hlásí u aliasů, jejichž název sám odpovídá názvu předdefinovaného typu v metadatech (například `using Int32 = System.UInt32;`).

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost:
```ini
stylecop.readability.allowBuiltInTypeAliases = true
```

## Pravidla řazení

Tato část popisuje funkce pravidel řazení, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `orderingRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "orderingRules": {
    }
  }
}
```

### Pořadí prvků

Následující vlastnosti slouží ke konfiguraci řazení prvků StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `elementOrder` | `[ "kind", "accessibility", "constant", "static", "readonly" ]` | 1.0.0 | Určuje vlastnosti používané k řazení prvků v dokumentu a jejich vzájemnou prioritu |

Vlastnost `elementOrder` je pole vlastností prvků. Pravidla řazení (SA1201, SA1202, SA1203, SA1204, SA1214 a SA1215)
vyhodnocují tyto vlastnosti v uvedeném pořadí, aby určily problémy s řazením. Oprava kódu tuto vlastnost používá při
změně pořadí prvků kódu. Vynechané vlastnosti se ignorují. Podporovány jsou tyto vlastnosti:

* `kind`: Prvky se řadí podle druhu (předdefinované pořadí najdete v [SA1201](SA1201.md)).
* `accessibility`: Prvky se řadí podle deklarované přístupnosti (předdefinované pořadí najdete v [SA1202](SA1202.md)).
* `constant`: Konstantní prvky se řadí před nekonstantní.
* `static`: Statické prvky se řadí před nestatické.
* `readonly`: Prvky pouze pro čtení se řadí před ostatní prvky.

Tato konfigurační vlastnost umožňuje řadu různých nastavení řazení, jak ukazují následující příklady.

#### Příklad: nejprve všechny konstanty

Následující příklad ukazuje vlastní pořadí prvků, ve kterém jsou *všechna* konstantní pole před nekonstantními poli
bez ohledu na jejich přístupnost.

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

#### Příklad: ignorování přístupnosti

Následující příklad ukazuje vlastní pořadí prvků, které ignoruje přístupnost prvků, ale ostatní pravidla řazení nadále
uplatňuje.

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

> :bulb: Tuto vlastnost lze nastavit také v souboru **.editorconfig**:
>
> ```ini
> stylecop.ordering.elementOrder = kind, accessibility, constant, static, readonly
> ```

<a id="member-kind-order"></a>
### Pořadí druhů členů

`memberKindOrder` přizpůsobuje vlastnost `kind` pro členy tříd, struktur, rozhraní, záznamů a bloků rozšíření.
Nemění pořadí deklarací na úrovni souboru nebo oboru názvů.
Pokud toto nastavení chybí, [SA1201](SA1201.md) zachová předdefinované pořadí.

| Vlastnost | Výchozí hodnota | Popis |
| --- | --- | --- |
| `memberKindOrder` | Předdefinované pořadí SA1201 | Určuje vzájemné pořadí souvislých skupin druhů členů |

```json
{
  "settings": {
    "orderingRules": {
      "memberKindOrder": [
        "enum",
        "field",
        "property",
        "constructor",
        "method"
      ]
    }
  }
}
```

Odpovídající vlastnost v **.editorconfig** nebo **.globalconfig**:

```ini
stylecop.ordering.memberKindOrder = enum, field, property, constructor, method
```

Podporované hodnoty jsou `field`, `constructor`, `destructor`, `delegate`, `event`, `enum`, `interface`, `extension`, `property`, `indexer`, `conversionOperator`, `operator`, `method`, `struct` a `class`.
Hodnoty musí být jedinečné; neznámé nebo duplicitní hodnoty hlásí SA0002.
Záznamy a třídy záznamů používají `class`; struktury záznamů a sjednocení používají `struct`.
Obě formy deklarace událostí používají `event`.
Jde o druhy deklarací, nikoli sémantické kategorie: například vlastnost typu `ICommand` používá `property`, nikoli samostatný druh příkazu.

Seznam nemusí být úplný.
Uvedené druhy musí být v zadaném pořadí a každý uvedený druh musí tvořit souvislou skupinu.
Neuvedené druhy mohou být před těmito skupinami, za nimi nebo mezi nimi, ale nesmějí rozdělit skupinu uvedeného druhu.
Pro uvedený příklad platí:

* Vlastnost, destruktor, vlastnost: neplatné, protože destruktor rozděluje skupinu vlastností.
* Pole, destruktor, vlastnost: platné.
* Třída, destruktor, třída: platné, protože ani jeden druh nemá zadané pořadí.
* Vlastnost, destruktor, pole: neplatné, protože pole musí předcházet vlastnostem.

SA1201 hlásí první deklaraci neuvedeného druhu v každé mezeře, která rozděluje skupinu uvedeného druhu.
Oprava kódu přesune pozdější deklaraci uvedeného druhu do skupiny a zachová pořadí mezilehlých deklarací neuvedených druhů.
Stejně jako u výchozí opravy pořadí mohou přesuny přes direktivy preprocesoru nebo závislosti inicializátorů polí vyžadovat ruční opravu.
Oprava vlastního pořadí také zachovává pořadí vykonávání inicializátorů polí, událostí a automatických vlastností: nepřesune jeden běhový inicializátor přes jiný ve stejné posloupnosti statické nebo instanční inicializace.

Prázdné pole JSON nebo prázdná hodnota EditorConfig neomezuje pořadí druhů členů, ale nevypíná ostatní vlastnosti řazení.
Vynechání `kind` z `elementOrder` vypne kontroly druhů členů SA1201 bez ohledu na `memberKindOrder`.

Stávající nastavení `elementOrder` nadále určuje prioritu vlastností.
Pokud například `accessibility` předchází `kind`, skupiny druhů členů se kontrolují samostatně v každé souvislé skupině přístupnosti.
Pokud oba soubory určují `memberKindOrder`, má přednost celá hodnota **stylecop.json**; seznamy se neslučují.

<a id="using-directives"></a>
### Direktivy using

Následující vlastnosti slouží ke konfiguraci direktiv `using` v StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `systemUsingDirectivesFirst` | true | 1.0.0 | Určuje, zda se direktivy `using` pro `System` umisťují před ostatní direktivy `using`. |
| `usingDirectivesPlacement` | `"insideNamespace"` | 1.0.0 | Určuje požadované umístění direktiv `using`. |
| `blankLinesBetweenUsingGroups` | `"allow"` | 1.1.0 | Určuje, zda jsou k oddělení skupin direktiv `using` vyžadovány prázdné řádky. |

Vlastnost `systemUsingDirectivesFirst` ovlivňuje následující pravidla a jejich opravy kódu:

* [SA1208](SA1208.md) hlásí direktivy `using` pro `System` umístěné za ostatními direktivami `using` pouze tehdy, když je tato vlastnost nastavena na `true`.
* [SA1210](SA1210.md) řadí obory názvů `System` před ostatní obory názvů, pokud je tato vlastnost nastavena na `true`; při hodnotě `false` řadí všechny obory názvů společně podle abecedy.
* [SA1217](SA1217.md) řadí direktivy `using static` pro typy `System` před ostatní direktivy `using static`, pokud je tato vlastnost nastavena na `true`; při hodnotě `false` je řadí společně podle abecedy.

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít odpovídající vlastnosti pro
[formátování .NET/C#](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules):
```ini
dotnet_sort_system_directives_first = true
csharp_using_directive_placement = inside_namespace
dotnet_separate_import_directive_groups = true
```

> :memo: Obecné vlastnosti neumí vyjádřit všechny hodnoty: `csharp_using_directive_placement` nemá hodnotu `preserve` a
> `dotnet_separate_import_directive_groups` nemá hodnotu `omit`. V těchto případech použijte vlastnosti specifické pro
> StyleCop; mají před obecnými vlastnostmi přednost:
>
> ```ini
> stylecop.ordering.usingDirectivesPlacement = preserve
> stylecop.ordering.blankLinesBetweenUsingGroups = omit
> ```

#### Umístění direktiv using

Vlastnost `usingDirectivesPlacement` ovlivňuje chování následujících pravidel, která hlásí nesprávně umístěné direktivy
`using`.

* [SA1200 Using directives should be placed correctly](SA1200.md)

> :warning: Použití některých funkcí, například direktiv preprocesoru, může zabránit opravě kódu pro direktivy `using`
> v jejich automatickém přesunutí. Pokud se SA1200 hlásí i po použití operace Opravit vše pro direktivy `using`, bude
> nutné zbývající případy vyřešit ručně.

Tato vlastnost má tři povolené hodnoty:

##### `"insideNamespace"`

V tomto režimu se direktivy `using` mají umisťovat *dovnitř* deklarací oborů názvů. Jde o výchozí režim, který
odpovídá původnímu chování SA1200 ve StyleCop Classic.

* SA1200 hlásí direktivy `using` umístěné mimo deklaraci oboru názvů (existuje několik výjimek, kdy je to nutné).
* Oprava kódu pro direktivy `using` je přesune dovnitř deklarací oborů názvů, pokud je to možné.

##### `"outsideNamespace"`

V tomto režimu se direktivy `using` mají umisťovat *mimo* deklarace oborů názvů.

* SA1200 hlásí direktivy `using` umístěné uvnitř deklarace oboru názvů.
* Oprava kódu pro direktivy `using` je přesune mimo deklarace oborů názvů, pokud je to možné.

##### `"preserve"`

V tomto režimu lze direktivy `using` umisťovat dovnitř i mimo obory názvů.

* SA1200 nehlásí žádná porušení.
* Oprava kódu pro direktivy `using` je může seřadit, ale nepřesouvá je.

#### Prázdné řádky mezi skupinami
Vlastnost `blankLinesBetweenUsingGroups` ovlivňuje chování následujících pravidel, která hlásí přítomnost či
nepřítomnost prázdných řádků mezi skupinami direktiv `using`.

* [SA1516 Elements should be separated by blank line](SA1516.md)

Direktivy `using` lze seskupovat podle jejich účelu. StyleCop Analyzers rozpoznává následující typy skupin direktiv `using`:

- Direktivy `using` pro System (pouze pokud je `systemUsingDirectivesFirst` nastaveno na `true`)
- Běžné direktivy `using`
- Statické direktivy `using`
- Direktivy aliasů `using`

Tato vlastnost má tři povolené hodnoty:

##### `"allow"`

V tomto režimu je prázdný řádek mezi skupinami direktiv `using` *volitelný*.

* Nebude hlášena žádná diagnostika.
* Oprava kódu pro direktivy `using` nebude vkládat prázdné řádky.

##### `"require"`

V tomto režimu je prázdný řádek mezi skupinami direktiv `using` *povinný*.

* SA1516 hlásí chybějící prázdné řádky mezi skupinami direktiv `using`.
* Oprava kódu pro direktivy `using` bude vkládat prázdné řádky.
* Oprava kódu SA1516 přidá chybějící prázdný řádek.

##### `"omit"`

V tomto režimu není prázdný řádek mezi skupinami direktiv `using` *povolen*.

* SA1516 hlásí prázdné řádky mezi skupinami direktiv `using`.
* Oprava kódu pro direktivy `using` nebude vkládat prázdné řádky.
* Oprava kódu SA1516 odstraní prázdné řádky mezi skupinami direktiv `using`.

## Pravidla pojmenování

Tato část popisuje funkce pravidel pojmenování, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `namingRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "namingRules": {
    }
  }
}
```

### Maďarská notace

Následující vlastnosti slouží ke konfiguraci povolených předpon maďarské notace v StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `allowCommonHungarianPrefixes` | **true** | 1.0.0 | Určuje, zda se mají povolit běžné předpony, které nejsou maďarskou notací. Je-li hodnota `true`, lze jako předpony názvů proměnných použít dvoupísmenná slova 'as', 'at', 'by', 'do', 'go', 'if', 'in', 'is', 'it', 'no', 'of', 'on', 'or' a 'to'. |
| `allowedHungarianPrefixes` | `[ ]` | 1.0.0 | Určuje další předpony povolené v názvech proměnných. Další informace najdete v následujícím příkladu. |

Následující příklad ukazuje soubor nastavení, který povoluje běžné předpony i vlastní předpony 'md' a 'cd'.

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

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnosti:
```ini
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes = cd, md
```

### Části oboru názvů

Následující vlastnost slouží ke konfiguraci povolených částí oboru názvů (například těch, které začínají malým písmenem) v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `allowedNamespaceComponents` | `[ ]` | 1.2.0 | Určuje povolené části oboru názvů. Další informace najdete v následujícím příkladu. |

Následující příklad ukazuje soubor nastavení, který povoluje části oboru názvů, jako jsou `eBay` nebo `Apple.iPod`.

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

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost:
```ini
stylecop.naming.allowedNamespaceComponents = eBay, iPod
```

### Názvy prvků n-tic

Následující vlastnosti slouží ke konfiguraci chování analyzátorů názvů prvků n-tic v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `includeInferredTupleElementNames` | false | 1.2.0 | Určuje, zda se mají analyzovat i odvozené názvy prvků n-tice. Explicitní názvy prvků, včetně těch ve výrazech n-tic, se analyzují vždy. |
| `tupleElementNameCasing` | "PascalCase" | 1.2.0 | Určuje konvenci velikosti písmen používanou v názvech prvků n-tice. |

Následující příklad ukazuje soubor nastavení, který vyžaduje zápis názvů všech prvků n-tice (včetně odvozených názvů) ve formátu camelCase.

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

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnosti:
```ini
stylecop.naming.includeInferredTupleElementNames = true
stylecop.naming.tupleElementNameCasing = camelCase
```

#### Velikost písmen v názvech prvků n-tic
Vlastnost `tupleElementNameCasing` ovlivňuje chování analyzátoru [SA1316 Názvy prvků n-tic mají používat správnou velikost písmen](SA1316.md).

Tato vlastnost má dvě povolené hodnoty:

##### `"camelCase"`
V tomto režimu musí názvy prvků n-tice začínat malým písmenem.

##### `"PascalCase"`
V tomto režimu musí názvy prvků n-tice začínat velkým písmenem.


## Pravidla udržovatelnosti

Tato část popisuje funkce pravidel udržovatelnosti, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `maintainabilityRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "maintainabilityRules": {
    }
  }
}
```

Následující vlastnosti slouží ke konfiguraci pravidel udržovatelnosti StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `topLevelTypes` | `[ "class" ]` | 1.1.0 | Určuje druhy typů, které mají být umístěny v samostatných souborech. |

Vlastnost `topLevelTypes` je pole určující druhy typů, které mají být podle pravidla SA1402 umístěny v samostatných
souborech. Podporovány jsou následující typy:
* `class`
* `interface`
* `struct`
* `enum`
* `delegate`

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost (názvy oddělené čárkami, stejné jako výše):
```ini
stylecop.maintainability.topLevelTypes = class
```

<a id="layout-rules"></a>
## Pravidla rozložení

Tato část popisuje funkce pravidel rozložení, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `layoutRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "layoutRules": {
    }
  }
}
```

Následující vlastnosti slouží ke konfiguraci pravidel rozložení StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `newlineAtEndOfFile` | `"allow"` | 1.0.0 | Určuje způsob zpracování znaku nového řádku na konci souboru. |
| `allowConsecutiveUsings` | `true` | 1.1.0 | Určuje, zda SA1519 povolí po sobě jdoucí příkazy `using` bez složených závorek. |
| `allowBracelessIfStatements` | `"off"` | >1.2.0 | Allows a single-line jump statement as the body of an `if` with no `else`, on the configured line |
| `allowDoWhileOnClosingBrace` | `false` | >1.2.0 | Určuje, zda SA1500 povolí umístit výraz `while` cyklu `do`/`while` na stejný řádek jako uzavírací složenou závorku, jak to vytváří výchozí fragment kódu ve Visual Studiu. |

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít nastavení nového řádku (`insert_final_newline`) popsané na webu editorconfig.org a také následující vlastnosti:
```ini
stylecop.layout.newlineAtEndOfFile = allow
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowBracelessIfStatements = off
stylecop.layout.allowDoWhileOnClosingBrace = false
```

> :memo: `insert_final_newline = true` odpovídá hodnotě `"require"` a `false` hodnotě `"omit"`. Ani jedna z nich nevyjadřuje
> `"allow"`, proto pro tuto hodnotu použijte `stylecop.layout.newlineAtEndOfFile`. Vlastnost specifická pro StyleCop má přednost.

### Braceless if statements

The `allowBracelessIfStatements` property configures [SA1501](SA1501.md) and [SA1503](SA1503.md).
`"off"` (default) preserves the existing behavior. `"sameLine"` allows a standalone `if` to omit braces
when its single jump statement is on the same line as its closing `)`. `"nextLine"` allows the jump on
the immediately following line. The jump itself must fit on one line.
The entire `if` header, from `if` through the closing `)`, must fit on one line for either enabled option.
With `"sameLine"`, the whole `if` statement must therefore fit on one line. Multiline conditions require braces.

Supported jumps are `return` (with or without a value), `throw`, `break`, `continue`, `goto` (including
`goto case` and `goto default`), and `yield break`. `yield return`, assignments, method calls, loops,
and all branches of an `if`/`else if`/`else` chain still require braces. Existing braces remain allowed.
SA1503 fixes add braces; they do not remove existing braces. With SA1503 disabled, SA1501 fixes expand
disallowed same-line statements onto the next line. Correctly positioned guards are not changed.

```json
{
  "settings": {
    "layoutRules": {
      "allowBracelessIfStatements": "sameLine"
    }
  }
}
```

```csharp
if (value == null) throw new ArgumentNullException(nameof(value));
```

With `"nextLine"`:

```csharp
if (value == null)
    throw new ArgumentNullException(nameof(value));
```

### Řádky na konci souboru

Chování pravidla [SA1518](SA1518.md) lze přizpůsobit tak, aby určovalo způsob zpracování znaku nového řádku na konci
souboru. Vlastnost `newlineAtEndOfFile` podporuje následující hodnoty:

* `"allow"`: Soubory mohou končit jedním znakem nového řádku, není to však povinné.
* `"require"`: Soubory musí končit jedním znakem nového řádku.
* `"omit"`: Soubory nesmí končit znakem nového řádku.

### Po sobě jdoucí příkazy using bez složených závorek

Chování pravidla [SA1519](SA1519.md) lze přizpůsobit tak, aby určovalo zacházení s po sobě jdoucími příkazy `using` bez
složených závorek. Chování určuje vlastnost `allowConsecutiveUsings`:

* `true`: po sobě jdoucí příkazy `using` bez složených závorek nevyvolají diagnostiku.
* `false`: po sobě jdoucí příkazy `using` bez složených závorek vyvolají diagnostiku SA1519.

Toto nastavení povoluje vynechat složené závorky pouze u příkazu `using`, po kterém následuje další příkaz `using`.
Příkaz `using`, za kterým následuje jakýkoli jiný typ příkazu, bude i nadále vyžadovat složené závorky.

### Umístění cyklu do-while

Chování pravidla [SA1500](SA1500.md) lze přizpůsobit nastavením umístění výrazu `while` v cyklu `do`/`while`.
Chování určuje vlastnost `allowDoWhileOnClosingBrace`:

* `true`: výraz `while` cyklu `do`/`while` lze umístit na stejný řádek jako uzavírací složenou závorku nebo na samostatný řádek.
* `false`: výraz `while` cyklu `do`/`while` musí být na samostatném řádku od uzavírací složené závorky.

## Pravidla dokumentace

Tato část popisuje funkce pravidel dokumentace, které lze konfigurovat v souborech **stylecop.json** nebo **.editorconfig**. Všechny popsané vlastnosti se nastavují v objektu `documentationRules` souboru **stylecop.json**, jak ukazuje následující příklad.

```json
{
  "settings": {
    "documentationRules": {
    }
  }
}
```

### Hlavičky s informacemi o autorských právech

Následující vlastnosti slouží ke konfiguraci hlaviček s informacemi o autorských právech v StyleCop Analyzers v souboru **stylecop.json**.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `companyName` | `"PlaceholderCompany"` | 1.0.0 | Určuje název společnosti, který se má zobrazit v oznámeních o autorských právech. |
| `copyrightText` | `"Copyright (c) {companyName}. All rights reserved."` | 1.0.0 | Určuje výchozí text o autorských právech, který se má zobrazit v hlavičkách souborů. |
| `xmlHeader` | **true** | 1.0.0 | Určuje, zda mají hlavičky souborů používat standardní formát XML StyleCop, v němž je oznámení o autorských právech obklopeno elementem `<copyright>`. |
| `variables` | n/a | 1.0.0 | Určuje proměnné nahrazující text, na které lze odkazovat v hodnotě `copyrightText`. |
| `headerDecoration` | n/a | 1.1.0 | Touto hodnotou lze ozdobit komentář hlavičky, aby se hlavičky podobaly těm, které vytváří oprava ReSharperu ve StyleCop Classic. |

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnosti:
```ini
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.xmlHeader = true
stylecop.documentation.headerDecoration = -----------------
```

> :memo: Místo `stylecop.documentation.copyrightText` lze použít vlastnost `file_header_template` popsanou v pravidle
> [IDE0073 (Vyžadovat hlavičku souboru)](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0073).
> Vlastnost specifická pro StyleCop má přednost.

> :bulb: Proměnné lze nastavit v souboru **.editorconfig** pomocí zápisu
> `stylecop.documentation.variables.<name> = <value>`. EditorConfig převádí názvy vlastností na malá písmena, proto se
> `<name>` porovnává bez rozlišení velikosti písmen. To vyžaduje verzi kompilátoru, která zpřístupňuje seznam
> nakonfigurovaných klíčů (Roslyn 4.4 / Visual Studio 2022 17.4 nebo novější).

#### Konfigurace textu o autorských právech

Většina projektů musí nastavit vlastnost `companyName`, aby bylo možné správně používat hlavičky souborů kontrolované
StyleCopem.

> Vlastnost `companyName` se upravuje tak často, že je zahrnuta ve výchozím souboru **stylecop.json**, který vytváří
> oprava kódu.

Vlastnost `copyrightText` je řetězec, který může obsahovat zástupné symboly. Každý zástupný symbol má tvar `{variable}`,
kde `variable` je buď předdefinovaná proměnná (viz níže), nebo název vlastnosti v položce `variables`. Následující
příklad ukazuje vlastní soubor **stylecop.json**, jehož hodnota `copyrightText` odkazuje na `companyName` a dvě vlastní
proměnné.

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

Při výše uvedené konfiguraci se v souboru **TypeName.cs** očekává následující hlavička.

```csharp
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
```

##### Předdefinované proměnné

| Proměnná | Význam |
| --- | --- |
| `companyName` | Hodnota konfigurační vlastnosti `companyName` v souboru **stylecop.json** |
| `fileName` | Název aktuálního zdrojového souboru |

> :memo: Pokud je proměnná `fileName` výslovně uvedena ve vlastnosti `variables` souboru **stylecop.json**, použije se
> její hodnota namísto názvu aktuálního zdrojového souboru.

#### Konfigurace hlaviček XML

Je-li vlastnost `xmlHeader` nastavena na **true** (výchozí hodnota), StyleCop Analyzers očekává, že hlavičky souborů
budou odpovídat následujícímu standardnímu formátu StyleCop.

```csharp
// <copyright file="{fileName}" company="{companyName}">
// {copyrightText}
// </copyright>
```

Je-li vlastnost `xmlHeader` výslovně nastavena na **false**, StyleCop Analyzers očekává, že hlavičky souborů budou
odpovídat následujícímu přizpůsobitelnému formátu.

```csharp
// {copyrightText}
```

#### Konfigurace ozdobení hlavičky s textem o autorských právech

Vlastnost `headerDecoration` je řetězec, který může obsahovat text používaný k ozdobení generované hlavičky, aby se
podobala hlavičkám vytvářeným opravou ReSharperu ve StyleCop Classic.

Výchozí hodnota vlastnosti `headerDecoration` je prázdná, takže se žádné ozdobení nepřidá.

> :memo: Ozdobení hlavičky se nekontroluje; používá se pouze při opravě hlavičky.

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

Při výše uvedené konfiguraci by opravená hlavička souboru **TypeName.cs** vypadala následovně.

```csharp
// -----------------------------------------------------------------------
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
```

### Požadavky na dokumentaci

StyleCop Analyzers ve výchozím nastavení obsahuje pravidla vyžadující dokumentaci většiny kódové základny. Tento
požadavek může snadno zahltit tým, který StyleCop nepoužíval po celou dobu vývoje. Aby bylo možné postupně zvyšovat
požadavky na dokumentaci a vést vývojáře k řádně dokumentované kódové základně, nabízí **stylecop.json** několik
vlastností.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `documentInterfaces` | **true** | 1.0.0 | Určuje, zda je třeba dokumentovat členy rozhraní. Při hodnotě `true` vyžadují dokumentaci všichni členové rozhraní bez ohledu na přístupnost. |
| `documentExposedElements` | **true** | 1.0.0 | Určuje, zda je třeba dokumentovat zpřístupněné prvky. Při hodnotě `true` vyžadují dokumentaci všechny veřejně zpřístupněné typy a členy. |
| `documentInternalElements` | **true** | 1.0.0 | Určuje, zda je třeba dokumentovat interní prvky. Při hodnotě `true` vyžadují dokumentaci všechny interně zpřístupněné typy a členy. |
| `documentPrivateElements` | **false** | 1.0.0 | Určuje, zda je třeba dokumentovat soukromé prvky. Při hodnotě `true` vyžadují dokumentaci všechny typy a členy kromě výslovně deklarovaných soukromých polí. |
| `documentPrivateFields` | **false** | 1.0.0 | Určuje, zda je třeba dokumentovat soukromá pole. Při hodnotě `true` vyžadují dokumentaci všechna pole bez ohledu na přístupnost. |

Tyto vlastnosti ovlivňují následující pravidla hlásící chybějící dokumentaci, i když již část dokumentace existuje:

* [SA1600 Elements should be documented](SA1600.md)
* [SA1601 Partial elements should be documented](SA1601.md)
* [SA1602 Enumeration items should be documented](SA1602.md)
* [SA1604 Element documentation should have summary](SA1604.md)
* [SA1605 Partial element documentation should have summary](SA1605.md)
* [SA1609 Property documentation should have value](SA1609.md)
* [SA1611 Element parameters should be documented](SA1611.md)
* [SA1615 Element return value should be documented](SA1615.md)
* [SA1618 Generic type parameters should be documented](SA1618.md)
* [SA1619 Generic type parameters should be documented partial class](SA1619.md)

U prvku, který nevyžaduje dokumentaci, samotný tag `<summary>` nevyžaduje doplnění tagů
`<param>`, `<returns>`, `<typeparam>` ani `<value>`. Poskytnutá dokumentace se však nadále kontroluje:
prázdný tag `<param>` hlásí SA1614 a prázdný tag `<returns>` hlásí SA1616. Nadále se uplatňují také
kontroly interpunkce (SA1629) a standardního znění souhrnu.

Přístupnost se vyhodnocuje v kontextu obsahujících typů: veřejný člen interní třídy je přístupný interně,
nikoli veřejně. Nastavení nejsou nezávislé výjimky:
`documentPrivateElements: true` vyžaduje dokumentaci všech typů a členů kromě výslovně deklarovaných
soukromých polí, i když je `documentInternalElements` nebo `documentExposedElements` nastaveno na false.
`documentPrivateFields: true` vyžaduje dokumentaci všech polí. Rozhraní a jejich členy řídí samostatně
`documentInterfaces`; samotné vypnutí dokumentace interních prvků nevyjímá interní rozhraní,
pokud má `documentInterfaces` hodnotu `true` (`all`).

Následující příklad ukazuje konfigurační soubor, který vyžaduje dokumentaci všech veřejně přístupných členů a všech
rozhraní (bez ohledu na jejich přístupnost), ale nevyžaduje dokumentaci ostatních interních ani soukromých členů.

> :memo: Dokumentování rozhraní vyžaduje ve srovnání s dokumentováním celé kódové základny jen malé úsilí, ale přináší
> velký užitek, protože pokrývá části kódu, které nejspíše ovlivní používání napříč týmy.


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

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnosti:
```ini
stylecop.documentation.documentInterfaces = true
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
```

### Jazyková verze dokumentace

Některá pravidla dokumentace vyžadují, aby texty souhrnů začínaly určitými řetězci. Aby týmy mohly dokumentovat kód ve
svém jazyce, obsahuje **stylecop.json** vlastnost `documentationCulture`.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `documentationCulture` | `"en-US"` |  1.1.0 | Určuje jazykovou verzi nebo jazyk použitý pro určité texty dokumentace. |

Tato vlastnost ovlivňuje chování následujících pravidel, která hlásí nesprávnou dokumentaci.

* [SA1623 Property summary documentation should match accessors](SA1623.md)
* [SA1624 Property summary documentation should omit set accessor with restricted access](SA1624.md)
* [SA1642 Constructor summary documentation should begin with standard text](SA1642.md)
* [SA1643 Destructor summary documentation should begin with standard text](SA1643.md)

> :memo: Výchozí hodnota `documentationCulture` je pevně stanovena a neodvozuje se od jazyka systému uživatele. Díky
> tomu všichni vývojáři pracující na stejném projektu vždy používají stejnou hodnotu.

V současnosti jsou podporovány následující hodnoty. Nepodporované hodnoty se automaticky převedou na výchozí hodnotu.

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

```json
{
  "settings": {
    "documentationRules": {
      "documentationCulture": "de-DE"
    }
  }
}
```

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost:
```ini
stylecop.documentation.documentationCulture = de-DE
```

### Konvence pojmenování souborů

Vlastnost `fileNamingConvention` v souboru **stylecop.json** určuje, jak analyzátor [SA1649 Název souboru má odpovídat názvu typu](SA1649.md) kontroluje názvy souborů.

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `fileNamingConvention` | `"stylecop"` |  1.0.0 | Určuje konvenci názvů souborů pro obecné typy. |

Pro následující kód:

```csharp
public class Class1<T1, T2, T3>
{
}
```

Analyzátor bude očekávat názvy souborů podle následující tabulky. Pokud vlastnost `fileNamingConvention` není nastavena,
použije se jako výchozí konvence `stylecop`.

Konvence názvu souboru | Očekávaný název souboru
----------------------|------------------------
stylecop               | Class1{T1,T2,T3}.cs
metadata               | Class1`3.cs

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost:
```ini
stylecop.documentation.fileNamingConvention = stylecop
```

### Text zakončený tečkou

Analyzátor [SA1629 Text dokumentace musí končit tečkou](SA1629.md) kontroluje, zda oddíly dokumentace XML končí
tečkou. Chování analyzátoru lze řídit následujícími vlastnostmi v souboru **stylecop.json**:

| Vlastnost | Výchozí hodnota | Minimální verze | Popis |
| --- | --- | --- | --- |
| `excludeFromPunctuationCheck` | `[ "seealso" ]` |  1.1.0 | Určuje značky nejvyšší úrovně v dokumentaci XML, které se vynechají z analýzy. |

Při konfiguraci StyleCop Analyzers pomocí souboru **.editorconfig** lze použít následující vlastnost:
```ini
stylecop.documentation.excludeFromPunctuationCheck = seealso
```

## Sdílení konfigurace mezi řešeními

Preferovanou konfiguraci lze definovat jednou a opakovaně používat v několika nezávislých projektech. K tomu je třeba
vytvořit vlastní balíček NuGet, který bude obsahovat konfiguraci `stylecop.json` a případně také vlastní soubor sady
pravidel. Vlastní soubor `.props` propojí tuto konfiguraci s každým projektem, který balíček NuGet použije.

Příklad souboru `.nuspec`:

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

Příklad souboru `.props`:

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
