namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;

    /// <summary>
    /// Document change action that will not do any post processing.
    /// </summary>
    public class PreservingDocumentChangeAction : CodeAction
    {
        private readonly string title;
        private readonly Func<CancellationToken, Task<Document>> createChangedDocument;
        private readonly string equivalenceKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreservingDocumentChangeAction"/> class.
        /// </summary>
        /// <param name="title">Title of the <see cref="CodeAction"/>.</param>
        /// <param name="createChangedDocument">Function to create the <see cref="Document"/>.</param>
        /// <param name="equivalenceKey">Optional value used to determine the equivalence of the <see cref="CodeAction"/> with other <see cref="CodeAction"/>s. See <see cref="CodeAction.EquivalenceKey"/>.</param>
        public PreservingDocumentChangeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey = null)
        {
            this.title = title;
            this.createChangedDocument = createChangedDocument;
            this.equivalenceKey = equivalenceKey;
        }

        /// <inheritdoc/>
        public override string Title
        {
            get { return this.title; }
        }

        /// <inheritdoc/>
        public override string EquivalenceKey
        {
            get { return this.equivalenceKey; }
        }

        /// <inheritdoc/>
        protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
        {
            return this.createChangedDocument(cancellationToken);
        }

        /// <inheritdoc/>
        protected override Task<Document> PostProcessChangesAsync(Document document, CancellationToken cancellationToken)
        {
            return Task.FromResult(document);
        }
    }
}
