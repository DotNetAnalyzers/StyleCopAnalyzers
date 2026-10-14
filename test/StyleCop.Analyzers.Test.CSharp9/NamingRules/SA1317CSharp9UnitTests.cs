// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.NamingRules.SA1317IdentifierShouldBeNamedOnlyWithLatinLetters>;

    public partial class SA1317CSharp9UnitTests : SA1317CSharp8UnitTests
    {
        [Fact]
        public async Task TestRecordNameDoesNotContainNonLatinLettersAsync()
        {
            var testCode = @"public record RecordName {}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRecordNameContainsNonLatinLettersAsync()
        {
            var testCode = @"public record RеcоrdNаmе {}";

            var expected = new DiagnosticResult[]
                {
                    Diagnostic().WithArguments("RеcоrdNаmе", 1).WithLocation(1, 15),
                    Diagnostic().WithArguments("RеcоrdNаmе", 3).WithLocation(1, 15),
                    Diagnostic().WithArguments("RеcоrdNаmе", 7).WithLocation(1, 15),
                    Diagnostic().WithArguments("RеcоrdNаmе", 9).WithLocation(1, 15),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
