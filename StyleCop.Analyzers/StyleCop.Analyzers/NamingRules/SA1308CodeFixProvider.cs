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
    /// Implements a code fix for <see cref="SA1308VariableNamesMustNotBePrefixed"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the prefix from the beginning of the field name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1308CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1308CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1308VariableNamesMustNotBePrefixed.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1308VariableNamesMustNotBePrefixed.DiagnosticId))
                    continue;

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

                if (token.IsMissing)
                    continue;

                // The variable name is the full suffix. In this case we cannot generate a valid variable name and thus will not offer a code fix.
                if (token.ValueText.Length <= 2)
                {
                    continue;
                }

                var numberOfCharsToRemove = 2;

                // If a variable contains multiple prefixes that would result in this diagnostic, 
                // we should detect that and remove all of the bad prefixes such that after 
                // the fix is applied there are no more violations of this rule.
                for (int i = 2; i < token.ValueText.Length; i += 2)
                {
                    var nextTwoChars = token.ValueText.Substring(i, 2);

                    if (nextTwoChars.Equals("m_", System.StringComparison.CurrentCultureIgnoreCase) ||
                        nextTwoChars.Equals("s_", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        numberOfCharsToRemove += 2;
                        continue;
                    }

                    break;
                }

                if (!string.IsNullOrEmpty(token.ValueText))
                {
                    var newName = token.ValueText.Substring(numberOfCharsToRemove);
                    context.RegisterCodeFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
                }
            }
        }
    }
}
