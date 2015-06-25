namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1121UseBuiltInTypeAlias"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comma is followed by a single space, and is not preceded
    /// by any space.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1121CodeFixProvider))]
    [Shared]
    public class SA1121CodeFixProvider : CodeFixProvider
    {
        private static readonly Dictionary<SpecialType, SyntaxKind> PredefinedSpecialTypes = new Dictionary<SpecialType, SyntaxKind>
        {
            [SpecialType.System_Boolean] = SyntaxKind.BoolKeyword,
            [SpecialType.System_Byte] = SyntaxKind.ByteKeyword,
            [SpecialType.System_Char] = SyntaxKind.CharKeyword,
            [SpecialType.System_Decimal] = SyntaxKind.DecimalKeyword,
            [SpecialType.System_Double] = SyntaxKind.DoubleKeyword,
            [SpecialType.System_Int16] = SyntaxKind.ShortKeyword,
            [SpecialType.System_Int32] = SyntaxKind.IntKeyword,
            [SpecialType.System_Int64] = SyntaxKind.LongKeyword,
            [SpecialType.System_Object] = SyntaxKind.ObjectKeyword,
            [SpecialType.System_SByte] = SyntaxKind.SByteKeyword,
            [SpecialType.System_Single] = SyntaxKind.FloatKeyword,
            [SpecialType.System_String] = SyntaxKind.StringKeyword,
            [SpecialType.System_UInt16] = SyntaxKind.UShortKeyword,
            [SpecialType.System_UInt32] = SyntaxKind.UIntKeyword,
            [SpecialType.System_UInt64] = SyntaxKind.ULongKeyword
        };

        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1121UseBuiltInTypeAlias.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1121UseBuiltInTypeAlias.DiagnosticId))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create(ReadabilityResources.SA1121CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null)
            {
                return document;
            }

            var node = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

            var memberAccess = node.Parent as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                if (node == memberAccess.Name)
                {
                    node = memberAccess;
                }
            }

            var type = semanticModel.GetSymbolInfo(node, cancellationToken).Symbol as INamedTypeSymbol;

            SyntaxKind specialKind;
            if (!PredefinedSpecialTypes.TryGetValue(type.SpecialType, out specialKind))
            {
                return document;
            }

            SyntaxNode newNode;
            PredefinedTypeSyntax typeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(specialKind));
            if (node is CrefSyntax)
            {
                newNode = SyntaxFactory.TypeCref(typeSyntax);
            }
            else
            {
                newNode = typeSyntax;
            }

            newNode = newNode
                .WithTriviaFrom(node)
                .WithoutFormatting();

            var newRoot = root.ReplaceNode(node, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
