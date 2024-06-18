// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Test.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>.
    /// </summary>
    [UseCulture("en-US")]
    public class SA1642UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNoDocumentationAsync(string typeKind)
        {
            var testCode = $@"namespace FooNamespace
{{
    public {typeKind} Foo<TFoo, TBar>
    {{
        public Foo(int arg)
        {{
        }}
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestEmptyPublicConstructorAsync(string typeKind)
        {
            await TestEmptyConstructorAsync(typeKind, "public").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyNonPublicConstructorAsync(string typeKind)
        {
            await TestEmptyConstructorAsync(typeKind, "private").ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestEmptyStaticConstructorAsync(string typeKind)
        {
            await TestEmptyConstructorAsync(typeKind, "static").ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimpleAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomizedAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomizedAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.PrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.PrivateConstructorStandardTextSecondPart, typeKind),
                string.Empty,
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectExtendedDocumentationAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.PrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.PrivateConstructorStandardTextSecondPart, typeKind) + " externally",
                string.Empty,
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateSimpleAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateCustomizedAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGenericAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.PrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.PrivateConstructorStandardTextSecondPart, typeKind),
                string.Empty,
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectExtendedDocumentationGenericAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.PrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.PrivateConstructorStandardTextSecondPart, typeKind) + " externally",
                string.Empty,
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateSimpleAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateCustomizedAsync(string typeKind)
        {
            await TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorCorrectDocumentationAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                string.Empty,
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorCorrectDocumentationGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorCorrectDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                string.Empty,
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorMissingDocumentationAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorMissingDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorSimpleDocumentationAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestStaticConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            await TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, docTypeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, docTypeKind),
                true).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for "SA1642 misfires on nested structs, requiring text describing the outer type".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(676, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/676")]
        public async Task TestStructNestedInClassAsync()
        {
            string testCode = @"
class ClassName
{
    struct StructName
    {
        /// <summary>
        /// </summary>
        StructName(int argument)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    struct StructName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""StructName""/> struct.
        /// </summary>
        StructName(int argument)
        {
        }
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", "class", "class")]
        [InlineData("class", "struct", "struct")]
        [InlineData("struct", "class", "class")]
        [InlineData("struct", "struct", "struct")]
        public async Task TestAllowedOuterQualifiedNamesAsync(string outerTypeKind, string nestedTypeKind, string describedTypeKind)
        {
            string testCode = $@"
{outerTypeKind} OuterName
{{
    {nestedTypeKind} NestedName
    {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""OuterName.NestedName""/> {describedTypeKind}.
        /// </summary>
        NestedName(int argument)
        {{
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for
        /// <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/780">DotNetAnalyzers/StyleCopAnalyzers#780</see>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFailureIssue780Async()
        {
            string testCode = @"
internal abstract class CustomizableBlockSubscriberBase<TSource, TTarget, TSubscribedElement>
    where TSource : class
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""ProjectBuildSnapshotService""/> class.
    /// </summary>
    protected CustomizableBlockSubscriberBase()
    {
    }
}
";
            string fixedCode = @"
internal abstract class CustomizableBlockSubscriberBase<TSource, TTarget, TSubscribedElement>
    where TSource : class
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""CustomizableBlockSubscriberBase{TSource, TTarget, TSubscribedElement}""/> class.
    /// </summary>
    protected CustomizableBlockSubscriberBase()
    {
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 43);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestWithEmptySeeTagAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            string testCode = $@"
public {typeKind} TestClass
{{
    /// <summary>
    /// Initializes a new instance of the <see/> {docTypeKind}.
    /// </summary>
    public TestClass(int i)
    {{
    }}
}}
";
            string fixedCode = $@"
public {typeKind} TestClass
{{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> {docTypeKind}.
    /// </summary>
    public TestClass(int i)
    {{
    }}
}}
";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 43);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.DataTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestWithEmptySeeTagGenericAsync(string typeKind)
        {
            var docTypeKind = GetDocTypeKind(typeKind);

            string testCode = $@"
public {typeKind} TestClass<T>
{{
    /// <summary>
    /// Initializes a new instance of the <see/> {docTypeKind}.
    /// </summary>
    public TestClass(int i)
    {{
    }}
}}
";
            string fixedCode = $@"
public {typeKind} TestClass<T>
{{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass{{T}}""/> {docTypeKind}.
    /// </summary>
    public TestClass(int i)
    {{
    }}
}}
";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 43);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid second part of the default text is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithInvalidSecondPartAsync()
        {
            string testCode = @"
public class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> error.
    /// </summary>
    public TestClass()
    {
    }
}
";
            string fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// Initializes a new instance of the <see cref=""TestClass""/> error.
    /// </summary>
    public TestClass()
    {
    }
}
";

            // TODO: The codefix produces a wrong result for this scenario but its not easily fixed.
            DiagnosticResult expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with the correct summary text from included documentation will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithValidSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='ValidSummary.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with the missing summary tag from included documentation will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithMissingSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='MissingSummary.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with an empty summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithEmptySummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='EmptySummary.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with an invalid summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithInvalidSummaryInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidSummary.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with an invalid class reference in the summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithInvalidReferenceInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidReference.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}

public class WrongClass { }
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with an invalid class reference in the summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithInvalidReferenceToNamespaceInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidReference.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}

namespace WrongClass { }
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with an invalid class reference in the summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithInvalidReferenceToNothingInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidReference.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with a missing reference in the summary tag from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithNoReferenceInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='NoReference.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with invalid text after the reference from included documentation will produce a diagnostic and offer no codefix.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorWithInvalidSecondPartInIncludedDocsAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <include file='InvalidSecondPart.xml' path='/TestClass/TestClass/*'/>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(2236, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2236")]
        public async Task TestDocumentationCultureIsUsedAsync(string typeKind)
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentationCulture"": ""en-GB"",
    }
  }
}
";

            await TestConstructorCorrectDocumentationSimpleAsync(
                settings,
                typeKind,
                "public",
                "Initialises a new instance of the ",
                " " + typeKind,
                false).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the codefix will work properly with Visual Studio generated documentation headers.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithDefaultVisualStudioGenerationDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// 
    /// </summary>
    public TestClass() { }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// </summary>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the codefix will work properly when there are multiple empty lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithMultipleEmptyLinesAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public TestClass() { }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// </summary>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(2686, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2686")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestConstructorEmptyDocumentationSingleLineAsync(string emptyContent)
        {
            var testCode = $@"namespace FooNamespace
{{
    public class ClassName
    {{
        /// <summary>{emptyContent}</summary>
        public ClassName()
        {{
        }}
    }}
}}";

            var fixedCode = $@"namespace FooNamespace
{{
    public class ClassName
    {{
        /// <summary>Initializes a new instance of the <see cref=""ClassName""/> class.</summary>
        public ClassName()
        {{
        }}
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2963, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2963")]
        public async Task TestConstructorNoCRefDocumentationSingleLineAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>Initializes a new instance of the TestClass class.</summary>
    public TestClass() { }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>Initializes a new instance of the <see cref=""TestClass""/> class.</summary>
    public TestClass() { }
}
";

            var expected = Diagnostic().WithLocation(4, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(2686, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2686")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestConstructorEmptyDocumentationAsync(string emptyContent)
        {
            var testCode = $@"namespace FooNamespace
{{
    public class ClassName
    {{
        /// <summary>
        ///{emptyContent}
        /// </summary>
        public ClassName()
        {{
        }}
    }}
}}";

            var fixedCode = $@"namespace FooNamespace
{{
    public class ClassName
    {{
        /// <summary>
        /// Initializes a new instance of the <see cref=""ClassName""/> class.
        /// </summary>
        public ClassName()
        {{
        }}
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3575, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3575")]
        public async Task TestConstructorSummaryWithParaTagsAsync()
        {
            var testCode = @"
using System;
/// <summary>
/// Does a thing.
/// </summary>
public class B
{
    /// <summary>
    /// <para>
    /// Initializes a new instance of the <see cref=""B""/> class.
    /// </para>
    /// <para>
    /// Some more info about B.
    /// </para>
    /// </summary>
    public B()
    {
    }
}
";

            var fixedCode = @"
using System;
/// <summary>
/// Does a thing.
/// </summary>
public class B
{
    /// <summary>
    /// <para>
    /// Initializes a new instance of the <see cref=""B""/> class.
    /// </para>
    /// <para>
    /// Some more info about B.
    /// </para>
    /// </summary>
    public B()
    {
    }
}
";

            var expectedDiagnostics = DiagnosticResult.EmptyDiagnosticResults;

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task TestEmptyConstructorAsync(string typeKind, string modifiers)
        {
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo<TFoo, TBar>
    {{
        /// 
        /// 
        /// 
        {1} 
        Foo({2})
        {{

        }}
    }}
}}";

            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, modifiers, arguments), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task TestConstructorCorrectDocumentationAsync(string typeKind, string modifiers, string part1, string part2, string part3, bool generic)
            => TestConstructorCorrectDocumentationAsync(settings: null, typeKind, modifiers, part1, part2, part3, generic);

        private static async Task TestConstructorCorrectDocumentationAsync(string settings, string typeKind, string modifiers, string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>
        /// {4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}
        /// <see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6} Foo({7})
        {{

        }}
    }}
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task TestConstructorCorrectDocumentationSimpleAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
            => TestConstructorCorrectDocumentationSimpleAsync(settings: null, typeKind, modifiers, part1, part2, generic);

        private static async Task TestConstructorCorrectDocumentationSimpleAsync(string settings, string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await TestConstructorCorrectDocumentationAsync(settings, typeKind, modifiers, part1, part2, ".", generic).ConfigureAwait(false);
        }

        private static async Task TestConstructorCorrectDocumentationCustomizedAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await TestConstructorCorrectDocumentationAsync(settings: null, typeKind, modifiers, part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private static async Task TestConstructorMissingDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string typeParameters = generic ? "<T1, T2>" : string.Empty;
            string arguments = IsStruct(typeKind) && modifiers != "static" ? "int argument" : null;
            var testCode = $@"namespace FooNamespace
{{
    public {typeKind} Foo{typeParameters}
    {{
        /// <summary>
        /// </summary>
        {modifiers}
        Foo({arguments})
        {{

        }}
    }}
}}";

            string crefTypeParameters = generic ? "{T1, T2}" : string.Empty;
            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            var fixedCode = $@"namespace FooNamespace
{{
    public {typeKind} Foo{typeParameters}
    {{
        /// <summary>
        /// {part1}<see cref=""Foo{crefTypeParameters}""/>{part2}{part3}
        /// </summary>
        {modifiers}
        Foo({arguments})
        {{

        }}
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task TestConstructorSimpleDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string part3 = part2.EndsWith(".") ? string.Empty : ".";

            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}Foo{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);

            var fixedCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task TestConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string part3 = part2.EndsWith(".") ? string.Empty : ".";

            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}Bar{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);

            DiagnosticResult expected = Diagnostic().WithLocation(5, 13);

            var fixedCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// {3}<see cref=""Foo{2}""/>{4}{5}
        /// </summary>
        {6}
        Foo({7})
        {{

        }}
    }}
}}";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, testSettings: null, expected, cancellationToken);

        private static Task VerifyCSharpDiagnosticAsync(string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = CreateTest(testSettings, expected);
            test.TestCode = source;

            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = CreateTest(testSettings: null, expected);
            test.TestCode = source;
            test.FixedCode = fixedSource;

            return test.RunAsync(cancellationToken);
        }

        private static StyleCopCodeFixVerifier<SA1642ConstructorSummaryDocumentationMustBeginWithStandardText, SA1642SA1643CodeFixProvider>.CSharpTest CreateTest(string testSettings, DiagnosticResult[] expected)
        {
            string contentValidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""TestClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            string contentMissingSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
  </TestClass>
</TestClass>
";
            string contentEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary></summary>
  </TestClass>
</TestClass>
";
            string contentInvalidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Creates the <see cref=""TestClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            string contentInvalidReference = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""WrongClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            string contentNoReference = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see /> class.</summary>
  </TestClass>
</TestClass>
";
            string contentInvalidSecondPart = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""TestClass""/>.</summary>
  </TestClass>
</TestClass>
";

            var test = new StyleCopCodeFixVerifier<SA1642ConstructorSummaryDocumentationMustBeginWithStandardText, SA1642SA1643CodeFixProvider>.CSharpTest
            {
                XmlReferences =
                {
                    { "ValidSummary.xml", contentValidSummary },
                    { "MissingSummary.xml", contentMissingSummary },
                    { "EmptySummary.xml", contentEmptySummary },
                    { "InvalidSummary.xml", contentInvalidSummary },
                    { "InvalidReference.xml", contentInvalidReference },
                    { "NoReference.xml", contentNoReference },
                    { "InvalidSecondPart.xml", contentInvalidSecondPart },
                },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            if (testSettings != null)
            {
                test.Settings = testSettings;
            }

            return test;
        }

        private static string GetDocTypeKind(string codeTypeKind)
        {
            return IsStruct(codeTypeKind) ? "struct" : "class";
        }

        private static bool IsStruct(string codeTypeKind)
        {
            switch (codeTypeKind)
            {
            case "class":
            case "record":
            case "record class":
                return false;

            case "struct":
            case "record struct":
                return true;

            default:
                Debug.Fail($"Unexpected type kind {codeTypeKind}");
                return false;
            }
        }
    }
}
