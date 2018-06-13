// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>.
    /// </summary>
    [UseCulture("en-US")]
    public class SA1642UnitTests : CodeFixVerifier
    {
        private string settings = null;

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyPublicConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "public").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyNonPublicConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "private").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyStaticConstructorAsync(string typeKind)
        {
            await this.TestEmptyConstructorAsync(typeKind, "static").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimpleAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimpleAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomizedAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(
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
            await this.TestConstructorCorrectDocumentationAsync(
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
            await this.TestConstructorCorrectDocumentationSimpleAsync(
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
            await this.TestConstructorCorrectDocumentationCustomizedAsync(
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
            await this.TestConstructorCorrectDocumentationAsync(
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
            await this.TestConstructorCorrectDocumentationAsync(
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
            await this.TestConstructorCorrectDocumentationSimpleAsync(
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
            await this.TestConstructorCorrectDocumentationCustomizedAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentationAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                string.Empty,
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                string.Empty,
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(
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
            await this.TestConstructorMissingDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentationAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorMissingDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(
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
            await this.TestConstructorSimpleDocumentationAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationGenericAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "public",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
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
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "private",
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.NonPrivateConstructorStandardTextSecondPart, typeKind),
                true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
                false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorSimpleDocumentationGenericWrongTypeNameAsync(string typeKind)
        {
            await this.TestConstructorSimpleDocumentationWrongTypeNameAsync(
                typeKind,
                "static",
                string.Format(DocumentationResources.StaticConstructorStandardTextFirstPart, typeKind),
                string.Format(DocumentationResources.StaticConstructorStandardTextSecondPart, typeKind),
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 13);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 43);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty see tag is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithEmptySeeTagAsync()
        {
            string testCode = @"
public class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see/> class.
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
    /// </summary>
    public TestClass()
    {
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 43);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            //// TODO: The codefix produces a wrong result for this scenario but its not easily fixed.

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            ////await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(2236, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2236")]
        public async Task TestDocumentationCultureIsUsedAsync(string typeKind)
        {
            this.settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentationCulture"": ""en-GB"",
    }
  }
}
";

            await this.TestConstructorCorrectDocumentationSimpleAsync(
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
        /// <summary>
        /// Initializes a new instance of the <see cref=""ClassName""/> class.{emptyContent}</summary>
        public ClassName()
        {{
        }}
    }}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override Project ApplyCompilationOptions(Project project)
        {
            var resolver = new TestXmlReferenceResolver();

            string contentValidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""TestClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("ValidSummary.xml", contentValidSummary);

            string contentMissingSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("MissingSummary.xml", contentMissingSummary);

            string contentEmptySummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary></summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("EmptySummary.xml", contentEmptySummary);

            string contentInvalidSummary = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Creates the <see cref=""TestClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("InvalidSummary.xml", contentInvalidSummary);

            string contentInvalidReference = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""WrongClass""/> class.</summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("InvalidReference.xml", contentInvalidReference);

            string contentNoReference = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see /> class.</summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("NoReference.xml", contentNoReference);

            string contentInvalidSecondPart = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<TestClass>
  <TestClass>
    <summary>Initializes a new instance of the <see cref=""TestClass""/>.</summary>
  </TestClass>
</TestClass>
";
            resolver.XmlReferences.Add("InvalidSecondPart.xml", contentInvalidSecondPart);

            project = base.ApplyCompilationOptions(project);
            project = project.WithCompilationOptions(project.CompilationOptions.WithXmlReferenceResolver(resolver));
            return project;
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1642ConstructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }

        protected override string GetSettings()
        {
            if (this.settings == null)
            {
                return base.GetSettings();
            }

            return this.settings;
        }

        private async Task TestEmptyConstructorAsync(string typeKind, string modifiers)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationAsync(string typeKind, string modifiers, string part1, string part2, string part3, bool generic)
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationSimpleAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, modifiers, part1, part2, ".", generic).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationCustomizedAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentationAsync(typeKind, modifiers, part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        private async Task TestConstructorMissingDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            string typeParameters = generic ? "<T1, T2>" : string.Empty;
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestConstructorSimpleDocumentationAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(
                testCode,
                expected,
                CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        private async Task TestConstructorSimpleDocumentationWrongTypeNameAsync(string typeKind, string modifiers, string part1, string part2, bool generic)
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(
                testCode,
                expected,
                CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }
    }
}
