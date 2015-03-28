namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.DocumentationRules.SA1643DestructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    public class SA1643UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1643DestructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId;

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
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestEmptyDestructor()
        {
            var testCode = @"namespace FooNamespace
{
    public class Foo<TFoo, TBar>
    {
        /// 
        /// 
        /// 
        ~Foo()
        {

        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDestructorCorrectDocumentation(string part1, string part2, string part3, bool generic)
        {
            // First test it all on one line
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);

            // Then test splitting after the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>
        /// {3}{4}
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);

            // Then test splitting before the <see> element
            testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}
        /// <see cref=""Foo{1}""/>{3}{4}
        /// </summary>
        Foo()
        {{

        }}
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDestructorCorrectDocumentationSimpleImpl(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentation(part1, part2, ".", generic);
        }

        private async Task TestDestructorCorrectDocumentationCustomizedImpl(string part1, string part2, bool generic)
        {
            await this.TestDestructorCorrectDocumentation(part1, part2, " with A and B.", generic);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationSimple()
        {
            await this.TestDestructorCorrectDocumentationSimpleImpl(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationCustomized()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImpl(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [Fact]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimple()
        {
            await this.TestDestructorCorrectDocumentationSimpleImpl(DestructorStandardText[0], DestructorStandardText[1], true);
        }

        [Fact]
        public async Task TestDestructorCorrectDocumentationGenericCustomized()
        {
            await this.TestDestructorCorrectDocumentationCustomizedImpl(DestructorStandardText[0], DestructorStandardText[1], true);
        }

        private async Task TestDestructorMissingDocumentationImpl(string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// </summary>
        ~Foo()
        {{

        }}
    }}
}}";
            testCode = string.Format(testCode, generic ? "<T1, T2>" : string.Empty);

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
        ~Foo()
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3);
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentation()
        {
            await this.TestDestructorMissingDocumentationImpl(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [Fact]
        public async Task TestDestructorMissingDocumentationGeneric()
        {
            await this.TestDestructorMissingDocumentationImpl(DestructorStandardText[0], DestructorStandardText[1], true);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1643DestructorSummaryDocumentationMustBeginWithStandardText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1642SA1643CodeFixProvider();
        }
    }
}
