// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1408ConditionalExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    public class SA1408CSharp7UnitTests : SA1408UnitTests
    {
        /// <summary>
        /// Verifies that a code fix for SA1119 in a pattern matching expression does not trigger SA1408.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1119CSharp7UnitTests.TestPatternMatchingAsync"/>
        [Fact]
        public async Task TestPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if (new object() is bool b && b)
        {
            return;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
