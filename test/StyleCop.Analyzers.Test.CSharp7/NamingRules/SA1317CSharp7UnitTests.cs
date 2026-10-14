// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.NamingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
    StyleCop.Analyzers.NamingRules.SA1317IdentifierShouldBeNamedOnlyWithLatinLetters>;

public partial class SA1317CSharp7UnitTests : SA1317UnitTests
{
    [Theory]
    [InlineData(@"class C { void M() { void {|#0:а|}() {} } }")]
    [InlineData(@"class C { void M() { var ({|#0:а|}, b) = (1, 2); } }")]
    [InlineData(@"class C { void M(object o) { if (o is int {|#0:а|}) {} } }")]
    [InlineData(@"class C { (int {|#0:а|}, int b) M() => (1, 2); }")]
    public async Task TestCSharp7DeclarationFormsAsync(string testCode)
    {
        await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0).WithArguments("а", 0), CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestUnnamedTupleElementsAsync()
    {
        await VerifyCSharpDiagnosticAsync(@"class C { (int, int) M() => (1, 2); }", DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }
}
