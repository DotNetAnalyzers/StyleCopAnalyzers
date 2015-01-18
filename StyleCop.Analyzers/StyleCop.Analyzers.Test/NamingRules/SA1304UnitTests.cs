using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.NamingRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.NamingRules
{
    [TestClass]
    public class SA1304UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPublicReadonlyFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public readonly string bar = ""baz"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 28)
                        }
                },
                // Workaround because the diagnostic is called twice for the SyntaxNode
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 28)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPublicReadonlyFieldStartingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    public readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestProtectedReadonlyFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    protected readonly string bar = ""baz"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 31)
                        }
                },
                // Workaround because the diagnostic is called twice for the SyntaxNode
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 31)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestProtectedReadonlyFieldStartingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    protected readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestInternalReadonlyFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar = ""baz"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 30)
                        }
                },
                // Workaround because the diagnostic is called twice for the SyntaxNode
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Non-private readonly fields must begin with upper-case letter",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 30)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestInternalReadonlyFieldStartingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    internal readonly string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestlWithNoAccessibilityKeywordReadonlyFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestlPublicFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPrivateReadonlyFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    private readonly string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter();
        }
    }
}