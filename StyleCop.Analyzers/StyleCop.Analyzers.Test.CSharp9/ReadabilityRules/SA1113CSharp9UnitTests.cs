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
        StyleCop.Analyzers.ReadabilityRules.SA1113CommaMustBeOnSameLineAsPreviousParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1113CSharp9UnitTests : SA1113CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3972, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3972")]
        public async Task TestTargetTypedNewCommaPlacementAsync()
        {
            var testCode = @"
class TestClass
{
    public TestClass(int first, int second)
    {
    }
}

class Test
{
    void M()
    {
        TestClass value = new(1
            {|#0:,|} 2);
    }
}";

            var fixedCode = @"
class TestClass
{
    public TestClass(int first, int second)
    {
    }
}

class Test
{
    void M()
    {
        TestClass value = new(1,
            2);
    }
}";

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticAnonymousFunctionCommaOnNextLineAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int, int> func = static (int first
            {|#0:,|} int second) => first + second;
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int, int> func = static (int first,
            int second) => first + second;
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorCommaPlacedAtTheSameLineAsTheSecondParameterAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int a
    {{|#0:,|}} int b)
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo(int a,
    int b)
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorCommaPlacedAtTheSameLineAsTheSecondParameter();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.ReferenceTypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorBaseListCommaPlacedAtTheSameLineAsTheSecondArgumentAsync(string typeKeyword)
        {
            var testCode = $@"
{typeKeyword} Foo(int a, int b)
{{
}}

{typeKeyword} Bar(int a, int b) : Foo(a
    {{|#0:,|}} b)
{{
}}";

            var fixedCode = $@"
{typeKeyword} Foo(int a, int b)
{{
}}

{typeKeyword} Bar(int a, int b) : Foo(a,
    b)
{{
}}";

            var expected = this.GetExpectedResultTestPrimaryConstructorBaseListCommaPlacedAtTheSameLineAsTheSecondArgument();
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestPrimaryConstructorBaseListCommaPlacedAtTheSameLineAsTheSecondArgument()
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
