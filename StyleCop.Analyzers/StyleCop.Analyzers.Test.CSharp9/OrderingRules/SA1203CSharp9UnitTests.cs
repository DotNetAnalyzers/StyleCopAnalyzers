// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1203ConstantsMustAppearBeforeFields,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1203CSharp9UnitTests : SA1203CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task TestRecordConstantAfterFieldAsync(string keyword)
        {
            var testCode = $@"public {keyword} TestRecord
{{
    public static int field;
    public const int {{|#0:Constant|}} = 1;
}}
";

            var fixedCode = $@"public {keyword} TestRecord
{{
    public const int Constant = 1;
    public static int field;
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestMemberOrderInTypeWithPrimaryConstructorAsync(string typeKeyword)
        {
            var testCode = $@"public {typeKeyword} TestType(int X)
{{
    public int Field = 0;

    public const int {{|#0:Constant|}} = 1;
}}";

            var fixedCode = $@"public {typeKeyword} TestType(int X)
{{
    public const int Constant = 1;

    public int Field = 0;
}}";

            var expected = this.GetExpectedResultTestMemberOrderInTypeWithPrimaryConstructor();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestMemberOrderInTypeWithPrimaryConstructor()
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
