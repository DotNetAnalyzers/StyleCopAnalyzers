# Известные изменения

Этот документ описывает известные изменения поведения StyleCop Analyzers по сравнению со StyleCop Classic.
Перечисленные здесь изменения удовлетворяют всем следующим условиям:

1. Изменение поведения затрагивает код на C# 5 или более ранних версиях. StyleCop Classic не поддерживал C# 6,
   поэтому изменения, необходимые исключительно для поддержки новых возможностей языка, здесь не рассматриваются.
2. Изменение поведения в настоящее время является намеренным. Непреднамеренные изменения поведения регистрируются
   как ошибки при обнаружении.
3. Изменение затрагивает правило, существовавшее в StyleCop Classic. Новые правила StyleCop Analyzers можно
   отключить, чтобы приблизить поведение к StyleCop Classic.

Во многих случаях изменение поведения фактически сводится к исправлению документации: реализация StyleCop Classic
отклонялась от собственных документированных правил. Для полноты такие изменения также перечислены ниже,
если нам удалось их обнаружить. Случаи, не являющиеся простыми ошибками документации, помечены следующим символом:

> :warning: Этим символом отмечены случаи, когда реализация StyleCop Analyzers выдаёт для одного и того же кода
> предупреждения, отличающиеся от предупреждений StyleCop Classic.

## Отключённые правила

Несколько правил, присутствовавших в StyleCop Classic, намеренно не включены в StyleCop Analyzers. В следующей таблице
перечислены эти правила и ссылки на обсуждения, в которых было принято решение исключить правило.

