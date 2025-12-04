// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1408ConditionalExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    public partial class SA1408CSharp9UnitTests : SA1408CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestLogicalPatternsDeclarePrecedenceAsync()
        {
            const string testCode = @"
class C
{
    bool M(int value) => value is {|#0:> 0 and < 5|} or 10;
}";
            const string fixedCode = @"
class C
{
    bool M(int value) => value is (> 0 and < 5) or 10;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestPatternAndWithLogicalOrIsIgnoredAsync()
        {
            const string testCode = @"
class C
{
    bool M(int value, bool flag) => flag || value is > 0 and < 5;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestPatternOrWithLogicalAndIsIgnoredAsync()
        {
            const string testCode = @"
class C
{
    bool M(int value, bool flag) => flag && value is > 0 or < 5;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
