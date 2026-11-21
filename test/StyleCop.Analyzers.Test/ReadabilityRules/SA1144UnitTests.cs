// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1144ZeroWidthCharactersInLiteralsMustBeEscaped,
        StyleCop.Analyzers.ReadabilityRules.SA1144CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for SA1144.
    /// </summary>
    /// <seealso cref="SA1144ZeroWidthCharactersInLiteralsMustBeEscaped" />
    /// <seealso cref="SA1144CodeFixProvider" />
    public class SA1144UnitTests
    {
        private const string ZeroWidthSpace = "\u200B";

        /// <summary>
        /// Verifies that literals without zero-width characters, and zero-width joiners, are not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLiteralsWithoutZeroWidthCharactersAsync()
        {
            var testCode = @"
class C
{
    string A = ""ab"";
    string B = ""a\u200Bb"";
    string C2 = ""a" + "\u200D" + @"b"" + $""x{1}y"";
    char D = 'a';
    // a" + ZeroWidthSpace + @"b
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that string, character and interpolated string literals are reported and fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEscapableLiteralsAsync()
        {
            var testCode = @"
class C
{
    string A = {|#0:""a" + ZeroWidthSpace + @"b""|};
    char B = {|#1:'" + ZeroWidthSpace + @"'|};
    string D = $""{|#2:x" + ZeroWidthSpace + @"|}{1}{|#3:" + "\u2060" + @"y|}"";
}
";
            var fixedCode = @"
class C
{
    string A = ""a\u200Bb"";
    char B = '\u200B';
    string D = $""x\u200B{1}\u2060y"";
}
";

            await VerifyCSharpFixAsync(testCode, new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2), Diagnostic().WithLocation(3) }, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a literal with several zero-width characters is reported once and fully fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleCharactersInOneLiteralAsync()
        {
            var testCode = @"
class C
{
    string A = {|#0:""" + ZeroWidthSpace + "\uFEFF" + ZeroWidthSpace + @"""|};
}
";
            var fixedCode = @"
class C
{
    string A = ""\u200B\uFEFF\u200B"";
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the removal code fix applies to every kind of string literal, and not to character literals.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRemoveCodeFixAsync()
        {
            var testCode = @"
class C
{
    string A = {|#0:""a" + ZeroWidthSpace + "\uFEFF" + @"b""|};
    string B = {|#1:@""a" + ZeroWidthSpace + @"b""|};
    string D = $@""{|#2:a" + ZeroWidthSpace + @"|}{1}"";
}
";
            var fixedCode = @"
class C
{
    string A = ""ab"";
    string B = @""ab"";
    string D = $@""a{1}"";
}
";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CodeActionEquivalenceKey = nameof(SA1144CodeFixProvider) + "Remove",
            };

            test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2) });
            await test.RunAsync().ConfigureAwait(false);
        }
    }
}
