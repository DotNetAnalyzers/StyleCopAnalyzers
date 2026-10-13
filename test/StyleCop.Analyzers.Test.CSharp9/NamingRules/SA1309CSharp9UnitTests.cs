// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.NamingRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp8.NamingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.NamingRules.SA1309FieldNamesMustNotBeginWithUnderscore,
    StyleCop.Analyzers.NamingRules.SA1309CodeFixProvider>;

public partial class SA1309CSharp9UnitTests : SA1309CSharp8UnitTests
{
    [Fact]
    [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
    public async Task TestFieldRenameInWithInitializerAsync()
    {
        var testCode = @"public record C
{
    private int {|#0:_value|};

    public C Create(int value)
    {
        return this with { _value = _value + value };
    }
}";
        var fixedCode = @"public record C
{
    private int value;

    public C Create(int value)
    {
        return this with { value = this.value + value };
    }
}";

        await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_value").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
    }
}
