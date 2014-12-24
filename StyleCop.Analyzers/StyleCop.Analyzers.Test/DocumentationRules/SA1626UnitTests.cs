namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using StyleCop.Analyzers.DocumentationRules;

    [TestClass]
    public class SA1626UnitTests : CodeFixVerifier
    { 
            private const string DiagnosticId = SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes.DiagnosticId;
            protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

            [TestMethod]
            public async Task TestEmptySource()
            {
                var testCode = @"";
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
            }

            [TestMethod]
            public async Task TestClassWithXmlComment()
            {
                var testCode = @"/// <summary>
/// Xml Documentation
/// </summary>
public class Foo
{
    public void Bar()
    {
    }
}
";
                await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        // This is a comment
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithOneLineThreeSlashComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        /// This is a comment
    }
}
";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Single-line comments must not use documentation style slashes",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 9)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodWithMultiLineThreeSlashComment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        /// This is
        /// a comment
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
            public async Task TestMethodWithCodeComments()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        //// System.Console.WriteLine(""Bar"")
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

            protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            {
                return new SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes();
            }
        }
    }