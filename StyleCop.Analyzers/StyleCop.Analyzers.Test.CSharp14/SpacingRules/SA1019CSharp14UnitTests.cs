// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1019CSharp14UnitTests : SA1019CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldMemberAccessAsync()
        {
            var testCode = @"
public class TestClass
{
    private string result;

    public string Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = field {|#0:.|}ToUpperInvariant();
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private string result;

    public string Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = field.ToUpperInvariant();
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0).WithArguments(".");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
