namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1011UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handles valid closing square brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacingOfOpenSquareBracketAsync()
        {
            var testCode = @"using System;

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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle spaces before a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustNotBePrecededBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int this[[CLSCompliant(true) ]int index ]
    {
        get
        {
            int[ ][ ] ints = new int [ ][
]
            {
                new int[5 ], new int
[5 ]
            };
            return ints[0 ][0 ];
        }
    }
}";

            DiagnosticResult[] expected =
            {
                // 5, 41 should be reported by SA1017
                this.CSharpDiagnostic().WithLocation(5, 52).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(9, 18).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(9, 40).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(13, 4).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(15, 27).WithArguments(" not",  "preceded"),
                this.CSharpDiagnostic().WithLocation(15, 31).WithArguments(" not",  "preceded")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle spaces before a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustNotBeFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public Action[] TestMethod()
    {
        var x = new Action[] []
        {
            new Action[5] , new Action[5]
        };
        var y = (x[0]);
        y = (x[0] );
        x[0][0]();
        x[0][0] ();
        return x[0] ;
    }
}";

            // spacing violations should be covered SA1010, SA1001, SA1009, SA1008, and then SA1002
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle member access.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketWithMemberAccessAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var x = new object[] { };
        x[0]?.ToString();
        x[0] ?.ToString();
        x[0].ToString();
        x[0] .ToString();
    }
}
";

            // spacing violation should be reported by SA1019
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle string interpolation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketInStringInterpolationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod(object[] a)
    {
        var x = $""{a[0]}"";
        x = $""{a[0] }"";
    }
}
";

            // spacing violation should be reported by SA1013
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle required spaces after a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustBeFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int[]TestMethod(int[]a)
    {
        int[]ints  = new int[]{ };
        return ints;
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 16).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(5, 32).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(7, 13).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(7, 30).WithArguments(string.Empty, "followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1011ClosingSquareBracketsMustBeSpacedCorrectly();
        }
    }
}
