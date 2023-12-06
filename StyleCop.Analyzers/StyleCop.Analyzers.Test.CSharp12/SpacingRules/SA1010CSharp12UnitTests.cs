// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp12UnitTests : SA1010CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3687, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3687")]
        public async Task TestCollectionExpressionAsync()
        {
            var testCode = $@"
using System.Collections.Generic;

namespace TestNamespace
{{
    public class TestClass
    {{
        protected static readonly int[] DefaultMetadataPaths = [1, 2];

        public void TestMethod(List<int> x, int y)
        {{
            List<int> foo = [];
            foo = [42];
            foo = [..x, y];
            Bar([1 ,2, 3]);
        }}
        
        public void Bar(int[] x)
        {{
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
