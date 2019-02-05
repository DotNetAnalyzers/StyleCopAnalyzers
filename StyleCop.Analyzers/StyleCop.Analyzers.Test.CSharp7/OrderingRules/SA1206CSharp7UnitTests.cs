// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public class SA1206CSharp7UnitTests : SA1206UnitTests
    {
        [Theory]
        [InlineData("readonly")]
        [InlineData("ref")]
        [InlineData("readonly ref")]
        [InlineData("readonly partial")]
        [InlineData("ref partial")]
        [InlineData("readonly ref partial")]
        [WorkItem(2578, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2578")]
        public async Task TestReadonlyRefKeywordInStructDeclarationAsync(string keywords)
        {
            var testCode = $@"class OuterClass
{{
    private {keywords} struct BitHelper
    {{
    }}
}}
";
            await VerifyCSharpDiagnosticAsync(LanguageVersion.Latest, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReadonlyKeywordInStructDeclarationWrongOrderAsync()
        {
            // Note that we don't need a test for ref with the wrong order, because this would be a compile time error
            var testCode = @"class OuterClass
{
    readonly private struct BitHelper
    {
    }
}
";

            DiagnosticResult[] expected = new[]
            {
                Diagnostic().WithLocation(3, 14).WithArguments("private", "readonly"),
            };
            await VerifyCSharpDiagnosticAsync(LanguageVersion.Latest, testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
