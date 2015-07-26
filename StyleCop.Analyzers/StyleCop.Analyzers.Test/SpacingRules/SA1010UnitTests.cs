namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1010OpeningSquareBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1010UnitTests : CodeFixVerifier
    {
        private const string ExpectedCode = @"using System;

public class Foo
{
    public int this[[CLSCompliant(true)]int index]
    {
        get
        {
            int[][] ints = new int[][
]
            {
                new int[5], new int
[5]
            };
            return ints[0][0];
        }
    }
}";

        [Fact]
        public async Task TestValidSpacingOfOpenSquareBracketAsync()
        {
            await this.VerifyCSharpDiagnosticAsync(ExpectedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOpenSquareBracketMustNotBePrecededBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int this [[CLSCompliant(true)]int index]
    {
        get
        {
            int [] [] ints = new int [] [
]
            {
                new int   [5], new int
[5]
            };
            return ints [0] [0];
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 21).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 17).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 20).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 38).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 41).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(15, 25).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(15, 29).WithArguments("not be preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, ExpectedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOpenSquareBracketMustNotBeFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int this[ [CLSCompliant(true)]int index]
    {
        get
        {
            int[ ][ ] ints = new int[ ][
]
            {
                new int[   5], new int
[5]
            };
            return ints[ 0][ 0];
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 20).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 16).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 19).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 37).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(12, 24).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(15, 24).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(15, 28).WithArguments("not be followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, ExpectedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOpenSquareBracketMustNeitherPrecededNorFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int this [ [CLSCompliant(true)]int index]
    {
        get
        {
            int [ ] [ ] ints = new int [ ][
]
            {
                new int   [   5], new int
[5]
            };
            return ints [ 0] [ 0];
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 21).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(9, 17).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(9, 40).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(15, 25).WithArguments("neither preceded nor followed"),
                this.CSharpDiagnostic().WithLocation(15, 30).WithArguments("neither preceded nor followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, ExpectedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1010OpeningSquareBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1010CodeFixProvider();
        }
    }
}
