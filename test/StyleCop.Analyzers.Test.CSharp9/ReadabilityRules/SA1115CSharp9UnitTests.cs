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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1115ParameterMustFollowComma>;

    public partial class SA1115CSharp9UnitTests : SA1115CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticAnonymousFunctionParameterAfterBlankLineAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int, int> func = static (int first,

            {|#0:int second|}) => first + second;
    }
}
";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorEmptyLineBetweenParametersAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(
    int a,

    {{|#0:int b|}})
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorEmptyLineBetweenParameters();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorSecondParameterOnTheNextLineAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(
    int a,
    int b)
{{
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorBaseListEmptyLineBetweenArgumentsAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int a, int b)
{{
}}

{typeKeyword} Bar(int a, int b) : Foo(
    a,

    {{|#0:b|}})
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorBaseListEmptyLineBetweenArguments();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorBaseListSecondArgumentOnTheNextLineAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int a, int b)
{{
}}

{typeKeyword} Bar(int a, int b) : Foo(
    a,
    b)
{{
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorEmptyLineBetweenParameters()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseListEmptyLineBetweenArguments()
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
