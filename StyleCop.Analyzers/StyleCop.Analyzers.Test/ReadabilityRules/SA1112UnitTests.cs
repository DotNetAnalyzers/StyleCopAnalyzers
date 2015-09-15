// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.SpacingRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1112UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(16, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
