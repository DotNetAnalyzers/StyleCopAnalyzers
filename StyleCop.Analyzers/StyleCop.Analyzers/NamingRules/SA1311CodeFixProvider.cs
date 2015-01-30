namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Rename;

    /// <summary>
    /// Implements a code fix for <see cref="SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter"/>
    /// </summary>
    [ExportCodeFixProvider(nameof(SA1311CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1311CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> GetFixableDiagnosticIds()
        {
            return _fixableDiagnostics;
        }

        /// <inheritdoc/>
        public override async Task ComputeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1311StaticReadonlyFieldsMustBeginWithUpperCaseLetter.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true,
                    findInsideTrivia: true);
                if (node.IsMissing)
                {
                    continue;
                }

                var field = node as VariableDeclaratorSyntax;
                if (field == null)
                {
                    return;
                }

                var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                var symbol = semanticModel?.GetDeclaredSymbol(field, context.CancellationToken) as IFieldSymbol;
                if (symbol == null || string.IsNullOrEmpty(symbol.Name))
                {
                    continue;
                }

                var newName = char.ToUpper(symbol.Name[0]) + symbol.Name.Substring(1);

                var solution = context.Document.Project.Solution;
                var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);

                context.RegisterFix(CodeAction.Create("Change field name to " + newName + ".", newSolution), diagnostic);
            }
        }
    }
}