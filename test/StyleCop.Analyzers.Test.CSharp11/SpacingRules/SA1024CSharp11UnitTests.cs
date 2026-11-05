// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.SpacingRules;

public partial class SA1024CSharp11UnitTests : SA1024CSharp10UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestConditionalExpressionWithMissingColon()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS1003").WithSpan(6, 30, 6, 31).WithArguments(":"),
            DiagnosticResult.CompilerError("CS1525").WithSpan(6, 30, 6, 31).WithArguments(";"),
        };
    }

    protected override DiagnosticResult[] GetExpectedResultTestCaseLabelWithMissingColon()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS1003").WithSpan(8, 16, 8, 21).WithArguments(":"),
        };
    }
}
