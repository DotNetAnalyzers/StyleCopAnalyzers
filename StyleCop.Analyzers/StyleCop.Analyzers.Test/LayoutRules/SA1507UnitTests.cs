// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1507CodeMustNotContainMultipleBlankLinesInARow,
        StyleCop.Analyzers.LayoutRules.SA1507CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>.
    /// </summary>
    public class SA1507UnitTests
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
        /// (This will be handled by SA1517).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLinesAtStartOfFileAsync()
        {
            var testCode = @"

public class Foo
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty lines at the end of the file do not trigger any diagnostics.
        /// (This will be handled by SA1518).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLinesAtEndOfFileAsync()
        {
            var testCode = @"public class Foo
{
}


";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneEmptyLineBetweenSingleLineCommentAndFirstElementAsync()
        {
            string testCode = @"//

namespace Microsoft
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a verbatim string literal does not trigger any diagnostics.
        /// (This will be handled by SA1518).
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(12, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(19, 1),
                /* line 23 should not report a diagnostic, as it's part of the directive is inactive */
                Diagnostic().WithLocation(27, 1),
                /* line 30 should not report a diagnostic, as it's part of a comment */
                Diagnostic().WithLocation(35, 1),
            };

            await VerifyCSharpDiagnosticAsync(TestCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(6, 1),
                Diagnostic().WithLocation(12, 1),
                Diagnostic().WithLocation(16, 1),
                Diagnostic().WithLocation(19, 1),
                Diagnostic().WithLocation(27, 1),
                Diagnostic().WithLocation(35, 1),
            };
            await VerifyCSharpFixAsync(TestCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
