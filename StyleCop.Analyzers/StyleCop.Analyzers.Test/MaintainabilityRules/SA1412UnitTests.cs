// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1412UnitTests : CodeFixVerifier
    {
        private Encoding fileEncoding;

        public static IEnumerable<object[]> NonUtf8Encodings
        {
            get
            {
                yield return new object[] { Encoding.ASCII.CodePage };
                yield return new object[] { Encoding.BigEndianUnicode.CodePage };
                yield return new object[] { Encoding.Default.CodePage };
                yield return new object[] { Encoding.Unicode.CodePage };
                yield return new object[] { Encoding.UTF32.CodePage };
                yield return new object[] { Encoding.UTF7.CodePage };
            }
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFileWithWrongEncodingAsync(int codepage)
        {
            string testCode = "class TypeName { }";

            this.fileEncoding = Encoding.GetEncoding(codepage);

            var expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFileWithUtf8EncodingWithoutBOMAsync()
        {
            string testCode = "class TypeName { }";

            this.fileEncoding = new UTF8Encoding(false);

            var expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestCodeFixAsync(int codepage)
        {
            string testCode = "class TypeName { }";

            this.fileEncoding = Encoding.GetEncoding(codepage);

            // Create a project using the specified encoding
            Project project = this.CreateProject(new[] { testCode });
            Project oldProject = project;

            Workspace workspace = project.Solution.Workspace;

            var codeFixer = this.GetCSharpCodeFixProvider();

            var document = project.Documents.First();

            // Create a diagnostic for the document to fix
            var properties = ImmutableDictionary<string, string>.Empty.SetItem(SA1412StoreFilesAsUtf8.EncodingProperty, this.fileEncoding.WebName);
            var diagnostic = Diagnostic.Create(
                this.GetCSharpDiagnosticAnalyzers().First().SupportedDiagnostics.First(),
                Location.Create(await document.GetSyntaxTreeAsync().ConfigureAwait(false), TextSpan.FromBounds(0, 0)),
                properties);

            await codeFixer.RegisterCodeFixesAsync(
                new CodeFixContext(
                    document,
                    diagnostic,
                    (ca, d) =>
                    {
                        var operation = ca.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()[0];

                        operation.Apply(workspace, CancellationToken.None);
                    },
                    CancellationToken.None)).ConfigureAwait(false);

            // project should now have the "fixed document" in it.
            // Because of limitations in Roslyn the fixed document should
            // have a different DocumentId then the broken document
            project = workspace.CurrentSolution.Projects.First();

            Assert.Equal(1, project.DocumentIds.Count);

            SourceText sourceText = await project.Documents.First().GetTextAsync().ConfigureAwait(false);

            Assert.Equal(testCode, sourceText.ToString());

            Assert.Equal(Encoding.UTF8, sourceText.Encoding);
            Assert.NotEqual(oldProject.DocumentIds[0], project.DocumentIds[0]);
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFixAllAsync(int codepage)
        {
            await this.TestFixAllAsync(codepage, FixAllScope.Project).ConfigureAwait(false);
            await this.TestFixAllAsync(codepage, FixAllScope.Solution).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1412StoreFilesAsUtf8();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1412CodeFixProvider();
        }

        protected override Project CreateProject(string[] sources, string language = "C#", string[] filenames = null)
        {
            string fileNamePrefix = "Test";
            string fileExt = "cs";

            var projectId = ProjectId.CreateNewId(debugName: "TestProject");
            var solution = this.CreateSolution(projectId, language);

            int count = 0;
            for (int i = 0; i < sources.Length; i++)
            {
                string source = sources[i];
                var newFileName = filenames?[i] ?? fileNamePrefix + count + "." + fileExt;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source, encoding: this.fileEncoding));
                count++;
            }

            return solution.GetProject(projectId);
        }

        private async Task TestFixAllAsync(int codepage, FixAllScope scope)
        {
            string[] testCode = new[] { "class Foo { }", "class Bar { }" };

            this.fileEncoding = Encoding.GetEncoding(codepage);

            // Create a project using the specified encoding
            Project project = this.CreateProject(testCode);
            Project oldProject = project;

            Workspace workspace = project.Solution.Workspace;

            var codeFixer = this.GetCSharpCodeFixProvider();
            var fixAllProvider = codeFixer.GetFixAllProvider();
            var diagnostics = new List<Diagnostic>();
            var descriptor = this.GetCSharpDiagnosticAnalyzers().First().SupportedDiagnostics.First();
            foreach (var document in project.Documents)
            {
                // Create a diagnostic for the document to fix
                var properties = ImmutableDictionary<string, string>.Empty.SetItem(SA1412StoreFilesAsUtf8.EncodingProperty, (await document.GetTextAsync().ConfigureAwait(false)).Encoding.WebName);
                var diagnostic = Diagnostic.Create(
                    descriptor,
                    Location.Create(await document.GetSyntaxTreeAsync().ConfigureAwait(false), TextSpan.FromBounds(0, 0)),
                    properties);
                diagnostics.Add(diagnostic);
            }

            FixAllContext fixAllContext = new FixAllContext(
                project.Documents.First(),
                codeFixer,
                scope,
                nameof(SA1412CodeFixProvider) + "." + this.fileEncoding.WebName,
                new[] { SA1412StoreFilesAsUtf8.DiagnosticId },
                TestDiagnosticProvider.Create(diagnostics.ToImmutableArray()),
                CancellationToken.None);

            CodeAction codeAction = await fixAllProvider.GetFixAsync(fixAllContext).ConfigureAwait(false);
            var operation = codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()[0];

            operation.Apply(workspace, CancellationToken.None);

            // project should now have the "fixed document" in it.
            // Because of limitations in roslyn the fixed document should
            // have a different DocumentId then the broken document
            project = workspace.CurrentSolution.Projects.First();

            Assert.Equal(2, project.DocumentIds.Count);

            for (int i = 0; i < project.DocumentIds.Count; i++)
            {
                DocumentId documentId = project.DocumentIds[i];
                SourceText sourceText = await project.GetDocument(documentId).GetTextAsync().ConfigureAwait(false);

                Assert.Equal(testCode[i], sourceText.ToString());

                Assert.Equal(Encoding.UTF8, sourceText.Encoding);
                Assert.NotEqual(oldProject.DocumentIds[i], project.DocumentIds[i]);
            }
        }

        [Theory]
        [InlineData(FixAllScope.Solution)]
        [InlineData(FixAllScope.Project)]
        private async Task TestFixAllWithMultipleEncodingsAsync(FixAllScope scope)
        {
            string[] testCode = new[] { "class Foo { }", "class Bar { }" };

            this.fileEncoding = Encoding.Unicode;

            // Create a project using the specified encoding
            Project project = this.CreateProject(testCode);

            project = project.AddDocument("Test2.cs", SourceText.From("class FooBar { }", Encoding.UTF7)).Project;

            Project oldProject = project;

            Workspace workspace = project.Solution.Workspace;

            var codeFixer = this.GetCSharpCodeFixProvider();
            var fixAllProvider = codeFixer.GetFixAllProvider();
            var diagnostics = new List<Diagnostic>();
            var descriptor = this.GetCSharpDiagnosticAnalyzers().First().SupportedDiagnostics.First();
            foreach (var document in project.Documents)
            {
                // Create a diagnostic for the document to fix
                var properties = ImmutableDictionary<string, string>.Empty.SetItem(SA1412StoreFilesAsUtf8.EncodingProperty, (await document.GetTextAsync().ConfigureAwait(false)).Encoding.WebName);
                var diagnostic = Diagnostic.Create(
                    descriptor,
                    Location.Create(await document.GetSyntaxTreeAsync().ConfigureAwait(false), TextSpan.FromBounds(0, 0)),
                    properties);
                diagnostics.Add(diagnostic);
            }

            FixAllContext fixAllContext = new FixAllContext(
                project.Documents.First(),
                codeFixer,
                scope,
                nameof(SA1412CodeFixProvider) + "." + this.fileEncoding.WebName,
                new[] { SA1412StoreFilesAsUtf8.DiagnosticId },
                TestDiagnosticProvider.Create(diagnostics.ToImmutableArray()),
                CancellationToken.None);

            CodeAction codeAction = await fixAllProvider.GetFixAsync(fixAllContext).ConfigureAwait(false);
            var operation = codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()[0];

            operation.Apply(workspace, CancellationToken.None);

            // project should now have the "fixed document" in it.
            // Because of limitations in Roslyn the fixed document should
            // have a different DocumentId then the broken document
            project = workspace.CurrentSolution.Projects.First();

            Assert.Equal(3, project.DocumentIds.Count);

            // The order of the document ids is now different from the order before.
            for (int i = 1; i < 3; i++)
            {
                DocumentId documentId = project.DocumentIds[i];
                SourceText sourceText = await project.GetDocument(documentId).GetTextAsync().ConfigureAwait(false);

                Assert.Equal(testCode[i - 1], sourceText.ToString());

                Assert.Equal(Encoding.UTF8, sourceText.Encoding);
                Assert.NotEqual(oldProject.DocumentIds[i - 1], project.DocumentIds[i]);
            }

            // Check that Test2.cs was not changed
            DocumentId otherDocumentId = project.DocumentIds[0];
            var otherDocument = project.GetDocument(otherDocumentId);

            Assert.Equal("Test2.cs", otherDocument.Name);

            SourceText otherDocumentText = await otherDocument.GetTextAsync().ConfigureAwait(false);

            Assert.Equal("class FooBar { }", otherDocumentText.ToString());

            Assert.Equal(Encoding.UTF7, otherDocumentText.Encoding);
            Assert.Equal(oldProject.DocumentIds[2], project.DocumentIds[0]);
        }
    }
}
