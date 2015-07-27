namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter"/>
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the field so that it begins with an upper-case
    /// letter.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1304SA1311CodeFixProvider))]
    [Shared]
    public class SA1304SA1311CodeFixProvider : CodeFixProvider
    {
        private const string Suffix = "Value";

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                                  SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

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
                if (!FixableDiagnostics.Any(d => diagnostic.Id.Equals(d)))
                {
                    continue;
                }

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                var typeSyntax = this.GetParentTypeDeclaration(token);

                if (typeSyntax != null)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                    var typeDeclarationSymbol = semanticModel.GetDeclaredSymbol(typeSyntax) as INamedTypeSymbol;
                    if (typeDeclarationSymbol == null)
                    {
                        continue;
                    }

                    if (!this.IsValidNewMemberName(typeDeclarationSymbol, newName))
                    {
                        newName = newName + Suffix;
                    }

                    context.RegisterCodeFix(CodeAction.Create(string.Format(NamingResources.SA1304SA1311CodeFix, newName), cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken), equivalenceKey: nameof(SA1304SA1311CodeFixProvider)), diagnostic);
                }
            }
        }

        private bool IsValidNewMemberName(INamedTypeSymbol typeSymbol, string name)
        {
            if (typeSymbol == null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }
            else if (typeSymbol.Name == name || typeSymbol.GetMembers(name).Length > 0)
            {
                return false;
            }

            return true;
        }

        private TypeDeclarationSyntax GetParentTypeDeclaration(SyntaxToken token)
        {
            SyntaxNode parent = token.Parent;

            while (parent != null)
            {
                var declarationParent = parent as TypeDeclarationSyntax;
                if (declarationParent != null)
                {
                    return declarationParent;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }
}
