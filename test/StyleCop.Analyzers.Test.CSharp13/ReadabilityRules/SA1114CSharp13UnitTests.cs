// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1114ParameterListMustFollowDeclaration>;

    public partial class SA1114CSharp13UnitTests : SA1114CSharp12UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseListArgumentsList2LinesAfterOpeningParenthesis()
        {
            return new[]
            {
                // Diagnostic previously issued twice because of https://github.com/dotnet/roslyn/issues/70488
                Diagnostic().WithLocation(0),
            };
        }
    }
}
