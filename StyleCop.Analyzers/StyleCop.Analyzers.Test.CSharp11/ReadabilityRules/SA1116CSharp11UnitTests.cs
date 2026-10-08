// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public partial class SA1116CSharp11UnitTests : SA1116CSharp10UnitTests
    {
        [Fact]
        [WorkItem(1620, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1620")]
        public async Task TestMultiLineRawStringFirstArgumentAsync()
        {
            var testCode = @"
class Foo
{
    void Bar(string a, int b)
    {
        Bar({|#0:""""""
            line one
              line two
            """"""|},
            3);
    }
}";
            var fixedCode = @"
class Foo
{
    void Bar(string a, int b)
    {
        Bar(
            """"""
            line one
              line two
            """""",
            3);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1620, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1620")]
        public async Task TestMultiLineLambdaFirstArgumentContainingRawStringAsync()
        {
            var testCode = @"
class Foo
{
    void Bar(System.Action a, int b)
    {
        Bar({|#0:() =>
        {
            var s = """"""
                text
                """""";
        }|}, 1);
    }
}";
            var fixedCode = @"
class Foo
{
    void Bar(System.Action a, int b)
    {
        Bar(
            () =>
            {
                var s = """"""
                text
                """""";
            }, 1);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
