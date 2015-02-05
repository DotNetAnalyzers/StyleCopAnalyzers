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
    using static StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    [TestClass]
    public class SA1642UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId;

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
        public Foo()
        {

        }
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyPublicConstructor()
        {
            await TestEmptyConstructor("public");
        }

        [TestMethod]
        public async Task TestEmptyNonPublicConstructor()
        {
            await TestEmptyConstructor("private");
        }

        [TestMethod]
        public async Task TestEmptyStaticConstructor()
        {
            await TestEmptyConstructor("static");
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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);

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

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestConstructorCorrectDocumentationSimple(string modifiers, string part1, string part2, bool generic)
        {
            await TestConstructorCorrectDocumentation(modifiers, part1, part2, ".", generic);
        }

        private async Task TestConstructorCorrectDocumentationCustomized(string modifiers, string part1, string part2, bool generic)
        {
            await TestConstructorCorrectDocumentation(modifiers, part1, part2, " with A and B.", generic);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationSimple()
        {
            await TestConstructorCorrectDocumentationSimple("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationCustomized()
        {
            await TestConstructorCorrectDocumentationCustomized("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericSimple()
        {
            await TestConstructorCorrectDocumentationSimple("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationGenericCustomized()
        {
            await TestConstructorCorrectDocumentationCustomized("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentation()
        {
            await TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], string.Empty, false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateSimple()
        {
            await TestConstructorCorrectDocumentationSimple("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentation_NonPrivateCustomized()
        {
            await TestConstructorCorrectDocumentationCustomized("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric()
        {
            await TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], string.Empty, true);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateSimple()
        {
            await TestConstructorCorrectDocumentationSimple("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric_NonPrivateCustomized()
        {
            await TestConstructorCorrectDocumentationCustomized("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestStaticConstructorCorrectDocumentation()
        {
            await TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], string.Empty, false);
        }

        [TestMethod]
        public async Task TestStaticConstructorCorrectDocumentationGeneric()
        {
            await TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], string.Empty, true);
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Constructor summary documentation must begin with standard text",
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
        {5} 
        Foo()
        {{

        }}
    }}
}}";

            string part3 = part2.EndsWith(".") ? string.Empty : ".";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, part3, modifiers);
            await VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorMissingDocumentation()
        {
            await TestConstructorMissingDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorMissingDocumentationGeneric()
        {
            await TestConstructorMissingDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestPrivateConstructorMissingDocumentation()
        {
            await TestConstructorMissingDocumentation("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorMissingDocumentationGeneric()
        {
            await TestConstructorMissingDocumentation("private", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestStaticConstructorMissingDocumentation()
        {
            await TestConstructorMissingDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestStaticConstructorMissingDocumentationGeneric()
        {
            await TestConstructorMissingDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], true);
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