| ID | Название | Обсуждение |
| --- | --- | --- |
| SA1109 | Блоки операторов не должны содержать вложенные регионы | [#998](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/998) |
| SA1126 | Следует правильно указывать префиксы вызовов | [#59](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/59) |
| SA1215 | Элементы экземпляра только для чтения должны предшествовать остальным элементам экземпляра | [#1812](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1812) |
| SA1409 | Следует удалять ненужный код | [#1058](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1058) |
| SA1603 | Документация должна содержать корректный XML | [#1291](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1291) |
| SA1628 | Текст документации должен начинаться с прописной буквы | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1630 | Текст документации должен содержать пробельные символы | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1631 | Документация должна содержать достаточную долю буквенных символов | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1632 | Текст документации должен иметь минимально допустимую длину | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1644 | DocumentationHeadersMustNotContainBlankLines | [#164](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/164) |
| SA1645 | Включаемый файл документации не существует | [#165](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/165) |
| SA1646 | XPath включаемой документации не существует | [#166](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/166) |
| SA1647 | Узел include не содержит корректных file и path | [#167](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/167) |
| SA1650 | Документация элемента должна быть написана без орфографических ошибок | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |

## Правила расстановки пробелов

### SA1000: Ключевые слова должны быть правильно отделены пробелами

В SA1000 внесены следующие изменения:

1. :warning: Теперь после `await` и `case` требуется пробел.

2. Добавлено исключение из требования пробела после ключевого слова `throw`, если оно используется
   в операторе повторного создания исключения. Это исключение не упоминалось в документации StyleCop Classic.

   ```csharp
   throw;
   ```

3. :warning: Добавлено исключение из требования пробела после ключевого слова `new`, если оно используется
   в ограничении параметра обобщённого типа. StyleCop Classic не проверял пробелы вокруг `new`
   в таком ограничении.

   ```csharp
   public void Foo<T>() where T : IInterface, new()
   {
       // ...
   }
   ```

### SA1001

:warning: Немного изменена расстановка пробелов вокруг запятых в открытых обобщённых типах.
Следующая таблица демонстрирует это изменение.

| StyleCop Analyzers | StyleCop Classic |
| --- | --- |
| `typeof(Func<,>)` | `typeof(Func<, >)` |

### SA1002

:warning: StyleCop Classic требовал записывать бесконечный цикл `for` следующим образом:

```csharp
for (;;)
{
}
```

StyleCop Analyzers выдаёт SA1002 для этого кода и вместо этого требует следующую запись:

```csharp
for (; ;)
{
}
```

:bulb: При обнаружении такого кода рекомендуется переписать его следующим образом для улучшения читаемости:

```csharp
while (true)
{
}
```

### SA1003

:warning: StyleCop Classic разрешал размещать приведение типа в конце строки, например:

```csharp
uint value = (uint)
    3;
```

StyleCop Analyzers запрещает такой вариант и требует следующую запись:

```csharp
uint value = (uint)3;
```

### SA1025

:warning: StyleCop Classic разрешал несколько пробелов перед комментарием в конце строки, например:

```csharp
int x;    // comment
```

Он также разрешал несколько пробелов перед символом, как во второй строке следующего кода:

```csharp
int xyz = 1;
int w   = 1;
```

В настоящее время StyleCop Analyzers не делает исключения из SA1025 для этих случаев, в том числе для выровненных
операторов, например знаков `=` при задании значений в перечислениях флагов ([#3684](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3684)).

## Правила читаемости

### SA1110, SA1111, SA1113, SA1114

StyleCop Analyzers проверяет ряд конструкций, не упомянутых в исходной документации этих правил:

* Объявления делегатов
* Выражения анонимных методов
* Лямбда-выражения

### SA1119

:warning: StyleCop Classic не выдавал SA1119 для следующего кода:

```csharp
var a = (new[] { 1, 2, 3 }).ToArray();
```

StyleCop Analyzers выдаёт SA1119 для этого кода и требует следующего исправления:

```csharp
var a = new[] { 1, 2, 3 }.ToArray();
```

## Правила упорядочивания

### SA1208

StyleCop Analyzers считает директивы using директивами «System» только тогда, когда они не квалифицированы псевдонимом,
тогда как StyleCop Classic игнорировал псевдоним. Например, `using global::System;` не считается директивой using System
в StyleCop Analyzers, но считалась таковой в StyleCop Classic.

### SA1210

При сортировке директив using StyleCop Analyzers учитывает квалификаторы псевдонимов, чтобы соответствовать
поведению Visual Studio 2015 по умолчанию. StyleCop Classic игнорирует такие квалификаторы при сортировке.

Пример порядка сортировки StyleCop Analyzers:

```csharp
using Beer;
using global::Wine;
using Tea;
```

Пример порядка сортировки StyleCop Classic:

```csharp
using Beer;
using Tea;
using global::Wine;
```

### SA1214

StyleCop Classic выдаёт SA1214 только для нарушений, связанных со статическими полями. В StyleCop Analyzers правила
SA1214 и SA1215 объединены, чтобы пользователям было проще настраивать поведение нескольких правил
упорядочивания членов типа.

:warning: Нарушения, которые в StyleCop Classic регистрировались как SA1215, в StyleCop Analyzers регистрируются как SA1214.

## Правила именования

### SA1300

StyleCop Analyzers добавляет элементы перечислений в список элементов, имена которых должны начинаться с прописной
буквы, и выдаёт SA1300 при нарушениях. StyleCop Classic не выдавал сообщений для элементов перечислений,
имена которых начинались не с прописной буквы.

### SA1303

:warning: StyleCop Classic выдаёт SA1303 для локальных констант, имена которых начинаются со строчной буквы,
хотя они не являются полями. StyleCop Analyzers ограничивает SA1303 полями, поэтому локальные константы подчиняются
правилам именования локальных переменных и не вызывают предупреждения для следующего кода
([#2082](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2082)):

```csharp
public void SomeMethod()
{
    const string url = "some constant value";
}
```

### SA1305

В StyleCop Analyzers это правило отключено по умолчанию, но пользователи могут включить его с помощью файла набора правил.

:warning: StyleCop Analyzers не выдаёт SA1305 для параметров переопределяющих методов и методов, реализующих интерфейс.
StyleCop Classic выдавал SA1305 для всех методов.

### SA1313

:warning: StyleCop Classic разрешает параметры лямбда-выражений, имена которых состоят только из подчёркиваний
(например, `_`, `__`, `___`, …), без SA1313. StyleCop Analyzers делает исключение только для `_` и `__`;
более длинные имена, состоящие только из подчёркиваний, по-прежнему вызывают SA1313
([#2759](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2759)).

## Правила сопровождаемости

В настоящее время известных изменений нет.

## Правила оформления

### SA1514

:warning: Хотя StyleCop Classic в целом не разрешал директивы `#region` в коде (SA1124), он мог косвенно допускать
комментарий документации элемента сразу после директивы `#region`, без пустой строки (то есть не выдавать SA1514).
StyleCop Analyzers считает `#region` предшествующим содержимым, поэтому выдаёт SA1514, если после директивы
не добавить пустую строку ([#1280](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1280)).

### SA1515

:warning: Следующий код не вызывает предупреждения в StyleCop Classic:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]
// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

StyleCop Analyzers выдаёт SA1515 для этого кода и требует следующую запись:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]

// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

## Правила документирования

### SA1642

StyleCop Analyzers требует использовать элемент `<see>` при ссылке на имя класса в стандартном тексте документации
конструктора. В StyleCop Classic этот элемент был необязательным.

В StyleCop Classic существовал особый вариант текста документации для конструкторов `private`. StyleCop Analyzers
по-прежнему разрешает такую формулировку для закрытых конструкторов, но предпочитает стандартный текст.
При применении исправления SA1642 к закрытому конструктору вставленный текст удовлетворяет StyleCop Analyzers,
но вызывает предупреждение в StyleCop Classic. Например:

Следующий код не вызывает предупреждений ни в StyleCop Classic, ни в StyleCop Analyzers:

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

:warning: Следующий код вызывает предупреждение в StyleCop Classic, но не в StyleCop Analyzers:

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

Это правило изменено, чтобы точнее соответствовать использованию `<inheritdoc>` в Sandcastle Help File Builder.
В результате некоторый код, для которого StyleCop Classic выдавал SA1648, больше не вызывает
это предупреждение в StyleCop Analyzers.

### SA1649

StyleCop Analyzers изменяет SA1649 так, чтобы имя первого типа сравнивалось с фактическим именем файла, а не со значением
в заголовке файла. Для пользователей StyleCop Classic, у которых были включены и SA1638, и SA1649, это изменение
не создаёт новых предупреждений для кода, ранее удовлетворявшего StyleCop.
