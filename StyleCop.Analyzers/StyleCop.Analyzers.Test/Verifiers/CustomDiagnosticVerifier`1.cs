// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;

    internal static class CustomDiagnosticVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        internal static DiagnosticResult[] EmptyDiagnosticResults
            => DiagnosticVerifier<TAnalyzer>.EmptyDiagnosticResults;

        internal static DiagnosticResult Diagnostic(string diagnosticId = null)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(descriptor);
    }
}
