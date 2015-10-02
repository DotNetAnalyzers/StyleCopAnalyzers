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
    /// This class contains unit tests for <see cref="SA1002SemicolonsMustBeSpacedCorrectly"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1002UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 22),
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 27),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 14)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(6, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(6, 16)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(9, 51),
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(13, 19),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 33),
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 32),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(4, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1426.
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1426
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(6, 68);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#403.
        /// https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/403
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                new DiagnosticResult
                {
                    Id = "CS1002",
                    Severity = DiagnosticSeverity.Error,
                    Message = "; expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 14) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1002SemicolonsMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
