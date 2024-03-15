// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1110OpeningParenthesisMustBeOnDeclarationLine,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1110CSharp9UnitTests : SA1110CSharp8UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(3784, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3784")]
        public async Task TestPrimaryConstructorWithoutParametersAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo
    {{|#0:(|}})
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo()
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructor();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(3784, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3784")]
        public async Task TestPrimaryConstructorWithParametersAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo
    {{|#0:(|}}int x)
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo(
    int x)
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructor();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(3784, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3784")]
        public async Task TestPrimaryConstructorBaseListWithParametersOnSameLineAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int x)
{{
}}

{typeKeyword} Bar(int x) : Foo(x)
{{
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(3784, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3784")]
        public async Task TestPrimaryConstructorBaseListWithParametersAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int x)
{{
}}

{typeKeyword} Bar(int x) : Foo
    {{|#0:(|}}x)
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo(int x)
{{
}}

{typeKeyword} Bar(int x) : Foo(
    x)
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorBaseList();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(3784, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3784")]
        public async Task TestPrimaryConstructorBaseListWithoutParametersAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo()
{{
}}

{typeKeyword} Bar(int x) : Foo
    {{|#0:(|}})
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo()
{{
}}

{typeKeyword} Bar(int x) : Foo()
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorBaseList();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructor()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseList()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/70488
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }
    }
}
