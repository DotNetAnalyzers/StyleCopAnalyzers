namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1300UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestUpperCaseNamespaceAsync()
        {
            var testCode = @"namespace Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseNamespaceAsync()
        {
            var testCode = @"namespace test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 11);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseClassAsync()
        {
            var testCode = @"public class Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseClassAsync()
        {
            var testCode = @"public class test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 14);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInterfaceAsync()
        {
            var testCode = @"public interface Test
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInterfaceAsync()
        {
            var testCode = @"public interface test
{

}";

            // Reported as SA1302
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseStructAsync()
        {
            var testCode = @"public struct Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseStructAsync()
        {
            var testCode = @"public struct test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEnumAsync()
        {
            var testCode = @"public enum Test 
{ 

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEnumAsync()
        {
            var testCode = @"public enum test 
{ 

}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(1, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseDelegateAsync()
        {
            var testCode = @"public class TestClass
{ 
public delegate void Test();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseDelegateAsync()
        {
            var testCode = @"public class TestClass
{ 
public delegate void test();
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 22);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEventAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test TestEvent
    {
        add
        {
            _testEvent += value;
        }
        remove
        {
            _testEvent -= value;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    Test _testEvent;
    public event Test testEvent
    {
        add
        {
            _testEvent += value;
        }
        remove
        {
            _testEvent -= value;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("testEvent").WithLocation(5, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseEventFieldAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test TestEvent;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseEventFieldAsync()
        {
            var testCode = @"public class TestClass
{
    public delegate void Test();
    public event Test testEvent;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("testEvent").WithLocation(4, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseMethodAsync()
        {
            var testCode = @"public class TestClass
{
public void Test()
{
}
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseMethodAsync()
        {
            var testCode = @"public class TestClass
{
public void test()
{
}
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePropertyAsync()
        {
            var testCode = @"public class TestClass
{
public string Test { get; set; }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePropertyAsync()
        {
            var testCode = @"public class TestClass
{
public string test { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("test").WithLocation(3, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string Test = ""value"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUpperCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected string Test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
readonly string test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCasePublicFieldAsync()
        {
            var testCode = @"public class TestClass
{
public string test;
}";

            // Handled by SA1307
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseInternalFieldAsync()
        {
            var testCode = @"public class TestClass
{
internal string test;
}";

            // Handled by SA1307
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseConstFieldAsync()
        {
            var testCode = @"public class TestClass
{
const string test = ""value"";
}";

            // Reported as SA1303
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNativeMethodsExceptionAsync()
        {
            var testCode = @"public class TestNativeMethods
{
public string test;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseProtectedReadOnlyFieldAsync()
        {
            var testCode = @"public class TestClass
{
protected readonly string test;
}";

            // Handled by SA1304
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1300ElementMustBeginWithUpperCaseLetter();
        }
    }
}
