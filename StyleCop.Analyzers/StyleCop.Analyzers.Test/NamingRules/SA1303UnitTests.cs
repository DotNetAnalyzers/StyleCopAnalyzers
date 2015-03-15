using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.NamingRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.NamingRules
{
    public class SA1303UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1303ConstFieldNamesMustBeginWithUpperCaseLetter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleOne()
        {
            var testCode = @"public class NativeMethods    
{        
    public const string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsExampleTwo()
        {
            var testCode = @"public class MyNativeMethods    
{        
    public const string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerClassInNativeMethods()
        {
            var testCode = @"public class NativeMethods    
{        
    public class Foo
    {
        public const string bar = ""baz"";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerInnerClassInNativeMethods()
        {
            var testCode = @"public class NativeMethods    
{        
    public class Foo
    {
        public class FooInner
        {
            public const string bar = ""baz"";
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseNativeMethodsIncorrectName()
        {
            var testCode = @"public class MyNativeMethodsClass    
{        
    public const string bar = ""baz"";
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithLowerCaseInnerInnerClassInNativeMethodsIncorrectName()
        {
            var testCode = @"
namespace Test
{
   public class NativeMethodsClass    
   {        
       public class Foo
       {
           public class FooInner
           {
               public const string bar = ""baz"";
           }
       }
   }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 36);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    public const string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstFieldStartingWithUnderscore()
        {
            var testCode = @"public class Foo
{
    public const string _Bar = ""baz"";
}";

            // Fields starting with an underscore are reported as SA1309
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestFieldWhichIsNotConstStartingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1303ConstFieldNamesMustBeginWithUpperCaseLetter();
        }
    }
}