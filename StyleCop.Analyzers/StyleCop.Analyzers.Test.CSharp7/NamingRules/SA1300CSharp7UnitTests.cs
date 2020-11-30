// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1300CSharp7UnitTests : SA1300UnitTests
    {
        [Fact]
        public async Task TestUpperCaseLocalFunctionAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        void LocalFunction()
        {
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseLocalFunctionAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        void localFunction()
        {
        }
    }
}";
            var fixedCode = @"public class TestClass
{
    public void Method()
    {
        void LocalFunction()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("localFunction").WithLocation(5, 14);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLowerCaseLocalFunctionWithConflictAsync()
        {
            // Conflict resolution does not attempt to examine overloaded methods.
            var testCode = @"public class TestClass
{
    public void Method()
    {
        void localFunction()
        {
        }

        int LocalFunction(int value) => value;
    }
}";
            var fixedCode = @"public class TestClass
{
    public void Method()
    {
        void LocalFunction1()
        {
        }

        int LocalFunction(int value) => value;
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("localFunction").WithLocation(5, 14);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
