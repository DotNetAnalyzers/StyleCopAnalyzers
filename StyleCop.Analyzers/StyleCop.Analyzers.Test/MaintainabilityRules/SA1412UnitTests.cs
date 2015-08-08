namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1412UnitTests : CodeFixVerifier
    {
        private Encoding fileEncoding;

        public static IEnumerable<Encoding> NonUtf8Encodings
        {
            get
            {
                yield return Encoding.ASCII;
                yield return Encoding.BigEndianUnicode;
                yield return Encoding.Default;
                yield return Encoding.Unicode;
                yield return Encoding.UTF32;
                yield return Encoding.UTF7;
            }
        }

        [Fact]
        public async Task TestFileWithWrongEncodingAsync()
        {
            // Encoding is not IConvertable so xunit does not support it in theories
            foreach (var encoding in NonUtf8Encodings)
            {
                string testCode = "class Foo { }";

                this.fileEncoding = encoding;

                var expected = this.CSharpDiagnostic().WithLocation(1, 1);

                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestFileWithUtf8EncodingWithoutBOMAsync()
        {
            string testCode = "class Foo { }";

            this.fileEncoding = new UTF8Encoding(false);

            var expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            // Encoding is not IConvertable so xunit does not support it in theories
            foreach (var encoding in NonUtf8Encodings)
            {
                string testCode = "class Foo { }";

                this.fileEncoding = encoding;

                // Create a project using ASCII encoding
                Project project = this.CreateProject(new[] { testCode });
                Project oldProject = project;

                Workspace workspace = project.Solution.Workspace;

                var codeFixer = this.GetCSharpCodeFixProvider();

                var document = project.Documents.First();

                // Create a diagnostic for the document to fix
                var diagnostic = Diagnostic.Create(this.GetCSharpDiagnosticAnalyzers().First().SupportedDiagnostics.First(),
                    Location.Create(await document.GetSyntaxTreeAsync().ConfigureAwait(false), TextSpan.FromBounds(0, 0)));

                await codeFixer.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (ca, d) =>
                      {
                          var operation = ca.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult()[0];

                          operation.Apply(workspace, CancellationToken.None);
                      }, CancellationToken.None)).ConfigureAwait(false);

                // project should now have the "fixed document" in it.
                // Because of limitations in roslyn the fixed document should
                // have a different DocumentId then the broken document
                project = workspace.CurrentSolution.Projects.First();

                Assert.Equal(1, project.DocumentIds.Count);

                SourceText sourceText = await project.Documents.First().GetTextAsync().ConfigureAwait(false);

                Assert.Equal(testCode, sourceText.ToString());

                Assert.Equal(Encoding.UTF8, sourceText.Encoding);
                Assert.NotEqual(oldProject.DocumentIds[0], project.DocumentIds[0]);
            }
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
    }
}