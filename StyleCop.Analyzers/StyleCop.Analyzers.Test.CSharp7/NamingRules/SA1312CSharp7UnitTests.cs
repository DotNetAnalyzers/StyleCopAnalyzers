// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.NamingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

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
                Diagnostic().WithArguments("Bar").WithLocation(5, 35),
                Diagnostic().WithArguments("Par").WithLocation(7, 35),
            };

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""0"", out var bar);
        int.TryParse(""0"", out var car);
        int.TryParse(""0"", out var par);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithArguments("Bar").WithLocation(5, 14),
                Diagnostic().WithArguments("Par").WithLocation(5, 24),
            };

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        var (bar, car, par) = (1, 2, 3);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(5, 37);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("ArgEx").WithLocation(10, 60);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("X").WithLocation(5, 23);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("X").WithLocation(7, 18);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("Variable").WithLocation(6, 51);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        int.TryParse(variable.ToString(), out var variable1);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("Variable").WithLocation(6, 16);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""Text"", out var variable);
        string variable1 = variable.ToString();
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("Int").WithLocation(5, 38);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        int.TryParse(""text"", out var @int);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("Parameter").WithLocation(5, 52);

            var fixedCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        int.TryParse(parameter.ToString(), out var parameter1);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithArguments("__").WithLocation(7, 52);
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3031, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3031")]
        public async Task TestTupleDesconstructionCamelCaseAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName((string name, string value) obj)
    {
        (string name, string value) = obj;
    }
}
";
            var settings = $@"{{
  ""settings"": {{
    ""namingRules"": {{
      ""tupleElementNameCasing"": ""{TupleElementNameCase.CamelCase}""
    }}
  }}
}}
";

            await VerifyCSharpDiagnosticAsync(languageVersion: null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3031, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3031")]
        public async Task TestTupleDesconstructionPascalCaseAsync()
        {
            var testCode = @"
public class TypeName
{
    public void MethodName((string Name, string Value) obj)
    {
        (string name, string value) = obj;
    }
}
";
            var settings = $@"{{
  ""settings"": {{
    ""namingRules"": {{
      ""tupleElementNameCasing"": ""{TupleElementNameCase.PascalCase}""
    }}
  }}
}}
";

            await VerifyCSharpDiagnosticAsync(languageVersion: null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
