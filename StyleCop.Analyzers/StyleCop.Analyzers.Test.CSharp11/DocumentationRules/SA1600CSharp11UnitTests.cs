// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;

    public partial class SA1600CSharp11UnitTests : SA1600CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestRecordPrimaryConstructor(DiagnosticResult[] results)
        {
            // Roslyn bug fix: Diagnostics are reported twice in the C# 9 and C# 10 tests, but this is fixed in C# 11.
            return results;
        }
    }
}
