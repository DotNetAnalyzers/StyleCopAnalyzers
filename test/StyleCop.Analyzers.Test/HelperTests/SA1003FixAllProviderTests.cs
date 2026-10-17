// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.SpacingRules;
using Xunit;

public class SA1003FixAllProviderTests
{
    [Fact]
    [WorkItem(1458, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1458")]
    public void TestSA1003UsesDedicatedFixAllProvider()
    {
        var fixer = new SA1003CodeFixProvider();

        Assert.NotSame(WellKnownFixAllProviders.BatchFixer, fixer.GetFixAllProvider());
    }

    [Theory]
    [InlineData(FixAllScope.Document)]
    [InlineData(FixAllScope.Project)]
    [InlineData(FixAllScope.Solution)]
    public async Task TestFixAllScopeAsync(FixAllScope scope)
    {
        const string firstSource = "class C { int M() { return 1+2; } }\r\n";
        const string secondSource = "class D { int M() { return 3+4; } }\r\n";
        const string thirdSource = "class E { int M() { return 5+6; } }\r\n";
        const string fixedFirstSource = "class C { int M() { return 1 + 2; } }\r\n";
        const string fixedSecondSource = "class D { int M() { return 3 + 4; } }\r\n";
        const string fixedThirdSource = "class E { int M() { return 5 + 6; } }\r\n";

        using var workspace = new AdhocWorkspace();
        var firstProjectId = ProjectId.CreateNewId();
        var secondProjectId = ProjectId.CreateNewId();
        var firstId = DocumentId.CreateNewId(firstProjectId);
        var secondId = DocumentId.CreateNewId(firstProjectId);
        var thirdId = DocumentId.CreateNewId(secondProjectId);
        var solution = workspace.CurrentSolution
            .AddProject(firstProjectId, "First", "First", LanguageNames.CSharp)
            .AddProject(secondProjectId, "Second", "Second", LanguageNames.CSharp)
            .AddDocument(firstId, "First.cs", SourceText.From(firstSource))
            .AddDocument(secondId, "Second.cs", SourceText.From(secondSource))
            .AddDocument(thirdId, "Third.cs", SourceText.From(thirdSource));

        var diagnostics = new Dictionary<DocumentId, ImmutableArray<Diagnostic>>
        {
            [firstId] = await CreateDiagnosticsAsync(solution.GetDocument(firstId)).ConfigureAwait(false),
            [secondId] = await CreateDiagnosticsAsync(solution.GetDocument(secondId)).ConfigureAwait(false),
            [thirdId] = await CreateDiagnosticsAsync(solution.GetDocument(thirdId)).ConfigureAwait(false),
        };

        var fixer = new SA1003CodeFixProvider();
        var context = new FixAllContext(
            solution.GetDocument(firstId),
            fixer,
            scope,
            nameof(SA1003CodeFixProvider),
            fixer.FixableDiagnosticIds,
            new DiagnosticProvider(diagnostics),
            CancellationToken.None);
        var action = await fixer.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);
        Assert.NotNull(action);
        var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var changedSolution = Assert.Single(operations.OfType<ApplyChangesOperation>()).ChangedSolution;

        Assert.Equal(fixedFirstSource, (await changedSolution.GetDocument(firstId).GetTextAsync().ConfigureAwait(false)).ToString());
        Assert.Equal(
            scope == FixAllScope.Document ? secondSource : fixedSecondSource,
            (await changedSolution.GetDocument(secondId).GetTextAsync().ConfigureAwait(false)).ToString());
        Assert.Equal(
            scope == FixAllScope.Solution ? fixedThirdSource : thirdSource,
            (await changedSolution.GetDocument(thirdId).GetTextAsync().ConfigureAwait(false)).ToString());
    }

    private static async Task<ImmutableArray<Diagnostic>> CreateDiagnosticsAsync(Document document)
    {
        var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
        var source = await document.GetTextAsync().ConfigureAwait(false);
        var operatorPosition = source.ToString().IndexOf('+');
        var location = Location.Create(tree, new TextSpan(operatorPosition, 1));

        return ImmutableArray.Create(
            Diagnostic.Create(
                SA1003SymbolsMustBeSpacedCorrectly.DescriptorPrecededByWhitespace,
                location,
                ImmutableDictionary<string, string>.Empty.Add(
                    SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction,
                    SA1003SymbolsMustBeSpacedCorrectly.InsertBeforeTag),
                "+"),
            Diagnostic.Create(
                SA1003SymbolsMustBeSpacedCorrectly.DescriptorFollowedByWhitespace,
                location,
                ImmutableDictionary<string, string>.Empty.Add(
                    SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction,
                    SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag),
                "+"));
    }

    private sealed class DiagnosticProvider : FixAllContext.DiagnosticProvider
    {
        private readonly Dictionary<DocumentId, ImmutableArray<Diagnostic>> diagnostics;

        public DiagnosticProvider(Dictionary<DocumentId, ImmutableArray<Diagnostic>> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(this.diagnostics[document.Id]);

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult(Enumerable.Empty<Diagnostic>());

        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(project.DocumentIds.SelectMany(id => this.diagnostics[id]));
    }
}
