// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    [UseCulture("en-US")]
    public class SA1312UnitTests
    {
        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForConstVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        const string Bar = """", car = """", Dar = """";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

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
        public async Task TestThatDiagnosticIsNotReportedForParametersAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string bar, string Car)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_SingleVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string Bar;
        string car;
        string Par;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Bar").WithLocation(5, 16),
                Diagnostic().WithArguments("Par").WithLocation(7, 16),
            };

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar;
        string car;
        string par;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReported_MultipleVariablesAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string Bar, car, Par;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Bar").WithLocation(5, 16),
                Diagnostic().WithArguments("Par").WithLocation(5, 26),
            };

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar, car, par;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1312 is reported for variables starting with an underscore
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string _bar = ""baz"";
    }
}";

            var fixedTestCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar = ""baz"";
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("_bar").WithLocation(5, 16);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableStartingWithLetterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar = ""baz"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInCatchDeclarationAsync()
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
        catch (Exception Ex)
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
        catch (Exception ex)
        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Ex").WithLocation(10, 26),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInForEachStatementAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        foreach (var X in new int[0])
        {
        }
    }
}";
            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        foreach (var x in new int[0])
        {
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("X").WithLocation(5, 22),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInFromClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from X in new int[0]
            select X;
    }
}";
            var fixedCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            select x;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("X").WithLocation(8, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInQueryContinuationAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            select x into Y
            select Y;
    }
}";
            var fixedCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            select x into y
            select y;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Y").WithLocation(9, 27),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInLetClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            let Y = x
            select Y;
    }
}";
            var fixedCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            let y = x
            select y;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Y").WithLocation(9, 17),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInJoinClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join Y in new int[0] on x equals Y
            select x;
    }
}";
            var fixedCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join y in new int[0] on x equals y
            select x;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Y").WithLocation(9, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariableInJoinIntoClauseAsync()
        {
            var testCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join y in new int[0] on x equals y into Z
            select Z;
    }
}";
            var fixedCode = @"
using System.Linq;
public class TypeName
{
    public void MethodName()
    {
        var result =
            from x in new int[0]
            join y in new int[0] on x equals y into z
            select z;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("Z").WithLocation(9, 53),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVariablePlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public void MethodName()
    {
        string Bar = ""baz"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithVariableAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        string Variable = variable.ToString();
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Variable").WithLocation(6, 16);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        string variable1 = variable.ToString();
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithKeywordAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        string Int;
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Int").WithLocation(5, 16);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string @int;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRenameConflictsWithParameterAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        string Parameter = parameter.ToString();
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("Parameter").WithLocation(5, 16);

            var fixedCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        string parameter1 = parameter.ToString();
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnderscoreOnlyNamesDoNotTriggerCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        string _ = parameter.ToString();
        string __ = parameter.ToString();
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_").WithLocation(5, 16),
                Diagnostic().WithArguments("__").WithLocation(6, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
