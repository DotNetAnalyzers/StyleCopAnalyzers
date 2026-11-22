// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1145DoNotUseAssignmentAsCondition>;

public partial class SA1145CSharp8UnitTests : SA1145CSharp7UnitTests
{
    /// <summary>
    /// Verifies that a null-coalescing assignment used as a condition is reported, and allowed otherwise.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task TestCoalesceAssignmentAsync()
    {
        var testCode = """
            public class TestClass
            {
                public void TestMethod(bool? a)
                {
                    if ([|a ??= false|])
                    {
                    }

                    if ((a ??= false) == true)
                    {
                    }

                    var b = a ??= false;
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }
}
