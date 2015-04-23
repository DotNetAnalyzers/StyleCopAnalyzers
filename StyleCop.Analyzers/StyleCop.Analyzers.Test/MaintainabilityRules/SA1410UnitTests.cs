using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1410UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingParenthesis()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNonEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> getNumber = delegate (int i) { return i; };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 52);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFix()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveExteriorTrivia()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*/(/*Bar*/)/*Foo*/ { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate/*Foo*//*Bar*//*Foo*/ { return 3; };
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1410RemoveDelegateParenthesisWhenPossible();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}