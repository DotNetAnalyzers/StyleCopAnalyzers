// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    internal static class StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        internal static DiagnosticResult[] EmptyDiagnosticResults
            => DiagnosticVerifier<TAnalyzer>.EmptyDiagnosticResults;

        internal static DiagnosticResult Diagnostic(string diagnosticId = null)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(descriptor);

        internal static DiagnosticResult CompilerError(string errorIdentifier)
            => DiagnosticVerifier<TAnalyzer>.CompilerError(errorIdentifier);

        internal class CSharpTest : StyleCopDiagnosticVerifier<TAnalyzer>.CSharpTest
        {
            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
            {
                var codeFixProvider = new TCodeFix();
                Assert.NotSame(WellKnownFixAllProviders.BatchFixer, codeFixProvider.GetFixAllProvider());
                return new[] { codeFixProvider };
            }
        }
    }
}
