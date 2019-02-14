// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1112UnitTests
    {
        [Fact]
        public async Task TestMethodWithNoParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
)
    {

    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithNoParametersClosingParanthesisOnTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithNoParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
)
    {

    }
}";
            var fixedCode = @"
class Foo
{
    public Foo()
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithNoParametersClosingParanthesisOnTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public Foo()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallNoParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString(
);
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = this.Equals(new Foo()
);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallNoParametersClosingParanthesisOnTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var o = new object(
);
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var o = new object();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
public class CtorWithParams
{
    public CtorWithParams(string s)
    {
    }
}
class Foo
{
    public void Bar()
    {
        var o = new CtorWithParams(string.Empty
);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersClosingParanthesisOnTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var o = new object();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithParametersClosingParanthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public int this[int index
]
    {
        get
        {
            return 1;
        }    
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCreationOfObjectNoOpeningClosingParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Collections.Generic.Dictionary<int, int> cache = new System.Collections.Generic.Dictionary<int, int> { { 3, 3 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDirectiveTriviaAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod1(
#if true
        )
#endif
    {
    }

    public void TestMethod2()
    {
        TestMethod1(
#if true
#endif
            );
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void TestMethod1(
#if true
        )
#endif
    {
    }

    public void TestMethod2()
    {
        TestMethod1(
#if true
#endif
            );
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 9),
                Diagnostic().WithLocation(16, 13),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
