﻿namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

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
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(3, 59)
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
        public async Task TestParameterStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1313 is reported for parameters starting with an underscore
            var testCode = @"public class TypeName
{
    public void MethodName(string _bar)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_bar").WithLocation(3, 35);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            // Verify the code fix doesn't do anything in this case
            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
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

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1315")]
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
    public void MethodName(string parameterValue)
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

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1315")]
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
    public void MethodName(string parameterValue, int parameter)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact(Skip = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1315")]
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
    public void MethodName(string parameter, int parameterValue)
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
                this.CSharpDiagnostic().WithLocation(9, 52).WithArguments("Other")
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
                this.CSharpDiagnostic().WithLocation(9, 61).WithArguments("Other")
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
