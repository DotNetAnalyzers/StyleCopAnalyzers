using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1411UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1411AttributeConstructorMustNotUseUnnecessaryParenthesis.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMissingParenthesis()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNonEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(""bar"")]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNonEmptyParameterListNamedArgument()
        {
            var testCode = @"public class Foo
{
    [System.Runtime.CompilerServices.MethodImpl(MethodCodeType = MethodCodeType.IL)]
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete()]
    public void Bar()
    {
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyParameterListMultipleAttributes()
        {
            var testCode = @"public class Foo
{
    [System.Obsolete(), System.Runtime.CompilerServices.MethodImpl()]
    public void Bar()
    {
    }
}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(3, 21),
                    this.CSharpDiagnostic().WithLocation(3, 67)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
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

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveExteriorTrivia()
        {
            var oldSource = @"public class Foo
{
    [System.Obsolete/*Foo*/(/*Bar*/)/*Foo*/]
    public void Bar()
    {
    }
}";

            var newSource = @"public class Foo
{
    [System.Obsolete/*Foo*//*Bar*//*Foo*/]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
        }

        [Fact]
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

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
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