// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;

    public partial class SA1612CSharp9UnitTests : SA1612CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3977, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3977")]
        public async Task TestLambdaDiscardParametersAsync()
        {
            var testCode = @"
/// <summary>Test class.</summary>
public class TestClass
{
    /// <summary>Test method.</summary>
    public void TestMethod()
    {
        System.Func<int, int, int> handler = (_, _) => 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
