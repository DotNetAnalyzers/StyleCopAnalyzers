// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer>;

    public partial class SA1620CSharp11UnitTests : SA1620CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestGenericTypeWithPrimaryConstructorWithWrongTypeParameterName()
        {
            return new[]
            {
                Diagnostic(StyleCop.Analyzers.DocumentationRules.GenericTypeParameterDocumentationAnalyzer.SA1620MissingTypeParameterDescriptor).WithLocation(0).WithArguments("U"),
            };
        }
    }
}
