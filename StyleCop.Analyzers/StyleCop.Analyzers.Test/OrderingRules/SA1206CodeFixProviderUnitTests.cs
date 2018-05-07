// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/>.
    /// </summary>
    public class SA1206CodeFixProviderUnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly reorder keywords on a struct, interface and enum declaration.
        /// </summary>
        /// <param name="type">Data for this test</param>
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
partial public class TestClass
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAllFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1206DeclarationKeywordsMustFollowOrder();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1206CodeFixProvider();
        }
    }
}
