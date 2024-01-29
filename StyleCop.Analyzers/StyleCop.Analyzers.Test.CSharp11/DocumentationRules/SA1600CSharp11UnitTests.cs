// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented>;

    public partial class SA1600CSharp11UnitTests : SA1600CSharp10UnitTests
    {

        /* These methods reflect a fix for a Roslyn issue from C# 9 where diagnostics were reported twice. */

        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructorNoParameterDocumentation()
        {
            return new[]
            {
                // /0/Test0.cs(5,28): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(0),

                // /0/Test0.cs(5,43): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(1),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructorNoDocumentation()
        {
            return new[]
            {
                // /0/Test0.cs(2,15): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(0),

                // /0/Test0.cs(2,28): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(1),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructorPartialParameterDocumentation()
        {
            return new[]
            {
                // /0/Test0.cs(6,43): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructorIncludePartialParameterDocumentation()
        {

            return new[]
            {
                // /0/Test0.cs(3,28): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructorIncludeMissingParameterDocumentation()
        {
            return new[]
            {
                // /0/Test0.cs(3,28): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(0),

                // /0/Test0.cs(3,43): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(1),

                // /0/Test0.cs(3,56): warning SA1600: Elements should be documented
                Diagnostic().WithLocation(2),
            };
        }
    }
}
