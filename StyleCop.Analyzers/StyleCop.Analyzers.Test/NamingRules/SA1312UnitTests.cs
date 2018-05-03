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
    public class SA1312UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestThatDiagnosticIsNotReportedForParametersAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string bar, string Car)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Bar").WithLocation(5, 16),
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(7, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar;
        string car;
        string par;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Bar").WithLocation(5, 16),
                this.CSharpDiagnostic().WithArguments("Par").WithLocation(5, 26),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string bar, car, par;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("_bar").WithLocation(5, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Ex").WithLocation(10, 26),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("X").WithLocation(5, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("X").WithLocation(8, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Y").WithLocation(9, 27),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Y").WithLocation(9, 17),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Y").WithLocation(9, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("Z").WithLocation(9, 53),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Variable").WithLocation(6, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string variable = ""Text"";
        string variable1 = variable.ToString();
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
    public void MethodName()
    {
        string Int;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Int").WithLocation(5, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        string @int;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("Parameter").WithLocation(5, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TypeName
{
    public void MethodName(int parameter)
    {
        string parameter1 = parameter.ToString();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithArguments("_").WithLocation(5, 16),
                this.CSharpDiagnostic().WithArguments("__").WithLocation(6, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1312VariableNamesMustBeginWithLowerCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToLowerCaseCodeFixProvider();
        }
    }
}
