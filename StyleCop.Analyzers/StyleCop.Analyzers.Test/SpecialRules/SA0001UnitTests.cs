// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpecialRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpecialRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.SpecialRules.SA0001XmlCommentAnalysisDisabled>;

    /// <summary>
    /// Unit tests for <see cref="SA0001XmlCommentAnalysisDisabled"/>.
    /// </summary>
    public class SA0001UnitTests
    {
        [Theory]
        [InlineData(DocumentationMode.Parse)]
        [InlineData(DocumentationMode.Diagnose)]
        public async Task TestEnabledDocumentationModesAsync(DocumentationMode documentationMode)
        {
            var testCode = @"public class Foo
{
}
";

            await new CSharpTest
            {
                TestState =
                {
                    DocumentationMode = documentationMode,
                    Sources = { testCode },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationMode.None)]
        public async Task TestDisabledDocumentationModesAsync(DocumentationMode documentationMode)
        {
            var testCode = @"public class Foo
{
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestState =
                {
                    DocumentationMode = documentationMode,
                    Sources = { testCode },
                    ExpectedDiagnostics = { expected },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
