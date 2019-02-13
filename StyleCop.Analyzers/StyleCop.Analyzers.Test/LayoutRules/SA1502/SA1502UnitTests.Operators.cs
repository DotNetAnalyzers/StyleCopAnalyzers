// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.LayoutRules.SA1502ElementMustNotBeOnASingleLine,
        Analyzers.LayoutRules.SA1502CodeFixProvider>;

    /// <summary>
    /// Unit tests for the operators part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that valid operators will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOperatorsAsync()
        {
            var testCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value)
    {
        return value;
    }

    public static explicit operator TestClass(int value)
    {
        return new TestClass();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operators with their blocks on the same line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOperatorsOnSingleLineAsync()
        {
            var testCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value) { return value; }

    public static explicit operator TestClass(int value) { return new TestClass(); }
}";

            var fixedCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value)
    {
        return value;
    }

    public static explicit operator TestClass(int value)
    {
        return new TestClass();
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 57),
                Diagnostic().WithLocation(5, 58),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operators with their blocks on the next line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOperatorsWithBlockOnNextLineAsync()
        {
            var testCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value)
        { return value; }

    public static explicit operator TestClass(int value)
        { return new TestClass(); }
}";

            var fixedCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value)
    {
        return value;
    }

    public static explicit operator TestClass(int value)
    {
        return new TestClass();
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 9),
                Diagnostic().WithLocation(7, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operators with an expression body will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOperatorsWithExpressionBodyAsync()
        {
            var testCode = @"public class TestClass
{
    public static TestClass operator +(TestClass value) => value;

    public static explicit operator TestClass(int value) => new TestClass();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
