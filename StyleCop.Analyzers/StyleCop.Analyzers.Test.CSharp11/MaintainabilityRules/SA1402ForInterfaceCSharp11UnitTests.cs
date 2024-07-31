﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using Xunit;

    public partial class SA1402ForInterfaceCSharp11UnitTests : SA1402ForInterfaceCSharp10UnitTests
    {
        /// <summary>
        /// Verifies that SA1402 is not reported for file-local interface types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3803, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3803")]
        public async Task VerifyFileLocalInterfaceExemptionAsync()
        {
            var testCode = $@"namespace TestNamespace;

public interface TestInterface1 {{ }}

file interface TestInterface2 {{ }}
";

            var expectedDiagnostic = this.Diagnostic().WithLocation(0);

            await this.VerifyCSharpDiagnosticAsync(
                testCode,
                this.GetSettings(),
                Array.Empty<DiagnosticResult>(),
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
