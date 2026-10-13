// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp9.NamingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.NamingRules.SA1309FieldNamesMustNotBeginWithUnderscore,
    StyleCop.Analyzers.NamingRules.SA1309CodeFixProvider>;

public partial class SA1309CSharp10UnitTests : SA1309CSharp9UnitTests
{
    [Theory]
    [InlineData("_value", "value")]
    [InlineData("_value.Length", "value.Length")]
    [WorkItem(2939, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2939")]
    public async Task TestFieldRenameInExtendedPropertyPatternAsync(string pattern, string fixedPattern)
    {
        var testCode = @"public class C
{
    private string {|#0:_value|} = string.Empty;

    public bool Match(C obj, string value)
    {
        return obj is { " + pattern + @": var result } && _value == value;
    }
}";
        var fixedCode = @"public class C
{
    private string value = string.Empty;

    public bool Match(C obj, string value)
    {
        return obj is { " + fixedPattern + @": var result } && this.value == value;
    }
}";

        await VerifyCSharpFixAsync(testCode, Diagnostic().WithArguments("_value").WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
    }
}
