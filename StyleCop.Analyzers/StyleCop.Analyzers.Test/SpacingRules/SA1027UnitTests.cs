// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1027UseTabsCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1027CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1027UseTabsCorrectly"/>.
    /// </summary>
    public class SA1027UnitTests
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
                "    public const string ValidTestString = \"\tText\";\r\n" +
                "    public const char ValidTestChar = '\t';\r\n" +
                "}\r\n";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that tabs used inside disabled code are not producing diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDisabledCodeAsync()
        {
            var testCode =
                "public class Foo\r\n" +
                "{\r\n" +
                "#if false\r\n" +
                "\tpublic const string ValidTestString = \"Text\";\r\n" +
                "\tpublic const char ValidTestChar = 'c';\r\n" +
                "#endif\r\n" +
                "}\r\n";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTabsAsync()
        {
            var testCode =
                "using\tSystem.Diagnostics;\r\n" +
                "\r\n" +
                "public\tclass\tFoo\r\n" +
                "{\r\n" +
                "\tpublic void Bar()\r\n" +
                "\t{\r\n" +
                "\t  \t// Comment\r\n" +
                "\t \tDebug.Indent();\r\n" +
                "   \t}\r\n" +
                "}\r\n";

            var fixedTestCode = @"using   System.Diagnostics;

public  class   Foo
{
    public void Bar()
    {
        // Comment
        Debug.Indent();
    }
}
";

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
                "\t///\t<summary>\r\n" +
                "\t/// foo\tbar\r\n" +
                "\t///\t</summary>\r\n" +
                "\tpublic class Foo\r\n" +
                "\t{\r\n" +
                "\t \t/// <MyElement>\tValue </MyElement>\r\n" +
                "\t\t/**\t \t<MyElement> Value </MyElement>\t*/\r\n" +
                "\t}\r\n";

            var fixedTestCode = @"    /// <summary>
    /// foo bar
    /// </summary>
    public class Foo
    {
        /// <MyElement> Value </MyElement>
        /**     <MyElement> Value </MyElement>  */
    }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(1, 5),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(2, 9),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(6, 19),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(7, 6),
                Diagnostic().WithLocation(7, 39),
                Diagnostic().WithLocation(8, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTabsInCommentsAsync()
        {
            var testCode =
                "\tpublic class Foo\r\n" +
                "\t{\r\n" +
                "\t\tpublic void Bar()\r\n" +
                "\t\t{\r\n" +
                "\t\t \t//\tComment\t\t1\r\n" +
                "            ////\tCommented Code\t\t1\r\n" +
                "\t  \t\t// Comment 2\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n";

            var fixedTestCode =
                "    public class Foo\r\n" +
                "    {\r\n" +
                "        public void Bar()\r\n" +
                "        {\r\n" +
                "            //  Comment     1\r\n" +
                "            ////\tCommented Code\t\t1\r\n" +
                "            // Comment 2\r\n" +
                "        }\r\n" +
                "    }\r\n";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(5, 7),
                Diagnostic().WithLocation(5, 15),
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
                "\tpublic class Foo\r\n" +
                "\t{\r\n" +
                "\t\tpublic void Bar()\r\n" +
                "\t\t{\r\n" +
                "\t\t \t/*\r\n" +
                "\t\t\tComment\t\t1\r\n" +
                "\t  \t\tComment 2\r\n" +
                "  \t\t\t*/\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n";

            var fixedTestCode = @"    public class Foo
    {
        public void Bar()
        {
            /*
            Comment     1
            Comment 2
            */
        }
    }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(6, 11),
                Diagnostic().WithLocation(7, 1),
                Diagnostic().WithLocation(8, 1),
                Diagnostic().WithLocation(9, 1),
                Diagnostic().WithLocation(10, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
