// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

    public class SA1118UnitTests
    {
        public static IEnumerable<object[]> ArrayCreationExpressions { get; } = new List<object[]>
        {
            new object[]
            {
                @"new[]
                {
                    0,
                    1
                }",
            },
            new object[]
            {
                @"new int[]
                {
                    0,
                    1
                }",
            },
        };

        /// <summary>
        /// Verifies that verbatim strings, including interpolated strings, can span multiple lines.
        /// </summary>
        /// <param name="expression">The string expression to pass as an argument.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("@\"first\nsecond\"")]
        [InlineData("$@\"first {1}\nsecond\"")]
        [InlineData("$@\"{1 +\n2}\"")]
        [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
        public async Task TestMultilineVerbatimStringArgumentAsync(string expression)
        {
            await VerifyMultilineStringArgumentAsync(expression).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that verbatim strings in attribute arguments can span multiple lines.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
        public async Task TestMultilineVerbatimStringAttributeArgumentAsync()
        {
            var testCode = """
                [Example(0, @"first
                second", Text = @"third
                fourth")]
                class ExampleAttribute : System.Attribute
                {
                    public ExampleAttribute(int value, string text) { }
                    public string Text { get; set; }
                }
                """;

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the string exemption does not extend to concatenation expressions.
        /// </summary>
        /// <param name="prefix">The prefix of the string literal.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("@")]
        [InlineData("$@")]
        [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
        public async Task TestMultilineStringConcatenationArgumentAsync(string prefix)
        {
            var testCode = $$"""
                class C
                {
                    void M(int value, string text) { }
                    void Test()
                    {
                        M(0, {|#0:{{prefix}}"first
                second" +
                            "third"|});
                    }
                }
                """;

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }

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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestLambdaCallSecondParameterIsAnonymousMethodAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestAttributeSecondParameterSpansMultipleLinesAsync()
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

        [Theory]
        [MemberData(nameof(ArrayCreationExpressions))]
        [WorkItem(3676, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3676")]
        public async Task TestAttributeArrayCreationSpansMultipleLinesAsync(string arrayCreationExpression)
        {
            var testCode = $@"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{{
    public MyAttribute(int a, int[] b)
    {{
    }}
}}

[MyAttribute(
    1,
    {arrayCreationExpression})]
public class Foo
{{
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3676, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3676")]
        public async Task TestAttributeInvocationSpansMultipleLinesAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, string b)
    {
    }
}

[MyAttribute(
    1,
    nameof(
        Foo))]
public class Foo
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ArrayCreationExpressions))]
        public async Task TestArrayCreationSpansMultipleLinesAsync(string arrayCreationExpression)
        {
            var testCode = $@"
class Foo
{{
    public void Fun(int i, int[] j)
    {{
    }}

    public void Bar()
    {{
        Fun(
            1,
            {arrayCreationExpression});
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected static async Task VerifyMultilineStringArgumentAsync(string expression)
        {
            var testCode = $$"""
                class C
                {
                    C(int value, string text) { }
                    string this[int value, string text] { get { return text; } }
                    void M(int value, string text) { }
                    void Test()
                    {
                        M(0, {{expression}});
                        M(0, text: {{expression}});
                        var instance = new C(0, {{expression}});
                        var result = this[0, {{expression}}];
                    }
                }
                """;

            await new CSharpTest
            {
                TestCode = testCode,
                TestState = { MarkupHandling = MarkupMode.None },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
