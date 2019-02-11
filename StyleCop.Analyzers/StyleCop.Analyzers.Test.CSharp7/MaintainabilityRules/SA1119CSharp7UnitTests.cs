// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis>;

    public class SA1119CSharp7UnitTests : SA1119UnitTests
    {
        /// <summary>
        /// Verifies that extra parentheses in pattern matching is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1408CSharp7UnitTests.TestPatternMatchingAsync"/>
        [Fact]
        [WorkItem(2372, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2398")]
        public async Task TestPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        if ((new object() is bool b) && b)
        {
            return;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2372, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2372")]
        public async Task TestNegatedPatternMatchingAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        object obj = null;
        if (!(obj is string anythng))
        {
            // ...
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTupleDeconstructionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var (a, (b, c), d) = (1, (2, (3)), 4);
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        var (a, (b, c), d) = (1, (2, 3), 4);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(5, 38, 5, 41),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 38),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 40),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
