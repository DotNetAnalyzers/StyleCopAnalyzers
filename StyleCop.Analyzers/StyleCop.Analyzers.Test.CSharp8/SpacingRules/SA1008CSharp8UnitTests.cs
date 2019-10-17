// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
            StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
            StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1008CSharp8UnitTests : SA1008CSharp7UnitTests
    {
        private const string RangePrologue = @"namespace System
{
    public struct Range
    {
        public Range(Index a, Index b)
        {
        }
        public Index Start { get; }
        public Index End { get; }
    }

    public struct Index
    {
        public static implicit operator Index(int value) => throw null;
        public int GetOffset(int length) => throw null;
    }
}
";

        /// <summary>
        /// Verifies that spacing after a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to prefix the opening parenthesis with a space.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestBeforeRangeExpressionAsync()
        {
            var testCode = RangePrologue + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public string TestMethod()
        {
            string str = ""test"";
            int finalLen = 4;
            return str[.. (finalLen - 1)];
        }
    }
}
";

            var fixedCode = RangePrologue + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public string TestMethod()
        {
            string str = ""test"";
            int finalLen = 4;
            return str[..(finalLen - 1)];
        }
    }
}
";
            var expectedResults = new DiagnosticResult[]
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(28, 27),
            };

            await VerifyCSharpFixAsync(
                LanguageVersion.CSharp8,
                testCode,
                expectedResults,
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
