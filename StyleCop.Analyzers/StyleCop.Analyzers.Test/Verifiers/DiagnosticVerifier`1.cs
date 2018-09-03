// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
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
            => DiagnosticResult.CompilerError(errorIdentifier);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
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
