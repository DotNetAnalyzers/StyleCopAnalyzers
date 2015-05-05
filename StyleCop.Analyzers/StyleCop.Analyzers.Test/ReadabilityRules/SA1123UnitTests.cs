namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1123DoNotPlaceRegionsWithinElements"/> and
    /// <see cref="RemoveRegionCodeFixProvider"/>.
    /// </summary>
    public class SA1123UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
#endregion
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public class Foo
{
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
#endregion
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyInMethod2Async()
        {
            var testCode = @"public class Foo
{
    public void Bar()
#region Foo
    {
        string test = """";
    }
#endregion
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionPartialyMultipleMethodsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
#region Foo
        string test = """";
    }
    public void FooBar()
    {
        string test = """";
#endregion
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEndRegionInMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
#endregion
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethodAsync()
        {
            var testCode = @"public class Foo
{
#region Foo
#endregion
    public void Bar()
    {
        string test = """";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRegionOutsideMethod2Async()
        {
            var testCode = @"public class Foo
{
#region Foo
    public void Bar()
    {
        string test = """";
    }
#endregion
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1123DoNotPlaceRegionsWithinElements();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RemoveRegionCodeFixProvider();
        }
    }
}
