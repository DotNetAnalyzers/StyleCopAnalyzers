// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1312CSharp11UnitTests : SA1312CSharp10UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedRefLocalStartingWithUpperCaseLetterAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Bar()
    {
        int value = 5;
        scoped ref int {|#0:Bar|} = ref value;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void Bar()
    {
        int value = 5;
        scoped ref int bar = ref value;
    }
}
";

            var expected = Diagnostic().WithArguments("Bar").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
