// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1027UseTabsCorrectly"/>
    /// </summary>
    public class SA1027UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(1, 5),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(2, 9),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(6, 19),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(7, 6),
                this.CSharpDiagnostic().WithLocation(7, 39),
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
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(5, 7),
                this.CSharpDiagnostic().WithLocation(5, 15),
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
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(6, 11),
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
