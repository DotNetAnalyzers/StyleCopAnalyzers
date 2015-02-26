namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Implements a code fix for <see cref="SA1121UseBuiltInTypeAlias"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comma is followed by a single space, and is not preceded
    /// by any space.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1121CodeFixProvider), LanguageNames.CSharp)]
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
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1121UseBuiltInTypeAlias.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

                var semanticModel = await context.Document.GetSemanticModelAsync();

                var typeInfo = semanticModel?.GetTypeInfo(node);

                if (typeInfo?.Type != null)
                {
                    SpecialType specialType = typeInfo.Value.Type.SpecialType;
                    var newNode = SyntaxFactory.PredefinedType(SyntaxFactory.Token(PredefinedSpecialTypes[specialType]))
                        .WithTriviaFrom(node)
                        .WithoutFormatting();
                    var newRoot = root.ReplaceNode(node, newNode);

                    context.RegisterCodeFix(CodeAction.Create("Replace with built-in type", token => Task.FromResult(context.Document.WithSyntaxRoot(newRoot))), diagnostic);
                }
            }
        }
    }
}
