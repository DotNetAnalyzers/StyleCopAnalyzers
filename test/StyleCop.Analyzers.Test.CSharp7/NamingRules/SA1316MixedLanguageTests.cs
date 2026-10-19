// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using StyleCop.Analyzers.NamingRules;
using StyleCop.Analyzers.Test.Verifiers;
using Xunit;

/// <summary>
/// Mixed-language regression tests for SA1316. This class is not named UnitTests, so it is not inherited by later
/// compiler-version projects that do not reference the Visual Basic workspaces package.
/// </summary>
public class SA1316MixedLanguageTests
{
    /// <summary>
    /// Verifies that the fix is not offered when a Visual Basic document references the tuple element.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task TestFixIsNotOfferedWhenVisualBasicReferencesTupleElementAsync()
    {
        const string csharpSource = @"
public class Producer
{
    public (int nameA, int Other) Create()
    {
        return (1, 2);
    }
}
";
        const string visualBasicSource = @"
Public Class Consumer
    Public Function Use(producer As Producer) As Integer
        Return producer.Create().nameA
    End Function
End Class
";

        var references = await new ReferenceWorkspace().GetReferencesAsync().ConfigureAwait(false);

        var host = MefHostServices.Create(new[]
        {
            typeof(CSharpCompilation).Assembly,
            typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions).Assembly,
            typeof(VisualBasicCompilation).Assembly,
            Assembly.Load("Microsoft.CodeAnalysis.VisualBasic.Workspaces"),
            typeof(MefHostServices).Assembly,
        });
        using var workspace = new AdhocWorkspace(host);
        var csharpProject = workspace.AddProject("CSharpProject", LanguageNames.CSharp);
        var visualBasicProject = workspace.AddProject("VisualBasicProject", LanguageNames.VisualBasic);
        var csharpDocumentId = DocumentId.CreateNewId(csharpProject.Id);
        var solution = workspace.CurrentSolution
            .WithProjectCompilationOptions(csharpProject.Id, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default))
            .WithProjectParseOptions(csharpProject.Id, new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_1))
            .AddMetadataReferences(csharpProject.Id, references)
            .AddDocument(csharpDocumentId, "Producer.cs", SourceText.From(csharpSource))
            .WithProjectCompilationOptions(visualBasicProject.Id, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default))
            .WithProjectParseOptions(visualBasicProject.Id, new VisualBasicParseOptions(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest))
            .AddMetadataReferences(visualBasicProject.Id, references)
            .AddProjectReference(visualBasicProject.Id, new ProjectReference(csharpProject.Id))
            .AddDocument(DocumentId.CreateNewId(visualBasicProject.Id), "Consumer.vb", SourceText.From(visualBasicSource));
        Assert.True(workspace.TryApplyChanges(solution));
        solution = workspace.CurrentSolution;

        var document = solution.GetDocument(csharpDocumentId);
        var compilation = await document.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
        Assert.Empty(compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
        var visualBasicCompilation = await solution.GetProject(visualBasicProject.Id).GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
        Assert.Empty(visualBasicCompilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        var analyzer = new SA1316TupleElementNamesShouldUseCorrectCasing();
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
        var diagnostic = Assert.Single(diagnostics);

        var actions = new List<CodeAction>();
        var context = new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
        await new SA1316CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);
        Assert.Empty(actions);
    }

    private sealed class ReferenceWorkspace : StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<SA1316TupleElementNamesShouldUseCorrectCasing, SA1316CodeFixProvider>.CSharpTest
    {
        public ReferenceWorkspace()
            : base(Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_1)
        {
            this.TestCode = "class Reference { }";
        }

        public async Task<IReadOnlyList<MetadataReference>> GetReferencesAsync()
        {
            var projectId = ProjectId.CreateNewId();
            var state = new Microsoft.CodeAnalysis.Testing.Model.EvaluatedProjectState(this.TestState, this.ReferenceAssemblies);
            var solution = await this.CreateSolutionAsync(projectId, state, CancellationToken.None).ConfigureAwait(false);
            return solution.GetProject(projectId).MetadataReferences.ToList();
        }
    }
}
