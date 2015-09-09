// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1652EnableXmlDocumentationOutput"/>.
    /// </summary>
    public class SA1652UnitTests : DiagnosticVerifier
    {
        private DocumentationMode documentationMode;

        [Theory]
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
        [InlineData(DocumentationMode.Parse)]
        public async Task TestDisabledDocumentationModesAsync(DocumentationMode documentationMode)
        {
            var testCode = @"public class Foo
{
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(0, 0)
            };

            this.documentationMode = documentationMode;
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1652EnableXmlDocumentationOutput();
        }

        protected override Solution CreateSolution(ProjectId projectId, string language)
        {
            Solution solution = base.CreateSolution(projectId, language);
            Project project = solution.GetProject(projectId);

            return solution.WithProjectParseOptions(projectId, project.ParseOptions.WithDocumentationMode(this.documentationMode));
        }
    }
}
