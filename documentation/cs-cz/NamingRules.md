### Pravidla pojmenování (SA1300-)
Závažnost celé kategorie nastavíte v souboru **.editorconfig** takto:

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.NamingRules.severity = none
```

Pravidla, která vynucují požadavky na pojmenování členů, typů a proměnných.

Identifikátor | Název | Popis
-----------|------|------------
[SA1300](SA1300.md) | ElementMustBeginWithUpperCaseLetter | Název prvku jazyka C# nezačíná velkým písmenem.
[SA1301](SA1301.md) | ElementMustBeginWithLowerCaseLetter | V současnosti neexistuje situace, kdy by toto pravidlo mohlo být aktivováno.
[SA1302](SA1302.md) | InterfaceNamesMustBeginWithI | Název rozhraní jazyka C# nezačíná velkým písmenem I.
[SA1303](SA1303.md) | ConstFieldNamesMustBeginWithUpperCaseLetter | Název konstantního pole jazyka C# by měl začínat velkým písmenem.
[SA1304](SA1304.md) | NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter | Název neveřejného readonly pole jazyka C# by měl začínat velkým písmenem.
[SA1305](SA1305.md) | FieldNamesMustNotUseHungarianNotation | Název pole, proměnné nebo parametru v jazyce C# používá maďarskou notaci.
[SA1306](SA1306.md) | FieldNamesMustBeginWithLowerCaseLetter | Název pole v jazyce C# nezačíná malým písmenem.
[SA1307](SA1307.md) | AccessibleFieldsMustBeginWithUpperCaseLetter | Název veřejného nebo interního pole v jazyce C# nezačíná velkým písmenem.
[SA1308](SA1308.md) | VariableNamesMustNotBePrefixed | Název pole v jazyce C# má předponu *m_* nebo *s_*.
[SA1309](SA1309.md) | FieldNamesMustNotBeginWithUnderscore | Název pole v jazyce C# začíná podtržítkem.
[SA1310](SA1310.md) | FieldNamesMustNotContainUnderscore | Název pole v jazyce C# obsahuje podtržítko.
[SA1311](SA1311.md) | StaticReadonlyFieldsMustBeginWithUpperCaseLetter | Název statického readonly pole nezačíná velkým písmenem.
[SA1312](SA1312.md) | VariableNamesMustBeginWithLowerCaseLetter | Název proměnné v jazyce C# nezačíná malým písmenem.
[SA1313](SA1313.md) | ParameterNamesMustBeginWithLowerCaseLetter | Název parametru v jazyce C# nezačíná malým písmenem.
[SA1314](SA1314.md) | TypeParameterNamesMustBeginWithT | Název parametrického typu v jazyce C# nezačíná velkým písmenem T.
[SA1316](SA1316.md) | TupleElementNamesShouldUseCorrectCasing | Názvy prvků v typu tuple by měly mít správné velká písmena.
[SA1317](SA1317.md) | IdentifierShouldBeNamedOnlyWithLatinLetters | Názvy identifikátorů mají obsahovat pouze latinská písmena.
