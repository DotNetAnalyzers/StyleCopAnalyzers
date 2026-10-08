// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp15UnitTests : SA1600CSharp14UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            if (code == "public void {|#0:TestMember|}() { }")
            {
                return base.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
            }

            // The C# 15 compiler reports CS9348 instead of CS0116 for non-method members in the compilation unit
            return new[]
            {
                DiagnosticResult.CompilerError("CS9348").WithMessage("A compilation unit cannot directly contain members such as fields, methods or properties").WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }
    }
}
