// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1002SemicolonsMustBeSpacedCorrectly,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1002CSharp8UnitTests : SA1002CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3052, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3052")]
        public async Task TestClosingSquareBracketFollowedByExclamationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(object?[] arguments)
        {
            object o1 = arguments[0] !;
            object o2 = arguments[0]! ;
            object o3 = arguments[0] ! ;
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(object?[] arguments)
        {
            object o1 = arguments[0] !;
            object o2 = arguments[0]!;
            object o3 = arguments[0] !;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 39),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(9, 40),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
