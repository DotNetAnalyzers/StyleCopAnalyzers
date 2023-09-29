// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;

    public partial class SA1600CSharp9UnitTests : SA1600CSharp8UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            if (code == "public void {|#0:TestMember|}() { }")
            {
                return new[]
                {
                    // error CS8805: Program using top-level statements must be an executable.
                    DiagnosticResult.CompilerError("CS8805"),

                    // /0/Test0.cs(4,1): error CS0106: The modifier 'public' is not valid for this item
                    DiagnosticResult.CompilerError("CS0106").WithSpan(4, 1, 4, 7).WithArguments("public"),
                };
            }

            return base.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
        }
    }
}
