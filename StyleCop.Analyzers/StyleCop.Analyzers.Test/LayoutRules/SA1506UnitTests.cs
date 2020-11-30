// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1506CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine"/> class.
    /// </summary>
    public class SA1506UnitTests
    {
        public static IEnumerable<object[]> TypeTestData
        {
            get
            {
                yield return new object[] { "class" };
                yield return new object[] { "struct" };
                yield return new object[] { "interface" };
                yield return new object[] { "enum" };
            }
        }

        /// <summary>
        /// Verifies that type declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestValidTypeDeclarationAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType1
    {{
    }}

    /// <summary>
    /// This is a test type.
    /// </summary>
    public {typeKeyword} TestType2
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that type declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestInvalidTypeDeclarationAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    /// <summary>
    /// This is a test type.
    /// </summary>

    public {typeKeyword} TestType
    {{
    }}
}}
";

            var fixedTestCode = $@"namespace TestNamespace
{{
    /// <summary>
    /// This is a test type.
    /// </summary>
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var expectedResult = Diagnostic().WithLocation(6, 1);
            await VerifyCSharpFixAsync(testCode, expectedResult, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that method-like declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidMethodLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>
        public TestClass()
        {
        }

        private TestClass(int value)
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>
        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>
        public void TestMethod1()
        {
        }

        public void TestMethod2()
        {
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that method-like declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidMethodLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>

        public TestClass()
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>

        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>

        public void TestMethod()
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a constructor
        /// </summary>
        public TestClass()
        {
        }

        /// <summary>
        /// This is a destructor
        /// </summary>
        ~TestClass()
        {
        }

        /// <summary>
        /// This is a method.
        /// </summary>
        public void TestMethod()
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(24, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that property-like declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidPropertyLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>
        public int TestProperty1
        {
            get; set;
        }

        public int TestProperty2
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>
        public int this[int index]
        {
            get { return index; }
        }

        public int this[byte index]
        {
            get { return index; }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that property-like declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidPropertyLikeDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>

        public int TestProperty
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>

        public int this[int index]
        {
            get { return index; }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a property.
        /// </summary>
        public int TestProperty
        {
            get; set;
        }

        /// <summary>
        /// This is an indexer.
        /// </summary>
        public int this[int index]
        {
            get { return index; }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(17, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that field declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFieldDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField1;

        public int testField2;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that field declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidFieldDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>

        public int testField;
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField;
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that delegate declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidDelegateDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate1();

    public delegate void TestDelegate2();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that delegate declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidDelegateDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>

    public delegate void TestDelegate();
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate();
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(6, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEventDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;

        public event EventHandler TestEvent2;

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent3
        {
            add { }
            remove { }
        }

        public event EventHandler TestEvent4
        {
            add { }
            remove { }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidEventDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>

        public event EventHandler TestEvent1;

        /// <summary>
        /// This is an event.
        /// </summary>

        public event EventHandler TestEvent2
        {
            add { }
            remove { }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent2
        {
            add { }
            remove { }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(10, 1),
                Diagnostic().WithLocation(16, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that documentation followed by comments are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumentationFollowedByCommentAsync()
        {
            var testCode = @"
/// <summary>some documentation</summary>

// some comment
public class TestClass
{
    // yet another comment

    /// <summary>more documentation.</summary>

    // another comment
    public void TestMethod1() { }

    /// <summary>more documentation.</summary>

    /* another comment */
    public void TestMethod2() { }

    /// <summary>more documentation.</summary>
    // another comment (a specific regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1456)

    public void TestMethod3() { }
}
";

            var fixedCode = @"
/// <summary>some documentation</summary>
// some comment
public class TestClass
{
    // yet another comment

    /// <summary>more documentation.</summary>
    // another comment
    public void TestMethod1() { }

    /// <summary>more documentation.</summary>
    /* another comment */
    public void TestMethod2() { }

    /// <summary>more documentation.</summary>
    // another comment (a specific regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1456)
    public void TestMethod3() { }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(10, 1),
                Diagnostic().WithLocation(15, 1),
                Diagnostic().WithLocation(20, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that enum member declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEnumMemberDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public enum TestEnum
    {
        /// <summary>
        /// This is an enum member.
        /// </summary>
        Foo = 0,
        Bar = 1
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that enum member declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidEnumMemberDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public enum TestEnum
    {
        /// <summary>
        /// This is an enum member.
        /// </summary>

        Foo = 0
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public enum TestEnum
    {
        /// <summary>
        /// This is an enum member.
        /// </summary>
        Foo = 0
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operator declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOperatorDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is an operator declaration.
        /// </summary>
        public static Foo operator +(Foo x, Foo y)
        {
            return new Foo();
        }

        private int testField1;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operator declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidOperatorDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is an operator declaration.
        /// </summary>

        public static Foo operator +(Foo x, Foo y)
        {
            return new Foo();
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is an operator declaration.
        /// </summary>
        public static Foo operator +(Foo x, Foo y)
        {
            return new Foo();
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that conversion operator declarations with valid (or no) documentation will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidConversionOperatorDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is a conversion operator declaration.
        /// </summary>
        public static explicit operator Foo(string s)
        {
            return new Foo();
        }

        private int testField1;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that conversion operator declarations with invalid documentation will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidConversionOperatorDeclarationsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is a conversion operator declaration.
        /// </summary>

        public static explicit operator Foo(string s)
        {
            return new Foo();
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public struct Foo
    {
        /// <summary>
        /// This is a conversion operator declaration.
        /// </summary>
        public static explicit operator Foo(string s)
        {
            return new Foo();
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
