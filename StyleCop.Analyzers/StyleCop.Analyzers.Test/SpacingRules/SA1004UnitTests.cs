// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1004DocumentationLinesMustBeginWithSingleSpace,
        StyleCop.Analyzers.SpacingRules.SA1004CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>.
    /// </summary>
    public class SA1004UnitTests
    {
        public static IEnumerable<object[]> ParameterModifiers
        {
            get
            {
                yield return new[] { "out" };
                yield return new[] { "ref" };

                if (LightupHelpers.SupportsCSharp72)
                {
                    yield return new[] { "in" };
                }
            }
        }

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

        [Theory]
        [MemberData(nameof(ParameterModifiers))]
        [WorkItem(3817, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3817")]
        public async Task TestParameterModifierFirstOnLineAsync(string keyword)
        {
            string testCode = $@"
/// <summary>
/// Description of some remarks that refer to a method: <see cref=""SomeMethod(int, int,
/// {keyword} string)""/>.
/// </summary>
public class TypeName
{{
    public void SomeMethod(int x, int y, {keyword} string z)
    {{
        throw new System.Exception();
    }}
}}";

            var languageVersion = (LightupHelpers.SupportsCSharp8, LightupHelpers.SupportsCSharp72) switch
            {
                // Make sure to use C# 7.2 if supported, unless we are going to default to something greater
                (false, true) => LanguageVersionEx.CSharp7_2,
                _ => (LanguageVersion?)null,
            };

            await VerifyCSharpDiagnosticAsync(languageVersion, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
