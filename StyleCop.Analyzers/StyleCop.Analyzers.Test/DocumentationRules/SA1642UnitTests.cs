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

    /// <summary>
    /// This class contains unit tests for <see cref="SA1642ConstructorSummaryDocumentationMustBeginWithStandardText"/>-
    /// </summary>
    [TestClass]
    public class SA1642UnitTests : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };
        internal static string[] NonPrivateConstructorStandardText = new string[] { "Initializes a new instance of the ", " class." };
        internal static string[] PrivateConstructorStandardText = new string[] { "Prevents a default instance of the ", " class from being created." };
        internal static string[] StaticConstructorStandardText = new string[] { "Initializes static members of the ", " class." };


        public string DiagnosticId { get; } = SA1642ConstructorSummaryDocumentationMustBeginWithStandardText.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
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
{
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

        private async Task TestConstructorCorrectDocumentation(string modifiers, string part1, string part2, bool generic)
        {
            var testCode = @"namespace FooNamespace
{{
    public class Foo{0}
    {{
        /// <summary>
        /// {2}<see cref=""Foo{1}""/>{3}
        /// </summary>
        {4} Foo()
        {{

        }}
    }}
}}";


            await VerifyCSharpDiagnosticAsync(string.Format(testCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, modifiers), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentation()
        {
            await TestConstructorCorrectDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestNonPrivateConstructorCorrectDocumentationGeneric()
        {
            await TestConstructorCorrectDocumentation("public", NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentation()
        {
            await TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorCorrectDocumentationGeneric()
        {
            await TestConstructorCorrectDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], true);
        }

        [TestMethod]
        public async Task TestStaticConstructorCorrectDocumentation()
        {
            await TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestStaticConstructorCorrectDocumentationGeneric()
        {
            await TestConstructorCorrectDocumentation("static", StaticConstructorStandardText[0], StaticConstructorStandardText[1], true);
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
        /// {2}<see cref=""Foo{1}""/>{3}
        /// </summary>
        {4} 
        Foo()
        {{

        }}
    }}
}}";
            fixedCode = string.Format(fixedCode, generic ? "<T1, T2>" : string.Empty, generic ? "{T1, T2}" : string.Empty, part1, part2, modifiers);
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
            await TestConstructorMissingDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], false);
        }

        [TestMethod]
        public async Task TestPrivateConstructorMissingDocumentationGeneric()
        {
            await TestConstructorMissingDocumentation("private", PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], true);
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
            return new SA1642CodeFixProvider();
        }
    }
}
