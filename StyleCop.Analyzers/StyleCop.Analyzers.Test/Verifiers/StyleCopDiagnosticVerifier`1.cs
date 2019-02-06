// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;

    internal static class StyleCopDiagnosticVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        internal static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>.Diagnostic();

        internal static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => new DiagnosticResult(descriptor);

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

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(languageVersion, source, new[] { expected }, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(languageVersion)
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal class CSharpTest : StyleCopCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider>.CSharpTest
        {
            public CSharpTest()
                : this(languageVersion: null)
            {
            }

            public CSharpTest(LanguageVersion? languageVersion)
            {
                if (languageVersion != null)
                {
                    this.SolutionTransforms.Add((solution, projectId) =>
                    {
                        var parseOptions = (CSharpParseOptions)solution.GetProject(projectId).ParseOptions;
                        return solution.WithProjectParseOptions(projectId, parseOptions.WithLanguageVersion(languageVersion.Value));
                    });
                }
            }
        }
    }
}
