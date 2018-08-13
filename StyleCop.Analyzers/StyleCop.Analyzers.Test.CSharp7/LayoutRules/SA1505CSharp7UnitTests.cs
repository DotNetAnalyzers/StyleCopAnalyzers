// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    public class SA1505CSharp7UnitTests : SA1505UnitTests
    {
        /// <summary>
        /// Verifies that a valid local function will not produce any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidLocalFunctionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            void LocalFunction()
            {
                this.testField = -this.testField;
            }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid local function will produce the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidLocalFunctionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            void LocalFunction()
            {

                this.testField = -this.testField;
            }
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private int testField;

        public void TestMethod()
        {
            void LocalFunction()
            {
                this.testField = -this.testField;
            }
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(10, 13);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
