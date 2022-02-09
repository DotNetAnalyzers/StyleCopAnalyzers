// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1009CSharp7UnitTests : SA1009UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>]</c> character in tuple types and expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        /// <seealso cref="SA1011CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        [Fact]
        public async Task TestBracketsInTupleTypesNotFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public (int[] , int[] ) TestMethod((int[] , int[] ) a)
    {
        (int[] , int[] ) ints = (new int[][] { new[] { 3 } }[0] , new int[][] { new[] { 3 } }[0] );
        return ints;
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public (int[] , int[]) TestMethod((int[] , int[]) a)
    {
        (int[] , int[]) ints = (new int[][] { new[] { 3 } }[0] , new int[][] { new[] { 3 } }[0]);
        return ints;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 55),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 24),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 98),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies spacing around a <c>}</c> character in tuple expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestSpacingAroundClosingBraceInTupleExpressionsAsync"/>
        /// <seealso cref="SA1013CSharp7UnitTests.TestSpacingAroundClosingBraceInTupleExpressionsAsync"/>
        [Fact]
        public async Task TestSpacingAroundClosingBraceInTupleExpressionsAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (new[] { 3} , new[] { 3} );
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        var values = (new[] { 3} , new[] { 3});
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(7, 47);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies spacing around a <c>&gt;</c> character in tuple types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestClosingGenericBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        /// <seealso cref="SA1015CSharp7UnitTests.TestClosingGenericBracketsInTupleTypesNotPrecededBySpaceAsync"/>
        [Fact]
        public async Task TestClosingGenericBracketsInTupleTypesNotFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        (Func<int > , Func<int > ) value = (null, null);
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod()
    {
        (Func<int > , Func<int >) value = (null, null);
    }
}";

            DiagnosticResult expected = Diagnostic(DescriptorNotPreceded).WithLocation(7, 34);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing around tuple type casts is handled properly.
        /// </summary>
        /// <remarks>
        /// <para>Tuple type casts must be parenthesized, so there are only a limited number of special cases that need
        /// to be added after the ones in <see cref="SA1009UnitTests.TestSpaceInCastAsync"/>.</para>
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
            var test1 = ((int, int ) )(3, 3);
            var test2 = ((int, int ))(3, 3);
            var test3 = ((int, int) )(3, 3);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 36),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 38),

                // test2
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 36),

                // test3
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 35),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 37),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing around tuple types in parameters is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTupleParameterTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod1((int, int )arg) => 0;
        public int TestMethod2((int, int ) arg) => 0;
        public int TestMethod3((int, int)arg) => 0;

        public int TestMethod4((int, int) arg1, (int, int )arg2) => 0;
        public int TestMethod5((int, int) arg1, (int, int ) arg2) => 0;
        public int TestMethod6((int, int) arg1, (int, int)arg2) => 0;

        public int TestMethod7((int, int[] )arg1) => 0;
        public int TestMethod8((int, int[] ) arg1) => 0;
        public int TestMethod9((int, int[])arg1) => 0;

        public int TestMethod10((int, int ) [] arg1) => 0;
        public int TestMethod11((int, int )[] arg1) => 0;
        public int TestMethod12((int, int) [] arg1) => 0;
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

        public int TestMethod7((int, int[]) arg1) => 0;
        public int TestMethod8((int, int[]) arg1) => 0;
        public int TestMethod9((int, int[]) arg1) => 0;

        public int TestMethod10((int, int)[] arg1) => 0;
        public int TestMethod11((int, int)[] arg1) => 0;
        public int TestMethod12((int, int)[] arg1) => 0;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1, TestMethod2, TestMethod3
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 42),
                Diagnostic(DescriptorFollowed).WithLocation(5, 42),
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 42),
                Diagnostic(DescriptorFollowed).WithLocation(7, 41),

                // TestMethod4, TestMethod5, TestMethod6
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 59),
                Diagnostic(DescriptorFollowed).WithLocation(9, 59),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 59),
                Diagnostic(DescriptorFollowed).WithLocation(11, 58),

                // TestMethod7, TestMethod8, TestMethod9
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 44),
                Diagnostic(DescriptorFollowed).WithLocation(13, 44),
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 44),
                Diagnostic(DescriptorFollowed).WithLocation(15, 43),

                // TestMethod10, TestMethod11, TestMethod12
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 43),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 42),
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
        public (int, int )TestMethod1() => default((int, int ) );
        public (int, int ) TestMethod2() => default((int, int ));
        public (int, int)TestMethod3() => default((int, int) );

        public ((int, int ) , int) TestMethod4() => default(((int, int ) , int));
        public ((int, int ), int) TestMethod5() => default(((int, int ), int));
        public ((int, int) , int) TestMethod6() => default(((int, int) , int));

        public (int, (int, int ) ) TestMethod7() => default((int, (int, int ) ));
        public (int, (int, int )) TestMethod8() => default((int, (int, int )));
        public (int, (int, int) ) TestMethod9() => default((int, (int, int) ));

        public (int x, int y )TestMethod10() => default((int x, int y ) );
        public (int x, int y ) TestMethod11() => default((int x, int y ));
        public (int x, int y)TestMethod12() => default((int x, int y) );

        public ((int x, int y ) , int z) TestMethod13() => default(((int x, int y ) , int z));
        public ((int x, int y ), int z) TestMethod14() => default(((int x, int y ), int z));
        public ((int x, int y) , int z) TestMethod15() => default(((int x, int y) , int z));

        public (int x, (int y, int z ) ) TestMethod16() => default((int x, (int y, int z ) ));
        public (int x, (int y, int z )) TestMethod17() => default((int x, (int y, int z )));
        public (int x, (int y, int z) ) TestMethod18() => default((int x, (int y, int z) ));

        public ((int, int )x, int y) TestMethod19() => default(((int, int )x, int y));
        public ((int, int ) x, int y) TestMethod20() => default(((int, int ) x, int y));
        public ((int, int)x, int y) TestMethod21() => default(((int, int)x, int y));

        public (int x, (int, int )y) TestMethod22() => default((int x, (int, int )y));
        public (int x, (int, int ) y) TestMethod23() => default((int x, (int, int ) y));
        public (int x, (int, int)y) TestMethod24() => default((int x, (int, int)y));
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

        public ((int, int) x, int y) TestMethod19() => default(((int, int) x, int y));
        public ((int, int) x, int y) TestMethod20() => default(((int, int) x, int y));
        public ((int, int) x, int y) TestMethod21() => default(((int, int) x, int y));

        public (int x, (int, int) y) TestMethod22() => default((int x, (int, int) y));
        public (int x, (int, int) y) TestMethod23() => default((int x, (int, int) y));
        public (int x, (int, int) y) TestMethod24() => default((int x, (int, int) y));
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1, TestMethod2, TestMethod3
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 26),
                Diagnostic(DescriptorFollowed).WithLocation(5, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 62),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 62),
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 64),
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 63),
                Diagnostic(DescriptorFollowed).WithLocation(7, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 60),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 62),

                // TestMethod4, TestMethod5, TestMethod6
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 27),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 72),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 72),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 71),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 70),

                // TestMethod7, TestMethod8, TestMethod9
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 32),
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 34),
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 77),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 77),
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 79),
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 32),
                Diagnostic(DescriptorNotPreceded).WithLocation(14, 76),
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(15, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(15, 75),
                Diagnostic(DescriptorNotPreceded).WithLocation(15, 77),

                // TestMethod10, TestMethod11, TestMethod12
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 30),
                Diagnostic(DescriptorFollowed).WithLocation(17, 30),
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 71),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 71),
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 73),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 30),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 72),
                Diagnostic(DescriptorFollowed).WithLocation(19, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 69),
                Diagnostic(DescriptorNotPreceded).WithLocation(19, 71),

                // TestMethod13, TestMethod14, TestMethod15
                Diagnostic(DescriptorNotPreceded).WithLocation(21, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(21, 83),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 83),
                Diagnostic(DescriptorNotPreceded).WithLocation(22, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(22, 82),
                Diagnostic(DescriptorNotFollowed).WithLocation(23, 30),
                Diagnostic(DescriptorNotFollowed).WithLocation(23, 81),

                // TestMethod16, TestMethod17, TestMethod18
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 38),
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 38),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 40),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 90),
                Diagnostic(DescriptorNotFollowed).WithLocation(25, 90),
                Diagnostic(DescriptorNotPreceded).WithLocation(25, 92),
                Diagnostic(DescriptorNotPreceded).WithLocation(26, 38),
                Diagnostic(DescriptorNotPreceded).WithLocation(26, 89),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 37),
                Diagnostic(DescriptorNotPreceded).WithLocation(27, 39),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 88),
                Diagnostic(DescriptorNotPreceded).WithLocation(27, 90),

                // TestMethod19, TestMethod20, TestMethod21
                Diagnostic(DescriptorNotPreceded).WithLocation(29, 27),
                Diagnostic(DescriptorFollowed).WithLocation(29, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(29, 75),
                Diagnostic(DescriptorFollowed).WithLocation(29, 75),
                Diagnostic(DescriptorNotPreceded).WithLocation(30, 27),
                Diagnostic(DescriptorNotPreceded).WithLocation(30, 76),
                Diagnostic(DescriptorFollowed).WithLocation(31, 26),
                Diagnostic(DescriptorFollowed).WithLocation(31, 73),

                // TestMethod22, TestMethod23, TestMethod24
                Diagnostic(DescriptorNotPreceded).WithLocation(33, 34),
                Diagnostic(DescriptorFollowed).WithLocation(33, 34),
                Diagnostic(DescriptorNotPreceded).WithLocation(33, 82),
                Diagnostic(DescriptorFollowed).WithLocation(33, 82),
                Diagnostic(DescriptorNotPreceded).WithLocation(34, 34),
                Diagnostic(DescriptorNotPreceded).WithLocation(34, 83),
                Diagnostic(DescriptorFollowed).WithLocation(35, 33),
                Diagnostic(DescriptorFollowed).WithLocation(35, 80),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2476, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2476")]
        public async Task TestNullableAndArrayTupleReturnTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public (int, int ) ? TestMethod1() => default((int, int ) ?);
        public (int, int )? TestMethod2() => default((int, int )?);
        public (int, int) ? TestMethod3() => default((int, int) ?);

        public (int, int ) [] TestMethod4() => default((int, int ) []);
        public (int, int )[] TestMethod5() => default((int, int )[]);
        public (int, int) [] TestMethod6() => default((int, int) []);
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

        public (int, int)[] TestMethod4() => default((int, int)[]);
        public (int, int)[] TestMethod5() => default((int, int)[]);
        public (int, int)[] TestMethod6() => default((int, int)[]);
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                // TestMethod1, TestMethod2, TestMethod3
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(5, 65),
                Diagnostic(DescriptorNotFollowed).WithLocation(5, 65),
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(6, 64),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 63),

                // TestMethod4, TestMethod5, TestMethod6
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 66),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 66),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 26),
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 65),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 25),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 64),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for tuple expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
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
            var v1 = (1, 2 ) ;
            var v2 = (1, 2 );
            var v3 = (1, 2) ;

            // Nested first element
            var v4 = ((1, 3 ) , 2);
            var v5 = ((1, 3 ), 2);
            var v6 = ((1, 3) , 2);

            // Nested after first element
            var v7 = (1, (2, 3 ) );
            var v8 = (1, (2, 3 ));
            var v9 = (1, (2, 3) );

            // Top level, name inside
            var v10 = (1, x: 2 ) ;
            var v11 = (1, x: 2 );
            var v12 = (1, x: 2) ;

            // Nested first element, name inside
            var v13 = ((1, x: 3 ) , 2);
            var v14 = ((1, x: 3 ), 2);
            var v15 = ((1, x: 3) , 2);

            // Nested after first element, name inside
            var v16 = (1, (2, x: 3 ) );
            var v17 = (1, (2, x: 3 ));
            var v18 = (1, (2, x: 3) );

            // Nested first element, name outside
            var v19 = (x: (1, 3 ) , 2);
            var v20 = (x: (1, 3 ), 2);
            var v21 = (x: (1, 3) , 2);

            // Nested after first element, name outside
            var v22 = (1, x: (2, 3 ) );
            var v23 = (1, x: (2, 3 ));
            var v24 = (1, x: (2, 3) );

            // Indexer
            var v25 = dictionary[(1, 2 ) ];
            var v26 = dictionary[(1, 2 )];
            var v27 = dictionary[(1, 2) ];
            var v28 = dictionary[key: (1, 2 ) ];
            var v29 = dictionary[key: (1, 2 )];
            var v30 = dictionary[key: (1, 2) ];

            // First argument
            dictionary.Add((1, 2 ) , (1, 2));
            dictionary.Add((1, 2 ), (1, 2));
            dictionary.Add((1, 2) , (1, 2));
            dictionary.Add(key: (1, 2 ) , value: (1, 2));
            dictionary.Add(key: (1, 2 ), value: (1, 2));
            dictionary.Add(key: (1, 2) , value: (1, 2));

            // Second argument
            dictionary.Add((1, 2), (1, 2 ) );
            dictionary.Add((1, 2), (1, 2 ));
            dictionary.Add((1, 2), (1, 2) );
            dictionary.Add(key: (1, 2), value: (1, 2 ) );
            dictionary.Add(key: (1, 2), value: (1, 2 ));
            dictionary.Add(key: (1, 2), value: (1, 2) );

            // Returns
            (int, int) LocalFunction1() { return (1, 2 ) ; }
            (int, int) LocalFunction2() { return (1, 2 ); }
            (int, int) LocalFunction3() { return (1, 2) ; }
            (int, int) LocalFunction4() => (1, 2 ) ;
            (int, int) LocalFunction5() => (1, 2 );
            (int, int) LocalFunction6() => (1, 2) ;
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
            var v10 = (1, x: 2);
            var v11 = (1, x: 2);
            var v12 = (1, x: 2);

            // Nested first element, name inside
            var v13 = ((1, x: 3), 2);
            var v14 = ((1, x: 3), 2);
            var v15 = ((1, x: 3), 2);

            // Nested after first element, name inside
            var v16 = (1, (2, x: 3));
            var v17 = (1, (2, x: 3));
            var v18 = (1, (2, x: 3));

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

            // Returns
            (int, int) LocalFunction1() { return (1, 2); }
            (int, int) LocalFunction2() { return (1, 2); }
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
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 28),
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 28),
                Diagnostic(DescriptorNotPreceded).WithLocation(13, 28),
                Diagnostic(DescriptorNotFollowed).WithLocation(14, 27),

                // v4, v5, v6
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 29),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 28),

                // v7, v8, v9
                Diagnostic(DescriptorNotPreceded).WithLocation(22, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 32),
                Diagnostic(DescriptorNotPreceded).WithLocation(22, 34),
                Diagnostic(DescriptorNotPreceded).WithLocation(23, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(24, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(24, 33),

                // v10, v11, v12
                Diagnostic(DescriptorNotPreceded).WithLocation(27, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(27, 32),
                Diagnostic(DescriptorNotPreceded).WithLocation(28, 32),
                Diagnostic(DescriptorNotFollowed).WithLocation(29, 31),

                // v13, v14, v15
                Diagnostic(DescriptorNotPreceded).WithLocation(32, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(32, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(33, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(34, 32),

                // v16, v17, v18
                Diagnostic(DescriptorNotPreceded).WithLocation(37, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(37, 36),
                Diagnostic(DescriptorNotPreceded).WithLocation(37, 38),
                Diagnostic(DescriptorNotPreceded).WithLocation(38, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(39, 35),
                Diagnostic(DescriptorNotPreceded).WithLocation(39, 37),

                // v19, v20, v21
                Diagnostic(DescriptorNotPreceded).WithLocation(42, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(42, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(43, 33),
                Diagnostic(DescriptorNotFollowed).WithLocation(44, 32),

                // v22, v23, v24
                Diagnostic(DescriptorNotPreceded).WithLocation(47, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(47, 36),
                Diagnostic(DescriptorNotPreceded).WithLocation(47, 38),
                Diagnostic(DescriptorNotPreceded).WithLocation(48, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(49, 35),
                Diagnostic(DescriptorNotPreceded).WithLocation(49, 37),

                // v25, v26, v27
                Diagnostic(DescriptorNotPreceded).WithLocation(52, 40),
                Diagnostic(DescriptorNotFollowed).WithLocation(52, 40),
                Diagnostic(DescriptorNotPreceded).WithLocation(53, 40),
                Diagnostic(DescriptorNotFollowed).WithLocation(54, 39),
                Diagnostic(DescriptorNotPreceded).WithLocation(55, 45),
                Diagnostic(DescriptorNotFollowed).WithLocation(55, 45),
                Diagnostic(DescriptorNotPreceded).WithLocation(56, 45),
                Diagnostic(DescriptorNotFollowed).WithLocation(57, 44),

                // First argument
                Diagnostic(DescriptorNotPreceded).WithLocation(60, 34),
                Diagnostic(DescriptorNotFollowed).WithLocation(60, 34),
                Diagnostic(DescriptorNotPreceded).WithLocation(61, 34),
                Diagnostic(DescriptorNotFollowed).WithLocation(62, 33),
                Diagnostic(DescriptorNotPreceded).WithLocation(63, 39),
                Diagnostic(DescriptorNotFollowed).WithLocation(63, 39),
                Diagnostic(DescriptorNotPreceded).WithLocation(64, 39),
                Diagnostic(DescriptorNotFollowed).WithLocation(65, 38),

                // Second argument
                Diagnostic(DescriptorNotPreceded).WithLocation(68, 42),
                Diagnostic(DescriptorNotFollowed).WithLocation(68, 42),
                Diagnostic(DescriptorNotPreceded).WithLocation(68, 44),
                Diagnostic(DescriptorNotPreceded).WithLocation(69, 42),
                Diagnostic(DescriptorNotFollowed).WithLocation(70, 41),
                Diagnostic(DescriptorNotPreceded).WithLocation(70, 43),
                Diagnostic(DescriptorNotPreceded).WithLocation(71, 54),
                Diagnostic(DescriptorNotFollowed).WithLocation(71, 54),
                Diagnostic(DescriptorNotPreceded).WithLocation(71, 56),
                Diagnostic(DescriptorNotPreceded).WithLocation(72, 54),
                Diagnostic(DescriptorNotFollowed).WithLocation(73, 53),
                Diagnostic(DescriptorNotPreceded).WithLocation(73, 55),

                // Returns
                Diagnostic(DescriptorNotPreceded).WithLocation(76, 56),
                Diagnostic(DescriptorNotFollowed).WithLocation(76, 56),
                Diagnostic(DescriptorNotPreceded).WithLocation(77, 56),
                Diagnostic(DescriptorNotFollowed).WithLocation(78, 55),
                Diagnostic(DescriptorNotPreceded).WithLocation(79, 50),
                Diagnostic(DescriptorNotFollowed).WithLocation(79, 50),
                Diagnostic(DescriptorNotPreceded).WithLocation(80, 50),
                Diagnostic(DescriptorNotFollowed).WithLocation(81, 49),
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
        static Func<(int, int ) , (int, int)> Function1;
        static Func<(int, int ), (int, int)> Function2;
        static Func<(int, int) , (int, int)> Function3;

        static Func<(int, int), (int, int ) > Function4;
        static Func<(int, int), (int, int )> Function5;
        static Func<(int, int), (int, int) > Function6;
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 30),
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 43),
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 43),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 42),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2476, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2476")]
        public async Task TestNullableAndArrayTupleTypesAsGenericArgumentsAsync()
        {
            var testCode = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
        static Func<(int, int ) ?, (int, int ) ?> Function1;
        static Func<(int, int )?, (int, int )?> Function2;
        static Func<(int, int) ?, (int, int) ?> Function3;

        static Func<(int, int ) [], (int, int ) []> Function4;
        static Func<(int, int )[], (int, int )[]> Function5;
        static Func<(int, int) [], (int, int) []> Function6;
    }
}
";

            var fixedCode = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
        static Func<(int, int)?, (int, int)?> Function1;
        static Func<(int, int)?, (int, int)?> Function2;
        static Func<(int, int)?, (int, int)?> Function3;

        static Func<(int, int)[], (int, int)[]> Function4;
        static Func<(int, int)[], (int, int)[]> Function5;
        static Func<(int, int)[], (int, int)[]> Function6;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Function1, Function2, Function3
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 46),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 46),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 45),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 30),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 44),

                // Function4, Function5, Function6
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 47),
                Diagnostic(DescriptorNotFollowed).WithLocation(11, 47),
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 31),
                Diagnostic(DescriptorNotPreceded).WithLocation(12, 46),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 30),
                Diagnostic(DescriptorNotFollowed).WithLocation(13, 45),
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
            var (x1, y1 )= (1, 2);
            var (x2, y2 ) = (1, 2);
            var (x3, y3)= (1, 2);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 25),
                Diagnostic(DescriptorFollowed).WithLocation(7, 25),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 25),
                Diagnostic(DescriptorFollowed).WithLocation(9, 24),
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
            ref int t1 = ref (test ) ;
            ref int t2 = ref (test );
            ref int t3 = ref (test) ;
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
            ref int t1 = ref (test);
            ref int t2 = ref (test);
            ref int t3 = ref (test);
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(10, 36),
                Diagnostic(DescriptorNotPreceded).WithLocation(11, 36),
                Diagnostic(DescriptorNotFollowed).WithLocation(12, 35),
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
        [WorkItem(2494, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2494")]
        public async Task TestNullableVariableDeclarationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        (int a, string b)? testVar1;
        (int a, string b) ? testVar2;
    }
}
";

            var fixedCode = @"public class TestClass
{
    public void TestMethod()
    {
        (int a, string b)? testVar1;
        (int a, string b)? testVar2;
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(6, 25),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
