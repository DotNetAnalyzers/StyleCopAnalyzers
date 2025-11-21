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
        StyleCop.Analyzers.LayoutRules.SA1515SingleLineCommentMustBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1515CodeFixProvider>;

    public partial class SA1515CSharp12UnitTests : SA1515CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3766, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3766")]
        public async Task TestFirstInCollectionExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    private string[] elements =
    [
      // Hydrogen
      ""H"",

      // Helium
      ""He"",
    ];
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
