// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1001CSharp7UnitTests : SA1001UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>]</c> character in tuple types and expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1009CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
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
    public (int[], int[] ) TestMethod((int[], int[] ) a)
    {
        (int[], int[] ) ints = (new int[][] { new[] { 3 } }[0], new int[][] { new[] { 3 } }[0] );
        return ints;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 19).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(5, 47).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(7, 16).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(7, 65).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies spacing around a <c>}</c> character in tuple expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1009CSharp7UnitTests.TestSpacingAroundClosingBraceInTupleExpressionsAsync"/>
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
        var values = (new[] { 3}, new[] { 3} );
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 34).WithArguments(" not", "preceded");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies spacing around a <c>&gt;</c> character in tuple types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1009CSharp7UnitTests.TestClosingGenericBracketsInTupleTypesNotFollowedBySpaceAsync"/>
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
        (Func<int >, Func<int > ) value = (null, null);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 21).WithArguments(" not", "preceded");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[] { 1 , 1 };
            int* data2 = stackalloc int[] { 1 ,1 };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[] { 1, 1 };
            int* data2 = stackalloc int[] { 1, 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 47).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(8, 47).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(8, 47).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc[] { 1 , 1 };
            int* data2 = stackalloc[] { 1 ,1 };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc[] { 1, 1 };
            int* data2 = stackalloc[] { 1, 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(7, 43).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(8, 43).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(8, 43).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
