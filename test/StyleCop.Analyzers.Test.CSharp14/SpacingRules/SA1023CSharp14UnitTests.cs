// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1023CSharp14UnitTests : SA1023CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldPointerDereferenceAsync()
        {
            var testCode = @"
public unsafe class TestClass
{
    private int result;

    public int* Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = {|#0:*|} field;
        }
    }
}
";

            var fixedCode = @"
public unsafe class TestClass
{
    private int result;

    public int* Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = *field;
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
