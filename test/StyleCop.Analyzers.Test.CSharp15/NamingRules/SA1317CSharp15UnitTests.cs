// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.NamingRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp14.NamingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
    StyleCop.Analyzers.NamingRules.SA1317IdentifierShouldBeNamedOnlyWithLatinLetters>;

public partial class SA1317CSharp15UnitTests : SA1317CSharp14UnitTests
{
    [Fact]
    public async Task TestUnionDeclarationAsync()
    {
        await VerifyCSharpDiagnosticAsync(
            @"public union {|#0:а|}(int, string);",
            Diagnostic().WithLocation(0).WithArguments("а", 0),
            CancellationToken.None).ConfigureAwait(false);
    }
}
