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

    [ExportCodeFixProvider(nameof(SA1302CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1302CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
            ImmutableArray.Create(SA1302InterfaceNamesMustBeginWithI.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1302InterfaceNamesMustBeginWithI.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true,
                    findInsideTrivia: true);
                if (node.IsMissing)
                {
                    continue;
                }

                var interfaceDeclaration = node as InterfaceDeclarationSyntax;
                if (interfaceDeclaration == null)
                {
                    return;
                }

                var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                var symbol = semanticModel?.GetDeclaredSymbol(interfaceDeclaration, context.CancellationToken) as ITypeSymbol;
                if (symbol == null || string.IsNullOrEmpty(symbol.Name))
                {
                    continue;
                }

                var newName = "I" + symbol.Name;

                var solution = context.Document.Project.Solution;
                var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);

                context.RegisterFix(CodeAction.Create("Change interface name to " + newName + ".", newSolution), diagnostic);
            }
        }
    }
}
