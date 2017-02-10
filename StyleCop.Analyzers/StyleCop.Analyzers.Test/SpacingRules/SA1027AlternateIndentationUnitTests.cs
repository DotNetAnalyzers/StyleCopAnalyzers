// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1027UseTabsCorrectly"/> when <see cref="IndentationSettings.UseTabs"/> is
    /// <see langword="true"/> and <see cref="IndentationSettings.TabSize"/> is set to a non-default value.
    /// </summary>
    public class SA1027AlternateIndentationUnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that tabs used inside string and char literals are not producing diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidTabsAsync()
        {
            var testCode =
                "public class Foo\r\n" +
                "{\r\n" +
                "\tpublic const string ValidTestString = \"\tText\";\r\n" +
                "\tpublic const char ValidTestChar = '\t';\r\n" +
                "}\r\n";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spaces used inside disabled code are not producing diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDisabledCodeAsync()
        {
            var testCode =
                "public class Foo\r\n" +
                "{\r\n" +
                "#if false\r\n" +
                "    public const string ValidTestString = \"Text\";\r\n" +
                "    public const char ValidTestChar = 'c';\r\n" +
                "#endif\r\n" +
                "}\r\n";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSpacesAsync()
        {
            var testCode =
                "using\tSystem.Diagnostics;\r\n" +
                "\r\n" +
                "public\tclass\tFoo\r\n" +
                "{\r\n" +
                "    public void Bar()\r\n" +
                "    {\r\n" +
                "  \t  \t// Comment\r\n" +
                "\t \tDebug.Indent();\r\n" +
                "   \t}\r\n" +
                "}\r\n";

            var fixedTestCode =
                "using System.Diagnostics;\r\n" +
                "\r\n" +
                "public   class Foo\r\n" +
                "{\r\n" +
                "\t public void Bar()\r\n" +
                "\t {\r\n" +
                "\t\t// Comment\r\n" +
                "\t\tDebug.Indent();\r\n" +
                "\t\t}\r\n" +
                "}\r\n";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 6),
                this.CSharpDiagnostic().WithLocation(3, 7),
                this.CSharpDiagnostic().WithLocation(3, 13),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(9, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTabsInDocumentationCommentsAsync()
        {
            var testCode =
                "    ///\t<summary>\r\n" +
                "    /// foo\tbar\r\n" +
                "    ///\t</summary>\r\n" +
                "    public class Foo\r\n" +
                "    {\r\n" +
                "     \t/// <MyElement>\tValue </MyElement>\r\n" +
                "    \t/**\t \t<MyElement> Value </MyElement>\t*/\r\n" +
                "    }\r\n";

            var fixedTestCode =
                "\t ///  <summary>\r\n" +
                "\t /// foo bar\r\n" +
                "\t ///  </summary>\r\n" +
                "\t public class Foo\r\n" +
                "\t {\r\n" +
                "\t\t/// <MyElement>   Value </MyElement>\r\n" +
                "\t\t/**      <MyElement> Value </MyElement>   */\r\n" +
                "\t }\r\n";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(1, 8),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(2, 12),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(3, 8),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(6, 22),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(7, 9),
                this.CSharpDiagnostic().WithLocation(7, 42),
                this.CSharpDiagnostic().WithLocation(8, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTabsInCommentsAsync()
        {
            var testCode =
                "    public class Foo\r\n" +
                "    {\r\n" +
                "        public void Bar()\r\n" +
                "        {\r\n" +
                "         \t//\tComment\t\t1\r\n" +
                "\t\t\t////\tCommented Code\t\t1\r\n" +
                "    \t  \t// Comment 2\r\n" +
                "        }\r\n" +
                "    }\r\n";

            var fixedTestCode =
                "\t public class Foo\r\n" +
                "\t {\r\n" +
                "\t\t  public void Bar()\r\n" +
                "\t\t  {\r\n" +
                "\t\t\t\t// Comment     1\r\n" +
                "\t\t\t////\tCommented Code\t\t1\r\n" +
                "\t\t\t// Comment 2\r\n" +
                "\t\t  }\r\n" +
                "\t }\r\n";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(5, 13),
                this.CSharpDiagnostic().WithLocation(5, 21),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(9, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTabsInMultiLineCommentsAsync()
        {
            var testCode =
                "    public class Foo\r\n" +
                "    {\r\n" +
                "        public void Bar()\r\n" +
                "        {\r\n" +
                "         \t/*\r\n" +
                "        \tComment\t\t1\r\n" +
                "      \t\tComment 2\r\n" +
                "\t  \t    */\r\n" +
                "        }\r\n" +
                "    }\r\n";

            var fixedTestCode =
                "\t public class Foo\r\n" +
                "\t {\r\n" +
                "\t\t  public void Bar()\r\n" +
                "\t\t  {\r\n" +
                "\t\t\t\t/*\r\n" +
                "\t\t\tComment     1\r\n" +
                "\t\t\t\tComment 2\r\n" +
                "\t\t\t */\r\n" +
                "\t\t  }\r\n" +
                "\t }\r\n";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(6, 17),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(9, 1),
                this.CSharpDiagnostic().WithLocation(10, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings() =>
            @"
{
    ""settings"": {
        ""indentation"": {
            ""useTabs"": true,
            ""tabSize"": 3
        }
    }
}
";

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1027UseTabsCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1027CodeFixProvider();
        }
    }
}
