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
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.SpacingRules;
using Xunit;

public class CustomFixAllProvidersTests
{
    [Fact]
    public void TestBatchFixerUsesCompilerProvider()
    {
        Assert.Same(WellKnownFixAllProviders.BatchFixer, CustomFixAllProviders.BatchFixer);
        Assert.Same(WellKnownFixAllProviders.BatchFixer, new SA1018CodeFixProvider().GetFixAllProvider());
    }

    [Fact]
    public void TestDocumentBasedFixerIsPreserved()
    {
        Assert.NotSame(WellKnownFixAllProviders.BatchFixer, new SA1028CodeFixProvider().GetFixAllProvider());
    }

    [Theory]
    [InlineData(FixAllScope.Document)]
    [InlineData(FixAllScope.Project)]
    [InlineData(FixAllScope.Solution)]
    public async Task TestBatchFixScopeAsync(FixAllScope scope)
    {
        const string source = "class C\r\n{\r\n    int ? first;\r\n    int ? second;\r\n}\r\n";
        const string fixedSource = "class C\r\n{\r\n    int? first;\r\n    int? second;\r\n}\r\n";
        using var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var otherProjectId = ProjectId.CreateNewId();
        var firstId = DocumentId.CreateNewId(projectId);
        var secondId = DocumentId.CreateNewId(projectId);
        var thirdId = DocumentId.CreateNewId(otherProjectId);
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "First", "First", LanguageNames.CSharp)
            .AddProject(otherProjectId, "Other", "Other", LanguageNames.CSharp)
            .AddDocument(firstId, "First.cs", SourceText.From(source))
            .AddDocument(secondId, "Second.cs", SourceText.From(source.Replace("class C", "class D")))
            .AddDocument(thirdId, "Third.cs", SourceText.From(source));
        var diagnostics = new Dictionary<DocumentId, ImmutableArray<Diagnostic>>();
        var descriptor = Assert.Single(new SA1018NullableTypeSymbolsMustNotBePrecededBySpace().SupportedDiagnostics);
        foreach (var id in new[] { firstId, secondId, thirdId })
        {
            var tree = await solution.GetDocument(id).GetSyntaxTreeAsync().ConfigureAwait(false);
            var text = (await tree.GetTextAsync().ConfigureAwait(false)).ToString();
            diagnostics.Add(
                id,
                ImmutableArray.Create(
                    Diagnostic.Create(descriptor, Location.Create(tree, new TextSpan(text.IndexOf('?'), 1))),
                    Diagnostic.Create(descriptor, Location.Create(tree, new TextSpan(text.LastIndexOf('?'), 1)))));
        }

        var fixer = new SA1018CodeFixProvider();
        var context = new FixAllContext(
            solution.GetDocument(firstId),
            fixer,
            scope,
            nameof(SA1018CodeFixProvider),
            fixer.FixableDiagnosticIds,
            new DiagnosticProvider(diagnostics),
            CancellationToken.None);
        var action = await fixer.GetFixAllProvider().GetFixAsync(context).ConfigureAwait(false);
        Assert.NotNull(action);
        var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var changedSolution = Assert.Single(operations.OfType<ApplyChangesOperation>()).ChangedSolution;

        Assert.Equal(fixedSource, (await changedSolution.GetDocument(firstId).GetTextAsync().ConfigureAwait(false)).ToString());
        Assert.Equal(
            (scope == FixAllScope.Document ? source : fixedSource).Replace("class C", "class D"),
            (await changedSolution.GetDocument(secondId).GetTextAsync().ConfigureAwait(false)).ToString());
        Assert.Equal(
            scope == FixAllScope.Solution ? fixedSource : source,
            (await changedSolution.GetDocument(thirdId).GetTextAsync().ConfigureAwait(false)).ToString());
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
            => Task.FromResult(project.DocumentIds.SelectMany(id => this.diagnostics[id]));
    }
}
