// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.LayoutRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.LayoutRules;

    public partial class SA1504CSharp13UnitTests : SA1504CSharp12UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultAccessorWithoutBody()
        {
            return new DiagnosticResult[]
            {
                DiagnosticResult.CompilerError("CS8652").WithMessage("The feature 'field keyword' is currently in Preview and *unsupported*. To use Preview features, use the 'preview' language version.").WithLocation(4, 16),
            };
        }
    }
}
