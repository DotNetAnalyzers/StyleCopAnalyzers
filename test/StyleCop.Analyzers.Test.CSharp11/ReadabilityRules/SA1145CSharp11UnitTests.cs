// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1145DoNotUseAssignmentAsCondition>;

public partial class SA1145CSharp11UnitTests : SA1145CSharp10UnitTests
{
    /// <summary>
    /// Verifies that an unsigned right shift assignment used as a condition is reported, and allowed otherwise.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task TestUnsignedRightShiftAssignmentAsync()
    {
        var testCode = """
            public struct Flags
            {
                public static Flags operator >>>(Flags value, int shift) => value;

                public static implicit operator bool(Flags value) => true;
            }

            public class TestClass
            {
                public void TestMethod(Flags a)
                {
                    if ([|a >>>= 1|])
                    {
                    }

                    var b = a >>>= 1;
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
    }
}
