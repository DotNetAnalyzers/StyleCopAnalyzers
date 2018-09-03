// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1028CodeMustNotContainTrailingWhitespace,
        StyleCop.Analyzers.SpacingRules.SA1028CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1028CodeMustNotContainTrailingWhitespace"/> and
    /// <see cref="SA1028CodeFixProvider"/>.
    /// </summary>
    public class SA1028UnitTests
    {
        [Fact]
        public async Task TrailingWhitespaceAfterStatementAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    void MethodName()")
                .AppendLine("    {")
                .AppendLine("        System.Console.WriteLine(); ")
                .AppendLine("    }")
                .AppendLine("}")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    void MethodName()")
                .AppendLine("    {")
                .AppendLine("        System.Console.WriteLine();")
                .AppendLine("    }")
                .AppendLine("}")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 36),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDeclarationAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName ")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterSingleLineCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("// hi there    ")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("// hi there")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 12),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideMultiLineCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/* ")
                .AppendLine(" foo   ")
                .AppendLine("  bar   ")
                .AppendLine("*/  ")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("/*")
                .AppendLine(" foo")
                .AppendLine("  bar")
                .AppendLine("*/")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 3),
                Diagnostic().WithLocation(2, 5),
                Diagnostic().WithLocation(3, 6),
                Diagnostic().WithLocation(4, 3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceInsideXmlDocCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/// <summary>  ")
                .AppendLine("/// Some description    ")
                .AppendLine("/// </summary>  ")
                .AppendLine("class Foo { }")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("/// <summary>")
                .AppendLine("/// Some description")
                .AppendLine("/// </summary>")
                .AppendLine("class Foo { }")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 14),
                Diagnostic().WithLocation(2, 21),
                Diagnostic().WithLocation(3, 15),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceWithinAndAfterHereStringAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    string foo = @\"      ")
                .AppendLine("more text    ")
                .AppendLine("\";  ")
                .AppendLine("}")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .AppendLine("    string foo = @\"      ")
                .AppendLine("more text    ")
                .AppendLine("\";")
                .AppendLine("}")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for "SA1028 does not appear to catch single space after final closing brace".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1373, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1373")]
        public async Task TestTrailingWhitespaceAfterClosingBraceAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .Append("} ")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .Append("}")
                .ToString();

            DiagnosticResult expected = Diagnostic().WithLocation(3, 2);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for "SA1028 falsely reports when <c>[assembly: InternalsVisibleTo("...")]</c> is
        /// used at the end of file".
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1445, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1445")]
        public async Task TestWhitespaceBeforeClosingBraceAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .Append(" }")
                .ToString();

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterDirectivesAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("#define Zoot  ")
                .AppendLine("#undef Zoot2  ")
                .AppendLine("using System;  ")
                .AppendLine("#if Foo  ")
                .AppendLine("#elif Bar  ")
                .AppendLine("#else  ")
                .AppendLine("#endif ")
                .AppendLine("#warning Some warning  ")
                .AppendLine("#region Some region  ")
                .AppendLine("#endregion  ")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("#define Zoot")
                .AppendLine("#undef Zoot2")
                .AppendLine("using System;")
                .AppendLine("#if Foo")
                .AppendLine("#elif Bar")
                .AppendLine("#else")
                .AppendLine("#endif")
                .AppendLine("#warning Some warning")
                .AppendLine("#region Some region")
                .AppendLine("#endregion")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 13),
                Diagnostic().WithLocation(2, 13),
                Diagnostic().WithLocation(3, 14),
                Diagnostic().WithLocation(4, 8),
                Diagnostic().WithLocation(5, 10),
                Diagnostic().WithLocation(6, 6),
                Diagnostic().WithLocation(7, 7),
                Diagnostic().WithLocation(8, 22),
                Diagnostic().WithLocation(9, 20),
                Diagnostic().WithLocation(10, 11),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceFoundWithinFalseConditionalDirectiveBlocksAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("#if false")
                .AppendLine("using System;  ")
                .AppendLine("#endif")
                .ToString();

            // Note: we verify that no diagnostics are produced inside non-compiled blocks
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task NoTrailingWhitespaceAfterBlockCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class Program    /* some block comment that follows several spaces */")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TrailingWhitespaceAfterTextTokenAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/// <summary>")
                .AppendLine("/// &amp; ")
                .AppendLine("/// </summary>")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("/// <summary>")
                .AppendLine("/// &amp;")
                .AppendLine("/// </summary>")
                .ToString();

            DiagnosticResult expected = Diagnostic().WithLocation(2, 10);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that trailing whitespace after a multi-line documentation comment is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(821, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/821")]
        public async Task VerifyTrailingWhitespaceInsideMultiLineXmlDocumentationCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("/**")
                .AppendLine(" * <summary>  ")
                .AppendLine(" * Some description    ")
                .AppendLine(" * </summary>  ")
                .AppendLine(" */")
                .AppendLine("class Foo { }")
                .ToString();

            string fixedCode = new StringBuilder()
                .AppendLine("/**")
                .AppendLine(" * <summary>")
                .AppendLine(" * Some description")
                .AppendLine(" * </summary>")
                .AppendLine(" */")
                .AppendLine("class Foo { }")
                .ToString();

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(2, 13),
                Diagnostic().WithLocation(3, 20),
                Diagnostic().WithLocation(4, 14),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
