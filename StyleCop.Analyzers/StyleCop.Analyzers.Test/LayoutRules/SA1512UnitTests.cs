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
        Analyzers.LayoutRules.SA1512SingleLineCommentsMustNotBeFollowedByBlankLine,
        Analyzers.LayoutRules.SA1512CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1512SingleLineCommentsMustNotBeFollowedByBlankLine"/>.
    /// </summary>
    public class SA1512UnitTests
    {
        /// <summary>
        /// Verifies that all known types valid single line comment lines will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSingleLineCommentsAsync()
        {
            var testCode = @"// A single line comment at the start of the file is valid
namespace Foo
{
    public class Bar
    {
        // A single line comment at the start of the scope is valid
        private int field1;

        // This is valid as well ofcourse
        private int field2;
        private int field3; // This should not trigger ofcourse

#if (SPECIALTEST)
        // This is allowed because the statement is disabled by the directive

        private int field4;
#else
        // this is also allowed
        private double field4;
#endif

        // Two single line comments separated by

        // a single empty line are valid ofcourse
        private int field5;

        // Multiple single line comments
        // directly after each other are valid as well
        public int Baz()
        {
            var x = field1;

            ////return 0;

            return x;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid single line comment lines will produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSingleLineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid

        private int field2;

        public int Baz(int x)
        {
            // invalid as well

            return x;
        }
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid
        private int field2;

        public int Baz(int x)
        {
            // invalid as well
            return x;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(5, 9),
                Diagnostic().WithLocation(11, 13),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that single line comment followed by a blank line with spurious whitespace will produce the expected diagnostic and gets fixed correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSingleLineCommentWithSpuriousWhitespaceAsync()
        {
            var testCode =
                "namespace Foo\r\n" +
                "{\r\n" +
                "    public class Bar\r\n" +
                "    {\r\n" +
                "        // This is invalid\r\n" +
                "        \r\n" +
                "        private int field2;\r\n" +
                "    }\r\n" +
                "}\r\n";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid
        private int field2;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(5, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line comment followed by multiple blank lines will not produce the correct diagnostic when SA1507 is enabled or not.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <remarks><para>This cannot be tested properly until #522 is available, for now only situation where SA1507
        /// is enabled is tested.</para></remarks>
        [Fact]
        public async Task TestSingleLineCommentFollowedByMultipleBlankLinesAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid


        private int field2;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a standard file header will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStandardHeaderAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company =""FooBar Inc."">
//     Copyright (c) FooBar Inc. All rights reserved.
// </copyright>

namespace Foo
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a non-standard file header will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNonStandardHeaderAsync()
        {
            var testCode = @"// This file was originally obtained from 
// httpx://github.com/???/Test0.cs
// It is subject to {some license}
// This file has been modified since obtaining it from its original source.

namespace Foo
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the first comment (followed by a blank line) after a standard file header will produce the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStandardHeaderFollowedBySingleLineCommentWithTrailingBlankLineAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company =""FooBar Inc."">
//     Copyright (c) FooBar Inc. All rights reserved.
// </copyright>

// This is not part of the file header!

namespace Foo
{
}
";

            var fixedTestCode = @"// <copyright file=""Test0.cs"" company =""FooBar Inc."">
//     Copyright (c) FooBar Inc. All rights reserved.
// </copyright>

// This is not part of the file header!
namespace Foo
{
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                Diagnostic().WithLocation(5, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle comments followed by single line documentation comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCommentFollowedBySingleLineDocumenationCommentAsync()
        {
            var testCode = @"// some comment
                
/// <summary>Test summary.</summary>
public class TestClass
{
    // another comment

    /// <summary>Test summary.</summary>
    public void TestMethod() { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle comments followed by multi-line documentation comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCommentFollowedByMultiLineDocumentationCommentAsync()
        {
            var testCode = @"// some comment
                
/** <summary>Test summary.</summary> */
public class TestClass
{
    // another comment

    /** <summary>Test summary.</summary> */
    public void TestMethod() { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle comments followed by multi-line comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCommentFollowedByMultiLineCommentAsync()
        {
            var testCode = @"namespace TestNamespace {
    // some comment
                
    /* multi-line comment */
    internal class TestClass
    {
        // another comment

        /* another multi-line comment */
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
