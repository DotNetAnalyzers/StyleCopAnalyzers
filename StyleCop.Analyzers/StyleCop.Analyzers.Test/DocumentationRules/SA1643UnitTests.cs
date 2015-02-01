namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using static StyleCop.Analyzers.DocumentationRules.SA1643DestructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1643DestructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    [TestClass]
    public class SA1643UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1643DestructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDestructorCorrectDocumentationSimple(string part1, string part2, bool generic)
        {
            await TestDestructorCorrectDocumentation(part1, part2, ".", generic);
        }

        private async Task TestDestructorCorrectDocumentationCustomized(string part1, string part2, bool generic)
        {
            await TestDestructorCorrectDocumentation(part1, part2, " with A and B.", generic);
        }

        [TestMethod]
        public async Task TestDestructorCorrectDocumentationSimple()
        {
            await TestDestructorCorrectDocumentationSimple(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestDestructorCorrectDocumentationCustomized()
        {
            await TestDestructorCorrectDocumentationCustomized(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimple()
        {
            await TestDestructorCorrectDocumentationSimple(DestructorStandardText[0], DestructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestDestructorCorrectDocumentationGenericCustomized()
        {
            await TestDestructorCorrectDocumentationCustomized(DestructorStandardText[0], DestructorStandardText[1], true);
        }

        private async Task TestDestructorMissingDocumentation(string part1, string part2, bool generic)
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Destructor summary documentation must begin with standard text",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode,
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
            await VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [TestMethod]
        public async Task TestDestructorMissingDocumentation()
        {
            await TestDestructorMissingDocumentation(DestructorStandardText[0], DestructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestDestructorMissingDocumentationGeneric()
        {
            await TestDestructorMissingDocumentation(DestructorStandardText[0], DestructorStandardText[1], true);
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
