// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>.
    /// </summary>
    public class SA1507UnitTests : CodeFixVerifier
    {
        private const string TestCode = @"namespace MyTest
{


    using System;


    using System.Collections;

    public class Foo
    {


        public void Bar()
        {


#if !IGNORE


        return;
#else


        return;
#endif


/*


*/
        }
    }


}
";

        /// <summary>
        /// Verifies that empty lines at the start of the file do not trigger any diagnostics.
        /// (This will be handled by SA1517)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLinesAtStartOfFileAsync()
        {
            var testCode = @"

public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty lines at the end of the file do not trigger any diagnostics.
        /// (This will be handled by SA1518)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLinesAtEndOfFileAsync()
        {
            var testCode = @"public class Foo
{
}


";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneEmptyLineBetweenMultilineCommentAndFirstElementAsync()
        {
            string testCode = @"/*
*/

namespace Microsoft
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneEmptyLineBetweenSingleLineCommentAndFirstElementAsync()
        {
            string testCode = @"//

namespace Microsoft
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleEmptyLinesBetweenMultilineCommentAndFirstElementAsync()
        {
            string testCode = @"/*
*/


namespace Microsoft
{
}
";

            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a verbatim string literal does not trigger any diagnostics.
        /// (This will be handled by SA1518)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestVerbatimStringLiteralAsync()
        {
            var testCode = @"public class Foo
{
    private string testCode = @""namespace Bar
{


    using System;
}
"";

}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validate that all invalid multiple blank lines are reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidMultipleBlankLinesAsync()
        {
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(16, 1),
                this.CSharpDiagnostic().WithLocation(19, 1),
                /* line 23 should not report a diagnostic, as it's part of the directive is inactive */
                this.CSharpDiagnostic().WithLocation(27, 1),
                /* line 30 should not report a diagnostic, as it's part of a comment */
                this.CSharpDiagnostic().WithLocation(35, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(TestCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Validate that the code fixes for all invalid multiple blank lines works properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidMultipleBlankLinesCodeFixAsync()
        {
            var fixedTestCode = @"namespace MyTest
{

    using System;

    using System.Collections;

    public class Foo
    {

        public void Bar()
        {

#if !IGNORE

        return;
#else


        return;
#endif

/*


*/
        }
    }

}
";

            await this.VerifyCSharpFixAsync(TestCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidBlankLineInVariousPlacesAsync()
        {
            string testCode = @"using System;

class FooBar
{
    void Foo()
    {
    }

    void Bar()
    {
        Foo();

        Foo();
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1507CodeMustNotContainMultipleBlankLinesInARow();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1507CodeFixProvider();
        }
    }
}
