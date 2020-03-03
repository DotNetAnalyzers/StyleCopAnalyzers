// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/>.
    /// </summary>
    public class SA1206CodeFixProviderUnitTests
    {
        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a class declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInClassDeclarationAsync()
        {
            var testCode = @"abstract public class FooBar {}";
            var fixedTestCode = @"public abstract class FooBar {}";

            var expected = Diagnostic().WithLocation(1, 10).WithArguments("public", "abstract");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a struct declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInStructDeclarationAsync()
        {
            var testCode = @"unsafe public struct FooBar {}";
            var fixedTestCode = @"public unsafe struct FooBar {}";

            var expected = Diagnostic().WithLocation(1, 8).WithArguments("public", "unsafe");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a struct, interface and enum declaration.
        /// </summary>
        /// <param name="type">Data for this test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("struct")]
        [InlineData("interface")]
        [InlineData("enum")]
        public async Task VerifyKeywordReorderingInTypeDeclarationAsync(string type)
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

            var fixedTestCode = @"public class TestClass
{
    protected " + type + @" Nested
    {
    }
}

public class ExtendedTestClass : TestClass
{
    protected new " + type + @" Nested
    {
    }
}";

            var expected = Diagnostic().WithLocation(10, 9).WithArguments("protected", "new");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a property declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInPropertyDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { static public int P { get; } } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public static int P { get; } } }";

            var expected = Diagnostic().WithLocation(1, 41).WithArguments("public", "static");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a method declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInMethodDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { new static public void P() { } } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public static new void P() { } } }";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 38).WithArguments("static", "new"),
                Diagnostic().WithLocation(1, 45).WithArguments("public", "static"),
                Diagnostic().WithLocation(1, 45).WithArguments("public", "new"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a field declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInFieldDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { static public int p; } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public static int p; } }";

            var expected = Diagnostic().WithLocation(1, 41).WithArguments("public", "static");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a field declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInOperatorDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { extern public static C1 operator ++(C1 n); } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public static extern C1 operator ++(C1 n); } }";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 41).WithArguments("public", "extern"),
                Diagnostic().WithLocation(1, 48).WithArguments("static", "extern"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on an indexer declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInIndexerDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { extern public int this[int index] { get; set; } } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public extern int this[int index] { get; set; } } }";

            var expected = Diagnostic().WithLocation(1, 41).WithArguments("public", "extern");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on an event field declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInEventFieldDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { virtual public event System.EventHandler Changed; } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public virtual event System.EventHandler Changed; } }";

            var expected = Diagnostic().WithLocation(1, 42).WithArguments("public", "virtual");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a conversion declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInConversionDeclarationAsync()
        {
            var testCode = @"namespace N1 { public class C1 { public extern static explicit operator C1(int n); } }";
            var fixedTestCode = @"namespace N1 { public class C1 { public static extern explicit operator C1(int n); } }";

            var expected = Diagnostic().WithLocation(1, 48).WithArguments("static", "extern");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a delegate declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingInDelegateDeclarationAsync()
        {
            var testCode = @"public class TestClass
{
    protected delegate void SomeDelegate(int a);
}

public class ExtendedTestClass : TestClass
{
    new protected delegate void SomeDelegate(int a);
}";

            var fixedTestCode = @"public class TestClass
{
    protected delegate void SomeDelegate(int a);
}

public class ExtendedTestClass : TestClass
{
    protected new delegate void SomeDelegate(int a);
}";

            var expected = Diagnostic().WithLocation(8, 9).WithArguments("protected", "new");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder all keywords.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingWithFixAllAsync()
        {
            var testCode = @"
/// <summary>
/// Test class
/// </summary>
{|CS0267:partial|} public class TestClass
{
    /// <summary>
    /// Test delegate
    /// </summary>
    protected delegate void SomeDelegate(int a);

    /// <summary>
    /// Test method
    /// </summary>
    static public void Foo() { }
}

/// <summary>
/// Test class
/// </summary>
public class ExtendedTestClass : TestClass
{
    /// <summary>
    /// Test delegate
    /// </summary>
    new protected delegate void SomeDelegate(int a);
}";

            var fixedTestCode = @"
/// <summary>
/// Test class
/// </summary>
public partial class TestClass
{
    /// <summary>
    /// Test delegate
    /// </summary>
    protected delegate void SomeDelegate(int a);

    /// <summary>
    /// Test method
    /// </summary>
    public static void Foo() { }
}

/// <summary>
/// Test class
/// </summary>
public class ExtendedTestClass : TestClass
{
    /// <summary>
    /// Test delegate
    /// </summary>
    protected new delegate void SomeDelegate(int a);
}";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(5, 9).WithArguments("public", "partial"),
                    Diagnostic().WithLocation(15, 12).WithArguments("public", "static"),
                    Diagnostic().WithLocation(26, 9).WithArguments("protected", "new"),
                },
                FixedCode = fixedTestCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a class declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyKeywordReorderingPreservesPositionalTriviaAsync()
        {
            var testCode = @"namespace n
{
    public class TestClass
    {
        static   extern     public void FirstMethod();
    }
}
";

            var fixedTestCode = @"namespace n
{
    public class TestClass
    {
        public   static     extern void FirstMethod();
    }
}
";

            var expected = Diagnostic().WithLocation(5, 29).WithArguments("public", "extern");
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
