// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp9UnitTests : SA1003CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3974")]
        public async Task TestTargetTypedConditionalExpressionSpacingAsync()
        {
            var testCode = @"
class TestClass
{
    void M(bool flag)
    {
        object value = flag{|#0:?|} null{|#1::|}new();
    }
}";

            var fixedCode = @"
class TestClass
{
    void M(bool flag)
    {
        object value = flag ? null : new();
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithArguments("?").WithLocation(0),
                Diagnostic(DescriptorPrecededByWhitespace).WithArguments(":").WithLocation(1),
                Diagnostic(DescriptorFollowedByWhitespace).WithArguments(":").WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
