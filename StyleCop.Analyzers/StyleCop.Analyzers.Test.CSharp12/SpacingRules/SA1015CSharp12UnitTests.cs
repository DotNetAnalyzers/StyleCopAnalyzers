// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;

    public partial class SA1015CSharp12UnitTests : SA1015CSharp11UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultMissingToken()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithLocation(7, 35).WithArguments(","),
                DiagnosticResult.CompilerError("CS1003").WithLocation(7, 36).WithArguments(">"),
                DiagnosticResult.CompilerError("CS1026").WithLocation(7, 36),
            };
        }
    }
}
