namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1214UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonly()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 33)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonlyOneLine()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;private static readonly int j = 0;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 58)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedBeforeStaticNonReadonly()
        {
            var testCode = @"
public class Foo
{
    private static readonly int i = 0;
    private static int j = 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTwoFieldsInClassNonStaticReadonlyFieldPlacedAfterNonStaticNonReadonly()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;
    private readonly int j = 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTwoFieldsInStructStaticReadonlyFieldPlacedAfterStaticNonReadonly()
        {
            var testCode = @"
public struct Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 33)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task ComplexExample()
        {
            var testCode = @"
public class Foo
{
    public string s = ""qwe"";
    private static readonly int i = 0;

    public void Ff() {}

    public static string s2 = ""qwe"";

    public static readonly int  u = 5;

    public class FooInner 
    {
        private int aa = 0;
        public static readonly int t = 2;
        private static int z = 999;
        private static readonly int e = 1;
    }

    public static readonly int j = 0;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 11, 33)
                        }
                },
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 18, 37)
                        }
                },
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = "Static readonly elements must appear before static non-readonly elements.",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 21, 32)
                        }
                },
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements();
        }
    }
}