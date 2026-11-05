// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1113CommaMustBeOnSameLineAsPreviousParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1113CSharp11UnitTests : SA1113CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestArrayCreationWithMissingCloseBracket()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0443").WithSpan(7, 16, 7, 16),
                DiagnosticResult.CompilerError("CS1003").WithSpan(7, 16, 7, 17).WithArguments(","),
                DiagnosticResult.CompilerError("CS1003").WithSpan(7, 16, 7, 17).WithArguments("]"),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestConstructorInitializerWithMissingArgumentList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS7036").WithSpan(12, 11, 12, 15).WithArguments("first", "BaseClass.BaseClass(int, int)"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(12, 15, 12, 15).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(12, 15, 12, 15),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestDelegateWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 38, 4, 39).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 38, 4, 39),
            };
        }
    }
}
