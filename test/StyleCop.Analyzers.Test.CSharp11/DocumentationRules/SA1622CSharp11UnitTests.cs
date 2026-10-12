// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer>;

    public partial class SA1622CSharp11UnitTests : SA1622CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestGenericTypeWithPrimaryConstructorTypeParameterWithoutText()
        {
            return new[]
            {
                Diagnostic(StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer.SA1622Descriptor).WithLocation(0),
            };
        }
    }
}
