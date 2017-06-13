// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;

    public class SA1508CSharp7UnitTests : SA1508UnitTests
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(13, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
