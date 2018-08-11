// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

    public class SA1118UnitTests
    {
        [Fact]
        public async Task TestMethodCallWithTwoParametersSecondSpansMoreThanOneLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            +4);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 13);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersSecondSpansMultipleLinesThirdSpansMultipleLinesButIsAnonymousFunctionAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j, System.Action<int> action)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            + 4,
            delegate(int i)
            {
                    
            });
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 13);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersSecondSpansMultipleLinesThirdSpansMultipleLinesButIsLambdaExpressionAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j, System.Action<int> action)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            + 4,
            i =>
            {

            });
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 13);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersSecondSpansMultipleLinesThirdSpansMultipleLinesButIsInvocationExpressionAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j, int k)
    {
    }

    public void Bar()
    {
        Fun(1,
            System.Linq.Enumerable.Count(
                new int[0]) + 4,
            System.Linq.Enumerable.Count(
                new int[0]));
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 13);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersSecondSpansMultipleLinesButIsObjectCreationExpressionAsync()
        {
            var testCode = @"
class Foo
{
    Foo(int a, int b)
    {
    }

    public void FunA(int i, Foo j)
    {
    }

    public void FunB()
    {
        FunA(1,
             new Foo(
                 2,
                 3));
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersSecondSpansMultipleLinesButIsAnonymousObjectCreationExpressionAsync()
        {
            var testCode = @"
class Foo
{
    public void FunA(int i, object j)
    {
    }

    public void FunB()
    {
        FunA(1,
             new
             {
                 Foo = 1,
                 Bar = 2,
             });
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersFirstIsMultilineSecondIsOneLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j)
    {
    }

    public void Bar()
    {
        Fun(1
            +2,
            3);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCallSecondParameterSpansTwoLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> d = delegate(int i, int j)
                                    {

                                    };
        d(1,            
          2
          +3);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 11);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCallSecondParameterIsLambdaAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,System.Action<int>> d = delegate(int i, System.Action<int> a)
                                    {

                                    };
        d(1,            
          (k) => 
            {
            });
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCallSecondParameterSpansMultipleLinesThirdParameterIsInvocationAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int> d = delegate(int i, int j, int k)
                                    {

                                    };
        d(1,
          System.Linq.Enumerable.Count(
                new int[0]) + 1,
          System.Linq.Enumerable.Count(
                new int[0]));
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 11);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaCallSecondParameterSpansTwoLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> d = (i, j) => { };
        d(1,            
          2
          +3);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 11);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaCallSecondParameterIsAnonynousMethodAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, System.Action<int>> d = (i, j) => { };
        d(1,            
          delegate(int param)
        {});
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallSecondParameterSpansTwoLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo();
        var result = f[1,
                       3
                       +4];
    }

    public int this[int i, int j]
    {
        get
        {
            return 0;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 24);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallSecondParameterSpansTwoLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 }, { 9, 10 } };
        numbers[1, 1
            +1] = 5;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 20);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeSecondParameterSpandsMultipleLinesAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class,AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(1,2)]
[MyAttribute(1,2+
3)]
public class Foo
{
}";

            DiagnosticResult expected = Diagnostic().WithLocation(11, 16);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
