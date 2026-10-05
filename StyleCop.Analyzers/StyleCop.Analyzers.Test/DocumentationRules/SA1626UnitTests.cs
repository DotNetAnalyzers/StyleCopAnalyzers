// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes,
        StyleCop.Analyzers.DocumentationRules.SA1626CodeFixProvider>;

    public class SA1626UnitTests
    {
        [Fact]
        public async Task TestClassWithXmlCommentAsync()
        {
            var testCode = @"/// <summary>
/// XML Documentation
/// </summary>
public class TypeName
{
    public void Bar()
    {
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithCommentAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        // This is a comment
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestMethodWithOneLineThreeSlashCommentAsync(string lineEnding)
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        {|#0:///|} This is a comment
    }
}
".ReplaceLineEndings(lineEnding);
            var fixedCode = @"public class TypeName
{
    public void Bar()
    {
        // This is a comment
    }
}
".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithMultiLineThreeSlashCommentAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        /// This is
        /// a comment
    }
}
";
            var fixedCode = @"public class TypeName
{
    public void Bar()
    {
        // This is
        // a comment
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 9),
                Diagnostic().WithLocation(6, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithCodeCommentsAsync()
        {
            var testCode = @"public class TypeName
{
    public void Bar()
    {
        //// System.Console.WriteLine(""Bar"")
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithSingeLineDocumentationAsync()
        {
            var testCode = @"public class TypeName
{
    /// <summary>Summary text</summary>
    public void Bar()
    {
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStrayCommentInsideDocumentationAsync()
        {
            var testCode = @"public class Calculator
{
    /// <summary>
    // Adds the specified x.
    /// </summary>
    /// <param name=""x"">The x.</param>
    /// <param name=""y"">The y.</param>
    /// <returns>Return addition</returns>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";
            var fixedCode = @"public class Calculator
{
    /// <summary>
    /// Adds the specified x.
    /// </summary>
    /// <param name=""x"">The x.</param>
    /// <param name=""y"">The y.</param>
    /// <returns>Return addition</returns>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 5),
                Diagnostic().WithLocation(6, 5),
                Diagnostic().WithLocation(7, 5),
                Diagnostic().WithLocation(8, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleStrayCommentsInsideDocumentationAsync()
        {
            var testCode = @"public class Calculator
{
    /// <summary>
    // Adds the specified x
    // and the specified y.
    /// </summary>
    /// <returns>Return addition</returns>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";
            var fixedCode = @"public class Calculator
{
    /// <summary>
    /// Adds the specified x
    /// and the specified y.
    /// </summary>
    /// <returns>Return addition</returns>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 5),
                Diagnostic().WithLocation(7, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStrayCommentSeparatedByBlankLineFromDocumentationAsync()
        {
            var testCode = @"public class Calculator
{
    /// <summary>
    // Adds the specified x.

    /// </summary>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";
            var fixedCode = @"public class Calculator
{
    /// <summary>
    // Adds the specified x.

    // </summary>
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStrayCommentBetweenReportedLinesIsLeftAloneAsync()
        {
            var testCode = @"public class Calculator
{
    /// Adds the specified x.
    // and the specified y.
    /// Returns the sum.
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";
            var fixedCode = @"public class Calculator
{
    // Adds the specified x.
    // and the specified y.
    // Returns the sum.
    public int Add(int x, int y)
    {
        return x + y;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(5, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
