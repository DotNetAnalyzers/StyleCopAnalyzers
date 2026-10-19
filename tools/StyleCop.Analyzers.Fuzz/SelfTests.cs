// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.Fuzz;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

internal static class SelfTests
{
    internal static async Task RunAsync()
    {
        var engine = new AnalysisEngine([new TestAnalyzer()], [new TestFixer()]);
        var result = await engine.RunAsync(new Input("class C { }", "C.cs")).ConfigureAwait(false);
        Require(result.Compiles, "The baseline must compile.");
        Require(result.Actions == 4, "Every action must be exercised.");
        Require(result.Findings.Count == 3, "Exactly the three planted defects must be detected.");
        Require(result.Findings.Any(finding => finding.Kind == "CodeFixBreaksCode"), "A newly introduced compiler error must be detected.");
        Require(result.Findings.Any(finding => finding.Kind == "CodeFixDoesNotChangeSolution"), "A no-op must be detected.");
        Require(result.Findings.Any(finding => finding.Kind == "CodeFixException"), "A code action exception must be detected.");
        Require(!result.Findings.Any(finding => finding.Action == "Additional file"), "Adding only an additional file is a real change.");
        var invalid = await engine.RunAsync(new Input("class C { int x = ; }", "C.cs")).ConfigureAwait(false);
        Require(!invalid.Compiles, "Invalid source must still be analyzed.");
        Require(!invalid.Findings.Any(finding => finding.Kind == "CodeFixBreaksCode"), "Invalid input must not trigger the valid-to-invalid oracle.");
        var throwing = new AnalysisEngine([new ThrowingAnalyzer()], []);
        var exceptionResult = await throwing.RunAsync(new Input("class C { }", "C.cs")).ConfigureAwait(false);
        Require(exceptionResult.Findings.Any(finding => finding.Kind == "AnalyzerException"), "Analyzer exceptions must not be swallowed by the driver.");
        Console.WriteLine("Self-tests passed: compiler-error, no-op, action exception, additional-file change, invalid input, analyzer exception.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class TestAnalyzer : DiagnosticAnalyzer
    {
        internal static readonly DiagnosticDescriptor Descriptor = new("FUZZ0001", "Test", "Test", "Fuzz", DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Descriptor];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(context => context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, new TextSpan(0, 1)))));
        }
    }

    private sealed class ThrowingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [TestAnalyzer.Descriptor];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(_ => throw new InvalidOperationException("Planted analyzer exception."));
        }
    }

    private sealed class TestFixer : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ["FUZZ0001"];

        public override FixAllProvider? GetFixAllProvider() => null;

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            context.RegisterCodeFix(CodeAction.Create("Break code", _ => Task.FromResult(document.WithText(SourceText.From("class C { int x = ; }"))), "break"), context.Diagnostics);
            context.RegisterCodeFix(CodeAction.Create("No-op", _ => Task.FromResult(document), "noop"), context.Diagnostics);
            context.RegisterCodeFix(CodeAction.Create("Throw", _ => Task.FromException<Document>(new InvalidOperationException("Planted action exception.")), "throw"), context.Diagnostics);
            context.RegisterCodeFix(CodeAction.Create("Additional file", _ => Task.FromResult(document.Project.Solution.AddAdditionalDocument(DocumentId.CreateNewId(document.Project.Id), "stylecop.json", "{}")), "additional"), context.Diagnostics);
            return Task.CompletedTask;
        }
    }
}
