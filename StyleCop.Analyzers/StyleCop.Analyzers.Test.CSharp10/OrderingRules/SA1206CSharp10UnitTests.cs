// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CSharp10UnitTests : SA1206CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that a sealed <c>ToString</c> override in a record, which is allowed from C# 10, is not reported
        /// regardless of the order of <see langword="sealed"/> and <see langword="override"/>.
        /// </summary>
        /// <param name="modifiers">The modifiers after the access modifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("sealed override")]
        [InlineData("override sealed")]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestSealedToStringInRecordAsync(string modifiers)
        {
            var testCode = $@"public record R
{{
    public {modifiers} string ToString() => ""R"";
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an access modifier after <see langword="sealed"/> on a record <c>ToString</c> override is reported and
        /// fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestAccessModifierAfterSealedInRecordToStringAsync()
        {
            var testCode = @"public record R
{
    sealed {|#0:public|} override string ToString() => ""R"";
}
";

            var fixedCode = @"public record R
{
    public sealed override string ToString() => ""R"";
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "sealed");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
