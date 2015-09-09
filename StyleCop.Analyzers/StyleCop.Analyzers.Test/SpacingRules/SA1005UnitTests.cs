// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
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
        private DocumentationMode documentationMode = DocumentationMode.Diagnose;

        /// <summary>
        /// Verify that a correct single line comment will not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCorrectCommentAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that a single line comment without a leading space gets detected and fixed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoLeadingSpaceAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that a single line comment with multiple leading spaces gets detected and fixed properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleLeadingSpacesAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that multiple leading spaces in a file header do not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleLeadingSpacesInFileHeaderAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that three leading slashes followed by a non-space character do not trigger
        /// a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestThreeLeadingSlashesAsync()
        {
            var testCode = @"///whatever
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that two or more dashes at the start of a comment do not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTwoDashesAsync()
        {
            var testCode = @"//-----------------------
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that a comment that starts with a forward slash not prefixed by a space does not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestForwardSlashNoSpaceAsync()
        {
            var testCode = @"//\whatever
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that an empty comment does not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyCommentAsync()
        {
            var testCode = @"//
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that multiple leading spaces do not trigger a diagnostic on a comment that follows directly
        /// after another single line comment.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndentedSecondCommentLineAsync()
        {
            var testCode = @"// Some comment:
        //     Some indented comment.
        //         Even more indented comment.
        public class SomeClass
        {
        }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that multiple leading spaces trigger a diagnostic on a comment that follows directly
        /// after a blank line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndentedFirstCommentLineAfterBlankLineAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that a commented code will not trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCommentedCodeAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that the diagnostic is not reported for documentation comments
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDocumentationAsync()
        {
            var testCode = @"/// <summary>
/// Some text
/// </summary>
public class Bar
{
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            // Verify that this works if the project was configured to treat documentation comments as regular comments
            this.documentationMode = DocumentationMode.None;

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1005SingleLineCommentsMustBeginWithSingleSpace();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1005CodeFixProvider();
        }

        protected override Solution CreateSolution(ProjectId projectId, string language)
        {
            Solution solution = base.CreateSolution(projectId, language);
            Project project = solution.GetProject(projectId);

            return solution.WithProjectParseOptions(projectId, project.ParseOptions.WithDocumentationMode(this.documentationMode));
        }
    }
}
