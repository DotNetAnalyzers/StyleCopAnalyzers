// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1629DocumentationTextMustEndWithAPeriod,
        StyleCop.Analyzers.DocumentationRules.SA1629CodeFixProvider>;

    public partial class SA1629CSharp9UnitTests : SA1629CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestTypeWithPrimaryConstructorSummaryWithoutPeriodAsync(string typeKeyword)
        {
            var testCode = $@"/// <summary>The type{{|#0:<|}}/summary>
public {typeKeyword} TestType(int X);";

            var fixedCode = $@"/// <summary>The type.</summary>
public {typeKeyword} TestType(int X);";

            var expected = this.GetExpectedResultTestTypeWithPrimaryConstructorSummaryWithoutPeriod();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestTypeWithPrimaryConstructorSummaryWithoutPeriod()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }
    }
}
