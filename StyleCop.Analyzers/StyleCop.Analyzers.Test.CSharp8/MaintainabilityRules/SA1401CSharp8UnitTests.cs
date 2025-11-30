// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1401FieldsMustBePrivate>;

    public partial class SA1401CSharp8UnitTests : SA1401CSharp7UnitTests
    {
        [Theory]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        public async Task TestClassWithReadonlyFieldAsync(string accessModifier)
        {
            var testCode = $@"public class Foo
{{
    {accessModifier} readonly string [|bar|];
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyStructMembersAreIgnoredAsync()
        {
            var testCode = @"
public struct S
{
    public readonly int Property { get; }
    public readonly void Method() { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
