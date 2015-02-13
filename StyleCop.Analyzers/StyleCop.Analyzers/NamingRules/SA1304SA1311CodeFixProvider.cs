namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Helpers;

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
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!FixableDiagnostics.Any(d => diagnostic.Id.Equals(d)))
                    continue;

                var document = context.Document;
                var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                    continue;

                var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                var typeSyntax = this.GetParentTypeDeclaration(token);

                if (typeSyntax != null)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync();

                    var typeDeclarationSymbol = semanticModel.GetDeclaredSymbol(typeSyntax) as INamedTypeSymbol;
                    if (typeDeclarationSymbol == null)
                    {
                        return;
                    }

                    while (!this.IsValidNewMemberName(typeDeclarationSymbol, newName) || newName == typeSyntax.GetTypeName())
                    {
                        newName = newName + "Value";
                    }

                    context.RegisterCodeFix(CodeAction.Create($"Rename field to '{newName}'", cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken)), diagnostic);
                }
            }
        }

        private bool IsValidNewMemberName(INamedTypeSymbol typeSymbol, string name)
        {
            foreach (var member in typeSymbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Method)
                {
                    // Field names can't colide with method names
                    continue;
                }
                else
                {
                    if (member.Name == name)
                    {
                        return false;
                    }
                }
            }
            if (typeSymbol.SpecialType == SpecialType.System_Object)
            {
                return true;
            }
            else
            {
                return this.IsValidNewMemberName(typeSymbol.BaseType, name);
            }
        }

        private TypeDeclarationSyntax GetParentTypeDeclaration(SyntaxToken token)
        {
            SyntaxNode parent = token.Parent;
            while (!(parent is TypeDeclarationSyntax))
            {
                parent = parent.Parent;
                if (parent == null)
                {
                    return null;
                }
            }

            return (TypeDeclarationSyntax)parent;
        }
    }
}
