// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1012OpeningBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1012CSharp9UnitTests : SA1012CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3812, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3812")]
        public async Task TestInAndPropertyPatternsAsync()
        {
            var testCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ( {|#0:{|}Length: < 5 } and { Length: < 5 });
    }
}
";

            var fixedCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ({ Length: < 5 } and { Length: < 5 });
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                // SA1012: Opening brace should be followed by a space
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed"),

                // SA1012: Opening brace should not be preceded by a space
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(
                testCode,
                expectedResults,
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3812, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3812")]
        public async Task TestInOrPropertyPatternsAsync()
        {
            var testCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ( {|#0:{|}Length: < 5 } or { Length: < 5 });
    }
}
";

            var fixedCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ({ Length: < 5 } or { Length: < 5 });
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                // SA1012: Opening brace should be followed by a space
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed"),

                // SA1012: Opening brace should not be preceded by a space
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(
                testCode,
                expectedResults,
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3812, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3812")]
        public async Task TestInParenthesizedPropertyPatternsAsync()
        {
            var testCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ( {|#0:{|}Length: < 5 });
    }
}
";

            var fixedCode = @"
class C
{
    public static bool HasValidLength(string s)
    {
        return s is ({ Length: < 5 });
    }
}
";

            DiagnosticResult[] expectedResults =
            {
                // SA1012: Opening brace should be followed by a space
                Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed"),

                // SA1012: Opening brace should not be preceded by a space
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(
                testCode,
                expectedResults,
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
