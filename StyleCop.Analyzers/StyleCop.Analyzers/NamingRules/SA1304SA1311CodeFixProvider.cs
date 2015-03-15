namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter"/>
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the field so that it begins with an upper-case
    /// letter.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1304SA1311CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1304SA1311CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                                  SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!FixableDiagnostics.Any(d => diagnostic.Id.Equals(d)))
                    continue;

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                context.RegisterCodeFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
            }
        }
    }
}
