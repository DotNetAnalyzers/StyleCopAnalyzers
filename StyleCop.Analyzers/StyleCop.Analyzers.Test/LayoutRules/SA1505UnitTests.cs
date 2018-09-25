// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1505OpeningBracesMustNotBeFollowedByBlankLine"/> class.
    /// </summary>
    public class SA1505UnitTests
    {
        public static IEnumerable<object[]> TypeTestData
        {
            get
            {
                yield return new object[] { "class", "public " };
                yield return new object[] { "struct", "public " };
                yield return new object[] { "interface", string.Empty };
            }
        }

        /// <summary>
        /// Verifies that a valid block will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBlockAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            if (this.testField < 0)
            {
                this.testField = -this.testField;
            }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid block will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBlockAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            if (this.testField < 0)
            {

                this.testField = -this.testField;
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            if (this.testField < 0)
            {
                this.testField = -this.testField;
            }
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(10, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid object initializers will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidObjectInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private struct Helper
        {
            public int A;
            public int B;
        }

        public void TestMethod()
        {
            var v1 = new Helper { A = 10, B = 5  };
            var v2 = new Helper
            {
                A = 5,
                B = 10
            };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid object initializers will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidObjectInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private struct Helper
        {
            public int A;
            public int B;
        }

        public void TestMethod()
        {
            var v2 = new Helper
            {

                A = 5,
                B = 10
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private struct Helper
        {
            public int A;
            public int B;
        }

        public void TestMethod()
        {
            var v2 = new Helper
            {
                A = 5,
                B = 10
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(14, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid collection initializers will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidCollectionInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new List<int> { 1, 2, 3  };
            var v2 = new List<int>
            {
                1,
                2,
                4
            };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid collection initializers will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidCollectionInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new List<int>
            {

                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new List<int>
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(10, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid array initializers will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidArrayInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int[] v1 = { 1, 2, 3  };
            int[] v2 =
            {
                1,
                2,
                4
            };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid array initializers will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidArrayInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int[] v1 =
            {

                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            int[] v1 =
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid complex element initializers will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidComplexElementInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new Dictionary<int, string>
            {
                { 1, ""Test"" }
            };

            var v2 = new Dictionary<int, string>
            {
                { 
                    1, 
                    ""Test"" 
                }
            };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid complex element initializers will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidComplexElementInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new Dictionary<int, string>
            {
                {

                    1, 
                    ""Test"" 
                }
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new Dictionary<int, string>
            {
                {
                    1, 
                    ""Test"" 
                }
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(11, 17);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid anonymous object creation statements will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidAnonymousObjectCreationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new { x = 10, y = 5  };
            var v2 = new
            {
                x = 5,
                y = 10
            };
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid anonymous object creation statements will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidAnonymousObjectCreationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new
            {

                x = 5,
                y = 10
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = new
            {
                x = 5,
                y = 10
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid switch statements will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSwitchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int x)
        {
            switch (x)
            {
                case 1:
                    return 2;
                default:
                    return x;
            }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid switch statements will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSwitchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int x)
        {
            switch (x)
            {

                case 1:
                    return 2;
                default:
                    return x;
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int x)
        {
            switch (x)
            {
                case 1:
                    return 2;
                default:
                    return x;
            }
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid namespace declarations will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidNamespaceDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid namespace declarations will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidNamespaceDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{

    public class TestClass
    {
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid type declarations will not produce any diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <param name="accessModifier">The access modifier to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestValidTypeDeclarationAsync(string typeKeyword, string accessModifier)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
        {accessModifier}int TestProperty {{ get; set; }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid type declarations will produce the expected diagnostics.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <param name="accessModifier">The access modifier to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeTestData))]
        public async Task TestInvalidTypeDeclarationAsync(string typeKeyword, string accessModifier)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{

        {accessModifier}int TestProperty {{ get; set; }}
    }}
}}
";

            var fixedTestCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
        {accessModifier}int TestProperty {{ get; set; }}
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid enum declarations will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEnumDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public enum TestEnum
    {
        TestValue1,
        TestValue2
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid enum declarations will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidEnumDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public enum TestEnum
    {

        TestValue1,
        TestValue2
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public enum TestEnum
    {
        TestValue1,
        TestValue2
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid accessor lists will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidAccessorListAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get; set; }

        public int TestProperty2 
        {
            get; set;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid accessor lists will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidAccessorListAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty
        {

            get; set;
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty
        {
            get; set;
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(6, 9);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an opening brace followed by a comment will not trigger any diagnostics.
        /// </summary>
        /// <remarks><para>Tests regression for #971.</para></remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOpeningSingleLineCommentAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public object TestMethod1()
        {
            // Test comment
            return null;
        }

        public object TestMethod2()
        {
            /* Test comment */
            return null;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an opening brace at the end of the file will not trigger any diagnostics.
        /// </summary>
        /// <remarks>
        /// <para>This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#981.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOpeningBraceAtEndOfFileAsync()
        {
            var testCode = @"namespace TestNamespace
{
";

            var expected = DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(2, 2);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
