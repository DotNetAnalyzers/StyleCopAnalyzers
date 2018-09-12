// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1503BracesMustNotBeOmitted,
        StyleCop.Analyzers.LayoutRules.SA1503CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1503BracesMustNotBeOmitted"/>.
    /// </summary>
    public class SA1503UnitTests
    {
        /// <summary>
        /// Gets the statements that will be used in the theory test cases.
        /// </summary>
        /// <value>
        /// The statements that will be used in the theory test cases.
        /// </value>
        public static IEnumerable<object[]> TestStatements
        {
            get
            {
                yield return new[] { "if (i == 0)" };
                yield return new[] { "while (i == 0)" };
                yield return new[] { "for (var j = 0; j < i; j++)" };
                yield return new[] { "foreach (var j in new[] { 1, 2, 3 })" };
                yield return new[] { "lock (this)" };
                yield return new[] { "using (this)" };
                yield return new[] { "fixed (byte* ptr = new byte[10])" };
            }
        }

        /// <summary>
        /// Verifies that a statement followed by a block without braces will produce a warning.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestStatementWithoutBracesAsync(string statementText)
        {
            var expected = Diagnostic().WithLocation(7, 13);
            await VerifyCSharpDiagnosticAsync(this.GenerateTestStatement(statementText), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a <c>do</c> statement followed by a block without braces will produce a warning, and the
        /// code fix for this warning results in valid code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        do
            Debug.Assert(true);
        while (false);
    }
}";
            var fixedCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        do
        {
            Debug.Assert(true);
        }
        while (false);
    }
}";

            var expected = Diagnostic().WithLocation(7, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a statement followed by a block with braces will produce no diagnostics results.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestStatementWithBracesAsync(string statementText)
        {
            await VerifyCSharpDiagnosticAsync(this.GenerateFixedTestStatement(statementText), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if / else statement followed by a block without braces will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementWithoutBracesAsync()
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

            var expected = new[]
            {
                Diagnostic().WithLocation(7, 13),
                Diagnostic().WithLocation(9, 13),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block with braces will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementWithBracesAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a else if, followed by an else statement, all blocks with braces
        /// will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseIfElseStatementWithBracesAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested if statements followed by a block without braces will produce warnings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleIfStatementsWithoutBracesAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(6, 21),
                Diagnostic().WithLocation(6, 33),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderForIfElseStatementAsync()
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

            // While it looks like only one instance is fixed, what actually happened is fixing the first warning caused
            // the second warning to change from SA1503 to SA1520. Since the testing framework is only testing SA1503,
            // from its perspective all warnings have been corrected so it stops iterating.
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
            Debug.Assert(false);
    }
}";

            // The batch fixer does correct all problems in one pass.
            var batchFixedTestCode = @"using System.Diagnostics;
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 13),
                    Diagnostic().WithLocation(9, 13),
                },
                FixedCode = fixedTestCode,
                BatchFixedCode = batchFixedTestCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will properly handle alternate indentations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithAlternateIndentationAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
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
 }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 4),
                },
                FixedCode = fixedTestCode,
                IndentationSize = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly handle non-whitespace trivia.
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
    }
}";

            var expected = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly handle multiple cases of missing braces.
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
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            if (i == 0)
            {
                Debug.Assert(true);
            }
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 21),
                Diagnostic().WithLocation(6, 33),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
        Debug.WriteLine(""Foobar 2"");
    }
}";

            var expected = Diagnostic().WithLocation(8, 13);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [WorkItem(2623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2623")]
        public async Task TestMultipleUsingStatementsWithDefaultSettingsAsync(bool suppressSA1519)
        {
            var testCode = @"using System;
public class Foo
{
    public void Bar(int i)
    {
        using (default(IDisposable))
        using (default(IDisposable))
        {
        }
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
            };

            if (suppressSA1519)
            {
                test.DisabledDiagnostics.Add(SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId);
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, true)]
        [InlineData(false, false, false)]
        [WorkItem(2623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2623")]
        public async Task TestMultipleUsingStatementsWithExplicitSettingAsync(bool allowConsecutiveUsings, bool suppressSA1519, bool expectDiagnostic)
        {
            var consecutiveUsingsSettings = $@"
{{
  ""settings"": {{
    ""layoutRules"": {{
      ""allowConsecutiveUsings"": {(allowConsecutiveUsings ? "true" : "false")}
    }}
  }}
}}
";

            var testCode = @"using System;
public class Foo
{
    public void Bar(int i)
    {
        using (default(IDisposable))
        using (default(IDisposable))
        {
        }
    }
}";
            var fixedCode = @"using System;
public class Foo
{
    public void Bar(int i)
    {
        using (default(IDisposable))
        {
            using (default(IDisposable))
        {
        }
        }
    }
}";

            if (expectDiagnostic)
            {
                var test = new CSharpTest
                {
                    TestCode = testCode,
                    ExpectedDiagnostics = { Diagnostic().WithLocation(7, 9) },
                    FixedCode = fixedCode,
                    Settings = consecutiveUsingsSettings,
                };

                if (suppressSA1519)
                {
                    test.DisabledDiagnostics.Add(SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId);
                }

                await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var test = new CSharpTest
                {
                    TestCode = testCode,
                    Settings = consecutiveUsingsSettings,
                };

                if (suppressSA1519)
                {
                    test.DisabledDiagnostics.Add(SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId);
                }

                await test.RunAsync(CancellationToken.None).ConfigureAwait(false);

                // Test again with the fixedCode as the test code
                test.TestSources[0] = (test.TestSources[0].filename, SourceText.From(fixedCode));
                await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly for a statement.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        private async Task TestCodeFixForStatementAsync(string statementText)
        {
            var expected = Diagnostic().WithLocation(7, 13);
            await VerifyCSharpFixAsync(this.GenerateTestStatement(statementText), expected, this.GenerateFixedTestStatement(statementText), CancellationToken.None).ConfigureAwait(false);
        }

        private string GenerateTestStatement(string statementText)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT#
            Debug.Assert(true);
    }

    public void Dispose() {}
}";
            return testCodeFormat.Replace("#STATEMENT#", statementText);
        }

        private string GenerateFixedTestStatement(string statementText)
        {
            var fixedTestCodeFormat = @"using System.Diagnostics;
public class Foo : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT#
        {
            Debug.Assert(true);
        }
    }

    public void Dispose() {}
}";
            return fixedTestCodeFormat.Replace("#STATEMENT#", statementText);
        }
    }
}
