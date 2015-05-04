namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit test for <see cref="SA1005SingleLineCommentsMustBeginWithSingleSpace"/>
    /// </summary>
    public class SA1005UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that a correct single line comment will not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCorrectComment()
        {
            var testCode = @"public class Foo
{
    //
    // Correct comment
    //
    public class Bar
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that a single line comment without a leading space gets detected and fixed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoLeadingSpace()
        {
            var testCode = @"public class Foo
{
    //Wrong comment
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"public class Foo
{
    // Wrong comment
    public class Bar
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(3, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verify that a single line comment with multiple leading spaces gets detected and fixed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleLeadingSpaces()
        {
            var testCode = @"public class Foo
{
    //   Wrong comment
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"public class Foo
{
    // Wrong comment
    public class Bar
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(3, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verify that multiple leading spaces in a file header do not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleLeadingSpacesInFileHeader()
        {
            var testCode = @"// --------------------------------------------------------------------------------------------------------------------
// <copyright file=""SomeClass.cs"" company=""SomeCompany"">
//   Copyright © 2015 Some Company.
//   All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that three leading slashes followed by a non-space character do not trigger
        /// a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestThreeLeadingSlashes()
        {
            var testCode = @"///whatever
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that two or more dashes at the start of a comment do not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTwoDashes()
        {
            var testCode = @"//-----------------------
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that a comment that starts with a forward slash not prefixed by a space does not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestForwardSlashNoSpace()
        {
            var testCode = @"//\whatever
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that an empty comment does not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyComment()
        {
            var testCode = @"//
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that multiple leading spaces do not trigger a diagnostic on a comment that follows directly
        /// after another single line comment.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndentedSecondCommentLine()
        {
            var testCode = @"// Some comment:
        //     Some indented comment.
        //         Even more indented comment.
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verify that multiple leading spaces trigger a diagnostic on a comment that follows directly
        /// after a blank line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndentedFirstCommentLineAfterBlankLine()
        {
            var testCode = @"// Some comment:

        //         Even more indented comment.
        public class SomeClass
        {
        }
";

            var fixedTestCode = @"// Some comment:

        // Even more indented comment.
        public class SomeClass
        {
        }
";

            var expected = this.CSharpDiagnostic().WithLocation(3, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verify that a commented code will not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCommentedCode()
        {
            var testCode = @"public class Foo
{
    public class Bar
    {
        ////private int a;
////        private int b;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1005SingleLineCommentsMustBeginWithSingleSpace();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1005CodeFixProvider();
        }
    }
}
