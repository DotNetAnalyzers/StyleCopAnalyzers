namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1412StoreFilesAsUtf8"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the encoding to UTF-8 with preamble.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1412CodeFixProvider))]
    [Shared]
    public class SA1412CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1412StoreFilesAsUtf8.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return new SA1412FixAllProvider();
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!this.FixableDiagnosticIds.Contains(diagnostic.Id))
                {
                    continue;
                }

                string usedEncoding = await GetEncodingNameForDocumentAsync(context.Document).ConfigureAwait(false);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        string.Format(MaintainabilityResources.SA1412CodeFix, usedEncoding),
                        token => GetTransformedSolutionAsync(context.Document),
                        equivalenceKey: await GetEquivalenceKeyForDocumentAsync(context.Document).ConfigureAwait(false)), diagnostic);
            }
        }

        internal static async Task<string> GetEncodingNameForDocumentAsync(Document document)
        {
            return (await document.GetTextAsync().ConfigureAwait(false)).Encoding?.WebName ?? "<null>";
        }

        internal static async Task<string> GetEquivalenceKeyForDocumentAsync(Document document)
        {
            string usedEncoding = await GetEncodingNameForDocumentAsync(document).ConfigureAwait(false);
            return nameof(SA1412CodeFixProvider) + "." + usedEncoding;
        }

        internal static async Task<Solution> GetTransformedSolutionAsync(Document document)
        {
            SourceText text = await document.GetTextAsync().ConfigureAwait(false);

            string actualSourceText = text.ToString();

            text = SourceText.From(actualSourceText, Encoding.UTF8);

            // Changing the encoding as part of a "normal" text change does not work.
            // Roslyn will not see an encoding change as a text change and assumes that
            // there is nothing to do.
            Solution solutionWithoutDocument = document.Project.Solution.RemoveDocument(document.Id);
            return solutionWithoutDocument.AddDocument(DocumentId.CreateNewId(document.Project.Id), document.Name, text, document.Folders, document.FilePath);
        }
    }
}
