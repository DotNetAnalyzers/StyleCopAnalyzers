// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1313CSharp11UnitTests : SA1313CSharp10UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedParameterStartingWithUpperCaseLetterAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Bar(scoped System.Span<int> {|#0:Value|})
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void Bar(scoped System.Span<int> value)
    {
    }
}
";

            var expected = Diagnostic().WithArguments("Value").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
