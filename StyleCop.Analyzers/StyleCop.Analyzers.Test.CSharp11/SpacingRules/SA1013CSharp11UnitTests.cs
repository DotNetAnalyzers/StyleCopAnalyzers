// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.SpacingRules.SA1013ClosingBracesMustBeSpacedCorrectly>;

    public partial class SA1013CSharp11UnitTests : SA1013CSharp10UnitTests
    {
        /// <summary>
        /// Verifies the behavior of closing braces in interpolated strings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3914, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3914")]
        public async Task TestSpacingAroundClosingBraceInInterpolatedStringsAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public string TestMethod(object value)
    {
        // The space before '}' is not checked
        return $""Some random text {
                value
            } and some more random text."";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
