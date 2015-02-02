using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.SpacingRules;

namespace StyleCop.Analyzers.MaintainabilityRules
{
    [ExportCodeFixProvider(nameof(SA1401FieldsMustBePrivateCodeFix), LanguageNames.CSharp)]
    [Shared]
    public class SA1401FieldsMustBePrivateCodeFix : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1401FieldsMustBePrivate.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1401FieldsMustBePrivate.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var diagnosticSpan = diagnostic.Location.SourceSpan;

                var fieldDeclarationSyntax =
                    root.FindToken(diagnosticSpan.Start)
                        .Parent.AncestorsAndSelf()
                        .OfType<FieldDeclarationSyntax>()
                        .First();

                context.RegisterFix(
                    CodeAction.Create("Make field private",
                        c => MakeFieldPrivate(context.Document, fieldDeclarationSyntax, root, c)), diagnostic);
            }
        }

        private Task<Document> MakeFieldPrivate(Document document, FieldDeclarationSyntax fieldDeclaration,
            SyntaxNode root, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var firstToken = fieldDeclaration.GetFirstToken();
                var modifiers = new SyntaxTokenList();
                modifiers = modifiers.Add(SyntaxFactory.Token(firstToken.LeadingTrivia,
                    SyntaxKind.PrivateKeyword, firstToken.TrailingTrivia));
                modifiers = modifiers.AddRange(GetNoAccessModifiersTokens(fieldDeclaration));
                var newFieldDeclaration =
                    fieldDeclaration.WithModifiers(modifiers);
                var newRoot = root.ReplaceNode(fieldDeclaration, newFieldDeclaration);
                return document.WithSyntaxRoot(newRoot);
            }, cancellationToken);
        }

        private IEnumerable<SyntaxToken> GetNoAccessModifiersTokens(FieldDeclarationSyntax fieldDeclaration)
        {
            foreach (var modifier in fieldDeclaration.Modifiers)
            {
                var cSharpKind = modifier.CSharpKind();
                if (!accessModifiersSyntaxKinds.Any(am => am == cSharpKind))
                {
                    yield return modifier;
                }
            }
        }

        private readonly SyntaxKind[] accessModifiersSyntaxKinds = new SyntaxKind[]
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.ProtectedKeyword,
        };


    }
}