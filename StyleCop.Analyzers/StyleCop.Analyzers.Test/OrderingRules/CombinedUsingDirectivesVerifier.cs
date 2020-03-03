// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;

    internal static class CombinedUsingDirectivesVerifier
    {
        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => new DiagnosticResult(descriptor);

        internal class CSharpTest : StyleCopCodeFixVerifier<SA1200UsingDirectivesMustBePlacedCorrectly, UsingCodeFixProvider>.CSharpTest
        {
            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                yield return new SA1200UsingDirectivesMustBePlacedCorrectly();
                yield return new SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives();
                yield return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
                yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
                yield return new SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName();
                yield return new SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation();
                yield return new SA1217UsingStaticDirectivesMustBeOrderedAlphabetically();
            }
        }
    }
}
