// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public partial class SA1130CSharp12UnitTests : SA1130CSharp11UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultCodeFixSpecialCases()
        {
            return new[]
            {
                Diagnostic().WithLocation(8, 20),
                Diagnostic().WithLocation(9, 20),
                Diagnostic().WithLocation(10, 25),
                Diagnostic().WithLocation(11, 30),
                Diagnostic().WithLocation(12, 30),
                DiagnosticResult.CompilerError("CS1065").WithLocation(12, 53),
                Diagnostic().WithLocation(13, 30),
                DiagnosticResult.CompilerError("CS7014").WithLocation(13, 47),
                Diagnostic().WithLocation(14, 30),
                DiagnosticResult.CompilerError("CS1670").WithLocation(14, 47),
                Diagnostic().WithLocation(15, 25),
                DiagnosticResult.CompilerError("CS1669").WithLocation(15, 42),
                DiagnosticResult.CompilerError("CS0225").WithLocation(14, 47),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultAfterFixCodeFixSpecialCases()
        {
            return new DiagnosticResult[]
            {
                Diagnostic().WithLocation(12, 30),
                DiagnosticResult.CompilerError("CS1065").WithLocation(12, 53),
                Diagnostic().WithLocation(13, 30),
                DiagnosticResult.CompilerError("CS7014").WithLocation(13, 47),
                Diagnostic().WithLocation(14, 30),
                DiagnosticResult.CompilerError("CS1670").WithLocation(14, 47),
                Diagnostic().WithLocation(15, 25),
                DiagnosticResult.CompilerError("CS1669").WithLocation(15, 42),
                DiagnosticResult.CompilerError("CS0225").WithLocation(14, 47),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultVerifyInvalidCodeConstructions()
        {
            return new[]
            {
                Diagnostic().WithSpan(4, 50, 4, 58),
                DiagnosticResult.CompilerError("CS1660").WithLocation(4, 50),
            };
        }
    }
}
