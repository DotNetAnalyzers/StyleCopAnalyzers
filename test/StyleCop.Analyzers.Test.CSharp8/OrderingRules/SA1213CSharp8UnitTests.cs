// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules;

using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp7.OrderingRules;

public partial class SA1213CSharp8UnitTests : SA1213CSharp7UnitTests
{
    protected override DiagnosticResult[] GetExpectedResultTestEventWithMissingAccessorList()
    {
        return new[]
        {
            DiagnosticResult.CompilerError("CS0535").WithSpan(9, 26, 9, 31).WithArguments("TestClass", "ITest.TestEvent.add"),
            DiagnosticResult.CompilerError("CS0535").WithSpan(9, 26, 9, 31).WithArguments("TestClass", "ITest.TestEvent.remove"),
            DiagnosticResult.CompilerError("CS0071").WithSpan(11, 30, 11, 39),
        };
    }
}
