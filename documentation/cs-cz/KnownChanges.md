# Známé změny

Tento dokument popisuje známé změny v chování StyleCop Analyzers ve vztahu ke StyleCop Classic. Tyto změny splňují všechny následující podmínky:

1. Změna v chování ovlivňuje kód pro C# 5 nebo starší. StyleCop Classic nepodporoval C# 6, takže nezbytné změny provedené pouze pro podporu novějších jazykových funkcí nejsou považovány za změny zde.
2. Změna v chování je v současnosti zamýšlená. Neúmyslné změny chování jsou hlášeny jako chyby, když jsou objeveny.
3. Změna ovlivňuje pravidlo, které bylo v StyleCop Classic. Nová pravidla přidaná pro StyleCop Analyzers lze zakázat, aby se blížilo chování StyleCop Classic.

V mnoha případech byla změna v chování jednoduše změnou dokumentace, kde implementace StyleCop Classic odcházela od vlastní dokumentace pravidel. Pro úplnost jsou tyto změny zahrnuty níže, kdykoli jsme je nalezli. Následující symbol se používá pro případy, které nejsou jen chybou dokumentace:

> :warning: Případy, ve kterých implementace StyleCop Analyzers bude vytvářet jiná varování než StyleCop Classic pro stejný kód, jsou označeny tímto symbolem.

## Zakázaná pravidla

Některá pravidla přítomná v StyleCop Classic byla úmyslně vynechána v StyleCop Analyzers. Následující tabulka uvádí každý z těchto problémů spolu s odkazem na issue, kde byla rozhodnuta o vynechání pravidla.

