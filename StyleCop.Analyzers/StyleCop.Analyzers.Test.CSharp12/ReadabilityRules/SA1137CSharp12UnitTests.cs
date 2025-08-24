// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public partial class SA1137CSharp12UnitTests : SA1137CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestSingleLineCollectionExpressionAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField = [1, 2, 3];
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestMultiLineCollectionExpressionStartingOnPreviousLineAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField =
    [
        1,
[|         |]2,
[|       |]3
[|     |]];
}
";

            string fixedCode = @"
class TestClass
{
    private int[] testField =
    [
        1,
        2,
        3
    ];
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestMultiLineCollectionExpressionStartingOnNextLineAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField = [
        1,
[|         |]2,
[|       |]3
    ];
}
";

            string fixedCode = @"
class TestClass
{
    private int[] testField = [
        1,
        2,
        3
    ];
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
