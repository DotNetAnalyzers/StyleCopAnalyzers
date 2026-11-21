// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1144ZeroWidthCharactersInLiteralsMustBeEscaped,
        StyleCop.Analyzers.ReadabilityRules.SA1144CodeFixProvider>;

    public partial class SA1144CSharp11UnitTests : SA1144CSharp10UnitTests
    {
        private const string ZeroWidthSpace = "\u200B";

        /// <summary>
        /// Verifies that UTF-8 string literals are reported and fixed, and that escaping is not offered for verbatim
        /// UTF-8 strings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestUtf8StringLiteralsAsync()
        {
            var testCode = @"
class C
{
    byte[] A = {|#0:""a" + ZeroWidthSpace + @"b""u8|}.ToArray();
}
";
            var fixedCode = @"
class C
{
    byte[] A = ""a\u200Bb""u8.ToArray();
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp11, testCode, Diagnostic().WithLocation(0), fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that raw string literals are reported, but neither escaping nor removal is offered for them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRawStringLiteralsAreReportedWithoutFixAsync()
        {
            var testCode = @"
class C
{
    string A = {|#0:""""""a" + ZeroWidthSpace + @"b""""""|};
    string B = {|#1:"""""" " + ZeroWidthSpace + @" """"""|};
    string D = {|#2:""""""
        a" + ZeroWidthSpace + @"b
        """"""|};
    string E = $""""""{|#3:a" + ZeroWidthSpace + @"b|}{1}"""""";
}
";

            await VerifyCSharpFixAsync(
                LanguageVersion.CSharp11,
                testCode,
                new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2), Diagnostic().WithLocation(3) },
                testCode).ConfigureAwait(false);
        }
    }
}
