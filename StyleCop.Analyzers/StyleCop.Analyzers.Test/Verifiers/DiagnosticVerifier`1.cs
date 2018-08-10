// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;

    internal static class DiagnosticVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        internal static DiagnosticResult[] EmptyDiagnosticResults { get; } = { };

        internal static DiagnosticResult Diagnostic(string diagnosticId = null)
        {
            var analyzer = new TAnalyzer();
            var supportedDiagnostics = analyzer.SupportedDiagnostics;
            if (diagnosticId is null)
            {
                return Diagnostic(supportedDiagnostics.Single());
            }
            else
            {
                return Diagnostic(supportedDiagnostics.Single(i => i.Id == diagnosticId));
            }
        }

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        {
            return new DiagnosticResult(descriptor);
        }

        internal static DiagnosticResult CompilerError(string errorIdentifier)
        {
            return new DiagnosticResult
            {
                Id = errorIdentifier,
                Severity = DiagnosticSeverity.Error,
            };
        }

        internal class CSharpTest : GenericAnalyzerTest
        {
            public override string Language => LanguageNames.CSharp;

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
                => new[] { new TAnalyzer() };

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
                => Enumerable.Empty<CodeFixProvider>();
        }
    }
}
