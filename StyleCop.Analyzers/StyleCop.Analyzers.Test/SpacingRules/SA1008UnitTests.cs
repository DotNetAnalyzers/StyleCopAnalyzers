﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1008OpeningParenthesisMustBeSpacedCorrectly"/> class.
    /// </summary>
    public class SA1008UnitTests
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
                Diagnostic(DescriptorPreceded).WithLocation(9, 24),

                // test2
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 25),

                // test3
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 34),

                // test4
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 32),

                // test5
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 25),

                // test6
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 27),

                // test7
                Diagnostic(DescriptorNotPreceded).WithLocation(15, 29),

                // test8
                Diagnostic(DescriptorNotPreceded).WithLocation(16, 34),

                // TestMethod2 - 1st call
                Diagnostic(DescriptorNotFollowed).WithLocation(18, 24),

                // TestMethod2 - 2nd call
                Diagnostic(DescriptorNotFollowed).WithLocation(20, 17),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 32),

                // TestMethod2
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 32),

                // TestMethod3
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 33),

                // TestMethod4
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 33),

                // TestMethod5
                Diagnostic(DescriptorNotPreceded).WithLocation(21, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 33),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic(DescriptorPreceded).WithLocation(9, 21),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 22),

                // v3
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 22),
                Diagnostic(DescriptorPreceded).WithLocation(11, 33),

                // v4
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 24),

                // v5
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 31),

                // v6
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for await expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAwaitExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Threading.Tasks;

    public class TestClass
    {
        public async Task TestMethod1()
        {
            await( Task.Delay(1000));
        }

        public async Task TestMethod2()
        {
            await ( Task.Delay(1000));
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Threading.Tasks;

    public class TestClass
    {
        public async Task TestMethod1()
        {
            await(Task.Delay(1000));
        }

        public async Task TestMethod2()
        {
            await (Task.Delay(1000));
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 18),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            var v1 = checked ( 1 + 2);
            var v2 = checked( 1 + 2);

            var v3 = unchecked ( 1 + 2);
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
            var v1 = checked (1 + 2);
            var v2 = checked(1 + 2);

            var v3 = unchecked (1 + 2);
            var v4 = unchecked(1 + 2);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 30),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 29),

                // v3
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 32),

                // v4
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 31),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            var v1 = default ( T);
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
            var v1 = default (T);
            var v2 = default(T);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 30),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for query expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestQueryExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testList = { 1, 2, 3 };

        public void TestMethod()
        {
            var testResult1 =
                from x in( testList)
                where( x > 0)
                select( x);

            var testResult2 =
                from x in ( testList)
                where ( x > 0)
                select ( x);
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    using System.Linq;

    public class TestClass
    {
        private int[] testList = { 1, 2, 3 };

        public void TestMethod()
        {
            var testResult1 =
                from x in(testList)
                where(x > 0)
                select(x);

            var testResult2 =
                from x in (testList)
                where (x > 0)
                select (x);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st query expression
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 23),

                // 2nd query expression
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 27),
                Diagnostic(DescriptorNotFollowed).WithLocation(18, 23),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 24),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for throw expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestThrowExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod1()
        {
            throw( new Exception());
        }

        public void TestMethod2()
        {
            throw ( new Exception());
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
            throw(new Exception());
        }

        public void TestMethod2()
        {
            throw (new Exception());
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 18),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            var v1 = typeof ( int);
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
            var v1 = typeof (int);
            var v2 = typeof(int);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 29),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 28),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            var v1 = sizeof ( int);
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
            var v1 = sizeof (int);
            var v2 = sizeof(int);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 29),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 28),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 29),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 28),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 20),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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

            while( i < 100)
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

            while(i < 100)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 18),

                // 2nd while statement
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 19),

                // 3rd while statement
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 19),
                Diagnostic(DescriptorPreceded).WithLocation(19, 33),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            while( i < 100);

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
            while(i < 100);

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
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 18),

                // 2nd do statement
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 19),

                // 3rd do statement
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 19),
                Diagnostic(DescriptorPreceded).WithLocation(25, 33),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            for( var i = 0; i < 9; i++)
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
            for(var i = 0; i < 9; i++)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 16),

                // 2nd for statement
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 17),

                // 3rd for statement
                Diagnostic(DescriptorPreceded).WithLocation(15, 28),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            foreach( var i in testList)
            {
            }

            foreach ( var i in testList)
            {
            }

            foreach (var i in( IEnumerable)testList)
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
            foreach(var i in testList)
            {
            }

            foreach (var i in testList)
            {
            }

            foreach (var i in(IEnumerable)testList)
            {
            }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 1st foreach statement
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 20),

                // 2nd foreach statement
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 21),

                // 3rd foreach statement
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 30),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for return statements is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestReturnStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1()
        {
            return( 0);
        }

        public int TestMethod2()
        {
            return ( 0);
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1()
        {
            return(0);
        }

        public int TestMethod2()
        {
            return (0);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 19),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 20),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            using( s1)
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
            using(s1)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 18),

                // 2nd using statement
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                fixed( int* p = &x.A)
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
                fixed(int* p = &x.A)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 22),

                // 2nd using statement
                Diagnostic(DescriptorNotFollowed).WithLocation(20, 23),
                Diagnostic(DescriptorNotPreceded).WithLocation(23, 31),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            lock( this)
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
            lock(this)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 17),

                // 2nd using statement
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 18),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            if( x > 0)
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
            if(x > 0)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 15),

                // 2nd if statement
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 16),

                // 3rd if statement
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 18),
                Diagnostic(DescriptorPreceded).WithLocation(17, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            switch( x)
            {
            case( 0):
            default:
                return;
            }

            switch ( x)
            {
            case ( 0):
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
            switch(x)
            {
            case(0):
            default:
                return;
            }

            switch (x)
            {
            case (0):
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
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 19),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 17),

                // 2nd switch statement
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 20),
                Diagnostic(DescriptorNotFollowed).WithLocation(16, 18),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
            catch( InvalidOperationException)
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
            catch(InvalidOperationException)
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
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 18),

                // 1st catch clause
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 19),

                // 3rd catch clause
                Diagnostic(DescriptorPreceded).WithLocation(18, 53),

                // 4th catch clause
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 51),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
        public T TestMethod1<T>() where T : new ()
        {
            return default(T);
        }

        public T TestMethod2<T>() where T : new()
        {
            return default(T);
        }

        public T TestMethod3<T>() where T : new ()
        {
            return default(T);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // 2nd constructor constraint
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 48),

                // 3rd constructor constraint
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 49),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 19),

                // 2nd attribute
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 18),

                // 3rd attribute
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 19),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                Diagnostic(DescriptorPreceded).WithLocation(9, 34),

                // v2
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 35),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 43),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1585, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1585")]
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

            var expected = Diagnostic().WithLocation(4, 27).WithArguments(" not", "preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that calls on 'var' are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1000UnitTests.TestVarIdentifierInvocationAsync"/>
        [Fact]
        [WorkItem(2419, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2419")]
        public async Task TestVarIdentifierInvocationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System;
    using System.Linq;

    public class TestClass
    {
        public void TestMethod()
        {
            Func<int>[] x = null;
            x.Select(var => var ( ));
            x.Select(var => var ());
            x.Select(var => var( ));
        }
    }
}";

            var fixedTestCode = @"namespace TestNamespace
{
    using System;
    using System.Linq;

    public class TestClass
    {
        public void TestMethod()
        {
            Func<int>[] x = null;
            x.Select(var => var());
            x.Select(var => var());
            x.Select(var => var());
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 32),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, '(' expected").WithLocation(5, 15),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2475, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2475")]
        public async Task TestSingleLineIfStatementAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        if (true) (true ? 1 : 0).ToString();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
