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
        public string DiagnosticId { get; } = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNoDocumentation()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {                                                                                                 
        public Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }


        private async Task TestEmptyConstructor(string modifiers)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo<TFoo, TBar>
    {{
        /// 
        /// 
        ///                                                                                                 
        {0} 
        Foo()
        {{

        }}
    }}
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyPublicConstructor()
        {
            await this.TestEmptyConstructor("public");
        }

        [Fact]
        public async Task TestEmptyNonPublicConstructor()
        {
            await this.TestEmptyConstructor("private");
        }

        [Fact]
        public async Task TestEmptyStaticConstructor()
        {
            await this.TestEmptyConstructor("static");
        }

        private async Task TestConstructorCorrectDocumentation(string modifiers, string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        {5} Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>
        /// {3}{4}
        /// </summary>
        {5} Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}
        /// <see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        {5} Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstructorCorrectDocumentationSimple(string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentation(modifiers, part1, part2, ".", generic);
        }

        private async Task TestConstructorCorrectDocumentationCustomized(string modifiers, string part1, string part2, bool generic)
        {
            await this.TestConstructorCorrectDocumentation(modifiers, part1, part2, " with A and B.", generic);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimple()
        {
            await this.TestConstructorCorrectDocumentationSimple("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomized()
        {
            await this.TestConstructorCorrectDocumentationCustomized("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimple()
        {
            await this.TestConstructorCorrectDocumentationSimple("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomized()
        {
            await this.TestConstructorCorrectDocumentationCustomized("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentation()
        {
            await this.TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], string.Empty, false);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateSimple()
        {
            await this.TestConstructorCorrectDocumentationSimple("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateCustomized()
        {
            await this.TestConstructorCorrectDocumentationCustomized("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric()
        {
            await this.TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], string.Empty, true);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateSimple()
        {
            await this.TestConstructorCorrectDocumentationSimple("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateCustomized()
        {
            await this.TestConstructorCorrectDocumentationCustomized("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestStaticConstructorCorrectDocumentation()
        {
            await this.TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], string.Empty, false);
        }

        [Fact]
        public async Task TestStaticConstructorCorrectDocumentationGeneric()
        {
            await this.TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], string.Empty, true);
        }

        private async Task TestConstructorMissingDocumentation(string modifiers, string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// </summary>
        {1} 
        Foo()
        {{

        }}
    }}
}}";
            testCode = string.Format(testCode, generic ? "<T1, T2>" : string.Empty, modifiers);

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode,
                expected, CancellationToken.None);


            var fixedCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        {5} 
        Foo()
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers);
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestNonPrivateConstructorMissingDocumentation()
        {
            await this.TestConstructorMissingDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorMissingDocumentationGeneric()
        {
            await this.TestConstructorMissingDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestPrivateConstructorMissingDocumentation()
        {
            await this.TestConstructorMissingDocumentation("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestPrivateConstructorMissingDocumentationGeneric()
        {
            await this.TestConstructorMissingDocumentation("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [Fact]
        public async Task TestStaticConstructorMissingDocumentation()
        {
            await this.TestConstructorMissingDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], false);
        }

        [Fact]
        public async Task TestStaticConstructorMissingDocumentationGeneric()
        {
            await this.TestConstructorMissingDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], true);
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
