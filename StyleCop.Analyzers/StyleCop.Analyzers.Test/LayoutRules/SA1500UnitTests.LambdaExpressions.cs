namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid lambda expressions defined in this test.
        /// </summary>
        /// <remarks>
        /// These blocks are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestLambdaExpressionValid()
        {
            var testCode = @"using System;

public class Foo
{
    private void TestMethod(Action action)
    {
    }

    private void Bar()
    {
        // Valid lambda expression #1
        Action item1 = () => { };
        
        // Valid lambda expression #2
        Action item2 = () => { int x; };

        // Valid lambda expression #3
        Action item3 = () =>
        {
        };

        // Valid lambda expression #4
        Action item4 = () =>
        {
            int x;
        };

        // Valid lambda expression #5
        this.TestMethod(() => { });

        // Valid lambda expression #6
        this.TestMethod(() =>
        { 
        });

        // Valid lambda expression #7
        this.TestMethod(() => { int x; });

        // Valid lambda expression #8
        this.TestMethod(() =>
        { 
            int x; 
        });
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid lambda expression definitions.
        /// </summary>
        [Fact]
        public async Task TestLambdaExpressionInvalid()
        {
            var testCode = @"using System;

public class Foo
{
    private void TestMethod(Action action)
    {
    }

    private void Bar()
    {
        // Invalid lambda expression #1
        Action item1 = () => {
        };
        
        // Invalid lambda expression #2
        Action item2 = () => {
            int x; 
        };

        // Invalid lambda expression #3
        Action item3 = () => {
            int x; };

        // Invalid lambda expression #4
        Action item4 = () => { int x; 
        };

        // Invalid lambda expression #5
        Action item5 = () =>
        {
            int x; };

        // Invalid lambda expression #6
        Action item6 = () =>
        { int x; 
        };

        // Invalid lambda expression #7
        Action item7 = () =>
        { int x; };

        // Invalid lambda expression #8
        this.TestMethod(() => {
        });

        // Invalid lambda expression #9
        this.TestMethod(() => {
            int x;
        });

        // Invalid lambda expression #10
        this.TestMethod(() => {
            int x; });

        // Invalid lambda expression #11
        this.TestMethod(() => { int x;
        });

        // Invalid lambda expression #12
        this.TestMethod(() =>
        { 
            int x; });

        // Invalid lambda expression #13
        this.TestMethod(() =>
        { int x; 
        });

        // Invalid lambda expression #14
        this.TestMethod(() =>
        { int x; });
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid lambda expression #1
                this.CSharpDiagnostic().WithLocation(12, 30),
                // Invalid lambda expression #2
                this.CSharpDiagnostic().WithLocation(16, 30),
                // Invalid lambda expression #3
                this.CSharpDiagnostic().WithLocation(21, 30),
                this.CSharpDiagnostic().WithLocation(22, 20),
                // Invalid lambda expression #4
                this.CSharpDiagnostic().WithLocation(25, 30),
                // Invalid lambda expression #5
                this.CSharpDiagnostic().WithLocation(31, 20),
                // Invalid lambda expression #6
                this.CSharpDiagnostic().WithLocation(35, 9),
                // Invalid lambda expression #7
                this.CSharpDiagnostic().WithLocation(40, 9),
                this.CSharpDiagnostic().WithLocation(40, 18),
                // Invalid lambda expression #8
                this.CSharpDiagnostic().WithLocation(43, 31),
                // Invalid lambda expression #9
                this.CSharpDiagnostic().WithLocation(47, 31),
                // Invalid lambda expression #10
                this.CSharpDiagnostic().WithLocation(52, 31),
                this.CSharpDiagnostic().WithLocation(53, 20),
                // Invalid lambda expression #11
                this.CSharpDiagnostic().WithLocation(56, 31),
                // Invalid lambda expression #12
                this.CSharpDiagnostic().WithLocation(62, 20),
                // Invalid lambda expression #13
                this.CSharpDiagnostic().WithLocation(66, 9),
                // Invalid lambda expression #14
                this.CSharpDiagnostic().WithLocation(71, 9),
                this.CSharpDiagnostic().WithLocation(71, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}