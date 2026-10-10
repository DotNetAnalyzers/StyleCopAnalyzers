### Правила документирования (SA1600-)
Правила, проверяющие содержимое и оформление документации кода.

Идентификатор | Имя | Описание
--------------|-----|---------
[SA1600](SA1600.md) | ElementsMustBeDocumented | У элемента кода C# отсутствует комментарий документации.
[SA1601](SA1601.md) | PartialElementsMustBeDocumented | У частичного элемента C# отсутствует комментарий документации.
[SA1602](SA1602.md) | EnumerationItemsMustBeDocumented | У элемента перечисления C# отсутствует XML-комментарий документации.
[SA1603](SA1603.md) | DocumentationMustContainValidXml | XML в комментарии документации элемента C# имеет некорректную структуру.
[SA1604](SA1604.md) | ElementDocumentationMustHaveSummary | В XML-комментарии документации элемента C# отсутствует тег `<summary>`.
[SA1605](SA1605.md) | PartialElementDocumentationMustHaveSummary | Тег `<summary>` или `<content>` в комментарии документации элемента кода C# отсутствует или пуст.
[SA1606](SA1606.md) | ElementDocumentationMustHaveSummaryText | Тег `<summary>` в комментарии документации элемента кода C# пуст.
[SA1607](SA1607.md) | PartialElementDocumentationMustHaveSummaryText | Тег `<summary>` или `<content>` в комментарии документации элемента кода C# пуст.
[SA1608](SA1608.md) | ElementDocumentationMustNotHaveDefaultSummary | Тег `<summary>` в XML-комментарии документации элемента содержит текст по умолчанию, сгенерированный Visual Studio при создании элемента.
[SA1609](SA1609.md) | PropertyDocumentationMustHaveValue | XML-комментарий документации свойства C# не содержит тега `<value>`.
[SA1610](SA1610.md) | PropertyDocumentationMustHaveValueText | XML-комментарий документации свойства C# содержит пустой тег `<value>`.
[SA1611](SA1611.md) | ElementParametersMustBeDocumented | У метода, конструктора, делегата или индексатора C# отсутствует документация для одного или нескольких параметров.
[SA1612](SA1612.md) | ElementParameterDocumentationMustMatchElementParameters | Документация параметров метода, конструктора, делегата или индексатора C# не соответствует фактическим параметрам элемента.
[SA1613](SA1613.md) | ElementParameterDocumentationMustDeclareParameterName | У тега `<param>` в комментарии документации элемента C# отсутствует атрибут name с именем параметра.
[SA1614](SA1614.md) | ElementParameterDocumentationMustHaveText | Тег `<param>` в комментарии документации элемента C# пуст.
[SA1615](SA1615.md) | ElementReturnValueMustBeDocumented | У элемента C# отсутствует документация возвращаемого значения.
[SA1616](SA1616.md) | ElementReturnValueDocumentationMustHaveText | Тег `<returns>` в комментарии документации элемента C# пуст.
[SA1617](SA1617.md) | VoidReturnValueMustNotBeDocumented | Элемент кода C# не возвращает значения или возвращает void, но его комментарий документации содержит тег `<returns>`.
[SA1618](SA1618.md) | GenericTypeParametersMustBeDocumented | У обобщённого элемента C# отсутствует документация для одного или нескольких параметров типа.
[SA1619](SA1619.md) | GenericTypeParametersMustBeDocumentedPartialClass | У обобщённого частичного элемента C# отсутствует документация для одного или нескольких параметров типа, а документация элемента содержит тег `<summary>`.
[SA1620](SA1620.md) | GenericTypeParameterDocumentationMustMatchTypeParameters | Теги `<typeparam>` в XML-комментарии документации обобщённого элемента C# не соответствуют параметрам типа элемента.
[SA1621](SA1621.md) | GenericTypeParameterDocumentationMustDeclareParameterName | У тега `<typeparam>` в XML-комментарии документации обобщённого элемента C# отсутствует атрибут name или этот атрибут пуст.
[SA1622](SA1622.md) | GenericTypeParameterDocumentationMustHaveText | Тег `<typeparam>` в XML-комментарии документации обобщённого элемента C# пуст.
[SA1623](SA1623.md) | PropertySummaryDocumentationMustMatchAccessors | Текст документации в теге `<summary>` свойства C# не соответствует методам доступа свойства.
[SA1624](SA1624.md) | PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess | Текст документации в теге `<summary>` свойства C# учитывает все методы доступа свойства, хотя один из них имеет ограниченную доступность.
[SA1625](SA1625.md) | ElementDocumentationMustNotBeCopiedAndPasted | XML-документация элемента C# содержит две или более одинаковые записи, что указывает на копирование и вставку документации.
[SA1626](SA1626.md) | SingleLineCommentsMustNotUseDocumentationStyleSlashes | Код C# содержит однострочный комментарий, начинающийся с трёх косых черт подряд.
[SA1627](SA1627.md) | DocumentationTextMustNotBeEmpty | XML-комментарий документации элемента кода C# содержит пустой тег.
[SA1628](SA1628.md) | DocumentationTextMustBeginWithACapitalLetter | Раздел XML-комментария документации элемента C# не начинается с прописной буквы.
[SA1629](SA1629.md) | DocumentationTextMustEndWithAPeriod | Раздел XML-комментария документации элемента C# не заканчивается точкой.
[SA1630](SA1630.md) | DocumentationTextMustContainWhitespace | Раздел XML-комментария документации элемента C# не содержит пробельных символов между словами.
[SA1631](SA1631.md) | DocumentationMustMeetCharacterPercentage | Раздел XML-комментария документации элемента C# содержит недостаточно букв.
[SA1632](SA1632.md) | DocumentationTextMustMeetMinimumCharacterLength | Начиная со StyleCop 4.5 это правило отключено по умолчанию.
[SA1633](SA1633.md) | FileMustHaveHeader | В файле кода C# отсутствует стандартный заголовок файла.
[SA1634](SA1634.md) | FileHeaderMustShowCopyright | В заголовке файла в начале файла кода C# отсутствует тег copyright.
[SA1635](SA1635.md) | FileHeaderMustHaveCopyrightText | В заголовке файла в начале файла кода C# отсутствует текст об авторских правах.
[SA1636](SA1636.md) | FileHeaderCopyrightTextMustMatch | Заголовок файла в начале файла кода C# не содержит требуемого текста об авторских правах.
[SA1637](SA1637.md) | FileHeaderMustContainFileName | В заголовке файла в начале файла кода C# отсутствует имя файла.
[SA1638](SA1638.md) | FileHeaderFileNameDocumentationMustMatchFileName | Атрибут file в заголовке файла в начале файла кода C# не содержит имя этого файла.
[SA1639](SA1639.md) | FileHeaderMustHaveSummary | Заголовок файла в начале файла кода C# не содержит заполненного тега summary.
[SA1640](SA1640.md) | FileHeaderMustHaveValidCompanyText | Заголовок файла в начале файла кода C# не содержит названия компании.
[SA1641](SA1641.md) | FileHeaderCompanyNameTextMustMatch | Заголовок файла в начале файла кода C# не содержит требуемого названия компании.
[SA1642](SA1642.md) | ConstructorSummaryDocumentationMustBeginWithStandardText | XML-комментарий документации конструктора C# не содержит требуемого текста краткого описания.
[SA1643](SA1643.md) | DestructorSummaryDocumentationMustBeginWithStandardText | XML-комментарий документации финализатора C# не содержит требуемого текста краткого описания.
[SA1644](SA1644.md) | DocumentationHeadersMustNotContainBlankLines | Раздел XML-комментария документации элемента C# содержит пустые строки.
[SA1645](SA1645.md) | IncludedDocumentationFileDoesNotExist | Включаемый файл XML-документации не существует.
[SA1646](SA1646.md) | IncludedDocumentationXPathDoesNotExist | Ссылка на включаемую XML-документацию содержит недопустимый путь.
[SA1647](SA1647.md) | IncludeNodeDoesNotContainValidFileAndPath | Тег include в XML-комментарии документации не содержит корректных атрибутов file и path.
[SA1648](SA1648.md) | InheritDocMustBeUsedWithInheritingClass | Тег `<inheritdoc>` использован у элемента, который не наследует базовый класс и не реализует интерфейс.
[SA1649](SA1649.md) | FileNameMustMatchTypeName | Имя файла кода C# не соответствует первому типу, объявленному в файле.
[SA1650](SA1650.md) | ElementDocumentationMustBeSpelledCorrectly | Документация элемента содержит одну или несколько орфографических ошибок либо нераспознанных слов.
[SA1651](SA1651.md) | DoNotUsePlaceholderElements | Документация элемента содержит один или несколько элементов `<placeholder>`.
[SA1652](SA1652.md) | EnableXmlDocumentationOutput | Это правило перенесено в [SA0001](SA0001.md).
