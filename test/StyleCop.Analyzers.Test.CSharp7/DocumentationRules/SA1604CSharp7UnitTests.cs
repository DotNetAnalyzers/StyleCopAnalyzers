// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.DocumentationRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.DocumentationRules;

public partial class SA1604CSharp7UnitTests : SA1604UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestDestructorWithMissingName()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS0574").WithSpan(6, 6, 6, 6),
            DiagnosticResult.CompilerError("CS1001").WithSpan(6, 6, 6, 7),
        };
    }
}
