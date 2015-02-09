namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    [ExportCodeFixProvider(nameof(SA1309CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1309CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
          ImmutableArray.Create(SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return FixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId))
                    continue;

                var document = context.Document;
                var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    var newName = token.ValueText.Substring(1);
                    context.RegisterFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
                }
            }
        }
    }
}
