### Pravidla pro mezery (SA1000-)
Závažnost celé kategorie nastavíte v souboru **.editorconfig** takto:

```ini
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.SpacingRules.severity = none
```

Pravidla, která vynucují požadavky na mezery kolem klíčových slov a symbolů v kódu.

Identifikátor | Název | Popis
-----------|------|-------------
[SA1000](SA1000.md) | KeywordsMustBeSpacedCorrectly | Mezery kolem klíčového slova jazyka C# jsou nesprávné.
[SA1001](SA1001.md) | CommasMustBeSpacedCorrectly | Mezery kolem čárky jsou nesprávné v souboru kódu C#.
[SA1002](SA1002.md) | SemicolonsMustBeSpacedCorrectly | Mezery kolem středníku jsou nesprávné v souboru kódu C#.
[SA1003](SA1003.md) | SymbolsMustBeSpacedCorrectly | Mezery kolem symbolu operátoru jsou nesprávné v souboru kódu C#.
[SA1004](SA1004.md) | DocumentationLinesMustBeginWithSingleSpace | Řádek v dokumentační hlavičce nad prvkem C# nezačíná jednou mezerou.
[SA1005](SA1005.md) | SingleLineCommentsMustBeginWithSingleSpace | Jednořádkový komentář v souboru kódu C# nezačíná jednou mezerou.
[SA1006](SA1006.md) | PreprocessorKeywordsMustNotBePrecededBySpace | Klíčové slovo typu preprocesoru jazyka C# je předcházena mezerou.
[SA1007](SA1007.md) | OperatorKeywordMustBeFollowedBySpace | Klíčové slovo operátoru v metodě přetížení operátoru C# není následováno mezerou.
[SA1008](SA1008.md) | OpeningParenthesisMustBeSpacedCorrectly | Otevřená závorka ve výrazu jazyka C# je nesprávně oddělena mezerami.
[SA1009](SA1009.md) | ClosingParenthesisMustBeSpacedCorrectly | Uzavírací závorka ve výrazu jazyka C# je nesprávně oddělena mezerami.
[SA1010](SA1010.md) | OpeningSquareBracketsMustBeSpacedCorrectly | Otevřená hranatá závorka ve výrazu jazyka C# je nesprávně oddělena mezerami.
[SA1011](SA1011.md) | ClosingSquareBracketsMustBeSpacedCorrectly | Uzavírací hranatá závorka ve výrazu jazyka C# je nesprávně oddělena mezerami.
[SA1012](SA1012.md) | OpeningBracesMustBeSpacedCorrectly | Otevřená složená závorka v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1013](SA1013.md) | ClosingBracesMustBeSpacedCorrectly | Uzavírací složená závorka v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1014](SA1014.md) | OpeningGenericBracketsMustBeSpacedCorrectly | Otevřená obecná závorka v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1015](SA1015.md) | ClosingGenericBracketsMustBeSpacedCorrectly | Uzavírací obecná závorka v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1016](SA1016.md) | OpeningAttributeBracketsMustBeSpacedCorrectly | Otevřená závorka atributu v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1017](SA1017.md) | ClosingAttributeBracketsMustBeSpacedCorrectly | Uzavírací závorka atributu v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1018](SA1018.md) | NullableTypeSymbolsMustNotBePrecededBySpace | Symbol nullable typu v prvku jazyka C# je nesprávně oddělen mezerami.
[SA1019](SA1019.md) | MemberAccessSymbolsMustBeSpacedCorrectly | Mezery kolem symbolu přístupu k členu jsou nesprávné v souboru kódu C#.
[SA1020](SA1020.md) | IncrementDecrementSymbolsMustBeSpacedCorrectly | Symbol inkrementu nebo dekrementu v prvku jazyka C# je nesprávně oddělen mezerami.
[SA1021](SA1021.md) | NegativeSignsMustBeSpacedCorrectly | Záporné znaménko v prvku jazyka C# je nesprávně odděleno mezerami.
[SA1022](SA1022.md) | PositiveSignsMustBeSpacedCorrectly | Kladné znaménko v prvku jazyka C# je nesprávně odděleno mezerami.
[SA1023](SA1023.md) | DereferenceAndAccessOfMustBeSpacedCorrectly | Symbol dereference nebo symbol přístupu v prvku jazyka C# je nesprávně oddělen mezerami.
[SA1024](SA1024.md) | ColonsMustBeSpacedCorrectly | Dvojtečka v prvku jazyka C# je nesprávně oddělena mezerami.
[SA1025](SA1025.md) | CodeMustNotContainMultipleWhitespaceInARow | Kód obsahuje více mezer po sobě jdoucích.
[SA1026](SA1026.md) | CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation | Implicitně typované vytvoření pole `new` v souboru kódu C# je nesprávně odděleno mezerami.
[SA1027](SA1027.md) | UseTabsCorrectly | Kód obsahuje znak tabulátoru nebo mezery, který není v souladu s aktuálním nastavením projektu.
[SA1028](SA1028.md) | CodeMustNotContainTrailingWhitespace | Řádek kódu končí mezerou, tabulátorem nebo jinými bílými znaky před koncem řádku.
