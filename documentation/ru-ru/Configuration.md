<a id="configuring-stylecop-analyzers"></a>

# Настройка StyleCop Analyzers

Конфигурация StyleCop Analyzers бывает двух видов: **какие правила выполняются и насколько они серьёзны** и **параметры**,
которые точно настраивают отдельные правила (например, название компании, используемое в заголовках файлов).

**Рекомендация:** используйте для обоих видов файл **.editorconfig** или файл **.globalconfig**. В них можно задать
любой параметр, поэтому одного файла достаточно. **stylecop.json** и файлы наборов правил по-прежнему работают в
существующих проектах; они описаны ниже вместе со способами перехода с них.

| Механизм | Уровни серьёзности правил | Параметры |
| --- | --- | --- |
| **.editorconfig** / **.globalconfig** (рекомендуется) | Да (`dotnet_diagnostic.<ID>.severity`) | Все (`variables` требует Roslyn 4.4 или новее) |
| **stylecop.json** | Нет | Все |
| Файлы наборов правил (`.ruleset`) | Да | Нет |

Если один и тот же параметр задан и в **stylecop.json**, и в файле **.editorconfig**/**.globalconfig**, используется
значение из **stylecop.json**.

<a id="unified-configuration-with-editorconfig-or-globalconfig"></a>

## Единая конфигурация с помощью .editorconfig или .globalconfig

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

* Уровень серьёзности правила задаётся стандартным свойством `dotnet_diagnostic.<идентификатор правила>.severity`
  (`error`, `warning`, `suggestion`, `silent`, `none`). См. [Настройка анализаторов кода](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/configuration-files).
* Файлы **.editorconfig** применяются к каждому исходному файлу в соответствии с заголовками разделов (например,
  `[*.cs]`) и иерархией папок.
* Файл **.globalconfig** подхватывается автоматически, если он называется именно **.globalconfig** и расположен в папке
  проекта или выше. Для других имён файл необходимо добавить в элемент MSBuild `GlobalAnalyzerConfigFiles`. В глобальных
  файлах конфигурации нет заголовков разделов: сначала запишите `is_global = true`, затем свойства.
* Шаблона по умолчанию для копирования нет. Выберите только те уровни серьёзности и параметры, которые хотите изменить;
  значения по умолчанию описаны в разделах ниже. Список всех правил и их уровней серьёзности по умолчанию приведён в
  [состоянии реализации правил](RuleStatus.md) и на странице каждого правила.
* SA0001 нельзя отключить через **.editorconfig** (ограничение Roslyn). Используйте файл набора правил или `NoWarn`.
* Имена свойств не чувствительны к регистру. Логические значения — `true` или `false`, а `unset` отменяет значение,
  заданное в родительском файле.

<a id="settings-reference"></a>

## Справочник параметров

В этой таблице перечислены все параметры, способы их задания в каждом механизме, допустимые значения и значения по
умолчанию. Подробности по каждому параметру приведены в следующих разделах.

| Раздел | **stylecop.json** (объект `settings`) | **.editorconfig** / **.globalconfig** | Значения | По умолчанию |
| --- | --- | --- | --- | --- |
| Отступы | `indentation.indentationSize` | `indent_size` | целое число | `4` |
| Отступы | `indentation.tabSize` | `tab_width` | целое число | `4` |
| Отступы | `indentation.useTabs` | `indent_style` | `true`/`false` ⇄ `tab`/`space` | `false` / `space` |
| Читаемость | `readabilityRules.allowBuiltInTypeAliases` | `stylecop.readability.allowBuiltInTypeAliases` | логическое | `false` |
| Упорядочивание | `orderingRules.elementOrder` | `stylecop.ordering.elementOrder` | список из `kind`, `accessibility`, `constant`, `static`, `readonly` | `kind, accessibility, constant, static, readonly` |
| Упорядочивание | `orderingRules.systemUsingDirectivesFirst` | `dotnet_sort_system_directives_first` | логическое | `true` |
| Упорядочивание | `orderingRules.usingDirectivesPlacement` | `stylecop.ordering.usingDirectivesPlacement` (или `csharp_using_directive_placement`) | `insideNamespace`, `outsideNamespace`, `preserve` ⇄ `inside_namespace`, `outside_namespace` | `insideNamespace` |
| Упорядочивание | `orderingRules.blankLinesBetweenUsingGroups` | `stylecop.ordering.blankLinesBetweenUsingGroups` (или `dotnet_separate_import_directive_groups`) | `allow`, `require`, `omit` ⇄ `false`, `true` | `allow` |
| Именование | `namingRules.allowCommonHungarianPrefixes` | `stylecop.naming.allowCommonHungarianPrefixes` | логическое | `true` |
| Именование | `namingRules.allowedHungarianPrefixes` | `stylecop.naming.allowedHungarianPrefixes` | список через запятую | пусто |
| Именование | `namingRules.allowedNamespaceComponents` | `stylecop.naming.allowedNamespaceComponents` | список через запятую | пусто |
| Именование | `namingRules.includeInferredTupleElementNames` | `stylecop.naming.includeInferredTupleElementNames` | логическое | `false` |
| Именование | `namingRules.tupleElementNameCasing` | `stylecop.naming.tupleElementNameCasing` | `camelCase`, `pascalCase` | `pascalCase` |
| Сопровождаемость | `maintainabilityRules.topLevelTypes` | `stylecop.maintainability.topLevelTypes` | список из `class`, `interface`, `struct`, `enum`, `delegate` | `class` |
| Оформление | `layoutRules.newlineAtEndOfFile` | `stylecop.layout.newlineAtEndOfFile` (или `insert_final_newline`) | `allow`, `require`, `omit` ⇄ `true`, `false` | `allow` |
| Оформление | `layoutRules.allowConsecutiveUsings` | `stylecop.layout.allowConsecutiveUsings` | логическое | `true` |
| Оформление | `layoutRules.allowDoWhileOnClosingBrace` | `stylecop.layout.allowDoWhileOnClosingBrace` | логическое | `false` |
| Документирование | `documentationRules.documentExposedElements` | `stylecop.documentation.documentExposedElements` | логическое | `true` |
| Документирование | `documentationRules.documentInternalElements` | `stylecop.documentation.documentInternalElements` | логическое | `true` |
| Документирование | `documentationRules.documentPrivateElements` | `stylecop.documentation.documentPrivateElements` | логическое | `false` |
| Документирование | `documentationRules.documentPrivateFields` | `stylecop.documentation.documentPrivateFields` | логическое | `false` |
| Документирование | `documentationRules.documentInterfaces` | `stylecop.documentation.documentInterfaces` | `all`, `exposed`, `none` (или логическое) | `all` |
| Документирование | `documentationRules.companyName` | `stylecop.documentation.companyName` | текст | `PlaceholderCompany` |
| Документирование | `documentationRules.copyrightText` | `stylecop.documentation.copyrightText` (или `file_header_template`) | текст; допускаются `\n` и `\r` | `Copyright (c) {companyName}. All rights reserved.` |
| Документирование | `documentationRules.variables.<name>` | `stylecop.documentation.variables.<name>` (требует Roslyn 4.4 или новее) | текст | нет |
| Документирование | `documentationRules.headerDecoration` | `stylecop.documentation.headerDecoration` | текст | нет |
| Документирование | `documentationRules.xmlHeader` | `stylecop.documentation.xmlHeader` | логическое | `true` |
| Документирование | `documentationRules.fileNamingConvention` | `stylecop.documentation.fileNamingConvention` | `stylecop`, `metadata` | `stylecop` |
| Документирование | `documentationRules.documentationCulture` | `stylecop.documentation.documentationCulture` | название культуры | `en-US` |
| Документирование | `documentationRules.excludeFromPunctuationCheck` | `stylecop.documentation.excludeFromPunctuationCheck` | список через запятую | `seealso` |

> :memo: Если в строке в скобках указано универсальное свойство, то при задании обоих свойств приоритет имеет
> специальное свойство StyleCop. Универсальные свойства являются общими для .NET SDK и среды разработки, поэтому
> удобны, если вы их уже используете.

<a id="annotated-template"></a>

### Шаблон с комментариями

Редакторы не предлагают автодополнение для ключей `stylecop.*`, поэтому в этом шаблоне перечислены все параметры с
их значениями по умолчанию. Скопируйте только те строки, которые хотите изменить; значения по умолчанию указывать не
нужно.

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

> :warning: EditorConfig считает `#` комментарием только в начале строки. Перед использованием строки удалите из этого
> шаблона комментарии в конце строк; иначе они станут частью значения.

<a id="migrating-from-stylecopjson"></a>

## Переход со stylecop.json

1. Создайте файл **.editorconfig** (или **.globalconfig**) и добавьте каждый параметр по таблице *Справочник
   параметров* выше. Путь JSON `documentationRules.companyName` превращается в `stylecop.documentation.companyName`;
   в общем случае JSON `<section>Rules.<name>` превращается в `stylecop.<section>.<name>`, за исключением `indentation`
   (используются `indent_size`, `tab_width`, `indent_style`) и перечисленных выше универсальных свойств.
2. Преобразуйте массивы в списки через запятую, например `["a", "b"]` превращается в `a, b`.
3. Преобразуйте каждый элемент `variables` в свойство `stylecop.documentation.variables.<name>`. Имена переменных в
   **.editorconfig** при сравнении приводятся к нижнему регистру, поэтому ссылка `{name}` в `copyrightText` совпадает
   независимо от регистра.
4. Удалите **stylecop.json** (и соответствующий элемент `AdditionalFiles`, если он есть). Пока существуют оба файла,
   значения из **stylecop.json** имеют приоритет.

> :warning: Чтение `variables` из **.editorconfig** требует компилятора, предоставляющего список заданных ключей
> (Roslyn 4.4 / Visual Studio 2022 17.4 или новее). Если необходимо поддерживать более старые компиляторы, оставьте
> `variables` в **stylecop.json**.

<a id="migrating-from-rule-set-files"></a>

## Переход с файлов наборов правил

Каждый элемент набора правил преобразуется в свойство уровня серьёзности:

| Действие набора правил | .editorconfig |
| --- | --- |
| `Action="None"` | `dotnet_diagnostic.SA1000.severity = none` |
| `Action="Hidden"` | `dotnet_diagnostic.SA1000.severity = silent` |
| `Action="Info"` | `dotnet_diagnostic.SA1000.severity = suggestion` |
| `Action="Warning"` | `dotnet_diagnostic.SA1000.severity = warning` |
| `Action="Error"` | `dotnet_diagnostic.SA1000.severity = error` |

Такие средства, как `dotnet format` и Visual Studio, также могут создать эти записи из набора правил.

Прежние механизмы описаны в остальной части этого документа.

<a id="rule-sets"></a>

## Наборы правил

Наборы правил анализа кода традиционно использовались для настройки большинства диагностических анализаторов в Visual Studio.
Сведения о создании и изменении таких файлов приведены в документации [Использование наборов правил для группировки правил анализа кода](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) на docs.microsoft.com.

Пример файла набора правил с конфигурацией StyleCop Analyzers по умолчанию доступен по адресу <https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/rulesets/StyleCopAnalyzersDefault.ruleset>.

<a id="getting-started-with-stylecopjson"></a>

## Начало работы со **stylecop.json**

Самый простой способ добавить файл конфигурации **stylecop.json** в новый проект — воспользоваться исправлением кода,
предоставляемым проектом. Откройте любой файл, в котором выдаётся SA1633¹, и нажмите Ctrl+., чтобы открыть меню быстрых
исправлений. В меню выберите **Add StyleCop settings file to the project** («Добавить файл настроек StyleCop в проект»).

Также поддерживаются имена файлов с точкой в начале, поэтому файл конфигурации можно назвать **.stylecop.json**.

<a id="json-schema-for-intellisense"></a>

### JSON-схема для IntelliSense

Для **stylecop.json** доступна JSON-схема. Если добавить в **stylecop.json** ссылку на эту схему, Visual Studio будет
предоставлять IntelliSense (завершение кода, краткие сведения и т. д.) при редактировании файла.
Для подключения схемы добавьте следующее свойство верхнего уровня в **stylecop.json**:

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"
}
```

> :bulb: Описанное выше исправление кода автоматически добавляет ссылку на схему в **stylecop.json**.
> Если схема в Visual Studio кажется устаревшей, щёлкните правой кнопкой мыши в любом месте документа **stylecop.json**
> и выберите **Reload Schemas** («Перезагрузить схемы»).

<a id="source-control"></a>

### Управление версиями

Для наилучших результатов файл **stylecop.json** следует включить в систему управления версиями. Это автоматически
распространит требуемые настройки среди всех участников команды, работающих над проектом.

> :warning: Если вы используете Git, убедитесь, что файл **.gitignore** *не содержит* следующую строку.
> Если она присутствует, её следует удалить.
>
> ```
> [Ss]tyle[Cc]op.*
> ```

<a id="indentation"></a>

## Отступы

В этом разделе описаны правила отступов, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства можно задать в объекте `indentation` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "indentation": {
    }
  }
}
```

<a id="basic-indentation"></a>

### Основные настройки отступов

Следующие свойства **stylecop.json** задают основные настройки отступов в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `indentationSize` | **4** | 1.1.0 | Число столбцов для каждого уровня отступа кода. В зависимости от настроек `useTabs` и `tabSize` отступ заполняется символами табуляции и/или пробелами. |
| `tabSize` | **4** | 1.1.0 | Ширина символа табуляции в исходном коде. Это значение используется при преобразовании табуляции в пробелы и обратно. |
| `useTabs` | **false** | 1.1.0 | **true** — использовать для отступов символы табуляции; **false** — использовать пробелы. |

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать основные параметры отступов
(`indent_size`, `tab_width` и `indent_style`), описанные на editorconfig.org.

> :bulb: При работе в Visual Studio среда разработки не изменяет настройки редактора автоматически в соответствии
> со значениями в **stylecop.json**. Для этого рекомендуется использовать файл **.editorconfig**.
> Пользователям расширения [EditorConfig](https://marketplace.visualstudio.com/items?itemName=EditorConfigTeam.EditorConfig)
> для Visual Studio не придётся вручную изменять настройки отступов C#, чтобы они соответствовали стилю проекта.

<a id="spacing-rules"></a>

## Правила расстановки пробелов

В этом разделе описаны возможности правил расстановки пробелов, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `spacingRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "spacingRules": {
    }
  }
}
```

> В настоящее время у правил расстановки пробелов нет настраиваемых параметров.

<a id="readability-rules"></a>

## Правила читаемости

В этом разделе описаны возможности правил читаемости, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `readabilityRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "readabilityRules": {
    }
  }
}
```

<a id="aliases-for-built-in-types"></a>

### Псевдонимы встроенных типов

Следующее свойство **stylecop.json** настраивает псевдонимы встроенных типов.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `allowBuiltInTypeAliases` | **false** | 1.1.0-beta007 | Определяет, разрешены ли псевдонимы встроенных типов. |

По умолчанию SA1121 выдаёт диагностику при использовании именованных псевдонимов встроенных типов:

```csharp
using HRESULT = System.Int32;

HRESULT hr = SomeNativeOperation(); // SA1121
```

Свойство конфигурации `allowBuiltInTypeAliases` можно установить в `true`, чтобы разрешить подобные случаи,
сохранив диагностику для прямых ссылок на имя типа в метаданных — `Int32`.

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство:

```ini
stylecop.readability.allowBuiltInTypeAliases = true
```

<a id="ordering-rules"></a>

## Правила упорядочивания

В этом разделе описаны возможности правил упорядочивания, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `orderingRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "orderingRules": {
    }
  }
}
```

<a id="element-order"></a>

### Порядок элементов

Следующие свойства **stylecop.json** настраивают порядок элементов в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `elementOrder` | `[ "kind", "accessibility", "constant", "static", "readonly" ]` | 1.0.0 | Задаёт признаки, используемые для упорядочивания элементов в документе, и их приоритет. |

Свойство `elementOrder` — массив признаков элементов. Правила упорядочивания (SA1201, SA1202, SA1203, SA1204, SA1214
и SA1215) оценивают эти признаки в порядке их перечисления, чтобы выявить нарушения порядка; исправление кода
использует это свойство при переупорядочивании элементов. Признаки, отсутствующие в массиве, игнорируются.
Поддерживаются следующие признаки:

* `kind`: элементы упорядочиваются по виду (предопределённый порядок приведён в [SA1201](SA1201.md)).
* `accessibility`: элементы упорядочиваются по объявленной доступности (предопределённый порядок приведён в
  [SA1202](SA1202.md)).
* `constant`: константные элементы располагаются перед неконстантными.
* `static`: статические элементы располагаются перед нестатическими.
* `readonly`: элементы только для чтения располагаются перед остальными.

Это свойство позволяет задавать самые разные варианты порядка элементов, как показано в следующих примерах.

<a id="example-all-constants-first"></a>

#### Пример: все константы в начале

Следующий пример задаёт порядок, при котором *все* константные поля располагаются перед неконстантными
независимо от доступности.

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

#### Пример: игнорирование доступности

Следующий пример задаёт порядок, при котором доступность элементов игнорируется, но остальные правила
упорядочивания продолжают действовать.

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

> :bulb: Это свойство также можно задать в файле **.editorconfig**:
>
> ```ini
> stylecop.ordering.elementOrder = kind, accessibility, constant, static, readonly
> ```

<a id="using-directives"></a>

### Директивы using

Следующие свойства **stylecop.json** настраивают директивы using в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `systemUsingDirectivesFirst` | true | 1.0.0 | Определяет, должны ли директивы using для `System` располагаться перед остальными директивами using. |
| `usingDirectivesPlacement` | `"insideNamespace"` | 1.0.0 | Задаёт желаемое расположение директив using. |
| `blankLinesBetweenUsingGroups` | `"allow"` | 1.1.0 | Определяет, нужны ли пустые строки для разделения групп директив using. |

`systemUsingDirectivesFirst` влияет на следующие правила и их исправления кода:

* [SA1208](SA1208.md) сообщает о директивах using для `System`, расположенных после остальных, только когда это свойство равно `true`.
* [SA1210](SA1210.md) располагает пространства имён `System` перед остальными при значении `true`
  и сортирует все пространства имён вместе по алфавиту при значении `false`.
* [SA1217](SA1217.md) располагает директивы `using static` для типов `System` перед остальными директивами `using static`
  при значении `true` и сортирует их все вместе по алфавиту при значении `false`.

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать соответствующие свойства
[форматирования .NET/C#](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules):

```ini
dotnet_sort_system_directives_first = true
csharp_using_directive_placement = inside_namespace
dotnet_separate_import_directive_groups = true
```

> :memo: Универсальные свойства не могут выразить все значения: у `csharp_using_directive_placement` нет `preserve`, а у
> `dotnet_separate_import_directive_groups` нет `omit`. Для них используйте специальные свойства StyleCop; они имеют
> приоритет над универсальными:
>
> ```ini
> stylecop.ordering.usingDirectivesPlacement = preserve
> stylecop.ordering.blankLinesBetweenUsingGroups = omit
> ```

<a id="using-directives-placement"></a>

#### Расположение директив using

Свойство `usingDirectivesPlacement` влияет на следующие правила, сообщающие о неверно расположенных
директивах using.

* [SA1200 Директивы using должны быть правильно расположены](SA1200.md)

> :warning: Использование некоторых возможностей, в том числе директив препроцессора, может препятствовать
> автоматическому перемещению директив using исправлением кода. Если SA1200 всё ещё выдаётся после применения
> операции «Исправить все» для директив using, оставшиеся случаи необходимо исправить вручную.

У этого свойства есть три допустимых значения, описанных ниже.

##### `"insideNamespace"`

В этом режиме директивы using должны располагаться *внутри* объявлений пространств имён. Это режим по умолчанию,
соответствующий исходному поведению SA1200 в StyleCop Classic.

* SA1200 сообщает о директивах using вне объявления пространства имён (есть несколько исключений для случаев,
  когда такое расположение необходимо).
* Исправление директив using по возможности перемещает их внутрь объявлений пространств имён.

##### `"outsideNamespace"`

В этом режиме директивы using должны располагаться *вне* объявлений пространств имён.

* SA1200 сообщает о директивах using внутри объявления пространства имён.
* Исправление директив using по возможности перемещает их за пределы объявлений пространств имён.

##### `"preserve"`

В этом режиме директивы using могут располагаться как внутри, так и вне пространств имён.

* SA1200 не сообщает о нарушениях.
* Исправление директив using может переупорядочить их, но не меняет их расположение.

<a id="blank-lines-between-groups"></a>

#### Пустые строки между группами

Свойство `blankLinesBetweenUsingGroups` влияет на следующие правила, сообщающие о наличии или отсутствии
пустых строк между группами директив using.

* [SA1516 Элементы должны разделяться пустой строкой](SA1516.md)

Директивы using можно группировать по назначению.
StyleCop Analyzers распознаёт следующие виды групп директив using:

- Директивы using для System (только когда `systemUsingDirectivesFirst` равно true)
- Обычные директивы using
- Статические директивы using
- Директивы using с псевдонимами

У этого свойства есть три допустимых значения, описанных ниже.

##### `"allow"`

В этом режиме пустая строка между группами директив using *необязательна*.

* Диагностика не выдаётся.
* Исправление директив using не вставляет пустые строки.

##### `"require"`

В этом режиме пустая строка между группами директив using *обязательна*.

* SA1516 сообщает об отсутствующих пустых строках между группами директив using.
* Исправление директив using вставляет пустые строки.
* Исправление SA1516 добавляет отсутствующую пустую строку.

##### `"omit"`

В этом режиме пустые строки между группами директив using *не допускаются*.

* SA1516 сообщает о пустых строках между группами директив using.
* Исправление директив using не вставляет пустые строки.
* Исправление SA1516 удаляет пустые строки между группами директив using.

<a id="naming-rules"></a>

## Правила именования

В этом разделе описаны возможности правил именования, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `namingRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "namingRules": {
    }
  }
}
```

<a id="hungarian-notation"></a>

### Венгерская нотация

Следующие свойства **stylecop.json** настраивают допустимые префиксы венгерской нотации в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `allowCommonHungarianPrefixes` | **true** | 1.0.0 | Определяет, разрешены ли распространённые префиксы, не являющиеся венгерской нотацией. При значении true двубуквенные слова 'as', 'at', 'by', 'do', 'go', 'if', 'in', 'is', 'it', 'no', 'of', 'on', 'or' и 'to' разрешены в качестве префиксов имён переменных. |
| `allowedHungarianPrefixes` | `[ ]` | 1.0.0 | Задаёт дополнительные префиксы, разрешённые в именах переменных. Дополнительные сведения приведены в примере ниже. |

Следующий пример показывает файл настроек, разрешающий распространённые префиксы, а также пользовательские префиксы 'md' и 'cd'.

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

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующие свойства:

```ini
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes = cd, md
```

<a id="namespace-components"></a>

### Компоненты пространств имён

Следующее свойство **stylecop.json** настраивает допустимые компоненты пространств имён
(например, начинающиеся со строчной буквы).

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `allowedNamespaceComponents` | `[ ]` | 1.2.0 | Задаёт разрешённые компоненты пространств имён. Дополнительные сведения приведены в примере ниже. |

Следующий пример показывает файл настроек, разрешающий компоненты пространств имён, такие как `eBay` или `Apple.iPod`.

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

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство:

```ini
stylecop.naming.allowedNamespaceComponents = eBay, iPod
```

<a id="tuple-element-names"></a>

### Имена элементов кортежей

Следующие свойства **stylecop.json** настраивают поведение анализаторов имён элементов кортежей.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `includeInferredTupleElementNames` | false | 1.2.0 | Определяет, нужно ли анализировать также выведенные имена элементов кортежей. Явно заданные имена, в том числе в выражениях кортежей, анализируются всегда. |
| `tupleElementNameCasing` | "PascalCase" | 1.2.0 | Задаёт соглашение о регистре имён элементов кортежей. |

Следующий пример показывает файл настроек, требующий camelCase для имён всех элементов кортежей
(включая выведенные имена).

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

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующие свойства:

```ini
stylecop.naming.includeInferredTupleElementNames = true
stylecop.naming.tupleElementNameCasing = camelCase
```

<a id="tuple-element-name-casing"></a>

#### Регистр имён элементов кортежей

Свойство `tupleElementNameCasing` влияет на анализатор
[SA1316 Имена элементов кортежей должны использовать правильный регистр](SA1316.md).

У этого свойства есть два допустимых значения, описанных ниже.

##### `"camelCase"`

В этом режиме имена элементов кортежей должны начинаться со строчной буквы.

##### `"PascalCase"`

В этом режиме имена элементов кортежей должны начинаться с прописной буквы.

<a id="maintainability-rules"></a>

## Правила сопровождаемости

В этом разделе описаны возможности правил сопровождаемости, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `maintainabilityRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "maintainabilityRules": {
    }
  }
}
```

Следующие свойства **stylecop.json** настраивают правила сопровождаемости в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `topLevelTypes` | `[ "class" ]` | 1.1.0 | Задаёт виды типов, которые следует размещать в отдельных файлах. |

Свойство `topLevelTypes` — массив, задающий виды типов, которые следует размещать в отдельных файлах
в соответствии с правилом SA1402. Поддерживаются следующие виды:

* `class`
* `interface`
* `struct`
* `enum`
* `delegate`

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство (через запятую, с теми же именами):
```ini
stylecop.maintainability.topLevelTypes = class
```

<a id="layout-rules"></a>

## Правила оформления

В этом разделе описаны возможности правил оформления, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `layoutRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "layoutRules": {
    }
  }
}
```

Следующие свойства **stylecop.json** настраивают правила оформления в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `newlineAtEndOfFile` | `"allow"` | 1.0.0 | Задаёт обработку символов перевода строки в конце файла. |
| `allowConsecutiveUsings` | `true` | 1.1.0 | Определяет, разрешает ли SA1519 последовательные операторы using без фигурных скобок. |
| `allowDoWhileOnClosingBrace` | `false` | >1.2.0 | Определяет, разрешает ли SA1500 размещать выражение `while` цикла `do`/`while` в одной строке с закрывающей фигурной скобкой, как в стандартном фрагменте кода Visual Studio. |

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать параметр перевода строки
(`insert_final_newline`), описанный на editorconfig.org, и следующие дополнительные свойства:

```ini
stylecop.layout.newlineAtEndOfFile = allow
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowDoWhileOnClosingBrace = false
```

> :memo: `insert_final_newline = true` соответствует `"require"`, а `false` — `"omit"`. Ни одно из них не выражает `"allow"`,
> поэтому для него используйте `stylecop.layout.newlineAtEndOfFile`. Специальное свойство StyleCop имеет приоритет.

<a id="lines-at-end-of-file"></a>

### Строки в конце файла

Поведение [SA1518](SA1518.md) можно настроить в отношении обработки символов перевода строки в конце файла.
Свойство `newlineAtEndOfFile` поддерживает следующие значения:

* `"allow"`: файл может заканчиваться одним символом перевода строки, но это не обязательно.
* `"require"`: файл должен заканчиваться одним символом перевода строки.
* `"omit"`: файл не должен заканчиваться символом перевода строки.

<a id="consecutive-using-statements-without-braces"></a>

### Последовательные операторы using без фигурных скобок

Поведение [SA1519](SA1519.md) можно настроить в отношении последовательных операторов using без фигурных скобок.
Свойство `allowConsecutiveUsings` определяет это поведение:

* `true`: последовательные операторы using без фигурных скобок не вызывают диагностику.
* `false`: последовательные операторы using без фигурных скобок вызывают диагностику SA1519.

Это разрешает опускать фигурные скобки только у using, за которым следует другой оператор using.
Если за using следует оператор любого другого вида, фигурные скобки по-прежнему обязательны.

<a id="do-while-loop-placement"></a>

### Расположение цикла do-while

Поведение [SA1500](SA1500.md) можно настроить в отношении допустимого расположения выражения `while` цикла `do`/`while`.
Свойство `allowDoWhileOnClosingBrace` определяет это поведение:

* `true`: выражение `while` цикла `do`/`while` может располагаться в одной строке с закрывающей фигурной скобкой либо в отдельной строке.
* `false`: выражение `while` цикла `do`/`while` должно располагаться отдельно от строки с закрывающей фигурной скобкой.

<a id="documentation-rules"></a>

## Правила документирования

В этом разделе описаны возможности правил документирования, настраиваемые в **stylecop.json** или **.editorconfig**.
Все описанные свойства задаются в объекте `documentationRules` файла **stylecop.json**, показанном в следующем примере.

```json
{
  "settings": {
    "documentationRules": {
    }
  }
}
```

<a id="copyright-headers"></a>

### Заголовки с уведомлением об авторских правах

Следующие свойства **stylecop.json** настраивают заголовки с уведомлением об авторских правах в StyleCop Analyzers.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `companyName` | `"PlaceholderCompany"` | 1.0.0 | Задаёт название компании, которое должно присутствовать в уведомлениях об авторских правах. |
| `copyrightText` | `"Copyright (c) {companyName}. All rights reserved."` | 1.0.0 | Задаёт текст об авторских правах по умолчанию, который должен присутствовать в заголовках. |
| `xmlHeader` | **true** | 1.0.0 | Определяет, должны ли заголовки файлов использовать стандартный XML-формат StyleCop, в котором уведомление об авторских правах заключено в элемент `<copyright>`. |
| `variables` | н/д | 1.0.0 | Задаёт переменные подстановки, на которые можно ссылаться в значении `copyrightText`. |
| `headerDecoration` | н/д | 1.1.0 | Позволяет добавить оформление комментария заголовка, чтобы он был похож на заголовки, создаваемые исправлением StyleCop Classic для ReSharper. |

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующие свойства:

```ini
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.xmlHeader = true
stylecop.documentation.headerDecoration = -----------------
```

> :memo: Вместо `stylecop.documentation.copyrightText` можно использовать свойство `file_header_template`,
> описанное в [IDE0073 (Требовать заголовок файла)](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0073).
> Однако специальное свойство StyleCop имеет приоритет.

> :bulb: Переменные можно задать в файле **.editorconfig** с помощью `stylecop.documentation.variables.<name> = <value>`.
> EditorConfig приводит имена свойств к нижнему регистру, поэтому `<name>` сопоставляется без учёта регистра. Для этого
> требуется компилятор, предоставляющий список заданных ключей (Roslyn 4.4 / Visual Studio 2022 17.4 или новее).

<a id="configuring-copyright-text"></a>

#### Настройка текста об авторских правах

Для успешного использования проверяемых StyleCop заголовков файлов в большинстве проектов необходимо настроить
свойство `companyName`.

> Свойство `companyName` изменяется настолько часто, что оно включено в файл **stylecop.json** по умолчанию,
> создаваемый исправлением кода.

Свойство `copyrightText` — строка, которая может содержать заполнители. Каждый заполнитель имеет вид `{variable}`,
где `variable` — либо встроенная переменная (см. ниже), либо имя свойства в объекте `variables`.
Следующий пример **stylecop.json** ссылается на `companyName` и две пользовательские переменные
в значении `copyrightText`.

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

При такой конфигурации файл **TypeName.cs** должен иметь следующий заголовок.

```csharp
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
```

<a id="built-in-variables"></a>

##### Встроенные переменные

| Переменная | Значение |
| --- | --- |
| `companyName` | Значение свойства конфигурации `companyName` в **stylecop.json**. |
| `fileName` | Имя текущего исходного файла. |

> :memo: Если переменная `fileName` явно задана в объекте `variables` файла **stylecop.json**,
> используется её значение, а не имя текущего исходного файла.

<a id="configuring-xml-headers"></a>

#### Настройка XML-заголовков

Когда `xmlHeader` равно **true** (по умолчанию), StyleCop Analyzers ожидает заголовки файлов в следующем стандартном формате StyleCop.

```csharp
// <copyright file="{fileName}" company="{companyName}">
// {copyrightText}
// </copyright>
```

Когда свойство `xmlHeader` явно установлено в **false**, StyleCop Analyzers ожидает заголовки файлов в следующем настраиваемом формате.

```csharp
// {copyrightText}
```

<a id="configuring-copyright-text-header-decoration"></a>

#### Настройка оформления заголовка с текстом об авторских правах

Свойство `headerDecoration` — строка с текстом, используемым для оформления сгенерированного заголовка,
чтобы он выглядел подобно заголовкам, создаваемым исправлением StyleCop Classic для ReSharper.

По умолчанию `headerDecoration` имеет пустое значение, поэтому оформление не добавляется.

> :memo: Оформление заголовка не проверяется; оно используется только при исправлении заголовка.

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

При такой конфигурации исправление для файла **TypeName.cs** выглядит следующим образом.

```csharp
// -----------------------------------------------------------------------
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
```

<a id="documentation-requirements"></a>

### Требования к документации

StyleCop Analyzers содержит правила, по умолчанию требующие документировать большую часть кодовой базы.
Это требование может оказаться непосильным для команды, которая не использовала StyleCop на протяжении всего процесса разработки.
Чтобы помочь разработчикам постепенно перейти к хорошо документированной кодовой базе, в **stylecop.json** предусмотрены
свойства для поэтапного повышения требований к документации.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `documentInterfaces` | **true** | 1.0.0 | Определяет, требуется ли документировать члены интерфейсов. При значении true документация обязательна для всех членов интерфейсов независимо от доступности. |
| `documentExposedElements` | **true** | 1.0.0 | Определяет, требуется ли документировать доступные извне элементы. При значении true документация обязательна для всех открытых типов и членов. |
| `documentInternalElements` | **true** | 1.0.0 | Определяет, требуется ли документировать внутренние элементы. При значении true документация обязательна для всех типов и членов, доступных внутри сборки. |
| `documentPrivateElements` | **false** | 1.0.0 | Определяет, требуется ли документировать закрытые элементы. При значении true документация обязательна для всех типов и членов, кроме полей, объявленных как private. |
| `documentPrivateFields` | **false** | 1.0.0 | Определяет, требуется ли документировать закрытые поля. При значении true документация обязательна для всех полей независимо от доступности. |

Эти свойства влияют на следующие правила, сообщающие об отсутствии документации. Правила, сообщающие о неверной
или неполной документации, продолжают применяться ко всем комментариям документации в коде.

* [SA1600 Элементы должны быть документированы](SA1600.md)
* [SA1601 Частичные элементы должны быть документированы](SA1601.md)
* [SA1602 Элементы перечислений должны быть документированы](SA1602.md)

Следующий пример показывает конфигурацию, требующую документировать все открытые члены и все интерфейсы
(независимо от доступности), но не требующую документации для остальных внутренних или закрытых членов.

> :memo: Документирование интерфейсов требует меньше усилий, чем документирование всей кодовой базы,
> но приносит большую пользу, поскольку охватывает части кода, наиболее важные для взаимодействия между командами.

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

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующие свойства:

```ini
stylecop.documentation.documentInterfaces = true
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
```

<a id="documentation-culture"></a>

### Культура документации

Некоторые правила документирования требуют, чтобы краткие описания начинались с определённых строк.
Чтобы команды могли документировать код на своём родном языке, в **stylecop.json** предусмотрено свойство `documentationCulture`.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `documentationCulture` | `"en-US"` | 1.1.0 | Задаёт культуру или язык, используемые для определённых текстов документации. |

Это свойство влияет на следующие правила, сообщающие о неверной документации.

* [SA1623 Краткое описание свойства должно соответствовать методам доступа](SA1623.md)
* [SA1624 Краткое описание свойства не должно упоминать set с ограниченной доступностью](SA1624.md)
* [SA1642 Краткое описание конструктора должно начинаться со стандартного текста](SA1642.md)
* [SA1643 Краткое описание деструктора должно начинаться со стандартного текста](SA1643.md)

> :memo: Значение `documentationCulture` по умолчанию фиксировано и не зависит от языка системы пользователя.
> Это гарантирует, что разные разработчики одного проекта всегда используют одинаковое значение.

В настоящее время поддерживаются следующие значения. Неподдерживаемые значения автоматически заменяются значением по умолчанию.

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

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство:

```ini
stylecop.documentation.documentationCulture = de-DE
```

<a id="file-naming-conventions"></a>

### Соглашения об именовании файлов

Свойство `fileNamingConvention` в **stylecop.json** определяет, как анализатор
[SA1649 Имя файла должно соответствовать имени типа](SA1649.md) проверяет имена файлов.

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `fileNamingConvention` | `"stylecop"` | 1.0.0 | Задаёт соглашение об именовании файлов обобщённых типов. |

Для следующего кода:

```csharp
public class Class1<T1, T2, T3>
{
}
```

Анализатор ожидает имена файлов согласно таблице ниже. Если свойство `fileNamingConvention` не задано,
по умолчанию используется соглашение `stylecop`.

Соглашение об именовании файлов | Ожидаемое имя файла
-------------------------------|--------------------
stylecop                       | Class1{T1,T2,T3}.cs
metadata                       | Class1`3.cs

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство:

```ini
stylecop.documentation.fileNamingConvention = stylecop
```

<a id="text-ending-with-a-period"></a>

### Текст, заканчивающийся точкой

Анализатор [SA1629 Текст документации должен заканчиваться точкой](SA1629.md) проверяет,
заканчиваются ли разделы XML-документации точкой. Для управления его поведением в **stylecop.json**
можно использовать следующие свойства:

| Свойство | Значение по умолчанию | Минимальная версия | Описание |
| --- | --- | --- | --- |
| `excludeFromPunctuationCheck` | `[ "seealso" ]` | 1.1.0 | Задаёт теги верхнего уровня XML-документации, исключаемые из анализа. |

При настройке StyleCop Analyzers с помощью **.editorconfig** можно использовать следующее свойство:

```ini
stylecop.documentation.excludeFromPunctuationCheck = seealso
```

<a id="sharing-configuration-among-solutions"></a>

## Общая конфигурация для нескольких решений

Можно один раз определить предпочитаемую конфигурацию и повторно использовать её в нескольких независимых проектах.
Для этого создайте собственный пакет NuGet с конфигурацией `stylecop.json` и, при необходимости,
пользовательским файлом набора правил. Пользовательский файл `.props` подключает эту конфигурацию
к любому проекту, использующему пакет NuGet.

Пример файла `.nuspec`:

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

Пример файла `.props`:

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
