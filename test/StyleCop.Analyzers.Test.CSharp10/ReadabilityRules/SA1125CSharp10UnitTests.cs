// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1125UseShorthandForNullableTypes>;

    public partial class SA1125CSharp10UnitTests : SA1125CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestExplicitLambdaNullableTypesAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        var f = {|#0:Nullable<int>|} () => 0;
        var g = ({|#1:Nullable<int>|} value) => 0;
        var h = {|#2:System.Nullable<int>|} ({|#3:System.Nullable<int>|} x) => 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(
                testCode,
                new[]
                {
                    Diagnostic().WithLocation(0),
                    Diagnostic().WithLocation(1),
                    Diagnostic().WithLocation(2),
                    Diagnostic().WithLocation(3),
                },
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
