// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1145DoNotUseAssignmentAsCondition>;

/// <summary>
/// This class contains the unit tests for SA1145.
/// </summary>
/// <seealso cref="SA1145DoNotUseAssignmentAsCondition"/>
public class SA1145UnitTests
{
    [Fact]
    public async Task TestAssignmentsInConditionsAsync()
    {
        var testCode = @"using System;

public class TestClass
{
    public void TestMethod(bool a, bool b)
    {
        if ([|a = b|])
        {
        }

        while (([|a = b|]))
        {
        }

        do
        {
        }
        while ((([|a |= b|])));

        for (; [|a ^= b|]; )
        {
        }

        var x = ([|a = b|]) ? 1 : 2;

        try
        {
        }
        catch (Exception) when ([|a = b|])
        {
        }
    }
}
";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestAssignmentsAsLogicalOperandsAsync()
    {
        var testCode = @"public class TestClass
{
    public bool TestMethod(bool a, bool b, bool c)
    {
        var r1 = a && ([|b = c|]);
        var r2 = ([|b = c|]) || a;
        var r3 = !([|b = c|]);
        return r1 || r2 || r3;
    }
}
";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestValidUsagesAsync()
    {
        var testCode = @"using System;
using System.IO;

public class TestClass
{
    public void TestMethod(bool a, bool b, StringReader reader, int i)
    {
        a = b;
        var c = a = b;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
        }

        if ((i = 3) == 3)
        {
        }

        var x = a ? (i = 1) : (i = 2);
        var y = a && (i = 3) > 2;

        for (i = 0; i < 3; i += 1)
        {
        }

        Func<bool> f = () => a = b;
    }
}
";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }
}
