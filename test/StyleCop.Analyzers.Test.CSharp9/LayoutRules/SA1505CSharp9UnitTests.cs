// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    public partial class SA1505CSharp9UnitTests : SA1505CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3272, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3272")]
        public async Task TestSingleLineRecordAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public record TestRecord;
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3978, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3978")]
        public async Task TestLocalFunctionWithAttributeFollowedByBlankLineAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local()
        {|#0:{|}

            int value = 0;
        }
    }
}
";

            var fixedCode = @"using System;

class TestClass
{
    void Outer()
    {
        [Obsolete]
        void Local()
        {
            int value = 0;
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestBlankLineAfterOpeningBraceInTypeWithPrimaryConstructorAsync(string typeKeyword)
        {
            var testCode = $@"public {typeKeyword} TestType(int X)
{{|#0:{{|}}

    public int Property => 0;
}}";

            var fixedCode = $@"public {typeKeyword} TestType(int X)
{{
    public int Property => 0;
}}";

            var expected = this.GetExpectedResultTestBlankLineAfterOpeningBraceInTypeWithPrimaryConstructor();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestBlankLineAfterOpeningBraceInTypeWithPrimaryConstructor()
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
