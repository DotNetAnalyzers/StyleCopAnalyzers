// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public class SA1403UnitTests : FileMayOnlyContainTestBase
    {
        public override string Keyword => "namespace";

        public override bool SupportsCodeFix => false;

        protected override DiagnosticAnalyzer Analyzer => new SA1403FileMayOnlyContainASingleNamespace();

        protected override CodeFixProvider CodeFix => new EmptyCodeFixProvider();

        [Fact]
        public async Task TestNestedNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    namespace Bar
    {

    }
}";

            DiagnosticResult expected = this.Diagnostic().WithLocation(3, 15);
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
