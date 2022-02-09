// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
        /// <summary>
        /// Verifies that spacing after a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to prefix the opening parenthesis with a space.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3386, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3386")]
        public async Task TestAfterRangeExpressionAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            string str = ""test"";
            int finalLen = 4;
            var test1 = str[.. {|#0:(|}finalLen - 1)];
            var test2 = .. {|#1:(|}int)finalLen;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            string str = ""test"";
            int finalLen = 4;
            var test1 = str[..(finalLen - 1)];
            var test2 = ..(int)finalLen;
        }
    }
}
";

            await new CSharpTest(LanguageVersion.CSharp8)
            {
                ReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp31,
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorNotPreceded).WithLocation(0),
                    Diagnostic(DescriptorNotPreceded).WithLocation(1),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        [WorkItem(3198, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3198")]
        public async Task TestInPositionalPatternAsync()
        {
            var testCode = @"
class C
{
    void M((bool A, bool B) tuple)
    {
        _ = (tuple, tuple) is{|#0:(|} (true, true),{|#1:(|} true, true));
    }
}
";
            var fixedCode = @"
class C
{
    void M((bool A, bool B) tuple)
    {
        _ = (tuple, tuple) is ((true, true), (true, true));
    }
}
";
            DiagnosticResult[] expectedResults =
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(0),
                Diagnostic(DescriptorPreceded).WithLocation(1),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
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
