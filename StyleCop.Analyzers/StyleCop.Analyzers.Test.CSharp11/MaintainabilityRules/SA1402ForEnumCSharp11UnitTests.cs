// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using Xunit;

    public partial class SA1402ForEnumCSharp11UnitTests : SA1402ForEnumCSharp10UnitTests
    {
        /// <summary>
        /// Verifies that SA1402 is not reported for file-local enum types.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3803, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3803")]
        public async Task VerifyFileLocalEnumExemptionAsync()
        {
            var testCode = @"namespace TestNamespace;

public class TestClass1 { }

file enum TestEnum { }
";

            await VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
