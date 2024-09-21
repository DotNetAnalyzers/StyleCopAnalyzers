﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    [UseCulture("en-US")]
    public class SA1313UnitTests
    {
        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForFieldsAsync()
        {
            var testCode = @"public class TypeName
{
    const string bar = nameof(bar);
    const string Bar = nameof(Bar);
    string car = nameof(car);
    string Car = nameof(Car);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForEventFieldsAsync()
        {
            var testCode = @"using System;
public class TypeName
{
    static event EventHandler bar;
    static event EventHandler Bar;
    event EventHandler car;
    event EventHandler Car;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForVariablesAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        const string bar = nameof(bar);
        const string Bar = nameof(Bar);
        string car = nameof(car);
        string Car = nameof(Car);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_SingleParameterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Bar)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Bar").WithLocation(3, 35);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string bar)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_MultipleParametersAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Bar, string car, string Par)
    {
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Bar").WithLocation(3, 35),
                Diagnostic().WithArguments("Par").WithLocation(3, 59),
            };

            var fixedCode = @"public class TypeName
{
    public void MethodName(string bar, string car, string par)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterStartingWithLetterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string bar)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParameterPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public void MethodName(string Bar)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Parameter)
    {
        string parameter = ""Text"";
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Parameter").WithLocation(3, 35);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter1)
    {
        string parameter = ""Text"";
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithKeywordAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Int)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Int").WithLocation(3, 35);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string @int)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithLaterParameterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Parameter, int parameter)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Parameter").WithLocation(3, 35);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter1, int parameter)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithEarlierParameterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string parameter, int Parameter)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Parameter").WithLocation(3, 50);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter, int parameter1)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoViolationOnInterfaceParameterNameAsync()
        {
            var testCode = @"
public interface ITest
{
    void Method(int Param1, int param2, int Param3);
}

public class Test : ITest
{
    public void Method(int Param1, int param2, int param3)
    {
    }
}";
            var expected = new[]
            {
                Diagnostic().WithLocation(4, 21).WithArguments("Param1"),
                Diagnostic().WithLocation(4, 45).WithArguments("Param3"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationOnRenamedInterfaceParameterNameAsync()
        {
            var testCode = @"
public interface ITest
{
    void Method(int Param1, int param2, int Param3);
}

public class Test : ITest
{
    public void Method(int Param1, int param2, int Other)
    {
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(4, 21).WithArguments("Param1"),
                Diagnostic().WithLocation(4, 45).WithArguments("Param3"),
                Diagnostic().WithLocation(9, 52).WithArguments("Other"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3555, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3555")]
        public async Task TestNoViolationOnExplicitlyImplementedInterfaceParameterNameAsync()
        {
            var testCode = @"
public interface ITest
{
    void Method(int param1, int {|#0:Param2|});
}

public class Test : ITest
{
    void ITest.Method(int param1, int Param2)
    {
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("Param2"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3555, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3555")]
        public async Task TestViolationOnRenamedExplicitlyImplementedInterfaceParameterNameAsync()
        {
            var testCode = @"
public interface ITest
{
    void Method(int param1, int {|#0:Param2|});
}

public class Test : ITest
{
    public void Method(int param1, int {|#1:Other|})
    {
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("Param2"),
                Diagnostic().WithLocation(1).WithArguments("Other"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoViolationOnAbstractParameterNameAsync()
        {
            var testCode = @"
public abstract class TestBase
{
    public abstract void Method(int Param1, int param2, int Param3);
}

public class Test : TestBase
{
    public override void Method(int Param1, int param2, int param3)
    {
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(4, 37).WithArguments("Param1"),
                Diagnostic().WithLocation(4, 61).WithArguments("Param3"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationOnRenamedAbstractParameterNameAsync()
        {
            var testCode = @"
public abstract class Testbase
{
    public abstract void Method(int Param1, int param2, int Param3);
}

public class Test : Testbase
{
    public override void Method(int Param1, int param2, int Other)
    {
    }
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(4, 37).WithArguments("Param1"),
                Diagnostic().WithLocation(4, 61).WithArguments("Param3"),
                Diagnostic().WithLocation(9, 61).WithArguments("Other"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1442, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1442")]
        public async Task TestSimpleLambaExpressionAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action = Ignored => { };
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Ignored").WithLocation(5, 37);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1343, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1343")]
        public async Task TestLambdaParameterNamedUnderscoreAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action1 = _ => { };
        System.Action<int> action2 = (_) => { };
        System.Action<int> action3 = delegate(int _) { };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies this diagnostic does not check whether or not a parameter named <c>_</c> is being used.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(1343, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1343")]
        public async Task TestLambdaParameterNamedUnderscoreUsageAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Func<int, int> function1 = _ => _;
        System.Func<int, int> function2 = (_) => _;
        System.Func<int, int> function3 = delegate(int _) { return _; };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1606, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1606")]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestLambdaParameterNamedDoubleUnderscoreAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action1 = __ => { };
        System.Action<int> action2 = (__) => { };
        System.Action<int> action3 = delegate(int __) { };
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("__").WithLocation(5, 38),
                Diagnostic().WithArguments("__").WithLocation(6, 39),
                Diagnostic().WithArguments("__").WithLocation(7, 51),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies this diagnostic does not check whether or not a parameter named <c>__</c> is being used.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(1606, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1606")]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestLambdaParameterNamedDoubleUnderscoreUsageAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Func<int, int> function1 = __ => __;
        System.Func<int, int> function2 = (__) => __;
        System.Func<int, int> function3 = delegate(int __) { return __; };
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("__").WithLocation(5, 43),
                Diagnostic().WithArguments("__").WithLocation(6, 44),
                Diagnostic().WithArguments("__").WithLocation(7, 56),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1343, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1343")]
        public async Task TestLambdaParameterWithThreeUnderscoresAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action1 = ___ => { };
        System.Action<int> action2 = (___) => { };
        System.Action<int> action3 = delegate(int ___) { };
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("___").WithLocation(5, 38),
                Diagnostic().WithArguments("___").WithLocation(6, 39),
                Diagnostic().WithArguments("___").WithLocation(7, 51),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestLambdaParameterWithIncreasingNumberOfUnderscoresAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        System.Action<int> action1 = _ => { };
        System.Action<int, int> action2 = (_, __) => { };
        System.Action<int, int, int> action3 = (_, __, ___) => { };
        System.Action<int, int, int, int> action4 = (_, __, ___, ____) => { };
        System.Action<int> action5 = delegate(int _) { };
        System.Action<int, int> action6 = delegate(int _, int __) { };
        System.Action<int, int, int> action7 = delegate(int _, int __, int ___) { };
        System.Action<int, int, int, int> action8 = delegate(int _, int __, int ___, int ____) { };

        System.Action<int, int> action9 = (a, _) => { };
        System.Action<int, int> action10 = (a, __) => { };
        System.Action<int, int, int> action11 = (_, a, __) => { };
        System.Action<int, int, int> action12 = (_, __, a) => { };
        System.Action<int, int, int> action13 = (_, a, ___) => { };
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("__").WithLocation(15, 48),
                Diagnostic().WithArguments("___").WithLocation(18, 56),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1343, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1343")]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestMethodParameterNamedUnderscoreAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int _)
    {
        ++_;
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_").WithLocation(3, 32);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestMethodParameterNamedUnderscoresUsedAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName1(int _1, short _2)
    {
        ++_1;
        ++_2;
    }

    public void MethodName2(int _, short __)
    {
        ++_;
        ++__;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_1").WithLocation(3, 33),
                Diagnostic().WithArguments("_2").WithLocation(3, 43),
                Diagnostic().WithArguments("_").WithLocation(9, 33),
                Diagnostic().WithArguments("__").WithLocation(9, 42),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1606, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1606")]
        public async Task TestMethodParameterNamedDoubleUnderscoreAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int __)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("__").WithLocation(3, 32);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1529, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1529")]
        public async Task TestInheritedInterfacesWithOverloadedMembersAsync()
        {
            var testCode = @"
public interface ITest
{
    void Method(int Param1, int param2, int Param3);
    void Method();
}

public interface IEmptyInterface { }

public interface IDerivedTest : ITest, IEmptyInterface
{
    void Method(int Param1, int param2, int param3);
}";
            var expected = new[]
            {
                Diagnostic().WithLocation(4, 21).WithArguments("Param1"),
                Diagnostic().WithLocation(4, 45).WithArguments("Param3"),
                Diagnostic().WithLocation(12, 21).WithArguments("Param1"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1604, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1604")]
        public async Task TestCodeFixProperlyRemovesUnderscoreAsync()
        {
            var testCode = @"
public class TestClass
{
    public TestClass(string _text)
        : this(_text, false)
    {
    }

    public TestClass(string text, bool flag)
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public TestClass(string text)
        : this(text, false)
    {
    }

    public TestClass(string text, bool flag)
    {
    }
}
";

            var expected = Diagnostic().WithLocation(4, 29).WithArguments("_text");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2974")]
        public async Task TestMethodParameterNamedUnusedAsync()
        {
            var testCode = @"public class TypeName
{
    public int MethodName(int _used, int _unused, int _, int _1, int _2, int ___, int __)
    {
        return _used + _2;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_used").WithLocation(3, 31),
                Diagnostic().WithArguments("_2").WithLocation(3, 70),
                Diagnostic().WithArguments("___").WithLocation(3, 78),
                Diagnostic().WithArguments("__").WithLocation(3, 87),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that an invalid method override will not produce a diagnostic nor crash the analyzer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2189, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2189")]
        public async Task InvalidMethodOverrideShouldNotProduceDiagnosticAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public abstract class BaseClass
    {
        public abstract void TestMethod(int p1, int p2);
    }

    public class TestClass : BaseClass
    {
        public override void TestMethod(int p1, X int P2)
        {
        }
    }
}
";
            DiagnosticResult[] expected = this.GetInvalidMethodOverrideShouldNotProduceDiagnosticAsyncDiagnostics();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetInvalidMethodOverrideShouldNotProduceDiagnosticAsyncDiagnostics()
        {
            return new DiagnosticResult[]
            {
                DiagnosticResult.CompilerError("CS0534").WithLocation(9, 18).WithMessage("'TestClass' does not implement inherited abstract member 'BaseClass.TestMethod(int, int)'"),
                DiagnosticResult.CompilerError("CS0115").WithLocation(11, 30).WithMessage("'TestClass.TestMethod(int, X, int)': no suitable method found to override"),
                DiagnosticResult.CompilerError("CS0246").WithLocation(11, 49).WithMessage("The type or namespace name 'X' could not be found (are you missing a using directive or an assembly reference?)"),
                DiagnosticResult.CompilerError("CS1001").WithLocation(11, 51).WithMessage("Identifier expected"),
                DiagnosticResult.CompilerError("CS1003").WithLocation(11, 51).WithMessage("Syntax error, ',' expected"),
            };
        }
    }
}
