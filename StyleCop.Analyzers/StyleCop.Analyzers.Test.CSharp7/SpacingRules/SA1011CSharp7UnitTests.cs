// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Helpers.LanguageVersionTestExtensions;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp7UnitTests : SA1011UnitTests
    {
        /// <summary>
        /// Verifies spacing around a <c>]</c> character in tuple types and expressions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1001CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        /// <seealso cref="SA1009CSharp7UnitTests.TestBracketsInTupleTypesNotFollowedBySpaceAsync"/>
        [Fact]
        public async Task TestBracketsInTupleTypesNotFollowedBySpaceAsync()
        {
            // No diagnostics reported because the offending tokens are followed by comma (SA1001) or a closing
            // parenthesis (SA1009)
            const string testCode = @"using System;

public class Foo
{
    public (int[] , int[] ) TestMethod((int[] , int[] ) a)
    {
        (int[] , int[] ) ints = (new int[][] { new[] { 3 } }[0] , new int[][] { new[] { 3 } }[0] );
        return ints;
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
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
            int* data1 = stackalloc int[ ] { 1 , 1 };
            int* data2 = stackalloc int[ ]{ 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1 };
            int* data4 = stackalloc int[]{ 1 , 1 };
            int* data5 = stackalloc int[]
{ 1 , 1 };
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
            int* data1 = stackalloc int[] { 1 , 1 };
            int* data2 = stackalloc int[] { 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1 };
            int* data4 = stackalloc int[] { 1 , 1 };
            int* data5 = stackalloc int[]
{ 1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 42),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 42),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 42),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(10, 41),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            int* data1 = stackalloc[ ] { 1 , 1 };
            int* data2 = stackalloc[ ]{ 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 };
            int* data4 = stackalloc[]{ 1 , 1 };
            int* data5 = stackalloc[]
{ 1 , 1 };
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
            int* data1 = stackalloc[] { 1 , 1 };
            int* data2 = stackalloc[] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 };
            int* data4 = stackalloc[] { 1 , 1 };
            int* data5 = stackalloc[]
{ 1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 38),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 38),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 38),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(10, 37),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
