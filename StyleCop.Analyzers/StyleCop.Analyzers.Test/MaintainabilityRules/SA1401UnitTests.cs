// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.MaintainabilityRules.SA1401FieldsMustBePrivate>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1401FieldsMustBePrivate"/>.
    /// </summary>
    public class SA1401UnitTests
    {
        [Fact]
        public async Task TestClassWithPublicFieldAsync()
        {
            var testCode = @"public class Foo
{
    public string bar;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 19);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithInternalFieldAsync()
        {
            var testCode = @"public class Foo
{
    internal string bar;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 21);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithFieldNoAccessModifierAsync()
        {
            var testCode = @"public class Foo
{
    string bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructWithPublicFieldAsync()
        {
            var testCode = @"public struct Foo
{
    public string bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithConstFieldAsync()
        {
            var testCode = @"public class Foo
{
    public const string bar = ""qwe"";
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        public async Task TestClassWithStaticReadonlyFieldAsync(string accessModifier)
        {
            var testCode = $@"public class TestClass
{{
    {accessModifier} static readonly string TestField = ""qwe"";
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
