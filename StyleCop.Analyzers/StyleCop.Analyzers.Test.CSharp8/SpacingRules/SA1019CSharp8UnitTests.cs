// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        [Fact]
        [WorkItem(3008, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3008")]
        public async Task TestMemberAccessAfterIndexAndRangeExpressionsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(string[] values)
        {
            var value1 = values[^1] {|#0:.|}ToString();
            var value2 = values[^1]{|#1:.|} ToString();
            var value3 = values[1..^1] {|#2:.|}ToString();
            var value4 = values[1..^1]{|#3:.|} ToString();
            var value5 = values[^1] {|#4:?|}.ToString();
            var value6 = values[^1]?{|#5:.|} ToString();
            var value7 = values[1..^1] {|#6:?|}.ToString();
            var value8 = values[1..^1]?{|#7:.|} ToString();
        }
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(string[] values)
        {
            var value1 = values[^1].ToString();
            var value2 = values[^1].ToString();
            var value3 = values[1..^1].ToString();
            var value4 = values[1..^1].ToString();
            var value5 = values[^1]?.ToString();
            var value6 = values[^1]?.ToString();
            var value7 = values[1..^1]?.ToString();
            var value8 = values[1..^1]?.ToString();
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithArguments('.').WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithArguments('.').WithLocation(1),
                Diagnostic(DescriptorNotPreceded).WithArguments('.').WithLocation(2),
                Diagnostic(DescriptorNotFollowed).WithArguments('.').WithLocation(3),
                Diagnostic(DescriptorNotPreceded).WithArguments('?').WithLocation(4),
                Diagnostic(DescriptorNotFollowed).WithArguments('.').WithLocation(5),
                Diagnostic(DescriptorNotPreceded).WithArguments('?').WithLocation(6),
                Diagnostic(DescriptorNotFollowed).WithArguments('.').WithLocation(7),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
