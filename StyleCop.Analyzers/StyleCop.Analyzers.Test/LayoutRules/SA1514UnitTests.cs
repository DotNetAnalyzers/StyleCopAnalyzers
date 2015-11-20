// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1514ElementDocumentationHeaderMustBePrecededByBlankLine"/> class.
    /// </summary>
    public class SA1514UnitTests : CodeFixVerifier
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
    /// <summary>
    /// This is a test type.
    /// </summary>
    public {typeKeyword} TestType1
    {{
    }}

    public {typeKeyword} TestType2
    {{
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var fixedTestCode = $@"namespace TestNamespace
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

            var expectedResult = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedResult, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        private int testField;
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
        private int testField;

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
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(12, 9),
                this.CSharpDiagnostic().WithLocation(18, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        private int testField;
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
        private int testField;

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
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(13, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public int testField1;
        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField2;
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        public int testField1;

        /// <summary>
        /// This is a field.
        /// </summary>
        public int testField2;
    }
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
    public class TestClass  
    {
        /// <summary>
        /// This is an operator.
        /// </summary>
        public static TestClass operator +(TestClass t1, TestClass t2)
        {
            return new TestClass();
        }

        public int testField1;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public class TestClass  
    {
        public int testField1;
        /// <summary>
        /// This is an operator.
        /// </summary>
        public static TestClass operator +(TestClass t1, TestClass t2)
        {
            return new TestClass();
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        public int testField1;

        /// <summary>
        /// This is an operator.
        /// </summary>
        public static TestClass operator +(TestClass t1, TestClass t2)
        {
            return new TestClass();
        }
    }
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
    public class TestClass  
    {
        /// <summary>
        /// This is a conversion operator.
        /// </summary>
        public static explicit operator TestClass(string foo)
        {
            return new TestClass();
        }

        public int testField1;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public class TestClass  
    {
        public int testField1;
        /// <summary>
        /// This is a conversion operator.
        /// </summary>
        public static explicit operator TestClass(string foo)
        {
            return new TestClass();
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass  
    {
        public int testField1;

        /// <summary>
        /// This is a conversion operator.
        /// </summary>
        public static explicit operator TestClass(string foo)
        {
            return new TestClass();
        }
    }
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public delegate void TestDelegate1();
    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate2();
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public delegate void TestDelegate1();

    /// <summary>
    /// This is a delegate.
    /// </summary>
    public delegate void TestDelegate2();
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(4, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        private int testField;
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;
        /// <summary>
        /// This is an event.
        /// </summary>

        /// <remarks>
        /// This is an interesting test case
        /// </remarks>
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
        private int testField;

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;

        /// <summary>
        /// This is an event.
        /// </summary>

        /// <remarks>
        /// This is an interesting test case
        /// </remarks>
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
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(12, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with documentation without a leading blank line within directive trivia will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEventDeclarationsInsideDirectiveTriviaAsync()
        {
            var testCode = @"#define TEST1

namespace TestNamespace
{
    using System;

    public class TestClass
    {
#if TEST1
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;
#endif

#if TEST2
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent2;
#elif TEST1
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent2;
#endif

#if TEST3
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent3;
#else
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent3;
#endif
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that event declarations with documentation without a leading blank line within directive trivia will not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidEventDeclarationsAfterDirectiveTriviaAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
#if TEST1
        public int testField;
#endif
        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
#if TEST1
        public int testField;
#endif

        /// <summary>
        /// This is an event.
        /// </summary>
        public event EventHandler TestEvent1;
    }
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(10, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments before documentation are properly handled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumenationPrecededByCommentNotReportedAsync()
        {
            var testCode = @"namespace TestNamespace
{
    // some comment
    /// <summary>
    /// some documentation.
    /// </summary>
    public class TestClass
    {
        // another comment
        /// <summary>more documentation.</summary>
        public void TestMethod() { }
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    // some comment

    /// <summary>
    /// some documentation.
    /// </summary>
    public class TestClass
    {
        // another comment

        /// <summary>more documentation.</summary>
        public void TestMethod() { }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5),
                this.CSharpDiagnostic().WithLocation(10, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments before documentation are properly handled, when the comment is preceded by empty lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumenationPrecededByCommentNotReportedForLooseCommentAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    // some comment
    /// <summary>
    /// some documentation.
    /// </summary>
    public class TestClass
    {
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    using System;

    // some comment

    /// <summary>
    /// some documentation.
    /// </summary>
    public class TestClass
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that trailing comments before documentation are properly handled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTrailingCommentPrecedingDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool SomeMethod() => true; // some comment
    /// <summary>
    /// ...
    /// </summary>
    public void SomeOtherMethod()
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool SomeMethod() => true; // some comment

    /// <summary>
    /// ...
    /// </summary>
    public void SomeOtherMethod()
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(5, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a pragma before a documentation header is properly handled.
        /// This is regression for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1223
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPragmaPrecedingDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
    #pragma warning disable RS1012
    /// <summary>
    /// ...
    /// </summary>
    public void Method1()
    {
    }

    #pragma checksum ""test0.cs"" ""{00000000-0000-0000-0000-000000000000}"" ""{01234567}"" // comment
    /// <summary>
    /// ...
    /// </summary>
    public void Method2()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that preprocessor directives before a documentation header are properly handled.
        /// This is regression for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1231
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDirectivesPrecedingDocumentationAsync()
        {
            var testCode = @"
public class TestClass
{
#if true
    /// <summary>
    /// ...
    /// </summary>
    public void Method1()
    {
    }
#else
    /// <summary>
    /// ...
    /// </summary>
    public void Method2()
    {
    }
#endif
    /// <summary>
    /// ...
    /// </summary>
    public void Method3()
    {
    }

#region SomeRegion
    /// <summary>
    /// ...
    /// </summary>
    public void Method4()
    {
    }
#endregion SomeRegion
    /// <summary>
    /// ...
    /// </summary>
    public void Method5()
    {
    }

#region AnotherRegion // comment
    /// <summary>
    /// ...
    /// </summary>
    public void Method6()
    {
    }
#endregion AnotherRegion // comment
    /// <summary>
    /// ...
    /// </summary>
    public void Method7()
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
#if true
    /// <summary>
    /// ...
    /// </summary>
    public void Method1()
    {
    }
#else
    /// <summary>
    /// ...
    /// </summary>
    public void Method2()
    {
    }
#endif

    /// <summary>
    /// ...
    /// </summary>
    public void Method3()
    {
    }

#region SomeRegion

    /// <summary>
    /// ...
    /// </summary>
    public void Method4()
    {
    }
#endregion SomeRegion

    /// <summary>
    /// ...
    /// </summary>
    public void Method5()
    {
    }

#region AnotherRegion // comment

    /// <summary>
    /// ...
    /// </summary>
    public void Method6()
    {
    }
#endregion AnotherRegion // comment

    /// <summary>
    /// ...
    /// </summary>
    public void Method7()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(19, 5),
                this.CSharpDiagnostic().WithLocation(27, 5),
                this.CSharpDiagnostic().WithLocation(34, 5),
                this.CSharpDiagnostic().WithLocation(42, 5),
                this.CSharpDiagnostic().WithLocation(49, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that documentation of enum members is preceded by a blank line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumMembersMustBePrecededByDocumentationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    /// <summary>
    /// some documentation.
    /// </summary>
    public enum TestEnum
    {
        /// <summary>
        /// some documentation.
        /// </summary>
        Value1,
        /// <summary>
        /// some documentation.
        /// </summary>
        Value2,
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    using System;

    /// <summary>
    /// some documentation.
    /// </summary>
    public enum TestEnum
    {
        /// <summary>
        /// some documentation.
        /// </summary>
        Value1,

        /// <summary>
        /// some documentation.
        /// </summary>
        Value2,
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(14, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1514ElementDocumentationHeaderMustBePrecededByBlankLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1514CodeFixProvider();
        }
    }
}
