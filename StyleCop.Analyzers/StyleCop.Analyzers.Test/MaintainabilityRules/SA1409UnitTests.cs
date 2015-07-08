using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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

        // Code Fixes

        private async Task TestKeywordCodeFixNoFix(string keyword)
        {
            string code = @"class Foo
{
    public void Bar()
    {
        %keyword%
        {
            Bar();
        }
    }
}";

            await this.VerifyCSharpFixAsync(code.Replace("%keyword%", keyword), code.Replace("%keyword%", keyword));
        }

        private async Task TestKeywordCodeFix(string keyword)
        {
            string oldSource = @"class Foo
{
    public void Bar()
    {
        %keyword%
        {

        }
    }
}";
            string newSource = @"class Foo
{
    public void Bar()
    {
        
    }
}";

            await this.VerifyCSharpFixAsync(oldSource.Replace("%keyword%", keyword), newSource.Replace("%keyword%", keyword));
        }

        [Fact]
        public async Task TestUnsafeCodeFixNoFix()
        {
            await this.TestKeywordCodeFixNoFix("unsafe");
        }

        [Fact]
        public async Task TestUnsafeCodeFix()
        {
            await this.TestKeywordCodeFix("unsafe");
        }

        [Fact]
        public async Task TestCheckedCodeFixNoFix()
        {
            await this.TestKeywordCodeFixNoFix("checked");
        }

        [Fact]
        public async Task TestCheckedCodeFix()
        {
            await this.TestKeywordCodeFix("checked");
        }

        [Fact]
        public async Task TestUncheckedCodeFixNoFix()
        {
            await this.TestKeywordCodeFixNoFix("unchecked");
        }

        [Fact]
        public async Task TestUncheckedCodeFix()
        {
            await this.TestKeywordCodeFix("unchecked");
        }

        [Fact]
        public async Task TestStaticConstructorCodeFixNoFix()
        {
            string code = @"class Foo
{
    static Foo()
    {
        System.Console.WriteLine();
    }
}";

            await this.VerifyCSharpFixAsync(code, code);
        }

        [Fact]
        public async Task TestStaticConstructorCodeFix()
        {
            string oldSource = @"class Foo
{
    static Foo()
    {

    }
}";
            string newSource = @"class Foo
{
    
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource);
        }

        [Fact]
        public async Task TestIfElseCodeFixNoFix()
        {
            string code = @"class Foo
{
    public void Bar()
    {
        if(true)
        {
            Bar();
        }
        else
        {
            Bar();
        }
    }
}";

            await this.VerifyCSharpFixAsync(code, code);
        }

        [Fact]
        public async Task TestIfElseCodeFix()
        {
            string oldSource = @"class Foo
{
    public void Bar()
    {
        if(true)
        {
            Bar();
        }
        else
        {

        }
    }
}";
            string newSource = @"class Foo
{
    public void Bar()
    {
        if(true)
        {
            Bar();
        }
        
    }
}";

            await this.VerifyCSharpFixAsync(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1409RemoveUnnecessaryCode();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1409CodeFixProvider();
        }
    }
}