namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SA1310FieldNamesMustNotContainUnderscore"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the underscore from the name of the field, or place the item
    /// within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1310CodeFixProvider))]
    [Shared]
    public class SA1310CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
          ImmutableArray.Create(SA1310FieldNamesMustNotContainUnderscore.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1310FieldNamesMustNotContainUnderscore.DiagnosticId))
                {
                    continue;
                }

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                string currentName = token.ValueText;
                string proposedName = BuildProposedName(currentName);
                if (proposedName != currentName)
                {
                    context.RegisterCodeFix(CodeAction.Create($"Rename field to '{proposedName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, proposedName, cancellationToken)), diagnostic);
                }
            }
        }

        private static string BuildProposedName(string currentName)
        {
            StringBuilder builder = new StringBuilder(currentName.Length);

            bool foundNonUnderscore = false;
            bool capitalizeNextLetter = false;
            for (int i = 0; i < currentName.Length; i++)
            {
                char c = currentName[i];
                if (c != '_')
                {
                    foundNonUnderscore = true;

                    if (capitalizeNextLetter)
                    {
                        builder.Append(char.ToUpperInvariant(c));
                        capitalizeNextLetter = false;
                        continue;
                    }
                    else
                    {
                        builder.Append(c);
                        continue;
                    }
                }

                if (!foundNonUnderscore)
                {
                    // Leave leading underscores as-is (they are handled by SA1309)
                    builder.Append(c);
                    continue;
                }

                // drop the underscore(s), and attempt to capitalize the next character
                capitalizeNextLetter = true;
            }

            return builder.ToString();
        }
    }
}
