// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.SpacingRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.SpacingRules;

    public partial class SA1015CSharp13UnitTests : SA1015CSharp12UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultMissingToken()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithLocation(7, 35).WithArguments(">"),
            };
        }
    }
}
