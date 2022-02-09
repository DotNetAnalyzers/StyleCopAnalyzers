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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1509OpeningBracesMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1509CodeFixProvider>;

    public class SA1509CSharp7UnitTests : SA1509UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()

        {
        }
    }
}";
            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasTwoBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()


        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = Diagnostic().WithLocation(9, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = Diagnostic().WithLocation(9, 13);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
