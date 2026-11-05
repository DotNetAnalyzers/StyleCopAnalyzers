// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;

public partial class SA1111CSharp10UnitTests : SA1111CSharp9UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestIndexerWithMissingParameterList()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS1551"),
            DiagnosticResult.CompilerError("CS0548").WithSpan(4, 16, 4, 20).WithArguments("TestClass.this"),
            DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("[", ";"),
            DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("]", ";"),
            DiagnosticResult.CompilerError("CS1014").WithSpan(4, 20, 4, 21),
            DiagnosticResult.CompilerError("CS1514").WithSpan(4, 20, 4, 21),
            DiagnosticResult.CompilerError("CS1513").WithSpan(5, 2, 5, 2),
        };
    }
}
