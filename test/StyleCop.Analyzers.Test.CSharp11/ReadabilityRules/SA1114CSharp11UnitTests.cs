// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1114ParameterListMustFollowDeclaration>;

    public partial class SA1114CSharp11UnitTests : SA1114CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorParametersList2LinesAfterOpeningParenthesis()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestConstructorInitializerWithMissingArgumentList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS7036").WithSpan(12, 11, 12, 15).WithArguments("value", "BaseClass.BaseClass(int)"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(12, 15, 12, 15).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(12, 15, 12, 15),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestIndexerWithMissingOpenBracket()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 20).WithArguments("["),
            };
        }
    }
}
