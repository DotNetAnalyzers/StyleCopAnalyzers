// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;

public partial class SA1110CSharp10UnitTests : SA1110CSharp9UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingType()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS1001").WithSpan(4, 37, 4, 38),
        };
    }

    protected override DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingParameterList()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS0501").WithSpan(4, 37, 4, 46).WithArguments("TestClass.implicit operator TestClass()"),
            DiagnosticResult.CompilerError("CS1019").WithSpan(4, 46, 4, 46),
            DiagnosticResult.CompilerError("CS1003").WithSpan(4, 46, 4, 47).WithArguments("(", ";"),
            DiagnosticResult.CompilerError("CS1026").WithSpan(4, 46, 4, 47),
        };
    }

    protected override DiagnosticResult[] GetExpectedResultTestOperatorWithMissingParameterList()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS0501").WithSpan(4, 38, 4, 39).WithArguments("TestClass.operator +()"),
            DiagnosticResult.CompilerError("CS1534").WithSpan(4, 38, 4, 39).WithArguments("+"),
            DiagnosticResult.CompilerError("CS1003").WithSpan(4, 39, 4, 40).WithArguments("(", ";"),
            DiagnosticResult.CompilerError("CS1026").WithSpan(4, 39, 4, 40),
        };
    }

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
