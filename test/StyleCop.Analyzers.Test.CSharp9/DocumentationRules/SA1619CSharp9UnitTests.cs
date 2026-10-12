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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1619GenericTypeParametersMustBeDocumentedPartialClass>;

    public partial class SA1619CSharp9UnitTests : SA1619CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestGenericPartialTypeWithPrimaryConstructorWithoutTypeParameterDocumentationAsync(string typeKeyword)
        {
            var testCode = $@"/// <summary>The type.</summary>
public partial {typeKeyword} TestType<{{|#0:T|}}>(T X);";

            var expected = this.GetExpectedResultTestGenericPartialTypeWithPrimaryConstructorWithoutTypeParameterDocumentation();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestGenericPartialTypeWithPrimaryConstructorWithoutTypeParameterDocumentation()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0).WithArguments("T"),
                Diagnostic().WithLocation(0).WithArguments("T"),
            };
        }
    }
}
