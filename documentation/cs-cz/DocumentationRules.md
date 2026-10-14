### Pravidla dokumentace (SA1600-)
Pravidla, která ověřují obsah a formátování dokumentace kódu.

Identifikátor | Název | Popis
-----------|------|------------
[SA1600](SA1600.md) | ElementsMustBeDocumented | Prvek kódu C# postrádá hlavičku dokumentace.
[SA1601](SA1601.md) | PartialElementsMustBeDocumented | Částečný prvek jazyka C# postrádá hlavičku dokumentace.
[SA1602](SA1602.md) | EnumerationItemsMustBeDocumented | Položka v výčtu C# postrádá hlavičku XML dokumentace.
[SA1603](SA1603.md) | DocumentationMustContainValidXml | XML v hlavičce dokumentace prvku C# je špatně vytvořené.
[SA1604](SA1604.md) | ElementDocumentationMustHaveSummary | Dokumentace XML v hlavičce prvku C# postrádá značku `<summary>`.
[SA1605](SA1605.md) | PartialElementDocumentationMustHaveSummary | Značka `<summary>` nebo `<content>` v hlavičce dokumentace prvku kódu C# chybí nebo je prázdná.
[SA1606](SA1606.md) | ElementDocumentationMustHaveSummaryText | Značka `<summary>` v hlavičce dokumentace prvku kódu C# je prázdná.
[SA1607](SA1607.md) | PartialElementDocumentationMustHaveSummaryText | Značka `<summary>` nebo `<content>` v hlavičce dokumentace prvku kódu C# je prázdná.
[SA1608](SA1608.md) | ElementDocumentationMustNotHaveDefaultSummary | Značka `<summary>` v dokumentaci XML prvku obsahuje výchozí text vygenerovaný Visual Studiem při vytváření prvku.
[SA1609](SA1609.md) | PropertyDocumentationMustHaveValue | Dokumentace XML vlastnosti C# neobsahuje značku `<value>`.
[SA1610](SA1610.md) | PropertyDocumentationMustHaveValueText | Dokumentace XML vlastnosti C# obsahuje prázdnou značku `<value>`.
[SA1611](SA1611.md) | ElementParametersMustBeDocumented | Metoda, konstruktor, delegát nebo indexer C# postrádá dokumentaci pro jeden nebo více parametrů.
[SA1612](SA1612.md) | ElementParameterDocumentationMustMatchElementParameters | Dokumentace popisující parametry metody, konstruktoru, delegáta nebo indexeru C# neodpovídá skutečným parametrům prvku.
[SA1613](SA1613.md) | ElementParameterDocumentationMustDeclareParameterName | Značka `<param>` v dokumentační hlavičce prvku C# postrádá atribut `name` obsahující jméno parametru.
[SA1614](SA1614.md) | ElementParameterDocumentationMustHaveText | Značka `<param>` v dokumentační hlavičce prvku C# je prázdná.
[SA1615](SA1615.md) | ElementReturnValueMustBeDocumented | Prvek C# postrádá dokumentaci pro návratovou hodnotu.
[SA1616](SA1616.md) | ElementReturnValueDocumentationMustHaveText | Značka `<returns>` v dokumentační hlavičce prvku C# je prázdná.
[SA1617](SA1617.md) | VoidReturnValueMustNotBeDocumented | Prvek kódu C# neobsahuje návratovou hodnotu nebo vrací `void`, ale dokumentační hlavička prvku obsahuje značku `<returns>`.
[SA1618](SA1618.md) | GenericTypeParametersMustBeDocumented | Obecný prvek C# postrádá dokumentaci pro jeden nebo více obecných parametrů typu.
[SA1619](SA1619.md) | GenericTypeParametersMustBeDocumentedPartialClass | Obecný částečný prvek C# postrádá dokumentaci pro jeden nebo více obecných parametrů typu a dokumentace prvku obsahuje značku `<summary>`.
[SA1620](SA1620.md) | GenericTypeParameterDocumentationMustMatchTypeParameters | Značky `<typeparam>` v dokumentační hlavičce obecného prvku C# neodpovídají obecným parametrům typu na prvku.
[SA1621](SA1621.md) | GenericTypeParameterDocumentationMustDeclareParameterName | Značka `<typeparam>` v dokumentační hlavičce obecného prvku C# postrádá atribut `name` nebo obsahuje prázdný atribut `name`.
[SA1622](SA1622.md) | GenericTypeParameterDocumentationMustHaveText | Značka `<typeparam>` v dokumentační hlavičce obecného prvku C# je prázdná.
[SA1623](SA1623.md) | PropertySummaryDocumentationMustMatchAccessors | Dokumentační text v značce `<summary>` vlastnosti C# neodpovídá přístupovým objektům v této vlastnosti.
[SA1624](SA1624.md) | PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess | Dokumentační text v značce `<summary>` vlastnosti C# bere v úvahu všechny přístupové objekty vlastnosti, ale jeden z přístupových objektů má omezený přístup.
[SA1625](SA1625.md) | ElementDocumentationMustNotBeDuplicated | Dokumentace XML prvku C# obsahuje dvě nebo více shodných položek.
[SA1626](SA1626.md) | SingleLineCommentsMustNotUseDocumentationStyleSlashes | Kód C# obsahuje jednořádkový komentář, který začíná třemi lomítky za sebou.
[SA1627](SA1627.md) | DocumentationTextMustNotBeEmpty | Dokumentace XML pro prvek kódu C# obsahuje prázdnou značku.
[SA1628](SA1628.md) | DocumentationTextMustBeginWithACapitalLetter | Část XML dokumentace prvku C# nezačíná velkým písmenem.
[SA1629](SA1629.md) | DocumentationTextMustEndWithAPeriod | Část XML dokumentace prvku C# nekončí tečkou (plnou tečkou).
[SA1630](SA1630.md) | DocumentationTextMustContainWhitespace | Část XML dokumentace prvku C# neobsahuje žádnou mezeru mezi slovy.
[SA1631](SA1631.md) | DocumentationMustMeetCharacterPercentage | Část XML dokumentace prvku C# neobsahuje dostatečný počet alfabetických znaků.
[SA1632](SA1632.md) | DocumentationTextMustMeetMinimumCharacterLength | Od verze StyleCop 4.5 je toto pravidlo ve výchozím nastavení zakázáno.
[SA1633](SA1633.md) | FileMustHaveHeader | Soubor kódu C# postrádá standardní hlavičku souboru.
[SA1634](SA1634.md) | FileHeaderMustShowCopyright | Hlavička souboru v horní části kódu C# postrádá značku autorských práv.
[SA1635](SA1635.md) | FileHeaderMustHaveCopyrightText | Hlavička souboru v horní části kódu C# postrádá text autorských práv.
[SA1636](SA1636.md) | FileHeaderCopyrightTextMustMatch | Hlavička souboru v horní části kódu C# neobsahuje správný text autorských práv.
[SA1637](SA1637.md) | FileHeaderMustContainFileName | Hlavička souboru v horní části kódu C# postrádá název souboru.
[SA1638](SA1638.md) | FileHeaderFileNameDocumentationMustMatchFileName | Značka `file` v hlavičce souboru neobsahuje název souboru.
[SA1639](SA1639.md) | FileHeaderMustHaveSummary | Hlavička souboru v horní části kódu C# neobsahuje vyplněnou značku `summary`.
[SA1640](SA1640.md) | FileHeaderMustHaveValidCompanyText | Hlavička souboru v horní části kódu C# neobsahuje text názvu společnosti.
[SA1641](SA1641.md) | FileHeaderCompanyNameTextMustMatch | Hlavička souboru v horní části kódu C# neobsahuje správný text názvu společnosti.
[SA1642](SA1642.md) | ConstructorSummaryDocumentationMustBeginWithStandardText | Hlavička dokumentace konstruktoru C# neobsahuje správný počáteční text souhrnu.
[SA1643](SA1643.md) | DestructorSummaryDocumentationMustBeginWithStandardText | Hlavička dokumentace finalizéru C# neobsahuje správný počáteční text souhrnu.
[SA1644](SA1644.md) | DocumentationHeadersMustNotContainBlankLines | Část XML dokumentace prvku C# obsahuje prázdné řádky.
[SA1645](SA1645.md) | IncludedDocumentationFileDoesNotExist | Zahrnutý soubor XML dokumentace neexistuje.
[SA1646](SA1646.md) | IncludedDocumentationXPathDoesNotExist | Zahrnutý odkaz XML dokumentace obsahuje neplatnou cestu.
[SA1647](SA1647.md) | IncludeNodeDoesNotContainValidFileAndPath | Značka `include` v hlavičce XML dokumentace neobsahuje platné atributy souboru a cesty.
[SA1648](SA1648.md) | InheritDocMustBeUsedWithInheritingClass | `<inheritdoc>` bylo použito na prvku, který nezdědí z základní třídy ani neimplementuje rozhraní.
[SA1649](SA1649.md) | FileNameMustMatchTypeName | Název souboru kódu C# neodpovídá prvnímu typu deklarovanému v souboru.
[SA1650](SA1650.md) | ElementDocumentationMustBeSpelledCorrectly | Dokumentace prvku obsahuje jednu nebo více překlepů nebo neznámých slov.
[SA1651](SA1651.md) | DoNotUsePlaceholderElements | Dokumentace prvku obsahuje jednu nebo více značek `<placeholder>`.
[SA1652](SA1652.md) | EnableXmlDocumentationOutput | Toto pravidlo bylo přesunuto do [SA0001](SA0001.md)
[SA1653](SA1653.md) | OnlyOnePartialDeclarationShouldContainSummaryDocumentation | Více než jedna deklarace částečného typu obsahuje souhrnnou dokumentaci.
