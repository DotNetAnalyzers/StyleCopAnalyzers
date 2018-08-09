// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly, StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1000CSharp7UnitTests : SA1000UnitTests
    {
        [Fact]
        public async Task TestOutVariableDeclarationAsync()
        {
            string statementWithoutSpace = @"int.TryParse(""0"", out@Int32 x);";

            string statementWithSpace = @"int.TryParse(""0"", out @Int32 x);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = Diagnostic().WithArguments("out", string.Empty, "followed").WithLocation(12, 31);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2419, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2419")]
        public async Task TestOutVarDiscardAsync()
        {
            string statementWithSpace = @"int.TryParse(""0"", out var _);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2419, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2419")]
        public async Task TestOutDiscardAsync()
        {
            string statementWithSpace = @"int.TryParse(""0"", out _);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVarKeywordTupleTypeAsync()
        {
            string statementWithoutSpace = @"var(a, b) = (2, 3);";

            string statementWithSpace = @"var (a, b) = (2, 3);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = Diagnostic().WithArguments("var", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRefExpressionAndTypeAsync()
        {
            string statementWithoutSpace = @"
int a = 0;
ref@Int32 b = ref@Call(ref@a);

ref@Int32 Call(ref@Int32 p)
    => ref@p;
";

            string statementWithSpace = @"
int a = 0;
ref @Int32 b = ref @Call(ref @a);

ref @Int32 Call(ref @Int32 p)
    => ref @p;
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 1),
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 15),
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 24),
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(16, 1),
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(16, 16),
                Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(17, 8),
            };

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for tuple expressions is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1008CSharp7UnitTests.TestTupleExpressionsAsync"/>
        [Fact]
        public async Task TestReturnTupleExpressionsAsync()
        {
            var testCode = @"using System.Collections.Generic;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            // Returns (leading spaces after keyword checked in SA1000, not SA1008)
            (int, int) LocalFunction1() { return( 1, 2); }
            (int, int) LocalFunction2() { return(1, 2); }
            (int, int) LocalFunction3() { return ( 1, 2); }
        }
    }
}
";

            var fixedCode = @"using System.Collections.Generic;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            // Returns (leading spaces after keyword checked in SA1000, not SA1008)
            (int, int) LocalFunction1() { return ( 1, 2); }
            (int, int) LocalFunction2() { return (1, 2); }
            (int, int) LocalFunction3() { return ( 1, 2); }
        }
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    // Returns
                    Diagnostic().WithArguments("return", string.Empty, "followed").WithLocation(10, 43),
                    Diagnostic().WithArguments("return", string.Empty, "followed").WithLocation(11, 43),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for <c>new</c> expressions for an array of a tuple type is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1008CSharp7UnitTests.TestNewTupleArrayAsync"/>
        [Fact]
        public async Task TestNewTupleArrayAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new( int, int)[0];
            var y = new(int, int)[0];
            var z = new ( int, int)[0];
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new ( int, int)[0];
            var y = new (int, int)[0];
            var z = new ( int, int)[0];
        }
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithArguments("new", string.Empty, "followed").WithLocation(7, 21),
                    Diagnostic().WithArguments("new", string.Empty, "followed").WithLocation(8, 21),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing for <c>foreach</c> expressions using tuple deconstruction is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1008CSharp7UnitTests.TestForEachVariableStatementAsync"/>
        [Fact]
        public async Task TestForEachVariableStatementAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            foreach( var (x, y) in new (int, int)[0]) { }
            foreach(var (x, y) in new (int, int)[0]) { }
            foreach ( var (x, y) in new (int, int)[0]) { }
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            foreach ( var (x, y) in new (int, int)[0]) { }
            foreach (var (x, y) in new (int, int)[0]) { }
            foreach ( var (x, y) in new (int, int)[0]) { }
        }
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithArguments("foreach", string.Empty, "followed").WithLocation(7, 13),
                    Diagnostic().WithArguments("foreach", string.Empty, "followed").WithLocation(8, 13),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
