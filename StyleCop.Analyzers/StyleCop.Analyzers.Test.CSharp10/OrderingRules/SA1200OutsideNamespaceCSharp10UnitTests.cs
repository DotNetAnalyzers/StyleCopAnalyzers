﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;

    public partial class SA1200OutsideNamespaceCSharp10UnitTests : SA1200OutsideNamespaceCSharp9UnitTests
    {
        [Fact]
        public async Task TestInvalidUsingStatementsInFileScopedNamespaceAsync()
        {
            var testCode = @"namespace TestNamespace;

{|#0:using System;|}
{|#1:using System.Threading;|}
";
            var fixedTestCode = @"using System;
using System.Threading;

namespace TestNamespace;
";

            DiagnosticResult[] expectedResults =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(0),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
