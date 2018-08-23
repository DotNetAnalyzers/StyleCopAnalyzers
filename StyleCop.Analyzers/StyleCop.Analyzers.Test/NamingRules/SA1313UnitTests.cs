// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using TestHelper;
    using Xunit;

    [UseCulture("en-US")]
    public class SA1313UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Bar").WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string bar)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Bar").WithLocation(3, 35),
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(3, 59),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string bar, string car, string par)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Parameter").WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter1)
    {
        string parameter = ""Text"";
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Int").WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string @int)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Parameter").WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter1, int parameter)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Parameter").WithLocation(3, 50);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(string parameter, int parameter1)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 21).WithArguments("Param1"),
                this.CSharpDiagnostic().WithLocation(4, 45).WithArguments("Param3"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 21).WithArguments("Param1"),
                this.CSharpDiagnostic().WithLocation(4, 45).WithArguments("Param3"),
                this.CSharpDiagnostic().WithLocation(9, 52).WithArguments("Other"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 37).WithArguments("Param1"),
                this.CSharpDiagnostic().WithLocation(4, 61).WithArguments("Param3"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 37).WithArguments("Param1"),
                this.CSharpDiagnostic().WithLocation(4, 61).WithArguments("Param3"),
                this.CSharpDiagnostic().WithLocation(9, 61).WithArguments("Other"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Ignored").WithLocation(5, 37);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1606, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1606")]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies this diagnostic does not check whether or not a parameter named <c>__</c> is being used.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(1606, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1606")]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("___").WithLocation(5, 38),
                this.CSharpDiagnostic().WithArguments("___").WithLocation(6, 39),
                this.CSharpDiagnostic().WithArguments("___").WithLocation(7, 51),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1343, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1343")]
        public async Task TestMethodParameterNamedUnderscoreAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int _)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_").WithLocation(3, 32);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("__").WithLocation(3, 32);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 21).WithArguments("Param1"),
                this.CSharpDiagnostic().WithLocation(4, 45).WithArguments("Param3"),
                this.CSharpDiagnostic().WithLocation(12, 21).WithArguments("Param1"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic().WithLocation(4, 29).WithArguments("_text");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            DiagnosticResult[] expected =
            {
                this.CSharpCompilerError("CS0534").WithLocation(9, 18).WithMessage("'TestClass' does not implement inherited abstract member 'BaseClass.TestMethod(int, int)'"),
                this.CSharpCompilerError("CS0115").WithLocation(11, 30).WithMessage("'TestClass.TestMethod(int, X, int)': no suitable method found to override"),
                this.CSharpCompilerError("CS0246").WithLocation(11, 49).WithMessage("The type or namespace name 'X' could not be found (are you missing a using directive or an assembly reference?)"),
                this.CSharpCompilerError("CS1001").WithLocation(11, 51).WithMessage("Identifier expected"),
                this.CSharpCompilerError("CS1003").WithLocation(11, 51).WithMessage("Syntax error, ',' expected"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1313ParameterNamesMustBeginWithLowerCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToLowerCaseCodeFixProvider();
        }
    }
}
