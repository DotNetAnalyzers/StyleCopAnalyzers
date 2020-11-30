// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1120CommentsMustContainText,
        StyleCop.Analyzers.ReadabilityRules.SA1120CodeFixProvider>;

    public class SA1120UnitTests
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 40);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        System.Console.WriteLine(""A"");
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);
            DiagnosticResult expected2 = Diagnostic().WithLocation(8, 9);

            var expectedFixedCode = @"
class Foo
{
    public void Bar()
    {
        // Bar
    }
}";

            await VerifyCSharpFixAsync(testCode, new[] { expected, expected2 }, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);

            var expectedFixedCode = @"
using System;
class Foo
{
    public void Bar()
    {
        Console.WriteLine(""baz"");
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, expectedFixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(11, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(4, 5),
                Diagnostic().WithLocation(9, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty comment at the start of a source file will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1708, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1708")]
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

            var expected = Diagnostic().WithLocation(1, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(4, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an unclosed multi-line comment at the end of a source file will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2056, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2056")]
        public async Task VerifyThatUnclosedCommentAtFileEndWillBeHandledProperlyAsync()
        {
            var testCode = @"public class TestClass
{
}
[|{|CS1035:|}/*|]";

            var fixedTestCode = @"public class TestClass
{
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
                FixedState = { MarkupHandling = MarkupMode.Ignore },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
