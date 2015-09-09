// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1403UnitTests : FileMayOnlyContainTestBase
    {
        public override string Keyword
        {
            get
            {
                return "namespace";
            }
        }

        [Fact]
        public async Task TestNestedNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    namespace Bar
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 15);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1403FileMayOnlyContainASingleNamespace();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            throw new NotSupportedException();
        }
    }
}
