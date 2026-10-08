// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp11UnitTests : SA1101CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that <c>nameof</c> of a parameter in an attribute on the method, which is allowed from C# 11, is not
        /// reported even if a field has the same name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4003")]
        public async Task TestNameofParameterInMethodAttributeAsync()
        {
            var testCode = @"using System;
using System.Diagnostics.CodeAnalysis;

public class TestClass
{
    private string input;

    [return: NotNullIfNotNull(nameof(input))]
    public string TestMethod(string input) => input;

    [Obsolete(nameof(T))]
    public void Generic<T>()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
