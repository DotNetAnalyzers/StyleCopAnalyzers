### 文档规则 (SA1600-)
这些规则验证代码文档的内容和格式。

标识符 | 名称 | 说明
-----------|------|------------
[SA1600](SA1600.md) | ElementsMustBeDocumented | C# 代码元素缺少文档注释头。
[SA1601](SA1601.md) | PartialElementsMustBeDocumented | C# 分部元素缺少文档注释头。
[SA1602](SA1602.md) | EnumerationItemsMustBeDocumented | C# 枚举中的某个项缺少 XML 文档注释头。
[SA1603](SA1603.md) | DocumentationMustContainValidXml | C# 元素的文档注释头中的 XML 格式不正确。
[SA1604](SA1604.md) | ElementDocumentationMustHaveSummary | C# 元素的 XML 文档注释头缺少 `<summary>` 标记。
[SA1605](SA1605.md) | PartialElementDocumentationMustHaveSummary | C# 代码元素的文档注释头中的 `<summary>` 或 `<content>` 标记缺失或为空。
[SA1606](SA1606.md) | ElementDocumentationMustHaveSummaryText | C# 代码元素的文档注释头中的 `<summary>` 标记为空。
[SA1607](SA1607.md) | PartialElementDocumentationMustHaveSummaryText | C# 代码元素的文档注释头中的 `<summary>` 或 `<content>` 标记为空。
[SA1608](SA1608.md) | ElementDocumentationMustNotHaveDefaultSummary | 元素的 XML 文档注释头中的 `<summary>` 标记包含 Visual Studio 创建该元素时生成的默认文本。
[SA1609](SA1609.md) | PropertyDocumentationMustHaveValue | C# 属性的 XML 文档注释头不包含 `<value>` 标记。
[SA1610](SA1610.md) | PropertyDocumentationMustHaveValueText | C# 属性的 XML 文档注释头包含空的 `<value>` 标记。
[SA1611](SA1611.md) | ElementParametersMustBeDocumented | C# 方法、构造函数、委托或索引器元素缺少一个或多个参数的文档。
[SA1612](SA1612.md) | ElementParameterDocumentationMustMatchElementParameters | 描述 C# 方法、构造函数、委托或索引器元素参数的文档，与该元素的实际参数不匹配。
[SA1613](SA1613.md) | ElementParameterDocumentationMustDeclareParameterName | C# 元素的文档注释头中的 `<param>` 标记缺少包含参数名称的 name 属性。
[SA1614](SA1614.md) | ElementParameterDocumentationMustHaveText | C# 元素的文档注释头中的 `<param>` 标记为空。
[SA1615](SA1615.md) | ElementReturnValueMustBeDocumented | C# 元素缺少返回值的文档。
[SA1616](SA1616.md) | ElementReturnValueDocumentationMustHaveText | C# 元素的文档注释头中的 `<returns>` 标记为空。
[SA1617](SA1617.md) | VoidReturnValueMustNotBeDocumented | C# 代码元素没有返回值或返回 void，但其文档注释头包含 `<returns>` 标记。
[SA1618](SA1618.md) | GenericTypeParametersMustBeDocumented | C# 泛型元素缺少一个或多个泛型类型参数的文档。
[SA1619](SA1619.md) | GenericTypeParametersMustBeDocumentedPartialClass | C# 泛型分部元素缺少一个或多个泛型类型参数的文档，且该元素的文档包含 `<summary>` 标记。
[SA1620](SA1620.md) | GenericTypeParameterDocumentationMustMatchTypeParameters | C# 泛型元素的 XML 文档注释头中的 `<typeparam>` 标记，与该元素的泛型类型参数不匹配。
[SA1621](SA1621.md) | GenericTypeParameterDocumentationMustDeclareParameterName | C# 泛型元素的 XML 文档注释头中的 `<typeparam>` 标记缺少 name 属性，或其 name 属性为空。
[SA1622](SA1622.md) | GenericTypeParameterDocumentationMustHaveText | C# 泛型元素的 XML 文档注释头中的 `<typeparam>` 标记为空。
[SA1623](SA1623.md) | PropertySummaryDocumentationMustMatchAccessors | C# 属性的 `<summary>` 标记中的文档文本，与该属性的访问器不匹配。
[SA1624](SA1624.md) | PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess | C# 属性的 `<summary>` 标记中的文档文本描述了所有访问器，但其中一个访问器的访问权限受限。
[SA1625](SA1625.md) | ElementDocumentationMustNotBeCopiedAndPasted | C# 元素的 XML 文档包含两个或更多相同的条目，表明文档经过了复制粘贴。
[SA1626](SA1626.md) | SingleLineCommentsMustNotUseDocumentationStyleSlashes | C# 代码包含以三个连续正斜杠开头的单行注释。
[SA1627](SA1627.md) | DocumentationTextMustNotBeEmpty | C# 代码元素的 XML 文档注释头包含空标记。
[SA1628](SA1628.md) | DocumentationTextMustBeginWithACapitalLetter | C# 元素的 XML 文档注释头中的某一部分未以大写字母开头。
[SA1629](SA1629.md) | DocumentationTextMustEndWithAPeriod | C# 元素的 XML 文档注释头中的某一部分未以句号结尾。
[SA1630](SA1630.md) | DocumentationTextMustContainWhitespace | C# 元素的 XML 文档注释头中的某一部分在单词之间没有任何空白字符。
[SA1631](SA1631.md) | DocumentationMustMeetCharacterPercentage | C# 元素的 XML 文档注释头中的某一部分包含的字母字符不足。
[SA1632](SA1632.md) | DocumentationTextMustMeetMinimumCharacterLength | 从 StyleCop 4.5 开始，此规则默认禁用。
[SA1633](SA1633.md) | FileMustHaveHeader | C# 代码文件缺少标准文件头。
[SA1634](SA1634.md) | FileHeaderMustShowCopyright | C# 代码文件顶部的文件头缺少 copyright 标记。
[SA1635](SA1635.md) | FileHeaderMustHaveCopyrightText | C# 代码文件顶部的文件头缺少版权文本。
[SA1636](SA1636.md) | FileHeaderCopyrightTextMustMatch | C# 代码文件顶部的文件头不包含正确的版权文本。
[SA1637](SA1637.md) | FileHeaderMustContainFileName | C# 代码文件顶部的文件头缺少文件名。
[SA1638](SA1638.md) | FileHeaderFileNameDocumentationMustMatchFileName | C# 代码文件顶部文件头中的 file 标记不包含该文件的名称。
[SA1639](SA1639.md) | FileHeaderMustHaveSummary | C# 代码文件顶部的文件头不包含已填写的 summary 标记。
[SA1640](SA1640.md) | FileHeaderMustHaveValidCompanyText | C# 代码文件顶部的文件头不包含公司名称文本。
[SA1641](SA1641.md) | FileHeaderCompanyNameTextMustMatch | C# 代码文件顶部的文件头不包含正确的公司名称文本。
[SA1642](SA1642.md) | ConstructorSummaryDocumentationMustBeginWithStandardText | C# 构造函数的 XML 文档注释头不包含正确的摘要文本。
[SA1643](SA1643.md) | DestructorSummaryDocumentationMustBeginWithStandardText | C# 终结器的 XML 文档注释头不包含正确的摘要文本。
[SA1644](SA1644.md) | DocumentationHeadersMustNotContainBlankLines | C# 元素的 XML 文档注释头中的某一部分包含空行。
[SA1645](SA1645.md) | IncludedDocumentationFileDoesNotExist | 包含的 XML 文档文件不存在。
[SA1646](SA1646.md) | IncludedDocumentationXPathDoesNotExist | 包含的 XML 文档链接包含无效路径。
[SA1647](SA1647.md) | IncludeNodeDoesNotContainValidFileAndPath | XML 文档注释头中的 include 标记不包含有效的 file 和 path 属性。
[SA1648](SA1648.md) | InheritDocMustBeUsedWithInheritingClass | 在既不继承基类也不实现接口的元素上使用了 `<inheritdoc>`。
[SA1649](SA1649.md) | FileNameMustMatchTypeName | C# 代码文件的文件名与文件中声明的第一个类型不匹配。
[SA1650](SA1650.md) | ElementDocumentationMustBeSpelledCorrectly | 元素的文档包含一个或多个拼写错误或无法识别的单词。
[SA1651](SA1651.md) | DoNotUsePlaceholderElements | 元素的文档包含一个或多个 `<placeholder>` 元素。
[SA1652](SA1652.md) | EnableXmlDocumentationOutput | 此规则已移至 [SA0001](SA0001.md)。
