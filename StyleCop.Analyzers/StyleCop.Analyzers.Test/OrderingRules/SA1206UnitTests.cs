namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1206DeclarationKeywordsMustFollowOrder"/>.
    /// </summary>
    public class SA1206UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestKeywordsInClassDeclarationAsync()
        {
            var testCode = @"abstract public class T
{
}
";
            var expected = this.CSharpDiagnostic().WithLocation(1, 10).WithArguments("access modifier", "other");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestKeywordsWithExpressionBodiedMemberAndAsyncKeywordAsync()
        {
            var testCode = @"public class Test
{
    async static public void GetResult() => new object();
}";
            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 11).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(3, 18).WithArguments("access modifier", "static"),
                this.CSharpDiagnostic().WithLocation(3, 18).WithArguments("access modifier", "other")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestKeywordsInClassMembersAsync()
        {
            var testCode = @"public class TestClass
{
    public static extern void FirstMethod();
    virtual public void SecondMethod() {}
    static public void ThirdMethod() {}
    protected virtual new void FourthMethod() {}
    virtual new internal async protected void FifthMethod() { }
    public new static void SixthMethod() {}
    new static public string FirstProperty { get; set; }
    virtual public event System.EventHandler Changed;
    extern public int this[int index] { get; set; }
    extern public void ExternMethodOne();
    extern static void ExternMethodTwo();
    volatile internal static bool Test;
    public readonly string Field = string.Empty;
}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(4, 13).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(5, 12).WithArguments("access modifier", "static"),
                this.CSharpDiagnostic().WithLocation(7, 17).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(7, 32).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(8, 16).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(9, 9).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(9, 16).WithArguments("access modifier", "static"),
                this.CSharpDiagnostic().WithLocation(9, 16).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(10, 13).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(11, 12).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(12, 12).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(13, 12).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(14, 14).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(14, 23).WithArguments("static", "other")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("struct")]
        [InlineData("interface")]
        [InlineData("enum")]
        public async Task TestNewKeywordWithinNestedTypeDeclarationsAsync(string type)
        {
            var testCode = @"public class TestClass
{
    protected " + type + @" Nested
    {
    }
}

public class ExtendedTestClass : TestClass
{
    new protected " + type + @" Nested
    {
    }
}";
            var expected = this.CSharpDiagnostic().WithLocation(10, 9).WithArguments("access modifier", "other");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestHidedNestedDelegateDeclarationAsync()
        {
            var testCode = @"public class TestClass
{
    protected delegate void SomeDelegate(int a);
}

public class ExtendedTestClass : TestClass
{
    new protected delegate void SomeDelegate(int a);
}";
            var expected = this.CSharpDiagnostic().WithLocation(8, 9).WithArguments("access modifier", "other");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithNewKeywordWithConstantAsync()
        {
            var testCode = @"public class TestClass
{
    public const string Empty = """";
}

public class ExtendedClass : TestClass
{
    new public const string Empty = """";
}";
            var expected = this.CSharpDiagnostic().WithLocation(8, 9).WithArguments("access modifier", "other");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestKeywordsWithOperatorDeclarationsAsync()
        {
            var testCode = @"public class Number
{
    extern public static Number operator ++(Number n);
    extern static public Number operator %(Number left, Number right);
    public extern static explicit operator Number(int n);
}";
            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 12).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(3, 19).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(4, 12).WithArguments("static", "other"),
                this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("access modifier", "static"),
                this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(5, 19).WithArguments("static", "other")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithUnsafeKeywordAndFixedAsync()
        {
            var testCode = @"unsafe public struct SomeClass
{   
    unsafe public fixed int x[5];
}";
            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(1, 8).WithArguments("access modifier", "other"),
                this.CSharpDiagnostic().WithLocation(3, 12).WithArguments("access modifier", "other")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithIncompleteMembersAsync()
        {
            var testCode = @"public class Test
{
    new public string
}";

            var expected = new DiagnosticResult
            {
                Id = "CS1519",
                Message = "Invalid token '}' in class, struct, or interface member declaration",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 1) }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithPartialKeywordAsync()
        {
            var testCode = @"abstract partial class AbstractClass
{
    static partial void PartialTest();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1206DeclarationKeywordsMustFollowOrder();
        }
    }
}
