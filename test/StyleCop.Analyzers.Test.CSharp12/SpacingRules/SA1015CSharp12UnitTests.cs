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
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp12UnitTests : SA1015CSharp11UnitTests
    {
        [Theory]
        [InlineData(" M<int> ")]
        [InlineData("M<int>")]
        [WorkItem(3856, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3856")]
        public async Task TestSpacingAfterGenericMethodGroupInCollectionExpressionAsync(string item)
        {
            var testCode = $@"
using System;
using System.Collections.Generic;

public class TestClass
{{
    private List<Action> values = [{item}];
    
    private static void M<T>()
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultMissingToken()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithLocation(7, 35).WithArguments(","),
                DiagnosticResult.CompilerError("CS1003").WithLocation(7, 36).WithArguments(">"),
                DiagnosticResult.CompilerError("CS1026").WithLocation(7, 36),
            };
        }
    }
}
