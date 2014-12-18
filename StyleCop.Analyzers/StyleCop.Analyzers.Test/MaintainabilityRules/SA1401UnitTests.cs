using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    [TestClass]
    public class SA1401UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1401FieldsMustBePrivate.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestClassWithPublicField()
        {
            var testCode = @"public class Foo
{
    public string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 19)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private string bar;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithPublicStaticField()
        {
            var testCode = @"public class Foo
{
    public static string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 26)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private static string bar;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithPublicConstField()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""aaa"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 25)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private const string bar = ""aaa"";
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithPublicStaticVolatileField()
        {
            var testCode = @"public class Foo
{
    public static volatile string bar = ""aaa"";
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 35)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private static volatile string bar = ""aaa"";
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithInternalField()
        {
            var testCode = @"public class Foo
{
    internal string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private string bar;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithInternalReadonlyField()
        {
            var testCode = @"public class Foo
{
    internal readonly string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 30)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private readonly string bar;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithInternalStaticReadonlyField()
        {
            var testCode = @"public class Foo
{
    internal static readonly string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 37)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private static readonly string bar;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWith2FieldOnePublicOneInternal()
        {
            var testCode = @"public class Foo
{
    internal string bar;
    public int baz;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 21)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Field must be private",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 16)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var expectedCode = @"public class Foo
{
    private string bar;
    private int baz;
}";

            await VerifyCSharpFixAsync(testCode, expectedCode);
        }

        [TestMethod]
        public async Task TestClassWithFieldNoAccessModifier()
        {
            var testCode = @"public class Foo
{
    string bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestStructWithPublicField()
        {
            var testCode = @"public struct Foo
{
    public string bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1401FieldsMustBePrivate();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1401FieldsMustBePrivateCodeFix();
        }
    }
}