| ID | Titulek | Issue |
| --- | --- | --- |
| SA1109 | Block statements should not contain embedded regions | [#998](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/998) |
| SA1126 | Prefix calls correctly | [#59](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/59) |
| SA1215 | Instance readonly elements should appear before instance non-readonly elements | [#1812](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1812) |
| SA1409 | Remove unnecessary code | [#1058](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1058) |
| SA1603 | Documentation should contain valid XML | [#1291](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1291) |
| SA1628 | Documentation text should begin with a capital letter | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1630 | Documentation text should contain whitespace | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1631 | Documentation should meet character percentage | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1632 | Documentation text should meet minimum character length | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |
| SA1644 | DocumentationHeadersMustNotContainBlankLines | [#164](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/164) |
| SA1645 | Included documentation file does not exist | [#165](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/165) |
| SA1646 | Included documentation XPath does not exist | [#166](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/166) |
| SA1647 | Include node does not contain valid file and path | [#167](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/167) |
| SA1650 | Element documentation should be spelled correctly | [#1057](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1057) |

## Pravidla pro mezery

### SA1000: Klíčová slova by měla být správně oddělena mezerami

Pro SA1000 byly provedeny následující změny:

1. :warning: Po `await` a `case` je nyní vyžadována mezera.

2. Byla přidána výjimka pro požadavek mezery za klíčovým slovem `throw`, když se vyskytuje v příkazu rethrow. Tato výjimka nebyla zmíněna v dokumentaci StyleCop Classic.

   ```csharp
   throw;
   ```

3. :warning: Byla přidána výjimka pro požadavek mezery za klíčovým slovem `new`, když se vyskytuje jako omezení obecného typu. StyleCop Classic nekontroloval mezery kolem klíčového slova `new` v omezení obecného typu.

   ```csharp
   public void Foo<T>() where T : IInterface, new()
   {
       // ...
   }
   ```

### SA1001

:warning: Byla provedena drobná změna v mezeře kolem čárek v otevřených obecních typech. Následující tabulka ukazuje tuto změnu.

| StyleCop Analyzers | StyleCop Classic |
| --- | --- |
| `typeof(Func<,>)` | `typeof(Func<, >)` |

### SA1002

:warning: StyleCop Classic vyžadoval nekonečnou `for` smyčku napsanou takto:

```csharp
for (;;)
{
}
```

StyleCop Analyzers hlásí SA1002 pro stejný kód a místo toho vyžaduje, aby byl kód napsán takto:

```csharp
for (; ;)
{
}
```

:bulb: Při setkání s tímto kódem jsou uživatelé vyzváni, aby jej přepsali takto pro lepší čitelnost:

```csharp
while (true)
{
}
```

### SA1003

:warning: StyleCop Classic povoloval, aby se přetypování objevil na konci řádku, například takto:

```csharp
uint value = (uint)
    3;
```

StyleCop Analyzers zakazuje stejný případ a vyžaduje následující:

```csharp
uint value = (uint)3;
```

### SA1025

:warning: StyleCop Classic povoloval více mezer před komentářem umístěným na konci řádku, například takto:

```csharp
int x;    // comment
```

Povoloval také více mezer před symbolem, například ve druhém řádku následujícího kódu:

```csharp
int xyz = 1;
int w   = 1;
```

StyleCop Analyzers momentálně neuděluje výjimku pro pravidlo SA1025 pro tyto případy, včetně zarovnaných operátorů typu `=` v enum s příznaky ([#3684](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3684)).

## Pravidla čitelnosti

### SA1110, SA1111, SA1113, SA1114

StyleCop Analyzers zkoumá několik konstrukcí, které nejsou zmíněny v původní dokumentaci těchto pravidel:

* Deklarace delegátů
* Anonymní metody
* Výrazy lambda

### SA1119

:warning: StyleCop Classic nehlásil SA1119 pro následující kód:

```csharp
var a = (new[] { 1, 2, 3 }).ToArray();
```

StyleCop Analyzers hlásí SA1119 pro stejný kód a vyžaduje změnu pro opravu:

```csharp
var a = new[] { 1, 2, 3 }.ToArray();
```

## Pravidla uspořádání

### SA1208

StyleCop Analyzers považuje `using` direktivy za „System“ direktivy pouze tehdy, pokud nejsou aliasově kvalifikované, zatímco StyleCop Classic ignoroval alias. Například `using global::System;` by nebyl považován za direktivu System v StyleCop Analyzers, ale byl by považován za direktivu System v StyleCop Classic.

### SA1210

StyleCop Analyzers zohledňuje aliasové kvalifikátory při řazení direktiv `using`, aby odpovídal výchozímu chování Visual Studio 2015. StyleCop Classic ignoruje aliasové kvalifikátory při řazení direktiv `using`.

Příklad řazení v StyleCop Analyzers:
```csharp
using Beer;
using global::Wine;
using Tea;
```

Příklad řazení v StyleCop Classic:
```csharp
using Beer;
using Tea;
using global::Wine;
```

### SA1214

StyleCop Classic hlásí SA1214 pouze pro porušení zahrnující statická pole. V StyleCop Analyzers byly SA1214 a SA1215 sloučeny do jednoho pravidla, aby se zlepšila možnost uživatelů upravovat chování několika pravidel uspořádání zahrnujících členy typu.

:warning: Porušení hlášená jako SA1215 v StyleCop Classic jsou hlášena jako SA1214 v StyleCop Analyzers.

## Pravidla pojmenování

### SA1300

StyleCop Analyzers přidává členy enum do seznamu prvků, které by měly začínat velkým písmenem, a hlásí SA1300 pro porušení. StyleCop Classic nehlásil žádné zprávy pro členy enum, které nezačínaly velkým písmenem.

### SA1303

:warning: StyleCop Classic hlásí SA1303 pro lokální konstanty začínající malým písmenem, i když nejsou poli. StyleCop Analyzers omezuje SA1303 na pole, takže lokální konstanty následují pravidla pojmenování lokálních proměnných a nevytvářejí varování pro tento kód ([#2082](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2082)):

```csharp
public void SomeMethod()
{
    const string url = "some constant value";
}
```

### SA1305

Toto pravidlo je ve StyleCop Analyzers ve výchozím nastavení zakázáno, ale uživatelé jej mohou povolit prostřednictvím souboru sady pravidel.

:warning: StyleCop Analyzers nehlásí SA1305 pro parametry přepisujících metod a metod, které implementují rozhraní. StyleCop Classic hlásil SA1305 pro všechny metody.

### SA1313

:warning: StyleCop Classic povoluje lambda parametry skládající se výhradně z podtržítek (např. `_`, `__`, `___`, …) bez hlášení SA1313. StyleCop Analyzers special-case pouze `_` a `__`; delší jména složená pouze z podtržítek stále hlásí SA1313 ([#2759](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2759)).

## Pravidla udržovatelnosti

V současnosti neexistují žádné známé změny.

## Pravidla rozložení

### SA1514

:warning: Zatímco StyleCop Classic obecně nepovoloval `#region` direktivy v kódu (SA1124), mohl nepřímo povolit, aby hlavička dokumentace prvku bezprostředně následovala direktivu `#region` bez prázdného řádku (tj. nehlásil SA1514). StyleCop Analyzers považuje `#region` za předcházející obsah, takže SA1514 se hlásí, pokud nepřidáte prázdný řádek za direktivu ([#1280](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1280)).

### SA1515

:warning: Následující kód nehlásí varování v StyleCop Classic:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]
// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

StyleCop Analyzers hlásí SA1515 pro tento kód a namísto toho vyžaduje následující:

```csharp
[ContractClassFor(typeof(ILinkActivator<>))]

// ReSharper disable once InconsistentNaming
// Contract class containing only metadata pertaining to an interface.
// Using naming convention adopted by the Code Contracts team for .NET framework contracts.
public abstract class ILinkActivatorContract<T> : ILinkActivator<T> where T : LinkTemplate
```

## Pravidla dokumentace

### SA1642

StyleCop Analyzers vyžaduje použití elementu `<see>` při odkazování na název třídy ve standardním textu konstruktoru. Ve StyleCop Classic byl tento element volitelný.

StyleCop Classic obsahoval zvláštní případ pro text konstruktoru používaný u `private` konstruktorů. StyleCop Analyzers stále dovoluje tento text používat v dokumentaci soukromých konstruktorů, ale upřednostňuje standardní text konstruktoru. Pokud se oprava kódu pro SA1642 použije na soukromý konstruktor, vložený text bude odpovídat StyleCop Analyzers, ale ve StyleCop Classic vyvolá varování. Například:

Následující kód nevytváří varování ve StyleCop Classic ani StyleCop Analyzers:

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

:warning: Následující kód vytváří varování ve StyleCop Classic, ale nevytváří varování ve StyleCop Analyzers:

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

Toto pravidlo bylo upraveno tak, aby lépe odpovídalo použití `<inheritdoc>` s nástrojem Sandcastle Help File Builder. V důsledku toho už některý kód, pro který StyleCop Classic hlásil SA1648, toto varování ve StyleCop Analyzers nevyvolá.

### SA1649

StyleCop Analyzers upravil SA1649 tak, aby porovnával první typ se skutečným názvem souboru, namísto kontroly hodnoty uvedené v hlavičce souboru. U uživatelů StyleCop Classic, kteří měli povoleny SA1638 i SA1649, tato změna nevytváří nová varování v kódu, který dříve pravidlům StyleCop vyhovoval.
