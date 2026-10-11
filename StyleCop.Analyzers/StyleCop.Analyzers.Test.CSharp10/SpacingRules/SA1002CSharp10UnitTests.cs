// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1002SemicolonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1002CSharp10UnitTests : SA1002CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3983, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3983")]
        public async Task TestFileScopedNamespaceDeclarationAsync()
        {
            var testCode = @"namespace Foo {|#0:;|}
";

            var fixedCode = @"namespace Foo;
";

            var expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
