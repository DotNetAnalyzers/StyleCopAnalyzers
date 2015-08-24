namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for all analyzers that require a symbol to be upper case.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the name of the symbol so that it begins with an upper-case letter,
    /// or place the item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameToUpperCaseCodeFixProvider))]
    [Shared]
    public class RenameToUpperCaseCodeFixProvider : CodeFixProvider
    {
        private const string Suffix = "Value";

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(
                SA1300ElementMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1303ConstFieldNamesMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1304NonPrivateReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1307AccessibleFieldsMustBeginWithUpperCaseLetter.DiagnosticId,
                SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!FixableDiagnostics.Contains(diagnostic.Id))
                {
                    continue;
                }

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                var newName = char.ToUpper(token.ValueText[0]) + token.ValueText.Substring(1);
                var memberSyntax = this.GetParentTypeDeclaration(token);

                if (memberSyntax is NamespaceDeclarationSyntax)
                {
                    // namespaces are not symbols. So we are just renaming the namespace
                    Func<CancellationToken, Task<Document>> renameNamespace = t =>
                    {
                        IdentifierNameSyntax identifierSyntax = (IdentifierNameSyntax)token.Parent;

                        var newIdentifierSyntac = identifierSyntax.WithIdentifier(SyntaxFactory.Identifier(newName));

                        var newRoot = root.ReplaceNode(identifierSyntax, newIdentifierSyntac);
                        return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                    };

                    context.RegisterCodeFix(CodeAction.Create(string.Format(NamingResources.RenameToCodeFix, newName), renameNamespace, equivalenceKey: nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id), diagnostic);
                }
                else if (memberSyntax != null)
                {
                    SemanticModel semanticModel = await document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                    var typeDeclarationSymbol = semanticModel.GetDeclaredSymbol(memberSyntax) as INamedTypeSymbol;
                    if (typeDeclarationSymbol == null)
                    {
                        continue;
                    }

                    if (!this.IsValidNewMemberName(typeDeclarationSymbol, newName))
                    {
                        newName = newName + Suffix;
                    }

                    context.RegisterCodeFix(CodeAction.Create(string.Format(NamingResources.RenameToCodeFix, newName), cancellationToken => RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken), equivalenceKey: nameof(RenameToUpperCaseCodeFixProvider) + "_" + diagnostic.Id), diagnostic);
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

        private MemberDeclarationSyntax GetParentTypeDeclaration(SyntaxToken token)
        {
            SyntaxNode parent = token.Parent;

            while (parent != null)
            {
                var declarationParent = parent as MemberDeclarationSyntax;
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

