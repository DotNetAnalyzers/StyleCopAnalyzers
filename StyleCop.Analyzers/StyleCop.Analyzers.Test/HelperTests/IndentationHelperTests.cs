// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="IndentationHelper"/> class.
    /// </summary>
    public class IndentationHelperTests
    {
        public static readonly IEnumerable<object[]> IndentationTestData = new[]
        {
            // no indentation
            new object[] { string.Empty, 0, 4, 4 },

            // 1 space
            new object[] { " ", 0, 4, 4 },

            // 2 spaces
            new object[] { "  ", 1, 4, 4 },

            // 3 spaces
            new object[] { "   ", 1, 4, 4 },

            // 3 spaces, indentation size = 2
            new object[] { "   ", 2, 2, 4 },

            // 4 spaces
            new object[] { "    ", 1, 4, 4 },

            // 5 spaces
            new object[] { "     ", 1, 4, 4 },

            // 6 spaces
            new object[] { "      ", 2, 4, 4 },

            // 7 spaces
            new object[] { "       ", 2, 4, 4 },

            // 8 spaces
            new object[] { "        ", 2, 4, 4 },

            // 8 spaces indentation, indentation size = 2
            new object[] { "        ", 4, 2, 4 },

            // 9 spaces indentation, indentation size = 3
            new object[] { "         ", 3, 3, 4 },

            // single tab
            new object[] { "\t", 1, 4, 4 },

            // multiple tabs
            new object[] { "\t\t\t", 3, 4, 4 },

            // multiple tabs (difference between indentation size and tab size)
            new object[] { "\t\t\t", 6, 3, 6 },

            // spaces + tabs mix (regression for #1036)
            new object[] { " \t \t \t \t", 4, 4, 4 },

            // 1 space followed by a tab
            new object[] { " \t", 1, 4, 4 },

            // 2 spaces followed by a tab
            new object[] { "  \t", 1, 4, 4 },

            // 3 spaces followed by a tab
            new object[] { "   \t", 1, 4, 4 },

            // 4 spaces followed by a tab
            new object[] { "    \t", 2, 4, 4 },

            // tab followed by 1 space
            new object[] { "\t ", 1, 4, 4 },

            // tab followed by 2 space
            new object[] { "\t  ", 2, 4, 4 },

            // tab followed by 3 spaces
            new object[] { "\t   ", 2, 4, 4 },

            // tab followed by 4 spaces
            new object[] { "\t    ", 2, 4, 4 },
        };

        private const string TestProjectName = "TestProject";
        private const string TestFilename = "Test0.cs";

        /// <summary>
        /// Verify the workings of <see cref="IndentationHelper.GetIndentationSteps(IndentationSettings, SyntaxToken)"/>.
        /// </summary>
        /// <param name="indentationString">The indentation string to use with the test.</param>
        /// <param name="expectedIndentationSteps">The expected number of indentation steps.</param>
        /// <param name="indentationSize">The indentation size to use.</param>
        /// <param name="tabSize">The tab size to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(IndentationTestData))]
        public async Task VerifyGetIndentationStepsAsync(string indentationString, int expectedIndentationSteps, int indentationSize, int tabSize)
        {
            var testSource = $"{indentationString}public class TestClass {{}}";
            var document = await CreateTestDocumentAsync(testSource, indentationSize, false, tabSize, CancellationToken.None).ConfigureAwait(false);
            StyleCopSettings settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, CancellationToken.None);

            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
            var firstToken = syntaxRoot.GetFirstToken();

            Assert.Equal(expectedIndentationSteps, IndentationHelper.GetIndentationSteps(settings.Indentation, firstToken));
        }

        /// <summary>
        /// Verify the that <see cref="IndentationHelper.GetIndentationSteps(IndentationSettings, SyntaxToken)"/> will
        /// return zero (0) for tokens that are not the first token on a line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyGetIndentationStepsForTokenNotAtStartOfLineAsync()
        {
            var testSource = "    public class TestClass {}";
            var document = await CreateTestDocumentAsync(testSource, cancellationToken: CancellationToken.None).ConfigureAwait(false);
            StyleCopSettings settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, CancellationToken.None);

            var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
            var secondToken = syntaxRoot.GetFirstToken().GetNextToken();

            Assert.Equal(0, IndentationHelper.GetIndentationSteps(settings.Indentation, secondToken));
        }

        private static async Task<Document> CreateTestDocumentAsync(string source, int indentationSize = 4, bool useTabs = false, int tabSize = 4, CancellationToken cancellationToken = default)
        {
            var workspace = GenericAnalyzerTest.CreateWorkspace();
            workspace.Options = workspace.Options
                .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, indentationSize)
                .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, useTabs)
                .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, tabSize);

            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);
            var references = await GenericAnalyzerTest.ReferenceAssemblies.ResolveAsync(LanguageNames.CSharp, cancellationToken).ConfigureAwait(false);

            var solution = workspace.CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .WithProjectCompilationOptions(projectId, compilationOptions)
                .AddMetadataReferences(projectId, references)
                .AddDocument(documentId, TestFilename, SourceText.From(source));

            StyleCopSettings defaultSettings = new StyleCopSettings();
            if (indentationSize != defaultSettings.Indentation.IndentationSize
                || useTabs != defaultSettings.Indentation.UseTabs
                || tabSize != defaultSettings.Indentation.TabSize)
            {
                string settings = $@"
{{
  ""settings"": {{
    ""indentation"": {{
      ""indentationSize"": {indentationSize},
      ""useTabs"": {useTabs.ToString().ToLowerInvariant()},
      ""tabSize"": {tabSize}
    }}
  }}
}}
";

                solution = solution.AddAdditionalDocument(documentId, SettingsHelper.SettingsFileName, settings);
            }

            return solution.GetDocument(documentId);
        }
    }
}
