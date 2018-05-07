// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1312CSharp7UnitTests : SA1312UnitTests
    {
        [Fact]
        public async Task TestThatDiagnosticIsReported_SingleVariableDesignatorAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""0"", out var Bar);
        int.TryParse(""0"", out var car);
        int.TryParse(""0"", out var Par);
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments("Bar").WithLocation(5, 35),
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(7, 35),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""0"", out var bar);
        int.TryParse(""0"", out var car);
        int.TryParse(""0"", out var par);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_MultipleVariableDesignatorsAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        var (Bar, car, Par) = (1, 2, 3);
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments("Bar").WithLocation(5, 14),
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(5, 24),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        var (bar, car, par) = (1, 2, 3);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableDesignatorStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1312 is reported for variables starting with an underscore
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""baz"", out var _bar);
    }
}";

            var fixedTestCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""baz"", out var bar);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_bar").WithLocation(5, 37);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableDesignatorInWhenClauseAsync()
        {
            var testCode = @"
using System;
public class TypeName
{
    public void MethodName()
    {
        try
        {
        }
        catch (Exception ex) when (ex is ArgumentException ArgEx)
        {
        }
    }
}";
            var fixedCode = @"
using System;
public class TypeName
{
    public void MethodName()
    {
        try
        {
        }
        catch (Exception ex) when (ex is ArgumentException argEx)
        {
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("ArgEx").WithLocation(10, 60);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternInForEachStatementAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        foreach (var (X, y) in new (int, int)[0])
        {
        }
    }
}";
            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        foreach (var (x, y) in new (int, int)[0])
        {
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("X").WithLocation(5, 23);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternInSwitchCaseAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        switch (new object())
        {
        case int X:
        default:
            break;
        }
    }
}";
            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        switch (new object())
        {
        case int x:
        default:
            break;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("X").WithLocation(7, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public void MethodName()
    {
        int.TryParse(""baz"", out var Bar);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternVariableRenameConflictsWithVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        int.TryParse(variable.ToString(), out var Variable);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Variable").WithLocation(6, 51);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        int.TryParse(variable.ToString(), out var variable1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableRenameConflictsWithPatternVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""Text"", out var variable);
        string Variable = variable.ToString();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Variable").WithLocation(6, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""Text"", out var variable);
        string variable1 = variable.ToString();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternVariableRenameConflictsWithKeywordAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""text"", out var Int);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Int").WithLocation(5, 38);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""text"", out var @int);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPatternVariableRenameConflictsWithParameterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        int.TryParse(parameter.ToString(), out var Parameter);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Parameter").WithLocation(5, 52);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        int.TryParse(parameter.ToString(), out var parameter1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDiscardsDoNotTriggerCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        int.TryParse(parameter.ToString(), out var _);
        int.TryParse(parameter.ToString(), out var _);
        int.TryParse(parameter.ToString(), out var __); // This one isn't a discard
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("__").WithLocation(7, 52);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }
    }
}
