// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.LayoutRules.SA1515SingleLineCommentMustBePrecededByBlankLine,
        Analyzers.LayoutRules.SA1515CodeFixProvider>;

    public class SA1515CSharp7UnitTests : SA1515UnitTests
    {
        [Fact]
        public async Task TestCommentAfterCasePatternSwitchLabelAsync()
        {
            var testCode = @"
public class ClassName
{
    public void Method()
    {
        switch (new object())
        {
            case int x:
                // Single line comment after pattern-matching case statement is valid
                break;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
