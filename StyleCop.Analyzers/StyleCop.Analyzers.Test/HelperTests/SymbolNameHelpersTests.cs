// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SymbolNameHelpers"/> class.
    /// </summary>
    public class SymbolNameHelpersTests : IAsyncLifetime
    {
        private const string TestProjectName = "TestProject";
        private const string TestFilename = "Test.cs";

        private Solution testSolution;

        public async Task InitializeAsync()
        {
            var compilationOptions = this.GetCompilationOptions();
            this.testSolution = await CreateTestSolutionAsync(compilationOptions).ConfigureAwait(false);
        }

        public Task DisposeAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verify the workings of <see cref="SymbolNameHelpers.ToQualifiedString(ISymbol, NameSyntax)"/>
        /// for standard use cases.
        /// </summary>
        /// <param name="inputString">A string representation of a type or namespace to process.</param>
        /// <param name="isNamespace"><see langword="true"/> if <paramref name="inputString"/> is a namespace;
        /// <see langword="false"/> if <paramref name="inputString"/> is a type to be used as an alias target.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.ValueTuple<int, object>")]
        [InlineData("System.ValueTuple<System.Int32, System.Object>")]
        [InlineData("System.ValueTuple<System.Int32?, System.Object>")]
        [InlineData("System.ValueTuple<int, object[]>")]
        [InlineData("System.ValueTuple<int?, object>")]
        [InlineData("System.ValueTuple<int[], object>")]
        [InlineData("System.ValueTuple<int?[], object>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int?, object>")]
        [InlineData("System.Collections.Generic.KeyValuePair<int, object[]>")]
        [InlineData("System.Nullable<int>")]
        [InlineData("System", true)]
        [InlineData("System.Collections.Generic", true)]
        [WorkItem(3149, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3149")]
        public Task VerifyToQualifiedStringAsync(string inputString, bool isNamespace = false)
        {
            return this.PerformTestAsync(inputString, isNamespace);
        }

        /// <summary>
        /// Performs the actual testing work.
        /// </summary>
        /// <param name="inputString">A string representation of a type or namespace to process.</param>
        /// <param name="isNamespace"><see langword="true"/> if <paramref name="inputString"/> is a namespace;
        /// <see langword="false"/> if <paramref name="inputString"/> is a type to be used as an alias target.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        protected async Task PerformTestAsync(string inputString, bool isNamespace)
        {
            var fileContent = isNamespace ? "using " + inputString : "using AliasType = " + inputString;
            fileContent += ";";

            var document = GetDocument(this.testSolution, fileContent);
            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            var usingDirectiveSyntax = syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().First();
            var typeSymbol = semanticModel.GetSymbolInfo(usingDirectiveSyntax.Name).Symbol;

            var resultString = typeSymbol.ToQualifiedString(usingDirectiveSyntax.Name);
            Assert.Equal(inputString, resultString);
        }

        /// <summary>
        /// When overridden in derived classes, provides a <see cref="CSharpCompilationOptions"/> instance
        /// to be used for the test project in the workspace.
        /// </summary>
        /// <returns>An instance of <see cref="CSharpCompilationOptions"/> describing the project compilation options.</returns>
        protected virtual CSharpCompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }

        private static Document GetDocument(Solution solution, string sourceCode)
        {
            var projectId = solution.ProjectIds[0];
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, TestFilename, SourceText.From(sourceCode));
            return solution.GetDocument(documentId);
        }

        private static async Task<Solution> CreateTestSolutionAsync(CSharpCompilationOptions compilationOptions)
        {
            var workspace = GenericAnalyzerTest.CreateWorkspace();
            var projectId = ProjectId.CreateNewId();

            var references = await GenericAnalyzerTest.ReferenceAssemblies
                .ResolveAsync(LanguageNames.CSharp, CancellationToken.None).ConfigureAwait(false);

            var solution = workspace.CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, compilationOptions)
                .AddMetadataReferences(projectId, references);

            return solution;
        }
    }
}
