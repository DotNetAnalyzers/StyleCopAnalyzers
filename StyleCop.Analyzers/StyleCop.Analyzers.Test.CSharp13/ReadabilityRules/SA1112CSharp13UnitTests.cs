// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1112CSharp13UnitTests : SA1112CSharp12UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseListWithNoArgumentsClosingParenthesisOnTheNextLine()
        {
            return new[]
            {
                // Diagnostic previously issued twice because of https://github.com/dotnet/roslyn/issues/70488
                Diagnostic().WithLocation(0),
            };
        }
    }
}
