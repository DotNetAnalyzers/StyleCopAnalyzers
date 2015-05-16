namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SX1309FieldNamesMustBeginWithUnderscore"/> and
    /// <see cref="SX1309SStaticFieldNamesMustBeginWithUnderscore"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add an underscore to the beginning of the field name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SX1309CodeFixProvider))]
    [Shared]
    public class SX1309CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SX1309FieldNamesMustBeginWithUnderscore.DiagnosticId, SX1309SStaticFieldNamesMustBeginWithUnderscore.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SX1309FieldNamesMustBeginWithUnderscore.DiagnosticId)
                    && !diagnostic.Id.Equals(SX1309SStaticFieldNamesMustBeginWithUnderscore.DiagnosticId))
                {
                    continue;
                }

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    string newName = '_' + token.ValueText;
                    context.RegisterCodeFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
                }
            }
        }
    }
}
