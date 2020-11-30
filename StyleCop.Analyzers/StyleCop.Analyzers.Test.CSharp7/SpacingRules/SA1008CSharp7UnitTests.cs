// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1008CSharp7UnitTests : SA1008UnitTests
    {
        /// <summary>
        /// Verifies that spacing around tuple type casts is handled properly.
        /// </summary>
        /// <remarks>
        /// <para>Tuple type casts must be parenthesized, so there are only a limited number of special cases that need
        /// to be added after the ones in <see cref="SA1008UnitTests.TestTypeCastSpacingAsync"/>.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleTypeCastSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = ( ( int, int))(3, 3);
            var test2 = ( (int, int))(3, 3);
            var test3 = (( int, int))(3, 3);
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = ((int, int))(3, 3);
            var test2 = ((int, int))(3, 3);
            var test3 = ((int, int))(3, 3);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // test1
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 27),

                // test2
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 25),

                // test3
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 26),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing around tuple types in parameters is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1008UnitTests.TestParameterListsAsync"/>
        [Fact]
        public async Task TestTupleParameterTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1( ( int, int) arg) => 0;
        public int TestMethod2( (int, int) arg) => 0;
        public int TestMethod3(( int, int) arg) => 0;

        public int TestMethod4((int, int) arg1,( int, int) arg2) => 0;
        public int TestMethod5((int, int) arg1,(int, int) arg2) => 0;
        public int TestMethod6((int, int) arg1, ( int, int) arg2) => 0;
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1((int, int) arg) => 0;
        public int TestMethod2((int, int) arg) => 0;
        public int TestMethod3((int, int) arg) => 0;

        public int TestMethod4((int, int) arg1, (int, int) arg2) => 0;
        public int TestMethod5((int, int) arg1, (int, int) arg2) => 0;
        public int TestMethod6((int, int) arg1, (int, int) arg2) => 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 33),

                // TestMethod2
                Diagnostic(DescriptorNotFollowed).WithLocation(6, 31),

                // TestMethod3
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 32),

                // TestMethod4
                Diagnostic(DescriptorPreceded).WithLocation(9, 48),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 48),

                // TestMethod5
                Diagnostic(DescriptorPreceded).WithLocation(10, 48),

                // TestMethod6
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 49),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2472")]
        public async Task TestTupleArrayParameterTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1( ( int, int)[] arg) => 0;
        public int TestMethod2( (int, int)[] arg) => 0;
        public int TestMethod3(( int, int)[] arg) => 0;

        public int TestMethod4((int, int) arg1, params( int, int)[] arg2) => 0;
        public int TestMethod5((int, int) arg1, params(int, int)[] arg2) => 0;
        public int TestMethod6((int, int) arg1, params ( int, int)[] arg2) => 0;
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1((int, int)[] arg) => 0;
        public int TestMethod2((int, int)[] arg) => 0;
        public int TestMethod3((int, int)[] arg) => 0;

        public int TestMethod4((int, int) arg1, params (int, int)[] arg2) => 0;
        public int TestMethod5((int, int) arg1, params (int, int)[] arg2) => 0;
        public int TestMethod6((int, int) arg1, params (int, int)[] arg2) => 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 33),

                // TestMethod2
                Diagnostic(DescriptorNotFollowed).WithLocation(6, 31),

                // TestMethod3
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 32),

                // TestMethod4
                Diagnostic(DescriptorPreceded).WithLocation(9, 55),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 55),

                // TestMethod5
                Diagnostic(DescriptorPreceded).WithLocation(10, 55),

                // TestMethod6
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 56),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2472")]
        [WorkItem(2532, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2532")]
        public async Task TestTupleOutParametersAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1(out( int, int)[] arg) => throw null;
        public int TestMethod2(out(int, int)[] arg) => throw null;
        public int TestMethod3(out ( int, int)[] arg) => throw null;
        public void TestMethod4()
        {
            var x = new System.Collections.Generic.Dictionary<int, (int, bool)>();
            x.TryGetValue(1, out( int, bool) value1);
            x.TryGetValue(2, out(int, bool) value2);
            x.TryGetValue(3, out ( int, bool) value3);
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1(out (int, int)[] arg) => throw null;
        public int TestMethod2(out (int, int)[] arg) => throw null;
        public int TestMethod3(out (int, int)[] arg) => throw null;
        public void TestMethod4()
        {
            var x = new System.Collections.Generic.Dictionary<int, (int, bool)>();
            x.TryGetValue(1, out (int, bool) value1);
            x.TryGetValue(2, out (int, bool) value2);
            x.TryGetValue(3, out (int, bool) value3);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1
                Diagnostic(DescriptorPreceded).WithLocation(5, 35),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 35),

                // TestMethod2
                Diagnostic(DescriptorPreceded).WithLocation(6, 35),

                // TestMethod3
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 36),

                // TestMethod4
                Diagnostic(DescriptorPreceded).WithLocation(11, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 33),
                Diagnostic(DescriptorPreceded).WithLocation(12, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 34),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing around tuple return types is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleReturnTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public( int, int) TestMethod1() => default( ( int, int));
        public(int, int) TestMethod2() => default( (int, int));
        public ( int, int) TestMethod3() => default(( int, int));

        public ( ( int, int), int) TestMethod4() => default(( ( int, int), int));
        public ( (int, int), int) TestMethod5() => default(( (int, int), int));
        public (( int, int), int) TestMethod6() => default((( int, int), int));

        public (int,( int, int)) TestMethod7() => default((int,( int, int)));
        public (int,(int, int)) TestMethod8() => default((int,(int, int)));
        public (int, ( int, int)) TestMethod9() => default((int, ( int, int)));

        public( int x, int y) TestMethod10() => default( ( int x, int y));
        public(int x, int y) TestMethod11() => default( (int x, int y));
        public ( int x, int y) TestMethod12() => default(( int x, int y));

        public ( ( int x, int y), int z) TestMethod13() => default(( ( int x, int y), int z));
        public ( (int x, int y), int z) TestMethod14() => default(( (int x, int y), int z));
        public (( int x, int y), int z) TestMethod15() => default((( int x, int y), int z));

        public (int x,( int y, int z)) TestMethod16() => default((int x,( int y, int z)));
        public (int x,(int y, int z)) TestMethod17() => default((int x,(int y, int z)));
        public (int x, ( int y, int z)) TestMethod18() => default((int x, ( int y, int z)));
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public (int, int) TestMethod1() => default((int, int));
        public (int, int) TestMethod2() => default((int, int));
        public (int, int) TestMethod3() => default((int, int));

        public ((int, int), int) TestMethod4() => default(((int, int), int));
        public ((int, int), int) TestMethod5() => default(((int, int), int));
        public ((int, int), int) TestMethod6() => default(((int, int), int));

        public (int, (int, int)) TestMethod7() => default((int, (int, int)));
        public (int, (int, int)) TestMethod8() => default((int, (int, int)));
        public (int, (int, int)) TestMethod9() => default((int, (int, int)));

        public (int x, int y) TestMethod10() => default((int x, int y));
        public (int x, int y) TestMethod11() => default((int x, int y));
        public (int x, int y) TestMethod12() => default((int x, int y));

        public ((int x, int y), int z) TestMethod13() => default(((int x, int y), int z));
        public ((int x, int y), int z) TestMethod14() => default(((int x, int y), int z));
        public ((int x, int y), int z) TestMethod15() => default(((int x, int y), int z));

        public (int x, (int y, int z)) TestMethod16() => default((int x, (int y, int z)));
        public (int x, (int y, int z)) TestMethod17() => default((int x, (int y, int z)));
        public (int x, (int y, int z)) TestMethod18() => default((int x, (int y, int z)));
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1, TestMethod2, TestMethod3
                Diagnostic(DescriptorPreceded).WithLocation(5, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 51),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 53),
                Diagnostic(DescriptorPreceded).WithLocation(6, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(6, 50),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 53),

                // TestMethod4, TestMethod5, TestMethod6
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 18),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 61),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 63),
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 60),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 17),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 61),

                // TestMethod7, TestMethod8, TestMethod9
                Diagnostic(DescriptorPreceded).WithLocation(13, 21),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 21),
                Diagnostic(DescriptorPreceded).WithLocation(13, 64),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 64),
                Diagnostic(DescriptorPreceded).WithLocation(14, 21),
                Diagnostic(DescriptorPreceded).WithLocation(14, 63),
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 66),

                // TestMethod10, TestMethod11, TestMethod12
                Diagnostic(DescriptorPreceded).WithLocation(17, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 56),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 58),
                Diagnostic(DescriptorPreceded).WithLocation(18, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(18, 55),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 58),

                // TestMethod13, TestMethod14, TestMethod15
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 18),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 68),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 70),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 67),
                Diagnostic(DescriptorNotFollowed).WithLocation(23, 17),
                Diagnostic(DescriptorNotFollowed).WithLocation(23, 68),

                // TestMethod16, TestMethod17, TestMethod18
                Diagnostic(DescriptorPreceded).WithLocation(25, 23),
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 23),
                Diagnostic(DescriptorPreceded).WithLocation(25, 73),
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 73),
                Diagnostic(DescriptorPreceded).WithLocation(26, 23),
                Diagnostic(DescriptorPreceded).WithLocation(26, 72),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 24),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 75),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2472")]
        public async Task TestNullableTupleReturnTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public( int, int)? TestMethod1() => default( ( int, int)?);
        public(int, int)? TestMethod2() => default( (int, int)?);
        public ( int, int)? TestMethod3() => default(( int, int)?);
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public (int, int)? TestMethod1() => default((int, int)?);
        public (int, int)? TestMethod2() => default((int, int)?);
        public (int, int)? TestMethod3() => default((int, int)?);
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1, TestMethod2, TestMethod3
                Diagnostic(DescriptorPreceded).WithLocation(5, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 52),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 54),
                Diagnostic(DescriptorPreceded).WithLocation(6, 15),
                Diagnostic(DescriptorNotFollowed).WithLocation(6, 51),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 54),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for tuple expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1000CSharp7UnitTests.TestReturnTupleExpressionsAsync"/>
        [Fact]
        public async Task TestTupleExpressionsAsync()
        {
            var testCode = @"using System.Collections.Generic;

namespace TestNamespace
{
    public class TestClass
    {
        private static Dictionary<(int, int), (int, int)> dictionary;

        public void TestMethod()
        {
            // Top level
            var v1 =( 1, 2);
            var v2 =(1, 2);
            var v3 = ( 1, 2);

            // Nested first element
            var v4 = ( ( 1, 3), 2);
            var v5 = ( (1, 3), 2);
            var v6 = (( 1, 3), 2);

            // Nested after first element
            var v7 = (1,( 2, 3));
            var v8 = (1,(2, 3));
            var v9 = (1, ( 2, 3));

            // Top level, name inside
            var v10 =( x: 1, 2);
            var v11 =(x: 1, 2);
            var v12 = ( x: 1, 2);

            // Nested first element, name inside
            var v13 = ( ( x: 1, 3), 2);
            var v14 = ( (x: 1, 3), 2);
            var v15 = (( x: 1, 3), 2);

            // Nested after first element, name inside
            var v16 = (1,( x: 2, 3));
            var v17 = (1,(x: 2, 3));
            var v18 = (1, ( x: 2, 3));

            // Nested first element, name outside
            var v19 = (x:( 1, 3), 2);
            var v20 = (x:(1, 3), 2);
            var v21 = (x: ( 1, 3), 2);

            // Nested after first element, name outside
            var v22 = (1, x:( 2, 3));
            var v23 = (1, x:(2, 3));
            var v24 = (1, x: ( 2, 3));

            // Indexer
            var v25 = dictionary[ ( 1, 2)];
            var v26 = dictionary[ (1, 2)];
            var v27 = dictionary[( 1, 2)];
            var v28 = dictionary[key:( 1, 2)];
            var v29 = dictionary[key:(1, 2)];
            var v30 = dictionary[key: ( 1, 2)];

            // First argument
            dictionary.Add( ( 1, 2), (1, 2));
            dictionary.Add( (1, 2), (1, 2));
            dictionary.Add(( 1, 2), (1, 2));
            dictionary.Add(key:( 1, 2), value: (1, 2));
            dictionary.Add(key:(1, 2), value: (1, 2));
            dictionary.Add(key: ( 1, 2), value: (1, 2));

            // Second argument
            dictionary.Add((1, 2),( 1, 2));
            dictionary.Add((1, 2),(1, 2));
            dictionary.Add((1, 2), ( 1, 2));
            dictionary.Add(key: (1, 2), value:( 1, 2));
            dictionary.Add(key: (1, 2), value:(1, 2));
            dictionary.Add(key: (1, 2), value: ( 1, 2));

            // Returns (leading spaces after keyword checked in SA1000, not SA1008)
            (int, int) LocalFunction1() { return( 1, 2); }
            (int, int) LocalFunction2() { return(1, 2); }
            (int, int) LocalFunction3() { return ( 1, 2); }
            (int, int) LocalFunction4() =>( 1, 2);
            (int, int) LocalFunction5() =>(1, 2);
            (int, int) LocalFunction6() => ( 1, 2);
        }
    }
}
";

            var fixedCode = @"using System.Collections.Generic;

namespace TestNamespace
{
    public class TestClass
    {
        private static Dictionary<(int, int), (int, int)> dictionary;

        public void TestMethod()
        {
            // Top level
            var v1 = (1, 2);
            var v2 = (1, 2);
            var v3 = (1, 2);

            // Nested first element
            var v4 = ((1, 3), 2);
            var v5 = ((1, 3), 2);
            var v6 = ((1, 3), 2);

            // Nested after first element
            var v7 = (1, (2, 3));
            var v8 = (1, (2, 3));
            var v9 = (1, (2, 3));

            // Top level, name inside
            var v10 = (x: 1, 2);
            var v11 = (x: 1, 2);
            var v12 = (x: 1, 2);

            // Nested first element, name inside
            var v13 = ((x: 1, 3), 2);
            var v14 = ((x: 1, 3), 2);
            var v15 = ((x: 1, 3), 2);

            // Nested after first element, name inside
            var v16 = (1, (x: 2, 3));
            var v17 = (1, (x: 2, 3));
            var v18 = (1, (x: 2, 3));

            // Nested first element, name outside
            var v19 = (x: (1, 3), 2);
            var v20 = (x: (1, 3), 2);
            var v21 = (x: (1, 3), 2);

            // Nested after first element, name outside
            var v22 = (1, x: (2, 3));
            var v23 = (1, x: (2, 3));
            var v24 = (1, x: (2, 3));

            // Indexer
            var v25 = dictionary[(1, 2)];
            var v26 = dictionary[(1, 2)];
            var v27 = dictionary[(1, 2)];
            var v28 = dictionary[key: (1, 2)];
            var v29 = dictionary[key: (1, 2)];
            var v30 = dictionary[key: (1, 2)];

            // First argument
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));

            // Second argument
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add((1, 2), (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));
            dictionary.Add(key: (1, 2), value: (1, 2));

            // Returns (leading spaces after keyword checked in SA1000, not SA1008)
            (int, int) LocalFunction1() { return(1, 2); }
            (int, int) LocalFunction2() { return(1, 2); }
            (int, int) LocalFunction3() { return (1, 2); }
            (int, int) LocalFunction4() => (1, 2);
            (int, int) LocalFunction5() => (1, 2);
            (int, int) LocalFunction6() => (1, 2);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // v1, v2, v3
                Diagnostic(DescriptorPreceded).WithLocation(12, 21),
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 21),
                Diagnostic(DescriptorPreceded).WithLocation(13, 21),
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 22),

                // v4, v5, v6
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 24),
                Diagnostic(DescriptorNotFollowed).WithLocation(18, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 23),

                // v7, v8, v9
                Diagnostic(DescriptorPreceded).WithLocation(22, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 25),
                Diagnostic(DescriptorPreceded).WithLocation(23, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(24, 26),

                // v10, v11, v12
                Diagnostic(DescriptorPreceded).WithLocation(27, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 22),
                Diagnostic(DescriptorPreceded).WithLocation(28, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(29, 23),

                // v13, v14, v15
                Diagnostic(DescriptorNotFollowed).WithLocation(32, 23),
                Diagnostic(DescriptorNotFollowed).WithLocation(32, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(33, 23),
                Diagnostic(DescriptorNotFollowed).WithLocation(34, 24),

                // v16, v17, v18
                Diagnostic(DescriptorPreceded).WithLocation(37, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(37, 26),
                Diagnostic(DescriptorPreceded).WithLocation(38, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(39, 27),

                // v19, v20, v21
                Diagnostic(DescriptorPreceded).WithLocation(42, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(42, 26),
                Diagnostic(DescriptorPreceded).WithLocation(43, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(44, 27),

                // v22, v23, v24
                Diagnostic(DescriptorPreceded).WithLocation(47, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(47, 29),
                Diagnostic(DescriptorPreceded).WithLocation(48, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(49, 30),

                // v25, v26, v27
                Diagnostic(DescriptorNotPreceded).WithLocation(52, 35),
                Diagnostic(DescriptorNotFollowed).WithLocation(52, 35),
                Diagnostic(DescriptorNotPreceded).WithLocation(53, 35),
                Diagnostic(DescriptorNotFollowed).WithLocation(54, 34),

                // v28, v29, v30
                Diagnostic(DescriptorPreceded).WithLocation(55, 38),
                Diagnostic(DescriptorNotFollowed).WithLocation(55, 38),
                Diagnostic(DescriptorPreceded).WithLocation(56, 38),
                Diagnostic(DescriptorNotFollowed).WithLocation(57, 39),

                // First argument
                Diagnostic(DescriptorNotFollowed).WithLocation(60, 27),
                Diagnostic(DescriptorNotFollowed).WithLocation(60, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(61, 27),
                Diagnostic(DescriptorNotFollowed).WithLocation(62, 28),
                Diagnostic(DescriptorPreceded).WithLocation(63, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(63, 32),
                Diagnostic(DescriptorPreceded).WithLocation(64, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(65, 33),

                // Second argument
                Diagnostic(DescriptorPreceded).WithLocation(68, 35),
                Diagnostic(DescriptorNotFollowed).WithLocation(68, 35),
                Diagnostic(DescriptorPreceded).WithLocation(69, 35),
                Diagnostic(DescriptorNotFollowed).WithLocation(70, 36),
                Diagnostic(DescriptorPreceded).WithLocation(71, 47),
                Diagnostic(DescriptorNotFollowed).WithLocation(71, 47),
                Diagnostic(DescriptorPreceded).WithLocation(72, 47),
                Diagnostic(DescriptorNotFollowed).WithLocation(73, 48),

                // Returns
                Diagnostic(DescriptorNotFollowed).WithLocation(76, 49),
                Diagnostic(DescriptorNotFollowed).WithLocation(78, 50),
                Diagnostic(DescriptorPreceded).WithLocation(79, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(79, 43),
                Diagnostic(DescriptorPreceded).WithLocation(80, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(81, 44),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for tuple types used as generic arguments is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleTypesAsGenericArgumentsAsync()
        {
            var testCode = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
        static Func< ( int, int), (int, int)> Function1;
        static Func< (int, int), (int, int)> Function2;
        static Func<( int, int), (int, int)> Function3;

        static Func<(int, int),( int, int)> Function4;
        static Func<(int, int),(int, int)> Function5;
        static Func<(int, int), ( int, int)> Function6;
    }
}
";

            var fixedCode = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
        static Func<(int, int), (int, int)> Function1;
        static Func<(int, int), (int, int)> Function2;
        static Func<(int, int), (int, int)> Function3;

        static Func<(int, int), (int, int)> Function4;
        static Func<(int, int), (int, int)> Function5;
        static Func<(int, int), (int, int)> Function6;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 22),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 22),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 21),
                Diagnostic(DescriptorPreceded).WithLocation(11, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 32),
                Diagnostic(DescriptorPreceded).WithLocation(12, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 33),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for tuple variable declarations is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleVariablesAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var( x1, y1) = (1, 2);
            var(x2, y2) = (1, 2);
            var ( x3, y3) = (1, 2);
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var (x1, y1) = (1, 2);
            var (x2, y2) = (1, 2);
            var (x3, y3) = (1, 2);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorPreceded).WithLocation(7, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 16),
                Diagnostic(DescriptorPreceded).WithLocation(8, 16),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 17),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for <c>ref</c> expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRefExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Threading.Tasks;

    public class TestClass
    {
        public void TestMethod()
        {
            int test = 1;
            ref int t = ref( test);
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    using System.Threading.Tasks;

    public class TestClass
    {
        public void TestMethod()
        {
            int test = 1;
            ref int t = ref (test);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorPreceded).WithLocation(10, 28),
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 28),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for <c>new</c> expressions for an array of a tuple type is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1000CSharp7UnitTests.TestNewTupleArrayAsync"/>
        [Fact]
        public async Task TestNewTupleArrayAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new( int, int)[0];
            var y = new(int, int)[0];
            var z = new ( int, int)[0];
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new(int, int)[0];
            var y = new(int, int)[0];
            var z = new (int, int)[0];
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 24),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 25),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for <c>foreach</c> expressions using tuple deconstruction is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1000CSharp7UnitTests.TestForEachVariableStatementAsync"/>
        [Fact]
        public async Task TestForEachVariableStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            foreach( var (x, y) in new (int, int)[0]) { }
            foreach(var (x, y) in new (int, int)[0]) { }
            foreach ( var (x, y) in new (int, int)[0]) { }
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            foreach(var (x, y) in new (int, int)[0]) { }
            foreach(var (x, y) in new (int, int)[0]) { }
            foreach (var (x, y) in new (int, int)[0]) { }
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 20),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 21),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2475, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2475")]
        public async Task TestSingleLineIfStatementWithTupleExpressionAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        if (true) (1, 2).ToString();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2568, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2568")]
        public async Task TestTupleExpressionParamsAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod(params (string name, string value)[] options)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3117, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3117")]
        public async Task TestTupleParameterAttributesAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    class NotNullAttribute : System.Attribute { }
    class CustomAttribute : System.Attribute { }

    class TestClass
    {
        void TestMethod1([NotNull] (string, string) tuple) { }

        void TestMethod2([NotNull, Custom] (string, string) tuple) { }

        void TestMethod3([NotNull][Custom] (string, string) tuple) { }

        void TestMethod4([NotNull]{|#0:(|}string, string) tuple) { }

        void TestMethod5([NotNull, Custom]{|#1:(|}string, string) tuple) { }

        void TestMethod6([NotNull][Custom]{|#2:(|}string, string) tuple) { }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    class NotNullAttribute : System.Attribute { }
    class CustomAttribute : System.Attribute { }

    class TestClass
    {
        void TestMethod1([NotNull] (string, string) tuple) { }

        void TestMethod2([NotNull, Custom] (string, string) tuple) { }

        void TestMethod3([NotNull][Custom] (string, string) tuple) { }

        void TestMethod4([NotNull] (string, string) tuple) { }

        void TestMethod5([NotNull, Custom] (string, string) tuple) { }

        void TestMethod6([NotNull][Custom] (string, string) tuple) { }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorPreceded).WithLocation(1),
                Diagnostic(DescriptorPreceded).WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
