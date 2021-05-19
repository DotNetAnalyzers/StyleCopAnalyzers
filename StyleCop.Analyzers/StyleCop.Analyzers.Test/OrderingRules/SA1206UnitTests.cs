// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1206DeclarationKeywordsMustFollowOrder"/>.
    /// </summary>
    public class SA1206UnitTests
    {
        [Fact]
        public async Task TestKeywordsInClassDeclarationAsync()
        {
            var testCode = @"abstract public class T
{
}
";
            var expected = Diagnostic().WithLocation(1, 10).WithArguments("public", "abstract");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 11).WithArguments("static", "async"),
                Diagnostic().WithLocation(3, 18).WithArguments("public", "async"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(4, 13).WithArguments("public", "virtual"),
                Diagnostic().WithLocation(5, 12).WithArguments("public", "static"),
                Diagnostic().WithLocation(7, 17).WithArguments("internal", "new"),
                Diagnostic().WithLocation(7, 32).WithArguments("protected", "async"),
                Diagnostic().WithLocation(8, 16).WithArguments("static", "new"),
                Diagnostic().WithLocation(9, 9).WithArguments("static", "new"),
                Diagnostic().WithLocation(9, 16).WithArguments("public", "new"),
                Diagnostic().WithLocation(10, 13).WithArguments("public", "virtual"),
                Diagnostic().WithLocation(11, 12).WithArguments("public", "extern"),
                Diagnostic().WithLocation(12, 12).WithArguments("public", "extern"),
                Diagnostic().WithLocation(13, 12).WithArguments("static", "extern"),
                Diagnostic().WithLocation(14, 14).WithArguments("internal", "volatile"),
                Diagnostic().WithLocation(14, 23).WithArguments("static", "volatile"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(10, 9).WithArguments("protected", "new");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(8, 9).WithArguments("protected", "new");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            var expected = Diagnostic().WithLocation(8, 9).WithArguments("public", "new");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 12).WithArguments("public", "extern"),
                Diagnostic().WithLocation(3, 19).WithArguments("static", "extern"),
                Diagnostic().WithLocation(4, 12).WithArguments("static", "extern"),
                Diagnostic().WithLocation(4, 19).WithArguments("public", "extern"),
                Diagnostic().WithLocation(5, 19).WithArguments("static", "extern"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 8).WithArguments("public", "unsafe"),
                Diagnostic().WithLocation(3, 12).WithArguments("public", "unsafe"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithIncompleteMembersAsync()
        {
            var testCode = @"public class Test
{
    new public string
}";

            DiagnosticResult[] expected =
            {
                // /0/Test0.cs(4,1): error CS1519: Invalid token '}' in class, record, struct, or interface member declaration
                DiagnosticResult.CompilerError("CS1519").WithSpan(4, 1, 4, 2).WithArguments("}"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithPartialKeywordAsync()
        {
            var testCode = @"abstract partial class AbstractClass
{
    static partial void PartialTest();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
