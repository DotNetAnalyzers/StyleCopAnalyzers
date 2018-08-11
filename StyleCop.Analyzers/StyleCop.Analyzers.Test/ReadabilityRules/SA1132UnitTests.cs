// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1132DoNotCombineFields,
        StyleCop.Analyzers.ReadabilityRules.SA1132CodeFixProvider>;

    public class SA1132UnitTests
    {
        [Theory]
        [InlineData("private int a;")]
        [InlineData("public event System.Action a;")]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidDeclarationAsync()
        {
            const string testCode = @"
class Foo
{
    private int a, b,/*foo*/c,d;
    public event System.Action aa, bb;
}";
            const string fixedCode = @"
class Foo
{
    private int a;
    private int b;/*foo*/
    private int c;
    private int d;
    public event System.Action aa;
    public event System.Action bb;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 5),
                Diagnostic().WithLocation(5, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidDeclarationWithTrailingTriviaAsync()
        {
            const string testCode = @"
class Foo
{
    public const int a = 1, // foo
        b = 2,
        c = 3, // bar
        d = 4,
        e = 5; /* spam */
}";
            const string fixedCode = @"
class Foo
{
    public const int a = 1; // foo
    public const int b = 2;
    public const int c = 3; // bar
    public const int d = 4;
    public const int e = 5; /* spam */
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidFieldDeclarationWithAttributesAsync()
        {
            const string testCode = @"
class Foo
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int a, b;
}";
            const string fixedCode = @"
class Foo
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int a;
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private int b;
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("CS1001", 19, "Identifier expected", "private int a,, b;")]
        [InlineData("CS1001", 20, "Identifier expected", "private int e, = 3;")]
        [InlineData("CS1002", 21, "; expected", "private int c, d")]
        public async Task TestDeclarationWithMissingTokensAndNodesAsync(string id, int column, string message, string declaration)
        {
            string testCode = $@"
class Foo
{{
    {declaration}
}}";

            DiagnosticResult[] expected =
            {
                CompilerError(id).WithMessage(message).WithLocation(4, column),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidEventFieldDeclarationWithAttributesAsync()
        {
            const string testCode = @"
class Foo
{
#if true
    [Test]
    public event System.Action foo, bar;
#endif
}

[System.AttributeUsage(System.AttributeTargets.Event)]
class TestAttribute : System.Attribute
{
}";
            const string fixedCode = @"
class Foo
{
#if true
    [Test]
    public event System.Action foo;
    [Test]
    public event System.Action bar;
#endif
}

[System.AttributeUsage(System.AttributeTargets.Event)]
class TestAttribute : System.Attribute
{
}";

            var expected = Diagnostic().WithLocation(5, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2594")]
        public async Task VerifyThatDirectiveTriviaAreHandledCorrectlyAsync()
        {
            var testCode = @"
namespace StyleCopDemo
{
    class Program
    {
        #region some members

        string myString;

        #region some fields

        // this line:
        int myInt, yourInt;

        #endregion

        #region some other fields

        int secondInt, thirdInt;

        #endregion

        #endregion
    }
}";

            var fixedCode = @"
namespace StyleCopDemo
{
    class Program
    {
        #region some members

        string myString;

        #region some fields

        // this line:
        int myInt;

        // this line:
        int yourInt;

        #endregion

        #region some other fields

        int secondInt;
        int thirdInt;

        #endregion

        #endregion
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(13, 9),
                Diagnostic().WithLocation(19, 9),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
