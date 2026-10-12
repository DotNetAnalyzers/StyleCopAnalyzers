// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp8UnitTests : SA1010CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3008, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3008")]
        public async Task TestIndexAndRangeOpenBracketSpacingAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            _ = values {|#0:[|}^1];
            _ = values{|#1:[|} ^1];
            _ = values {|#2:[|} ^1];
            _ = values {|#3:[|}1..^2];
            _ = values{|#4:[|} 1..^2];
            _ = values[..];
            _ = values{|#5:[|} ..];
        }
    }
}
";
            var fixedCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            _ = values[^1];
            _ = values[^1];
            _ = values[^1];
            _ = values[1..^2];
            _ = values[1..^2];
            _ = values[..];
            _ = values[..];
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
                Diagnostic(DescriptorNotPreceded).WithLocation(2),
                Diagnostic(DescriptorNotFollowed).WithLocation(2),
                Diagnostic(DescriptorNotPreceded).WithLocation(3),
                Diagnostic(DescriptorNotFollowed).WithLocation(4),
                Diagnostic(DescriptorNotFollowed).WithLocation(5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
