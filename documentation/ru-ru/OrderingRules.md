### Правила упорядочивания (SA1200-)
Правила, обеспечивающие стандартный порядок содержимого кода.

Идентификатор | Имя | Описание
-----------|------|-------------
[SA1200](SA1200.md) | UsingDirectivesMustBePlacedCorrectly | Директива using языка C# размещена вне элемента пространства имён.
[SA1201](SA1201.md) | ElementsMustAppearInTheCorrectOrder | Элемент в файле кода C# расположен в неправильном порядке относительно других элементов.
[SA1202](SA1202.md) | ElementsMustBeOrderedByAccess | Элемент в файле кода C# расположен в неправильном порядке по уровню доступа относительно других элементов.
[SA1203](SA1203.md) | ConstantsMustAppearBeforeFields | Поле-константа размещено ниже поля, не являющегося константой.
[SA1204](SA1204.md) | StaticElementsMustAppearBeforeInstanceElements | Статический элемент размещён ниже элемента экземпляра того же вида.
[SA1205](SA1205.md) | PartialElementsMustDeclareAccess | У частичного элемента не указан модификатор доступа.
[SA1206](SA1206.md) | DeclarationKeywordsMustFollowOrder | Ключевые слова в объявлении элемента не следуют стандартному порядку.
[SA1207](SA1207.md) | ProtectedMustComeBeforeInternal | Ключевое слово *protected* размещено после *internal* в объявлении элемента C# с доступом protected internal.
[SA1208](SA1208.md) | SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives | В файле кода C# директива using для пространства имён *System* расположена после директивы using для другого пространства имён.
[SA1209](SA1209.md) | UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives | Директива using с псевдонимом размещена перед обычной директивой using.
[SA1210](SA1210.md) | UsingDirectivesMustBeOrderedAlphabeticallyByNamespace | Директивы using в файле кода C# не отсортированы по алфавиту по пространствам имён.
[SA1211](SA1211.md) | UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName | Директивы using с псевдонимами в файле кода C# не отсортированы по алфавиту по именам псевдонимов.
[SA1212](SA1212.md) | PropertyAccessorsMustFollowOrder | Метод доступа get расположен после set внутри свойства или индексатора.
[SA1213](SA1213.md) | EventAccessorsMustFollowOrder | Метод доступа add расположен после remove внутри события.
[SA1214](SA1214.md) | ReadonlyElementsMustAppearBeforeNonReadonlyElements | Поле readonly размещено ниже поля без модификатора readonly.
[SA1215](SA1215.md) | InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements | Элемент экземпляра readonly размещён ниже элемента экземпляра того же вида без модификатора readonly.
[SA1216](SA1216.md) | UsingStaticDirectivesMustBePlacedAtTheCorrectLocation | Директива `using static` размещена в неправильном месте: перед обычной директивой using или после директивы using с псевдонимом.
[SA1217](SA1217.md) | UsingStaticDirectivesMustBeOrderedAlphabetically | Директивы `using static` в файле кода C# не отсортированы по алфавиту по полным именам типов.
