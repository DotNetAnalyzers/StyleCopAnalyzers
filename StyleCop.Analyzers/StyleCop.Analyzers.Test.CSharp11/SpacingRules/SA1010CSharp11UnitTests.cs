// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp11UnitTests : SA1010CSharp10UnitTests
    {
        [Theory]
        [InlineData("x is [1]")]
        [InlineData("x is not [1]")]
        [InlineData("x is ([1] or [2])")]
        [InlineData("x is ([1] or not [2])")]
        [InlineData("x is ([1] and [1])")]
        [InlineData("x is ([1] and not [2])")]
        [WorkItem(3503, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3503")]
        public async Task TestListPatternAsync(string condition)
        {
            var testCode = $@"
using System.Collections.Generic;

namespace TestNamespace
{{
    public class TestClass
    {{
        public void TestMethod(List<int> x)
        {{
            _ = {condition};
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
