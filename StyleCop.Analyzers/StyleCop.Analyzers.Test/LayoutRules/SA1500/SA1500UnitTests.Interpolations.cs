// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostic is reported when the closing brace of a multi-line expression is followed by
        /// the closing brace of an interpolation in a verbatim interpolated string.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3997, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3997")]
        public async Task TestCloseBraceFollowedByInterpolationCloseBraceAsync()
        {
            var testCode = @"
public class TestClass
{
    public string TestMethod()
    {
        return $@""{new[]
        {
            1,
        }}"";
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a diagnostic is still reported when the closing brace of a multi-line expression is followed
        /// by other code inside an interpolation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3997, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3997")]
        public async Task TestCloseBraceFollowedByOtherCodeInInterpolationAsync()
        {
            var testCode = @"
public class TestClass
{
    public string TestMethod()
    {
        return $@""{new[]
        {
            1,
        {|#0:}|} + ""x""}"";
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public string TestMethod()
    {
        return $@""{new[]
        {
            1,
        }
        + ""x""}"";
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
