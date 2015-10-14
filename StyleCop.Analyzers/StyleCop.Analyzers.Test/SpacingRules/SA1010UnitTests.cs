// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
                this.CSharpDiagnostic().WithLocation(5, 21).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(5, 21).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 17).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 17).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(9, 40).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(9, 40).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(12, 27).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(15, 25).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(15, 25).WithArguments("not be followed"),
                this.CSharpDiagnostic().WithLocation(15, 30).WithArguments("not be preceded"),
                this.CSharpDiagnostic().WithLocation(15, 30).WithArguments("not be followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, ExpectedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that index initializers are properly handled.
        /// Regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1617
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyIndexInitializerAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IDictionary<ulong, string> items)
    {
        var test = new Dictionary<ulong, string>(items) { [100] = ""100"" };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that index initializer scope determination is working as intended.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThatIndexInitializerScopeIsDeterminedProperlyAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IDictionary<ulong, string> items)
    {
        int[] indexes = { 0 };
        var dictionary = new Dictionary<int, int> { [indexes [0]] = 0 };
    }
}
";

            var fixedTestCode = @"using System.Collections.Generic;

public class TestClass
{
    public void TestMethod(IDictionary<ulong, string> items)
    {
        int[] indexes = { 0 };
        var dictionary = new Dictionary<int, int> { [indexes[0]] = 0 };
    }
}
";
            var expected = this.CSharpDiagnostic().WithLocation(8, 62).WithArguments("not be preceded");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1010OpeningSquareBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
