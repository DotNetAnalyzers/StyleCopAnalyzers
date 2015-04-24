namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    public class SA1642UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNoDocumentation(string typeKind)
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

        private async Task TestEmptyConstructor(string typeKind, string modifiers)
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

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyPublicConstructor(string typeKind)
        {
            await this.TestEmptyConstructor(typeKind, "public").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyNonPublicConstructor(string typeKind)
        {
            await this.TestEmptyConstructor(typeKind, "private").ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyStaticConstructor(string typeKind)
        {
            await this.TestEmptyConstructor(typeKind, "static").ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentation(string typeKind, string modifiers, string part1, string part2, string part3, bool generic)
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

        private async Task TestConstructorCorrectDocumentationSimple(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, modifiers, part1, part2, ".", generic).ConfigureAwait(false);
        }

        private async Task TestConstructorCorrectDocumentationCustomized(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, modifiers, part1, part2, " with A and B.", generic).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimple(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimple(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomized(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomized(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimple(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimple(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomized(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomized(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation(string typeKind)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind}" + PrivateConstructorStandardText[1], string.Empty, false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateSimple(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimple(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateCustomized(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomized(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric(string typeKind)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, "private", PrivateConstructorStandardText[0], $" {typeKind}" + PrivateConstructorStandardText[1], string.Empty, true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateSimple(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationSimple(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateCustomized(string typeKind)
        {
            await this.TestConstructorCorrectDocumentationCustomized(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentation(string typeKind)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", string.Empty, false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorCorrectDocumentationGeneric(string typeKind)
        {
            await this.TestConstructorCorrectDocumentation(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", string.Empty, true).ConfigureAwait(false);
        }

        private async Task TestConstructorMissingDocumentation(string typeKind, string modifiers, string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public {0} Foo{1}
    {{
        /// <summary>
        /// </summary>
        {2}
        Foo({3})
        {{

        }}
    }}
}}";
            string arguments = typeKind == "struct" && modifiers != "static" ? "int argument" : null;
            testCode = string.Format(testCode, typeKind, generic ? "<T1, T2>" : string.Empty, modifiers, arguments);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode,
                expected, CancellationToken.None).ConfigureAwait(false);

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

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, typeKind, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers, arguments);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentation(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestNonPrivateConstructorMissingDocumentationGeneric(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "public", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentation(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestPrivateConstructorMissingDocumentationGeneric(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "private", NonPrivateConstructorStandardText, $" {typeKind}", true).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentation(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestStaticConstructorMissingDocumentationGeneric(string typeKind)
        {
            await this.TestConstructorMissingDocumentation(typeKind, "static", StaticConstructorStandardText, $" {typeKind}.", true).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#676 "SA1642 misfires on nested structs,
        /// requiring text describing the outer type"
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/676
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestStructNestedInClass()
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
        [InlineData("class", "struct", "class")]
        [InlineData("class", "struct", "struct")]
        [InlineData("struct", "class", "class")]
        [InlineData("struct", "struct", "class")]
        [InlineData("struct", "struct", "struct")]
        public async Task TestAllowedOuterQualifiedNames(string outerTypeKind, string nestedTypeKind, string describedTypeKind)
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

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1642ConstructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }
    }
}
