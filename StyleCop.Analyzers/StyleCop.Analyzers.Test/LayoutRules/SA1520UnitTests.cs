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

    public class SA1520UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that an if / else statement followed by a single-line statement without curly braces will not
        /// produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineIfElseStatementWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if / else statement followed by a multi-line statement without curly braces will not
        /// produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultiLineIfElseStatementWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(
                true);
        else
            Debug.Assert(
                false);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementWithCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a else if, followed by an else statement, all blocks with curly
        /// braces will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseIfElseStatementWithCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 1)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(true);
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested if statements followed by a single-line statement without curly braces will produce
        /// no diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleSingleLineIfStatementsWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested if statements followed by a multi-line statement without curly braces will not produce
        /// warnings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleMultiLineIfStatementsWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(
            true);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderForIfStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderForElseIfStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
            Debug.Assert(false);
        else
        {
            Debug.Assert(false);
        }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderForElseStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else
            Debug.Assert(false);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will properly handle alternate indentations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithAlternateIndentationAsync()
        {
            this.IndentationSize = 1;

            var testCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
  else
  {
   Debug.Assert(false);
  }
 }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
  {
   Debug.Assert(true);
  }
  else
  {
   Debug.Assert(false);
  }
 }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle non-whitespace trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithNonWhitespaceTriviaAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
            Debug.Assert(true);
        else
        {
            Debug.Assert(false);
        }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle multiple cases of missing brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithMultipleNestingsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else if (i == 4)
            Debug.Assert(false);
        else
            Debug.Assert(false);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 3)
        {
            Debug.Assert(false);
        }
        else if (i == 4)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will not perform a code fix when statement contains a preprocessor directive.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixWithPreprocessorDirectivesAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (true)
#if !DEBUG
            Debug.WriteLine(""Foobar"");
#endif
        else
        {
            Debug.WriteLine(""Foobar 2"");
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1520UseCurlyBracketsConsistently();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1503CodeFixProvider();
        }
    }
}
