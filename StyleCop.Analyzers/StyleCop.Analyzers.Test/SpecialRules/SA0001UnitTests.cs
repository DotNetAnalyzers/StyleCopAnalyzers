// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpecialRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.SpecialRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA0001XmlCommentAnalysisDisabled"/>.
    /// </summary>
    public class SA0001UnitTests : DiagnosticVerifier
    {
        private DocumentationMode documentationMode;

        [Theory]
        [InlineData(DocumentationMode.Parse)]
        [InlineData(DocumentationMode.Diagnose)]
        public async Task TestEnabledDocumentationModesAsync(DocumentationMode documentationMode)
        {
            var testCode = @"public class Foo
{
}
";

            this.documentationMode = documentationMode;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic();

            this.documentationMode = documentationMode;
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA0001XmlCommentAnalysisDisabled();
        }

        protected override Solution CreateSolution(ProjectId projectId, string language)
        {
            Solution solution = base.CreateSolution(projectId, language);
            Project project = solution.GetProject(projectId);

            return solution.WithProjectParseOptions(projectId, project.ParseOptions.WithDocumentationMode(this.documentationMode));
        }
    }
}
