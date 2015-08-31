namespace StyleCopTester
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    internal class CodeFixEquivalenceGroup
    {
        private CodeFixEquivalenceGroup(
            string equivalenceKey,
            Solution solution,
            FixAllProvider fixAllProvider,
            CodeFixProvider codeFixProvider,
            ImmutableArray<Diagnostic> diagnosticsToFix)
        {
            this.CodeFixEquivalenceKey = equivalenceKey;
            this.Solution = solution;
            this.FixAllProvider = fixAllProvider;
            this.CodeFixProvider = codeFixProvider;
            this.DiagnosticsToFix = diagnosticsToFix;
        }

        internal string CodeFixEquivalenceKey { get; }

        internal Solution Solution { get; }

        internal FixAllProvider FixAllProvider { get; }

        internal CodeFixProvider CodeFixProvider { get; }

        internal ImmutableArray<Diagnostic> DiagnosticsToFix { get; }

        internal static async Task<ImmutableList<CodeFixEquivalenceGroup>> CreateAsync(CodeFixProvider codeFixProvider, IEnumerable<Diagnostic> allDiagnostics, Solution solution)
        {
            var fixAllProvider = codeFixProvider.GetFixAllProvider();

            var relevantDiagnostics = allDiagnostics.Where(diagnostic => codeFixProvider.FixableDiagnosticIds.Contains(diagnostic.Id)).ToImmutableArray();

            if (fixAllProvider == null)
            {
                return ImmutableList.Create<CodeFixEquivalenceGroup>();
            }

            List<CodeAction> actions = new List<CodeAction>();

            foreach (var diagnostic in relevantDiagnostics)
            {
                actions.AddRange(await GetFixesAsync(solution, codeFixProvider, diagnostic).ConfigureAwait(false));
            }

            List<CodeFixEquivalenceGroup> groups = new List<CodeFixEquivalenceGroup>();

            foreach (var item in actions.GroupBy(x => x.EquivalenceKey))
            {
                groups.Add(new CodeFixEquivalenceGroup(item.Key, solution, fixAllProvider, codeFixProvider, relevantDiagnostics));
            }

            return groups.ToImmutableList();
        }

        internal async Task<ImmutableArray<CodeActionOperation>> GetOperationsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Diagnostic diagnostic = this.DiagnosticsToFix.First();
            Document document = this.Solution.GetDocument(diagnostic.Location.SourceTree);

            var diagnosticsProvider = TesterDiagnosticProvider.Create(this.DiagnosticsToFix);

            var context = new FixAllContext(document, this.CodeFixProvider, FixAllScope.Solution, this.CodeFixEquivalenceKey, this.DiagnosticsToFix.Select(x => x.Id), diagnosticsProvider, cancellationToken);

            CodeAction action = await this.FixAllProvider.GetFixAsync(context).ConfigureAwait(false);

            return await action.GetOperationsAsync(cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IEnumerable<CodeAction>> GetFixesAsync(Solution solution, CodeFixProvider codeFixProvider, Diagnostic diagnostic)
        {
            List<CodeAction> codeActions = new List<CodeAction>();

            await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => codeActions.Add(a), CancellationToken.None));

            return codeActions;
        }
    }
}
