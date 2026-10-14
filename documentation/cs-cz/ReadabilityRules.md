### Pravidla čitelnosti (SA1100-)
Závažnost celé kategorie nastavíte v souboru **.editorconfig** takto:

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.ReadabilityRules.severity = none
```

Pravidla, která zajišťují, že je kód správně formátovaný a čitelný.

Identifikátor | Název | Popis
-----------|------|-------------
[SA1100](SA1100.md) | DoNotPrefixCallsWithBaseUnlessLocalImplementationExists | Volání člena zděděné třídy začíná na `base.` a lokální třída neobsahuje přepsání ani implementaci člena.
[SA1101](SA1101.md) | PrefixLocalCallsWithThis | Volání instančního člena lokální třídy nebo základní třídy není předcházeno `this.`, v souboru kódu C#.
[SA1102](SA1102.md) | QueryClauses | Klauzule jazyka C# dotazu nezačíná stejným řádkem jako předchozí klauzule, nebo na následujícím řádku.
[SA1103](SA1103.md) | QueryClauses | Klauzule v dotazovém výrazu C# nejsou všechny umístěny na stejném řádku a každá klauzule není umístěna na samostatném řádku.
[SA1104](SA1104.md) | QueryClauses | Klauzule v dotazovém výrazu C# začíná na stejném řádku jako předchozí klauzule, když předchozí klauzule přesahuje více řádků.
[SA1105](SA1105.md) | QueryClauses | Klauzule v dotazovém výrazu C# přesahuje více řádků a nezačíná na vlastním řádku.
[SA1106](SA1106.md) | CodeMustNotContainEmptyStatements | Kód jazyka C# obsahuje přebytečné středníky.
[SA1107](SA1107.md) | CodeMustNotContainMultipleStatementsOnOneLine | Kód jazyka C# obsahuje více než jedno prohlášení na jednom řádku.
[SA1108](SA1108.md) | BlockStatementsMustNotContainEmbeddedComments | Příkaz v jazyce C# obsahuje komentář mezi deklarací příkazu a otevírací složenou závorkou.
[SA1109](SA1109.md) | BlockStatementsMustNotContainEmbeddedRegions | Příkaz v jazyce C# obsahuje značku regionu mezi deklarací příkazu a otevírací složenou závorkou.
[SA1110](SA1110.md) | OpeningParenthesisMustBeOnDeclarationLine | Otevírací závorka nebo hranatá závorka při volání metody nebo indexeru jazyka C# nebo při deklaraci metody nebo indexeru není umístěna na stejném řádku jako název metody nebo indexeru.
[SA1111](SA1111.md) | ClosingParenthesisMustBeOnLineOfLastParameter | Uzavírací závorka nebo hranatá závorka při volání metody nebo indexeru jazyka C# nebo při deklaraci metody nebo indexeru není umístěna na stejném řádku jako poslední parametr.
[SA1112](SA1112.md) | ClosingParenthesisMustBeOnLineOfOpeningParenthesis | Uzavírací závorka nebo hranatá závorka při volání metody nebo indexeru jazyka C# nebo při deklaraci metody nebo indexeru není umístěna na stejném řádku jako otevírací závorka, pokud prvek neobsahuje žádné parametry.
[SA1113](SA1113.md) | CommaMustBeOnSameLineAsPreviousParameter | Čárka mezi dvěma parametry při volání metody nebo indexeru jazyka C# nebo v deklaraci metody nebo indexeru není umístěna na stejném řádku jako předchozí parametr.
[SA1114](SA1114.md) | ParameterListMustFollowDeclaration | Začátek seznamu parametrů pro volání nebo deklaraci metody nebo indexeru nezačíná na stejném řádku jako otevírací závorka nebo na řádku po otevírací závorce.
[SA1115](SA1115.md) | ParameterMustFollowComma | Parametr v volání nebo deklaraci metody nebo indexeru v C# nezačíná na stejném řádku jako předchozí parametr nebo na dalším řádku.
[SA1116](SA1116.md) | SplitParametersMustStartOnLineAfterDeclaration | Parametry volání nebo deklarace metody nebo indexeru jazyka C# přesahují více řádků, ale první parametr nezačíná na řádku po otevírací závorce.
[SA1117](SA1117.md) | ParametersMustBeOnSameLineOrSeparateLines | Parametry volání nebo deklarace metody nebo indexeru jazyka C# nejsou všechny na stejném řádku nebo každý na samostatném řádku.
[SA1118](SA1118.md) | ParameterMustNotSpanMultipleLines | Parametr metody nebo indexeru jazyka C#, kromě prvního parametru, přesahuje více řádků.
[SA1120](SA1120.md) | CommentsMustContainText | Komentář v jazyce C# neobsahuje žádný text komentáře.
[SA1121](SA1121.md) | UseBuiltInTypeAlias | Kód používá jeden ze základních typů C#, ale nepoužívá vestavěný alias pro tento typ.
[SA1122](SA1122.md) | UseStringEmptyForEmptyStrings | Kód jazyka C# obsahuje prázdný řetězec zapsaný jako `""`.
[SA1123](SA1123.md) | DoNotPlaceRegionsWithinElements | Kód jazyka C# obsahuje region uvnitř těla elementu kódu.
[SA1124](SA1124.md) | DoNotUseRegions | Kód jazyka C# obsahuje region.
[SA1125](SA1125.md) | UseShorthandForNullableTypes | Nullable typ byl definován bez použití zkratky jazyka C#.
[SA1126](SA1126.md) | PrefixCallsCorrectly | Volání člena není předcházeno předponou `this.`, `base.`, `object.` nebo `typename.` pro označení zamýšleného volání metody v souboru kódu C#.
[SA1127](SA1127.md) | GenericTypeConstraintsMustBeOnOwnLine | Omezení obecného typu v deklaraci typu nebo metody je na stejném řádku jako deklarace, v souboru kódu C#.
[SA1128](SA1128.md) | ConstructorInitializerMustBeOnOwnLine | Inicializátor konstruktoru je na stejném řádku jako deklarace konstruktoru, v souboru kódu C#.
[SA1129](SA1129.md) | DoNotUseDefaultValueTypeConstructor | Typ hodnoty byl vytvořen pomocí syntaxe `new T()`.
[SA1130](SA1130.md) | UseLambdaSyntax | Anonymní metoda byla deklarována použitím tvaru `delegate (parameters) { }`, když by výraz lambda poskytoval ekvivalentní chování se syntaxí `(parameters) => { }`.
[SA1131](SA1131.md) | UseReadableConditions | Bylo provedeno porovnání mezi proměnnou a literálem nebo konstantou, a proměnná se objevila na pravé straně výrazu.
[SA1132](SA1132.md) | DoNotCombineFields | Byly deklarována dvě nebo více polí ve stejné syntaxi deklarace pole.
[SA1133](SA1133.md) | DoNotCombineAttributes | Dva nebo více atributů se objevilo ve stejné sadě hranatých závorek.
[SA1134](SA1134.md) | AttributesMustNotShareLine | Atribut je umístěn na stejném řádku kódu jako jiný atribut nebo prvek.
[SA1135](SA1135.md) | UsingDirectivesMustBeQualified | Direktiva `using` není kvalifikována.
[SA1136](SA1136.md) | EnumValuesShouldBeOnSeparateLines | Více hodnot enum je umístěno na stejném řádku kódu.
[SA1137](SA1137.md) | ElementsShouldHaveTheSameIndentation | Dva sourozenecké prvky, z nichž každý začíná na vlastním řádku, mají různé úrovně odsazení.
[SA1139](SA1139.md) | UseLiteralsSuffixNotationInsteadOfCasting | Použijte notaci přípon literálů místo přetypování.
[SA1141](SA1141.md) | UseTupleSyntax | Použijte syntaxi tuple místo základního typu `ValueTuple`.
[SA1142](SA1142.md) | ReferToTupleElementsByName | Prvek tuple byl odkazován podle metadatového názvu, když je k dispozici jméno prvku.
