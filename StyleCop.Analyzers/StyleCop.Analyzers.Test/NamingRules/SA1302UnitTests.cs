using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.NamingRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.NamingRules
{
    public class SA1302UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1302InterfaceNamesMustBeginWithI.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestInterfaceDeclarationDoesNotStartWithI()
        {
            var testCode = @"
public interface Foo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
public interface IFoo
{
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestInterfaceDeclarationDoesNotStartWithIPlusInterfaceUsed()
        {
            var testCode = @"
public interface Foo
{
}
public class Bar : Foo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
public interface IFoo
{
}
public class Bar : IFoo
{
}";


            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestInterfaceDeclarationStartsWithLowerI()
        {
            var testCode = @"
public interface iFoo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
public interface IiFoo
{
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestInnerInterfaceDeclarationDoesNotStartWithI()
        {
            var testCode = @"
public class Bar
{
    public interface Foo
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestInterfaceDeclarationDoesStartWithI()
        {
            var testCode = @"public interface IFoo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestInnerInterfaceDeclarationDoesStartWithI()
        {
            var testCode = @"
public class Bar
{
    public interface IFoo
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestComInterfaceInNativeMethodsClass()
        {
            var testCode = @"
public class NativeMethods
{
    [ComImport]
    public interface FileOpenDialog
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestComInterfaceInNativeMethodsClassWithIncorrectName()
        {
            var testCode = @"
public class NativeMethodsClass
{
    [ComImport]
    public interface FileOpenDialog
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"
public class NativeMethodsClass
{
    [ComImport]
    public interface IFileOpenDialog
    {
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestComInterfaceInInnerClassInNativeMethodsClass()
        {
            var testCode = @"
public class MyNativeMethods
{
    public class FileOperations
    {
        [ComImport]
        public interface FileOpenDialog111
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1302InterfaceNamesMustBeginWithI();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1302CodeFixProvider();
        }
    }
}