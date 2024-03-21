// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1513ClosingBraceMustBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1513CodeFixProvider>;

    public partial class SA1513CSharp12UnitTests : SA1513CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3720, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3720")]
        public async Task TestObjectInitializerInCollectionExpressionAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo[] TestMethod() =>
    [
        new Foo
        {
        }
    ];
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
