// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public class SA1137CSharp7UnitTests : SA1137UnitTests
    {
        [Fact]
        public async Task TestTupleTypeAsync()
        {
            string testCode = @"
class Container
{
    (
        int x,
      int y,
int z) NonZeroAlignment;

    (
int x,
      int y,
        int z) ZeroAlignment;
}
";
            string fixedCode = @"
class Container
{
    (
        int x,
        int y,
        int z) NonZeroAlignment;

    (
int x,
int y,
int z) ZeroAlignment;
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(12, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTupleExpressionAsync()
        {
            string testCode = @"
class Container
{
    (int x, int y, int z) NonZeroAlignment = (
        0,
      0,
0);

    (int x, int y, int z) ZeroAlignment = (
0,
      0,
        0);
}
";
            string fixedCode = @"
class Container
{
    (int x, int y, int z) NonZeroAlignment = (
        0,
        0,
        0);

    (int x, int y, int z) ZeroAlignment = (
0,
0,
0);
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(11, 1),
                Diagnostic().WithLocation(12, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
