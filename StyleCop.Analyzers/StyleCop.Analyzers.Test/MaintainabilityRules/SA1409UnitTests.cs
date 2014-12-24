using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1409UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1409RemoveUnnecessaryCode.DiagnosticId;
       
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTryWithFinallyAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        try
        {
            Bar();
        }
        finally
        {
            Bar();
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTryWithEmptyFinallyAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        try
        {
            Bar();
        }
        finally
        {
        }
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTryWithEmptyTryFinallyAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        try
        {
        }
        finally
        {
        }
    }
}";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(8, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfElseAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if (true)
        {
            Bar();
        }
        else
        {
            Bar();
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfWithEmptyElseBlockAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if (true)
        {
            Bar();
        }
        else
        {
        }
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIfWithNoElseBlockAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if (true)
        {
            Bar();
        }
        else;
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(9, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        private async Task TestKeywordAsync(string keyword)
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        {0}
        {
            Bar();
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("{0}", keyword), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestEmptyKeywordAsync(string keyword)
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        {0}
        {
        }
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(5, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("{0}", keyword), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestUnsafeAsync()
        {
            await this.TestKeywordAsync("unsafe");
        }

        [Fact]
        public async Task TestEmptyUnsafeAsync()
        {
            await this.TestEmptyKeywordAsync("unsafe");
        }

        [Fact]
        public async Task TestCheckedAsync()
        {
            await this.TestKeywordAsync("checked");
        }

        [Fact]
        public async Task TestEmptyCheckedAsync()
        {
            await this.TestEmptyKeywordAsync("checked");
        }

        [Fact]
        public async Task TestUncheckedAsync()
        {
            await this.TestKeywordAsync("unchecked");
        }

        [Fact]
        public async Task TestEmptyUncheckedAsync()
        {
            await this.TestEmptyKeywordAsync("unchecked");
        }

        [Fact]
        public async Task TestEmptyConstructorAsync()
        {
            var testCode = @"public class Foo
{
    public Foo()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestStaticConstructorAsync()
        {
            var testCode = @"public class Foo
{
    static Foo()
    {
        new Foo();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEmptyStaticConstructorAsync()
        {
            var testCode = @"public class Foo
{
    static Foo()
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(3, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1409RemoveUnnecessaryCode();
        }
    }
}