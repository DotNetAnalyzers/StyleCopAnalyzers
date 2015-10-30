// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1120UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestViolationWithSingleLineCommentAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithSingleLineCommentOnLineWithCodeAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Console.WriteLine(""A""); //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 40);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        System.Console.WriteLine(""A"");
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMultiLineCommentOnASingleLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        /* */
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMultiLineCommentMultiLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        /* 

        */
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoViolationOnCommentedOutCodeBlocksAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        ////
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationCodeBlockFirstLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        //
        // Foo
        // Bar
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationCodeBlockLastLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
        //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
    }
}";
            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationCodeBlockFirstAndLastLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // 
        // Bar
        //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(8, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2 }, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Bar
    }
}";

            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithTrailingContentOnLineAsync()
        {
            var testCode = @"
using System;
class Foo
{
    public void Bar()
    {
        /* */Console.WriteLine(""baz"");
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var expectedFixedCode = @"
using System;
class Foo
{
    public void Bar()
    {
        Console.WriteLine(""baz"");
    }
}";

            await this.VerifyCSharpFixAsync(testCode, expectedFixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeBlockNoDiagnosticOnInternalBlankLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // 
        //
        // Bar
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLeadingAndEndingViolationsInCodeBlockWithStatementAsync()
        {
            var testCode = @"using System.Collections.Generic;
using System.Linq;

class TestClass
{
    private IEnumerable<int> Build(int argument)
    {
        //
        // Text line 1
        // Text line 2
        //
        return Enumerable.Range(1, 10);
    }
}
";

            var fixedCode = @"using System.Collections.Generic;
using System.Linq;

class TestClass
{
    private IEnumerable<int> Build(int argument)
    {
        // Text line 1
        // Text line 2
        return Enumerable.Range(1, 10);
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(11, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLeadingAndEndingViolationsInClassAsync()
        {
            var testCode = @"using System;
public class SomeException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public SomeException()
    {
    }
}
";

            var fixedCode = @"using System;
public class SomeException : Exception
{
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp

    public SomeException()
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5),
                this.CSharpDiagnostic().WithLocation(9, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty comment at the start of a source file will be handled correctly.
        /// This is a regression test for #1708
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatEmptyCommentAtFileStartWillBeHandledProperlyAsync()
        {
            var testCode = @"//
public class TestClass
{
}
";

            var fixedTestCode = @"public class TestClass
{
}
";

            var expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty comment at the end of a source file will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatEmptyCommentAtFileEndWillBeHandledProperlyAsync()
        {
            var testCode = @"public class TestClass
{
}
//";

            var fixedTestCode = @"public class TestClass
{
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1120CommentsMustContainText();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1120CodeFixProvider();
        }
    }
}
