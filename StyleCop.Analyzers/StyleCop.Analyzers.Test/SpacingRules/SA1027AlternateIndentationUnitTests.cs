// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.SpacingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1027UseTabsCorrectly"/> when <see cref="IndentationSettings.UseTabs"/> is
    /// <see langword="true"/> and <see cref="IndentationSettings.TabSize"/> is set to a non-default value.
    /// </summary>
    public class SA1027AlternateIndentationUnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 6),
                Diagnostic().WithLocation(3, 7),
                Diagnostic().WithLocation(3, 13),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(9, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(1, 8),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(2, 12),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(3, 8),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(6, 22),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(7, 9),
                Diagnostic().WithLocation(7, 42),
                Diagnostic().WithLocation(8, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(5, 13),
                Diagnostic().WithLocation(5, 21),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(9, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(6, 17),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(9, 1),
                Diagnostic().WithLocation(10, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult Diagnostic()
            => StyleCopCodeFixVerifier<SA1027UseTabsCorrectly, SA1027CodeFixProvider>.Diagnostic();

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1027UseTabsCorrectly, SA1027CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                UseTabs = true,
                TabSize = 3,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1027UseTabsCorrectly, SA1027CodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                UseTabs = true,
                TabSize = 3,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
