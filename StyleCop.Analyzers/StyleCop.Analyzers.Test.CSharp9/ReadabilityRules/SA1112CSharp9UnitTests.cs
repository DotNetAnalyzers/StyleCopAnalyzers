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
        StyleCop.Analyzers.ReadabilityRules.SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1112CSharp9UnitTests : SA1112CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3972, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3972")]
        public async Task TestTargetTypedNewClosingParenthesisAsync()
        {
            var testCode = @"
struct TestStruct
{
}

class Test
{
    void M()
    {
        TestStruct value = new(
            {|#0:)|};
    }
}";

            var fixedCode = @"
struct TestStruct
{
}

class Test
{
    void M()
    {
        TestStruct value = new();
    }
}";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticAnonymousMethodClosingParenthesisOnNextLineAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Action action = static delegate(
            {|#0:)|}
            {
            };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorWithNoParametersClosingParenthesisOnTheNextLineAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(
{{|#0:)|}}
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo()
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorWithNoParametersClosingParenthesisOnTheNextLine();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorWithNoParametersClosingParenthesisOnTheNextLine()
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
