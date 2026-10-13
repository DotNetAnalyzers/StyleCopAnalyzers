### Pravidla udržovatelnosti (SA1400-)
Pravidla, která zlepšují udržovatelnost kódu.

Identifikátor | Název | Popis
-----------|------|------------
[SA1119](SA1119.md) | StatementMustNotUseUnnecessaryParenthesis | Příkaz v jazyce C# obsahuje nadbytečné závorky, které by měly být odstraněny.
[SA1400](SA1400.md) | AccessModifierMustBeDeclared | Modifikátor přístupu pro prvek jazyka C# nebyl explicitně definován.
[SA1401](SA1401.md) | FieldsMustBePrivate | Pole ve třídě C# má jiný modifikátor přístupu než `private`.
[SA1402](SA1402.md) | FileMayOnlyContainASingleType | Soubor kódu C# obsahuje více než jeden unikátní typ.
[SA1403](SA1403.md) | FileMayOnlyContainASingleNamespace | Soubor kódu C# obsahuje více než jeden jmenný prostor.
[SA1404](SA1404.md) | CodeAnalysisSuppressionMustHaveJustification | Atribut `SuppressMessage` pro analýzu kódu neobsahuje odůvodnění.
[SA1405](SA1405.md) | DebugAssertMustProvideMessageText | Volání `Debug.Assert` v kódu C# neobsahuje popisnou zprávu.
[SA1406](SA1406.md) | DebugFailMustProvideMessageText | Volání `Debug.Fail` v kódu C# neobsahuje popisnou zprávu.
[SA1407](SA1407.md) | ArithmeticExpressionsMustDeclarePrecedence | Příkaz v jazyce C# obsahuje složitý aritmetický výraz, který opomíjí závorky kolem operátorů.
[SA1408](SA1408.md) | ConditionalExpressionsMustDeclarePrecedence | Příkaz v jazyce C# obsahuje složitý podmíněný výraz, který opomíjí závorky kolem operátorů.
[SA1409](SA1409.md) | RemoveUnnecessaryCode | Soubor kódu C# obsahuje kód, který je zbytečný a lze jej odstranit bez změny celkové logiky kódu.
[SA1410](SA1410.md) | RemoveDelegateParenthesisWhenPossible | Volání anonymní metody v jazyce C# neobsahuje žádné parametry metody, přesto příkaz stále obsahuje závorky.
[SA1411](SA1411.md) | AttributeConstructorMustNotUseUnnecessaryParenthesis | Deklarace atributu neobsahuje žádné parametry, přesto stále obsahuje závorky.
[SA1412](SA1412.md) | StoreFilesAsUtf8 | Kódování souboru není UTF-8 s bajtovým pořadím (BOM).
[SA1413](SA1413.md) | UseTrailingCommasInMultiLineInitializers | Víceřádkový inicializátor, enum, výraz `switch` nebo vzor vlastnosti by měl používat čárku na posledním prvku.
[SA1414](SA1414.md) | TupleTypesInSignaturesShouldHaveElementNames | Typy tuple v signaturách členů by měly mít explicitně pojmenované prvky tuple.
