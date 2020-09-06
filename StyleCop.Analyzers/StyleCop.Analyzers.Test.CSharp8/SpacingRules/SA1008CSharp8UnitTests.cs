﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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

    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
            Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
            Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1008CSharp8UnitTests : SA1008CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that spacing after a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to prefix the opening parenthesis with a space.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAfterRangeExpressionAsync()
        {
            var testCode = SpecialTypeDefinitions.IndexAndRange + @"
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

            var fixedCode = SpecialTypeDefinitions.IndexAndRange + @"
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

        [Fact]
        [WorkItem(3141, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3141")]
        public async Task TestInPropertyPatternsAsync()
        {
            var testCode = @"
class C
{
    void M((string A, string B) tuple)
    {
        _ = tuple is{|#0:(|} { Length: 1 }, { Length: 2 });
    }
}
";
            var fixedCode = @"
class C
{
    void M((string A, string B) tuple)
    {
        _ = tuple is ({ Length: 1 }, { Length: 2 });
    }
}
";
            DiagnosticResult[] expectedResults =
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(0),
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
