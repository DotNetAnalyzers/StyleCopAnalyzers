// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

#pragma warning disable xUnit1000 // Test classes must be public
    internal class SA1403UnitTests : FileMayOnlyContainTestBase<SA1403FileMayOnlyContainASingleNamespace, EmptyCodeFixProvider>
#pragma warning restore xUnit1000 // Test classes must be public
    {
        public override string Keyword => "namespace";

        public override bool SupportsCodeFix => false;

        [Fact]
        public async Task TestNestedNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    namespace Bar
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 15);
            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
