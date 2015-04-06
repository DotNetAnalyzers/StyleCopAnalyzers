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
    /// Implements a code fix for <see cref="SA1307AccessibleFieldsMustBeginWithUpperCaseLetter"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the field so that it begins with an upper-case letter,
    /// or place the item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1307CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1307CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId))
                {
                    continue;
                }

                var document = context.Document;
                var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                    context.RegisterCodeFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
                }
            }
        }
    }
}
