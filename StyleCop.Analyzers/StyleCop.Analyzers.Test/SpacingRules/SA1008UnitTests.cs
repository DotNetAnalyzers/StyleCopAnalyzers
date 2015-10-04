// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;

    /// <summary>
    /// Unit tests for the <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/> class.
    /// </summary>
    public class SA1008UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that spacing between consecutive type casts is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTypeCastSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int[] array = { 1, 2, 3 };

        public void TestMethod()
        {
            var test1 =(double)0;
            var test2 = ( string)null;
            var test3 = (string) (object)null;
            var test4 = array[ (int)test1];
            var test5 = ( (int)test1 + 1);
            var test6 = - (int)test1;
            var test7 = $""{ (int)test1:X8}"";
            var test8 = (ushort) ((System.Array)array).Length;

            TestMethod2( (int)test1);
            TestMethod2(
                ( int)test1);
        }

        public void TestMethod2(int x)
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int[] array = { 1, 2, 3 };

        public void TestMethod()
        {
            var test1 = (double)0;
            var test2 = (string)null;
            var test3 = (string)(object)null;
            var test4 = array[(int)test1];
            var test5 = ((int)test1 + 1);
            var test6 = -(int)test1;
            var test7 = $""{(int)test1:X8}"";
            var test8 = (ushort)((System.Array)array).Length;

            TestMethod2((int)test1);
            TestMethod2(
                (int)test1);
        }

        public void TestMethod2(int x)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // test1
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 24),

                // test2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 25),

                // test3
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(11, 34),

                // test4
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(12, 32),

                // test5
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(13, 25),

                // test6
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(14, 27),

                // test7
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(15, 29),

                // test8
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(16, 34),

                // TestMethod2 - 1st call
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(18, 24),

                // TestMethod2 - 2nd call
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(20, 17)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for multi-line parameter lists is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultiLineParameterListAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod1(
            int x,
            int y,
            int z)
        {
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for parameter lists is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestParameterListsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod1( )
        {
        }

        public void TestMethod2( int x)
        {
        }

        public void TestMethod3 ()
        {
        }

        public void TestMethod4 ( )
        {
        }

        public void TestMethod5 ( int x)
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod1()
        {
        }

        public void TestMethod2(int x)
        {
        }

        public void TestMethod3()
        {
        }

        public void TestMethod4()
        {
        }

        public void TestMethod5(int x)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // TestMethod1
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(5, 32),

                // TestMethod2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(9, 32),

                // TestMethod3
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(13, 33),

                // TestMethod4
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(17, 33),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(17, 33),

                // TestMethod5
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(21, 33),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(21, 33)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for parenthesized expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestParenthesizedExpressionsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int[] array = { 1, 2, 3 };

        public void TestMethod()
        {
            var v1 =(1 + 2);
            var v2 = ( 1 + 2);
            var v3 = ( (1 + 2) *(3 + 4));
            var v4 = ! (v3 > 0);
            var v5 = (double) (v1 + v2);
            var v6 = array[ (v1 - v2)];
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int[] array = { 1, 2, 3 };

        public void TestMethod()
        {
            var v1 = (1 + 2);
            var v2 = (1 + 2);
            var v3 = ((1 + 2) * (3 + 4));
            var v4 = !(v3 > 0);
            var v5 = (double)(v1 + v2);
            var v6 = array[(v1 - v2)];
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 21),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 22),

                // v3
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(11, 22),
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(11, 33),

                // v4
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(12, 24),

                // v5
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(13, 31),

                // v6
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(14, 29)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for checked / unchecked expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCheckedExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = checked (1 + 2);
            var v2 = checked( 1 + 2);

            var v3 = unchecked (1 + 2);
            var v4 = unchecked( 1 + 2);
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
            var v1 = checked(1 + 2);
            var v2 = checked(1 + 2);

            var v3 = unchecked(1 + 2);
            var v4 = unchecked(1 + 2);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 30),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 29),

                // v3
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(10, 32),

                // v4
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(11, 31)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for default expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDefaultExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod<T>()
        {
            var v1 = default (T);
            var v2 = default( T);
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod<T>()
        {
            var v1 = default(T);
            var v2 = default(T);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 30),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 29)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for typeof expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTypeOfExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = typeof (int);
            var v2 = typeof( int);
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
            var v1 = typeof(int);
            var v2 = typeof(int);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 29),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 28)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for sizeof expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSizeOfExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = sizeof (int);
            var v2 = sizeof( int);
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
            var v1 = sizeof(int);
            var v2 = sizeof(int);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 29),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 28)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for nameof expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNameOfExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var v1 = nameof (TestMethod);
            var v2 = nameof( TestMethod);
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
            var v1 = nameof(TestMethod);
            var v2 = nameof(TestMethod);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 29),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(8, 28)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for argument lists is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestArgumentListAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private TestClass(int x, int y)
        {
        }

        public TestClass(int x)
            : this (x, 0)
        {
        }

        public TestClass()
            : this( 1, 0)
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private TestClass(int x, int y)
        {
        }

        public TestClass(int x)
            : this(x, 0)
        {
        }

        public TestClass()
            : this(1, 0)
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(10, 20),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(15, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for while statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWhileStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var i = 0;

            while(i < 100)
            {
                i++;
            }

            while ( i >= 0)
            {
                i--;
            }

            while ( (i < 100) &&(i * i < 5000))
            {
                i++;
            }
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
            var i = 0;

            while (i < 100)
            {
                i++;
            }

            while (i >= 0)
            {
                i--;
            }

            while ((i < 100) && (i * i < 5000))
            {
                i++;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st while statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 18),

                // 2nd while statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(14, 19),

                // 3rd while statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(19, 19),
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(19, 33)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for do ... while statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var i = 0;

            do
            {
                i++;
            }
            while(i < 100);

            do
            {
                i--;
            }
            while ( i >= 0);

            do
            {
                i++;
            }
            while ( (i < 100) &&(i * i < 5000));
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
            var i = 0;

            do
            {
                i++;
            }
            while (i < 100);

            do
            {
                i--;
            }
            while (i >= 0);

            do
            {
                i++;
            }
            while ((i < 100) && (i * i < 5000));
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st do statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(13, 18),

                // 2nd do statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(19, 19),

                // 3rd do statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(25, 19),
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(25, 33)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for for statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestForStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            for(var i = 0; i < 9; i++)
            {
            }

            for ( var i = 0; i < 9; i++)
            {
            }

            for (var i = 0;(i < 9); i++)
            {
            }
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
            for (var i = 0; i < 9; i++)
            {
            }

            for (var i = 0; i < 9; i++)
            {
            }

            for (var i = 0; (i < 9); i++)
            {
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st for statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(7, 16),

                // 2nd for statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(11, 17),

                // 3rd for statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(15, 28)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for foreach statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestForeachStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections;

    public class TestClass
    {
        private int[] testList = { 1, 2, 3 };

        public void TestMethod()
        {
            foreach(var i in testList)
            {
            }

            foreach ( var i in testList)
            {
            }

            foreach (var i in(IEnumerable)testList)
            {
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Collections;

    public class TestClass
    {
        private int[] testList = { 1, 2, 3 };

        public void TestMethod()
        {
            foreach (var i in testList)
            {
            }

            foreach (var i in testList)
            {
            }

            foreach (var i in (IEnumerable)testList)
            {
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st foreach statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(11, 20),

                // 2nd foreach statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(15, 21),

                // 3rd foreach statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(19, 30)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for using statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestUsingStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.IO;

    public class TestClass
    {
        public void TestMethod(Stream s1, Stream s2)
        {
            using(s1)
            {
                s1.Flush();
            }

            using ( s2)
            {
                s2.Flush();
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.IO;

    public class TestClass
    {
        public void TestMethod(Stream s1, Stream s2)
        {
            using (s1)
            {
                s1.Flush();
            }

            using (s2)
            {
                s2.Flush();
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st using statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 18),

                // 2nd using statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(14, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for fixed statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixedStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public class HelperClass
        {
            public int A;
            public int B;
        }

        public void TestMethod(HelperClass x)
        {
            unsafe
            {
                fixed(int* p = &x.A)
                {
                    *p = 1;
                }

                fixed ( int* p = &x.B)
                {
                    *p = 2;
                    var y = * (byte*)p;
                }
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public class HelperClass
        {
            public int A;
            public int B;
        }

        public void TestMethod(HelperClass x)
        {
            unsafe
            {
                fixed (int* p = &x.A)
                {
                    *p = 1;
                }

                fixed (int* p = &x.B)
                {
                    *p = 2;
                    var y = *(byte*)p;
                }
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st using statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(15, 22),

                // 2nd using statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(20, 23),
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(23, 31)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for lock statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLockStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            lock(this)
            {
                this.testField = 1;
            }

            lock ( this)
            {
                this.testField = 2;
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
            lock (this)
            {
                this.testField = 1;
            }

            lock (this)
            {
                this.testField = 2;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st using statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 17),

                // 2nd using statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(14, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for if statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            if(x > 0)
            {
                return;
            }

            if ( y > 0)
            {
                return;
            }

            if ( ( x < 0) ||( y < 0))
            {
                return;
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x, int y)
        {
            if (x > 0)
            {
                return;
            }

            if (y > 0)
            {
                return;
            }

            if ((x < 0) || (y < 0))
            {
                return;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st if statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(7, 15),

                // 2nd if statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(12, 16),

                // 3rd if statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(17, 16),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(17, 18),
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(17, 29),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(17, 29)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for switch statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSwitchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x)
        {
            switch(x)
            {
                default:
                    return;
            }

            switch ( x)
            {
                default:
                    return;
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x)
        {
            switch (x)
            {
                default:
                    return;
            }

            switch (x)
            {
                default:
                    return;
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st switch statement
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(7, 19),

                // 2nd switch statement
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(13, 20)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for try ... catch statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTryCatchStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int x)
        {
            try
            {
            }
            catch(InvalidOperationException)
            {
            }
            catch ( ArgumentNullException)
            {
            }
            catch (ArgumentOutOfRangeException) when(x > 0)
            {
            }
            catch (IndexOutOfRangeException) when ( x < 0)
            {
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int x)
        {
            try
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentOutOfRangeException) when (x > 0)
            {
            }
            catch (IndexOutOfRangeException) when (x < 0)
            {
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st catch clause
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(12, 18),

                // 1st catch clause
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(15, 19),

                // 3rd catch clause
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(18, 53),

                // 4th catch clause
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(21, 51)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for constructor constraints is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorConstraintAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public T TestMethod1<T>() where T : new ()
        {
            return default(T);
        }

        public T TestMethod2<T>() where T : new( )
        {
            return default(T);
        }

        public T TestMethod3<T>() where T : new ( )
        {
            return default(T);
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public T TestMethod1<T>() where T : new()
        {
            return default(T);
        }

        public T TestMethod2<T>() where T : new()
        {
            return default(T);
        }

        public T TestMethod3<T>() where T : new()
        {
            return default(T);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st constructor constraint
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(5, 49),

                // 2nd constructor constraint
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 48),

                // 3rd constructor constraint
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(15, 49),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(15, 49)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for attribute arguments is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAttributeArgumentsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        [Obsolete (""Test"")]
        public void TestMethod1()
        {
        }

        [Obsolete( ""Test"")]
        public void TestMethod2()
        {
        }

        [Obsolete ( ""Test"")]
        public void TestMethod3()
        {
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        [Obsolete(""Test"")]
        public void TestMethod1()
        {
        }

        [Obsolete(""Test"")]
        public void TestMethod2()
        {
        }

        [Obsolete(""Test"")]
        public void TestMethod3()
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st attribute
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(7, 19),

                // 2nd attribute
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(12, 18),

                // 3rd attribute
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(17, 19),
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(17, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an opening parenthesis that is first on a line is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFirstOnLineAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod1(int a, Action<int, int> b)
        {
        }

        public void TestMethod2()
        {
            TestMethod1(
                (100 + 10),
                (v1, v2) => { });
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for lambda expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLambdaExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod1()
        {
            Action<int, int> v1 =(a, b) => { };
            Action<int, int> v2 = ( a, b) => { };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod1()
        {
            Action<int, int> v1 = (a, b) => { };
            Action<int, int> v2 = (a, b) => { };
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                this.CSharpDiagnostic(DescriptorPreceded).WithLocation(9, 34),

                // v2
                this.CSharpDiagnostic(DescriptorNotFollowed).WithLocation(10, 35)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that multi-line comments are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultiLineCommentAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int a, Action<int, int> b)
        {
            byte[] values =
            {
                0,
                /*test*/(byte)a
            };
            
            if /*Strangely, this is valid*/(a > 0)
            {
                return;
            }

            while /*test*/(a > 0)
            {
                a--;
            }
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that named parameters are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamedParameterAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod1(int a, int b)
        {
        }

        public void TestMethod2(Action<int, int> a)
        {
        }

        public void TestMethod3()
        {
            TestMethod1(a: 10, b: (100 + 10) + 40);
            TestMethod1(
                a: 10,
                b: (100 + 10) + 40);
            TestMethod2(a: (v1, v2) => { });
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that interpolations are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterpolationCorrectSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            string fileName = ""Foo"";
            var FileContainer = new { HasContentChanged = true };
            var fileTitle = $""{fileName}{(FileContainer.HasContentChanged ? ""*"" : String.Empty)}"";
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that interpolations are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterpolationWrongSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            string fileName = ""Foo"";
            var FileContainer = new { HasContentChanged = true };
            var fileTitle = $""{fileName}{ (FileContainer.HasContentChanged ? ""*"" : String.Empty)}"";
        }
    }
}";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod()
        {
            string fileName = ""Foo"";
            var FileContainer = new { HasContentChanged = true };
            var fileTitle = $""{fileName}{(FileContainer.HasContentChanged ? ""*"" : String.Empty)}"";
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic(DescriptorNotPreceded).WithLocation(11, 43)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1585:
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1585
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCrefAttributeAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <see cref=""Method ()""/>
    public void Method()
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    /// <see cref=""Method()""/>
    public void Method()
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 27).WithArguments(" not", "preceded");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
class ClassName
{
    ClassName()
        : base)
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS1003",
                    Severity = DiagnosticSeverity.Error,
                    Message = "Syntax error, '(' expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 15) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1008OpeningParenthesisMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
