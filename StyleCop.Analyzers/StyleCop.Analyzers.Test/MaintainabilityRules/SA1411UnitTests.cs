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
    public class SA1411UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMissingParenthesis()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete]
    public void Bar()
    {
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNonEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(""bar"")]
    public void Bar()
    {
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestNonEmptyParameterListNamedArgument()
        {
            var testCode = @"public class Foo
{
    [System.Runtime.CompilerServices.MethodImpl(MethodCodeType = MethodCodeType.IL)]
    public void Bar()
    {
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete()]
    public void Bar()
    {
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Attribute constructor must not use unnecessary parenthesis",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyParameterListMultipleAttributes()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(), System.Runtime.CompilerServices.MethodImpl()]
    public void Bar()
    {
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Attribute constructor must not use unnecessary parenthesis",
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
                    Message = "Attribute constructor must not use unnecessary parenthesis",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 67)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCodeFix()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete()]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete]
    public void Bar()
    {
    }
}";

            await VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCodeFixMultipleAttributes()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete(), System.Runtime.CompilerServices.MethodImpl()]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete, System.Runtime.CompilerServices.MethodImpl]
    public void Bar()
    {
    }
}";

            await VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}