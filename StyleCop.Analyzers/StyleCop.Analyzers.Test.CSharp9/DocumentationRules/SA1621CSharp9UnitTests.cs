// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer>;

    public partial class SA1621CSharp9UnitTests : SA1621CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestRecordWithEmptyTypeParameterNameAsync(string keyword)
        {
            var testCode = $@"/// <summary>
/// A record.
/// </summary>
/// <typeparam name=""T"">The type.</typeparam>
/// <typeparam name="""">Another type.</typeparam>
public {keyword} TestRecord<T>
{{
}}
";

            var expected = Diagnostic(GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(5, 22);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestGenericTypeWithPrimaryConstructorTypeParameterWithoutNameAsync(string typeKeyword)
        {
            var testCode = $@"/// <summary>The type.</summary>
/// {{|#0:<typeparam>The type.</typeparam>|}}
public {typeKeyword} TestType<T>(T X);";

            var expected = this.GetExpectedResultTestGenericTypeWithPrimaryConstructorTypeParameterWithoutName();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestGenericTypeWithPrimaryConstructorTypeParameterWithoutName()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic(StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(0),
                Diagnostic(StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer.SA1621Descriptor).WithLocation(0),
            };
        }
    }
}
