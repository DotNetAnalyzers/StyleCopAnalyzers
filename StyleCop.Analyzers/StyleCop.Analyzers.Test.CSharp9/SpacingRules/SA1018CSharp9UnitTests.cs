// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1018NullableTypeSymbolsMustNotBePrecededBySpace,
        StyleCop.Analyzers.SpacingRules.SA1018CodeFixProvider>;

    public partial class SA1018CSharp9UnitTests : SA1018CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerNullableTypeSpacingAsync()
        {
            var testCode = @"#nullable enable

public class TestClass
{
    private unsafe delegate*<string {|#0:?|}, void> field1;
    private unsafe delegate*<string {|#1:?|}[], void> field2;
}
";

            var fixedCode = @"#nullable enable

public class TestClass
{
    private unsafe delegate*<string?, void> field1;
    private unsafe delegate*<string?[], void> field2;
}
";

            var expected = new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
