// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1028CodeMustNotContainTrailingWhitespace"/> and
    /// <see cref="SA1028CodeFixProvider"/>.
    /// </summary>
    public class SA1028UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithLocation(5, 36),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 12),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 3),
                this.CSharpDiagnostic().WithLocation(2, 5),
                this.CSharpDiagnostic().WithLocation(3, 6),
                this.CSharpDiagnostic().WithLocation(4, 3),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 14),
                this.CSharpDiagnostic().WithLocation(2, 21),
                this.CSharpDiagnostic().WithLocation(3, 15),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 3),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1373 "SA1028 does not appear to catch single
        /// space after final closing brace":
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1373
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1445 "SA1028 falsely reports when
        /// <c>[assembly: InternalsVisibleTo("...")]</c> is used at the end of file":
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1445
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWhitespaceBeforeClosingBraceAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class ClassName")
                .AppendLine("{")
                .Append(" }")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 13),
                this.CSharpDiagnostic().WithLocation(2, 13),
                this.CSharpDiagnostic().WithLocation(3, 14),
                this.CSharpDiagnostic().WithLocation(4, 8),
                this.CSharpDiagnostic().WithLocation(5, 10),
                this.CSharpDiagnostic().WithLocation(6, 6),
                this.CSharpDiagnostic().WithLocation(7, 7),
                this.CSharpDiagnostic().WithLocation(8, 22),
                this.CSharpDiagnostic().WithLocation(9, 20),
                this.CSharpDiagnostic().WithLocation(10, 11),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task NoTrailingWhitespaceAfterBlockCommentAsync()
        {
            string testCode = new StringBuilder()
                .AppendLine("class Program    /* some block comment that follows several spaces */")
                .AppendLine("{")
                .AppendLine("}")
                .ToString();

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 10);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1028CodeMustNotContainTrailingWhitespace();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1028CodeFixProvider();
        }
    }
}
