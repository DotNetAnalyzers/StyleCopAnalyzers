// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;

public partial class SA1611CSharp11UnitTests : SA1611CSharp10UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestOperatorWithMissingOperatorToken()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS1019").WithSpan(6, 38, 6, 39),
            DiagnosticResult.CompilerError("CS1003").WithSpan(6, 39, 6, 48).WithArguments("("),
        };
    }
}
