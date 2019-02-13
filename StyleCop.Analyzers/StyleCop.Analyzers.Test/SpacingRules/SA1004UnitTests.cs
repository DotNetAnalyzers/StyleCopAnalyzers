// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1004DocumentationLinesMustBeginWithSingleSpace,
        Analyzers.SpacingRules.SA1004CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>.
    /// </summary>
    public class SA1004UnitTests
    {
        [Fact]
        public async Task TestFixedExampleAsync()
        {
            string testCode = @"
public class TypeName
{
    /// <summary>
    /// The summary text.
    /// </summary>
    /// <param name=""x"">The document root.</param>
    /// <param name=""y"">The XML header token.</param>
    private void Method1(int x, int y)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRuleExampleAsync()
        {
            string testCode = @"
public class TypeName
{
    ///<summary>
    ///     The summary text.
    ///</summary>
    ///   <param name=""x"">The document root.</param>
    ///    <param name=""y"">The XML header token.</param>
    private void Method1(int x, int y)
    {
    }
}
";

            string fixedCode = @"
public class TypeName
{
    /// <summary>
    ///     The summary text.
    /// </summary>
    /// <param name=""x"">The document root.</param>
    /// <param name=""y"">The XML header token.</param>
    private void Method1(int x, int y)
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 8),
                Diagnostic().WithLocation(6, 8),
                Diagnostic().WithLocation(7, 8),
                Diagnostic().WithLocation(8, 8),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedElementIndentationAsync()
        {
            string testCode = @"
public class TypeName
{
    /// <summary>
    /// The summary text.
    /// </summary>
    /// <remarks>
    ///<ul>
    ///<li>Item 1</li>
    /// <li>Item 2</li>
    ///  <li>Item 3</li>
    ///</ul>
    /// </remarks>
    private void Method1()
    {
    }
}
";

            string fixedCode = @"
public class TypeName
{
    /// <summary>
    /// The summary text.
    /// </summary>
    /// <remarks>
    /// <ul>
    /// <li>Item 1</li>
    /// <li>Item 2</li>
    ///  <li>Item 3</li>
    /// </ul>
    /// </remarks>
    private void Method1()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 8),
                Diagnostic().WithLocation(9, 8),
                Diagnostic().WithLocation(12, 8),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyCommentLinesAsync()
        {
            string testCode = @"
public class TypeName
{
    /// <summary>
    ///
    /// </summary>
    private void Method1()
    {
    }

    /// <summary>
    /// </summary>
    ///
    private void Method2()
    {
    }

    ///
    /// <summary>
    /// </summary>
    private void Method3()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi-line documentation comment without leading spaces is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultilineDocumentationCommentWithoutLeadingSpacesAsync()
        {
            string testCode = @"
public class TypeName
{
    /**
     *<summary>
     *     The summary text.
     *</summary>
     *   <param name=""x"">The document root.</param>
     *    <param name=""y"">The XML header token.</param>
     */
    private void Method1(int x, int y)
    {
    }
}
";

            string fixedCode = @"
public class TypeName
{
    /**
     * <summary>
     *     The summary text.
     * </summary>
     * <param name=""x"">The document root.</param>
     * <param name=""y"">The XML header token.</param>
     */
    private void Method1(int x, int y)
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 7),
                Diagnostic().WithLocation(7, 7),
                Diagnostic().WithLocation(8, 7),
                Diagnostic().WithLocation(9, 7),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
