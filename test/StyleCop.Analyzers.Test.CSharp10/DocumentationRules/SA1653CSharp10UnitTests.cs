// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation>;

public partial class SA1653CSharp10UnitTests : SA1653CSharp9UnitTests
{
    [Fact]
    public async Task TestPartialRecordStructAsync()
    {
        var source = @"/// <summary>First.</summary>
public partial record struct {|#0:TestType|}(int Value);
/// <summary>Second.</summary>
public partial record struct {|#1:TestType|};";

        await VerifyCSharpDiagnosticAsync(
            source,
            new[]
            {
                Diagnostic().WithLocation(0).WithArguments("TestType"),
                Diagnostic().WithLocation(1).WithArguments("TestType"),
            },
            CancellationToken.None).ConfigureAwait(false);
    }
}
