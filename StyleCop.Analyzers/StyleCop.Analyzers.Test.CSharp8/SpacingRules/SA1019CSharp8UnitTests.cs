// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1019CSharp8UnitTests : SA1019CSharp7UnitTests
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
            string s1 = arguments[0] !.ToString();
            string s2 = arguments[0]! .ToString();
            string s3 = arguments[0] ! .ToString();
            string s4 = arguments[0] !?.ToString();
            string s5 = arguments[0]! ?.ToString();
            string s6 = arguments[0] ! ?.ToString();
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
            string s1 = arguments[0] !.ToString();
            string s2 = arguments[0]!.ToString();
            string s3 = arguments[0] !.ToString();
            string s4 = arguments[0] !?.ToString();
            string s5 = arguments[0]!?.ToString();
            string s6 = arguments[0] !?.ToString();
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithArguments(".").WithLocation(8, 39),
                Diagnostic(DescriptorNotPreceded).WithArguments(".").WithLocation(9, 40),
                Diagnostic(DescriptorNotPreceded).WithArguments("?").WithLocation(11, 39),
                Diagnostic(DescriptorNotPreceded).WithArguments("?").WithLocation(12, 40),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
