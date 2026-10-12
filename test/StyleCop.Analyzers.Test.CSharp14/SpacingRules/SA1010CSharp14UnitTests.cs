// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp14UnitTests : SA1010CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldIndexerAsync()
        {
            var testCode = @"
public class TestClass
{
    private int result;

    public int[] Items
    {
        get => field;
        set
        {
            field = value;
            this.result = field {|#0:[|}0];
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int result;

    public int[] Items
    {
        get => field;
        set
        {
            field = value;
            this.result = field[0];
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldInCollectionExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    private int[] items;

    public int Value
    {
        get => field;
        set
        {
            field = value;
            this.items = {|#0:[|} field ];
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int[] items;

    public int Value
    {
        get => field;
        set
        {
            field = value;
            this.items = [field ];
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
