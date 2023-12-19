// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Helpers.LanguageVersionTestExtensions;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    public partial class SA1505CSharp7UnitTests : SA1505UnitTests
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

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        public async Task TestStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc int[]
            {

                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc int[]
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc[]
            {

                1,
                2,
                3
            };
        }
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* v1 = stackalloc[]
            {
                1,
                2,
                3
            };
        }
    }
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3.OrLaterDefault(), testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
