// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public class SA1003CSharp8UnitTests : SA1003CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that spacing around a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to surround the range expression with spaces.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRangeExpressionAsync()
        {
            var testCode = SpecialTypeDefinitions.IndexAndRange + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = .. (int)1;
        }
    }
}
";

            var fixedCode = SpecialTypeDefinitions.IndexAndRange + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = ..(int)1;
        }
    }
}
";
            var expectedResults = new DiagnosticResult[]
            {
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(26, 28).WithArguments("(int)"),
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
