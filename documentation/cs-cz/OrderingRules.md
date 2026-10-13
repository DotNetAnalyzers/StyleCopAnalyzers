### Pravidla uspořádání (SA1200-)
Pravidla, která vynucují standardní systém uspořádání obsahu kódu.

Identifikátor | Název | Popis
-----------|------|-------------
[SA1200](SA1200.md) | UsingDirectivesMustBePlacedCorrectly | Direktiva `using` jazyka C# je umístěna mimo element jmenného prostoru.
[SA1201](SA1201.md) | ElementsMustAppearInTheCorrectOrder | Prvek v souboru kódu C# je umístěn v nesprávném pořadí ve vztahu k ostatním prvkům v kódu.
[SA1202](SA1202.md) | ElementsMustBeOrderedByAccess | Prvek v souboru kódu C# je umístěn v nesprávném pořadí s ohledem na úroveň přístupu ve vztahu k ostatním prvkům v kódu.
[SA1203](SA1203.md) | ConstantsMustAppearBeforeFields | Konstanta je umístěna pod nekonstantním polem.
[SA1204](SA1204.md) | StaticElementsMustAppearBeforeInstanceElements | Statický prvek je umístěn pod instančním prvkem stejného typu.
[SA1205](SA1205.md) | PartialElementsMustDeclareAccess | Částečný prvek nemá definovaný modifikátor přístupu.
[SA1206](SA1206.md) | DeclarationKeywordsMustFollowOrder | Klíčová slova v deklaraci prvku neodpovídají standardnímu pořadí.
[SA1207](SA1207.md) | ProtectedMustComeBeforeInternal | Klíčové slovo `protected` je umístěno po klíčovém slovu `internal` v deklaraci chráněného interního prvku jazyka C#.
[SA1208](SA1208.md) | SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives | Direktiva `using`, která deklaruje prvek z jmenného prostoru *System*, se objevuje po direktivě `using`, která deklaruje prvek z jiného jmenného prostoru, v souboru kódu C#.
[SA1209](SA1209.md) | UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives | Direktiva aliasu `using` je umístěna před běžnou direktivou `using`.
[SA1210](SA1210.md) | UsingDirectivesMustBeOrderedAlphabeticallyByNamespace | Direktivy `using` v souboru kódu C# nejsou seřazeny abecedně podle jmenného prostoru.
[SA1211](SA1211.md) | UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName | Direktivy aliasu `using` v souboru kódu C# nejsou seřazeny abecedně podle názvu aliasu.
[SA1212](SA1212.md) | PropertyAccessorsMustFollowOrder | Přístupový objekt `get` se objeví po objektu `set` v vlastnosti nebo indexeru.
[SA1213](SA1213.md) | EventAccessorsMustFollowOrder | Přístupový objekt `add` se objeví po objektu `remove` v události.
[SA1214](SA1214.md) | ReadonlyElementsMustAppearBeforeNonReadonlyElements | Pole jen pro čtení je umístěno pod polem, které není pouze pro čtení.
[SA1215](SA1215.md) | InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements | Instanční prvek jen pro čtení je umístěn pod instančním prvkem, který není pouze pro čtení, ve stejném typu.
[SA1216](SA1216.md) | UsingStaticDirectivesMustBePlacedAtTheCorrectLocation | Direktiva `using static` je umístěna na nesprávném místě (před běžnou direktivou `using` nebo za aliasovou direktivou `using`).
[SA1217](SA1217.md) | UsingStaticDirectivesMustBeOrderedAlphabetically | Direktivy `using static` v souboru kódu C# nejsou seřazeny abecedně podle plného názvu typu.
