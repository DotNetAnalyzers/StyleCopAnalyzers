// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1002SemicolonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1002SemicolonsMustBeSpacedCorrectly"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1002UnitTests
    {
        [Fact]
        public async Task TestForLoopAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        for (int x =0;x<10;x++)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        for (int x =0; x<10; x++)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 22),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 27),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInfiniteForLoopAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        for (;;)
        {
        }
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        for (; ;)
        {
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 14),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestReturnValueStatementAsync()
        {
            string testCode = @"
class ClassName
{
    int MethodName()
    {
        return 0 ;
    }
}
";
            string fixedCode = @"
class ClassName
{
    int MethodName()
    {
        return 0;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(6, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyReturnStatementAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        return ;
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        return;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(6, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThrowStatementsAsync()
        {
            string testCode = @"
using System;
class ClassName
{
    int MethodName()
    {
        try
        {
            throw new InvalidOperationException() ;
        }
        catch
        {
            throw ;
        }
    }
}
";
            string fixedCode = @"
using System;
class ClassName
{
    int MethodName()
    {
        try
        {
            throw new InvalidOperationException();
        }
        catch
        {
            throw;
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(9, 51),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(13, 19),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        ;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1777.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyStatementAfterCallAsync()
        {
            string testCode = @"using System;
class ClassName
{
    void MethodName()
    {
        Console.WriteLine();;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSingleLineAccessorsAsync()
        {
            string testCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property;}
        set{this.property=value;}
    }
}
";
            string fixedCode = @"
class ClassName
{
    int property;
    int PropertyName
    {
        get{return this.property; }
        set{this.property=value; }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 32),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFollowedByLineCommentAsync()
        {
            string testCode = @"
class ClassName
{
    int property;// Comment
}
";
            string fixedCode = @"
class ClassName
{
    int property; // Comment
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFollowedByBlockCommentAsync()
        {
            string testCode = @"
class ClassName
{
    int property;/* Comment
        Comment Line 2 */
}
";
            string fixedCode = @"
class ClassName
{
    int property; /* Comment
        Comment Line 2 */
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1426, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1426")]
        public async Task TestPrecededByBlockCommentAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        bool special = true ? true /*comment*/ : false /*comment*/ ;
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        bool special = true ? true /*comment*/ : false /*comment*/;
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(6, 68);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(403, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/403")]
        public async Task TestSemicolonAtBeginningOfLineAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        // The first one is indented (has leading trivia).
        bool special = false
            ;

        // The second one is not indented (no leading trivia).
        special = true
;

        // The third one is preceded by non-whitespace trivia.
        special = false
/*comment*/;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingSemicolonAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        int x
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1002").WithMessage("; expected").WithLocation(6, 14),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2699, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2699")]
        public async Task TestSemiColonAtEndOfFileAsync()
        {
            string testCode = @"using System;";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
