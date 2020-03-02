// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1302InterfaceNamesMustBeginWithI"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add the capital letter I to the front of the interface name, or place the
    /// item within a <c>NativeMethods</c> class if appropriate.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1302CodeFixProvider))]
    [Shared]
    internal class SA1302CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1302InterfaceNamesMustBeginWithI.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        NamingResources.SA1302CodeFix,
                        cancellationToken => CreateChangedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1302CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> CreateChangedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var baseName = "I" + token.ValueText;
            var index = 0;
            var newName = baseName;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var declaredSymbol = semanticModel.GetDeclaredSymbol(token.Parent, cancellationToken);
            while (!await RenameHelper.IsValidNewMemberNameAsync(semanticModel, declaredSymbol, newName, cancellationToken).ConfigureAwait(false))
            {
                index++;
                newName = baseName + index;
            }

            return await RenameHelper.RenameSymbolAsync(document, root, token, newName, cancellationToken).ConfigureAwait(false);
        }
    }
}
