// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using StyleCop.Analyzers.Test.SpacingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly>;

    public class SA1011CSharp7UnitTests : SA1011UnitTests
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

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpFixAsync(LanguageVersion languageVersion, string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(languageVersion)
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            if (source == fixedSource)
            {
                test.FixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.FixedState.MarkupHandling = MarkupMode.Allow;
                test.BatchFixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.BatchFixedState.MarkupHandling = MarkupMode.Allow;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private class CSharpTest : StyleCopCodeFixVerifier<SA1011ClosingSquareBracketsMustBeSpacedCorrectly, TokenSpacingCodeFixProvider>.CSharpTest
        {
            public CSharpTest(LanguageVersion languageVersion)
            {
                this.SolutionTransforms.Add((solution, projectId) =>
                {
                    var parseOptions = (CSharpParseOptions)solution.GetProject(projectId).ParseOptions;
                    return solution.WithProjectParseOptions(projectId, parseOptions.WithLanguageVersion(languageVersion));
                });
            }
        }
    }
}
