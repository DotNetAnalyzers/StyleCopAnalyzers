using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace StyleCop.Analyzers.NamingRules
{
    [ExportCodeFixProvider(nameof(SA1309CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1309CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> _fixableDiagnostics =
          ImmutableArray.Create(SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId);

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
                if (!diagnostic.Id.Equals(SA1309FieldNamesMustNotBeginWithUnderscore.DiagnosticId))
                    continue;

                var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true,
                    findInsideTrivia: true);
                if (node.IsMissing)
                {
                    continue;
                }

                var variable = node as VariableDeclaratorSyntax;
                if (variable == null)
                {
                    return;
                }

                var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                var symbol = semanticModel?.GetDeclaredSymbol(variable, context.CancellationToken) as IFieldSymbol;
                if (symbol == null || string.IsNullOrEmpty(symbol.Name))
                {
                    continue;
                }

                if (symbol.Name.StartsWith("_"))
                {
                    var newName = symbol.Name.Substring(1);

                    var solution = context.Document.Project.Solution;
                    var newSolution =
                        await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);

                    context.RegisterFix(CodeAction.Create("Change field name to " + newName + ".", newSolution),
                        diagnostic);
                }
            }
        }
    }
}