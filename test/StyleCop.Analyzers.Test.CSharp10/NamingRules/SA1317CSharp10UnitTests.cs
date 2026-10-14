// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp9.NamingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
    StyleCop.Analyzers.NamingRules.SA1317IdentifierShouldBeNamedOnlyWithLatinLetters>;

public partial class SA1317CSharp10UnitTests : SA1317CSharp9UnitTests
{
    [Theory]
    [InlineData(@"namespace A.{|#0:а|}; class C {}")]
    [InlineData(@"record struct {|#0:а|} {}")]
    public async Task TestCSharp10DeclarationFormsAsync(string testCode)
    {
        await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0).WithArguments("а", 0), CancellationToken.None).ConfigureAwait(false);
    }
}
