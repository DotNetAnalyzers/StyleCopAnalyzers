// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;
    using DiscardVerifier = StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.SA1312CodeFixProvider>;

    public partial class SA1312CSharp7UnitTests : SA1312UnitTests
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

        [Theory]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        [InlineData("var", "_")]
        [InlineData("var", "__")]
        [InlineData("string", "_")]
        [InlineData("object", "_")]
        [InlineData("System.IComparable<string>", "___")]
        public async Task TestLocalDeclarationReplacedWithDiscardAsync(string type, string name)
        {
            var testCode = $@"public class TypeName
{{
    public void MethodName()
    {{
        {type} {{|#0:{name}|}} = this.GetValue();
    }}

    private string GetValue() => string.Empty;
}}";

            var fixedCode = @"public class TypeName
{
    public void MethodName()
    {
        _ = this.GetValue();
    }

    private string GetValue() => string.Empty;
}";

            var expected = DiscardVerifier.Diagnostic().WithArguments(name).WithLocation(0);
            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestBoxingLocalDeclarationReplacedWithDiscardAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(int value)
    {
        // Comment
        object {|#0:_|} = value; // Trailing
    }
}";

            var fixedCode = @"public class TypeName
{
    public void MethodName(int value)
    {
        // Comment
        _ = value; // Trailing
    }
}";

            var expected = DiscardVerifier.Diagnostic().WithArguments("_").WithLocation(0);
            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestDesignationsReplacedWithDiscardAsync()
        {
            var testCode = @"public class TypeName
{
    public int MethodName(object value)
    {
        int.TryParse(""0"", out var {|#0:__|});
        int.TryParse(""0"", out int {|#1:___|});
        var ({|#2:____|}, x) = (1, 2);
        if (value is int {|#3:_____|})
        {
        }

        switch (value)
        {
        case string {|#4:______|}:
            break;
        }

        return x;
    }
}";

            var fixedCode = @"public class TypeName
{
    public int MethodName(object value)
    {
        int.TryParse(""0"", out _);
        int.TryParse(""0"", out int _);
        var (_, x) = (1, 2);
        if (value is int _)
        {
        }

        switch (value)
        {
        case string _:
            break;
        }

        return x;
    }
}";

            DiagnosticResult[] expected =
            {
                DiscardVerifier.Diagnostic().WithArguments("__").WithLocation(0),
                DiscardVerifier.Diagnostic().WithArguments("___").WithLocation(1),
                DiscardVerifier.Diagnostic().WithArguments("____").WithLocation(2),
                DiscardVerifier.Diagnostic().WithArguments("_____").WithLocation(3),
                DiscardVerifier.Diagnostic().WithArguments("______").WithLocation(4),
            };

            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestFixAllReplacesWithDiscardsAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName1()
    {
        var {|#0:_|} = this.GetValue();
    }

    public string MethodName3()
    {
        string {|#1:__|} = this.GetValue();
        return this.GetValue();
    }

    public void MethodName2()
    {
        int.TryParse(""0"", out var {|#2:___|});
        var ({|#3:____|}, {|#4:_____|}) = (1, 2);
        object {|#5:______|} = this.GetValue();
    }

    private string GetValue() => string.Empty;
}";

            var fixedCode = @"public class TypeName
{
    public void MethodName1()
    {
        _ = this.GetValue();
    }

    public string MethodName3()
    {
        _ = this.GetValue();
        return this.GetValue();
    }

    public void MethodName2()
    {
        int.TryParse(""0"", out _);
        var (_, _) = (1, 2);
        _ = this.GetValue();
    }

    private string GetValue() => string.Empty;
}";

            DiagnosticResult[] expected =
            {
                DiscardVerifier.Diagnostic().WithArguments("_").WithLocation(0),
                DiscardVerifier.Diagnostic().WithArguments("__").WithLocation(1),
                DiscardVerifier.Diagnostic().WithArguments("___").WithLocation(2),
                DiscardVerifier.Diagnostic().WithArguments("____").WithLocation(3),
                DiscardVerifier.Diagnostic().WithArguments("_____").WithLocation(4),
                DiscardVerifier.Diagnostic().WithArguments("______").WithLocation(5),
            };

            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestOutVarKeepsVarWhenUnderscoreIsInScopeAsync()
        {
            var testCode = @"public class TypeName
{
    private int _;

    public void MethodName()
    {
        int.TryParse(this._.ToString(), out var {|#0:__|});
    }
}";

            var fixedCode = @"public class TypeName
{
    private int _;

    public void MethodName()
    {
        int.TryParse(this._.ToString(), out var _);
    }
}";

            var expected = DiscardVerifier.Diagnostic().WithArguments("__").WithLocation(0);
            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestDiscardCodeFixNotOfferedWhenUnderscoreIsInScopeAsync()
        {
            var testCode = @"public class TypeName
{
    private int _;

    public void MethodName()
    {
        var {|#0:__|} = this._;
    }
}";

            var expected = DiscardVerifier.Diagnostic().WithArguments("__").WithLocation(0);
            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestDiscardCodeFixNotOfferedAsync()
        {
            var testCode = @"using System;
using System.Linq;

public class TypeName
{
    public void MethodName(object value)
    {
        var {|#0:_|} = 1;
        Console.WriteLine(_);

        int {|#1:__|} = 1, other = 2;
        long {|#2:___|} = 1;
        string {|#3:____|} = null;
        int {|#4:_____|};
        Wrapper {|#5:______|} = 1;
        Func<int> {|#6:_______|} = () => 1;
        string /* type */ {|#7:________|} = string.Empty;

        for (int {|#8:_________|} = 0; other < 1; other++)
        {
        }

        try
        {
        }
        catch (Exception {|#9:__________|})
        {
        }

        var query = from {|#10:___________|} in new[] { 1 } select 1;

        if (value is int {|#11:____________|} && ____________ > 0)
        {
        }

        int.TryParse(""0"", out var {|#12:_____________|});
        Console.WriteLine(_____________);

        var array = new int[1];
        ref int {|#13:______________|} = ref array[0];
    }

    private class Wrapper
    {
        public static implicit operator Wrapper(int value) => new Wrapper();
    }
}";

            DiagnosticResult[] expected =
            {
                DiscardVerifier.Diagnostic().WithArguments("_").WithLocation(0),
                DiscardVerifier.Diagnostic().WithArguments("__").WithLocation(1),
                DiscardVerifier.Diagnostic().WithArguments("___").WithLocation(2),
                DiscardVerifier.Diagnostic().WithArguments("____").WithLocation(3),
                DiscardVerifier.Diagnostic().WithArguments("_____").WithLocation(4),
                DiscardVerifier.Diagnostic().WithArguments("______").WithLocation(5),
                DiscardVerifier.Diagnostic().WithArguments("_______").WithLocation(6),
                DiscardVerifier.Diagnostic().WithArguments("________").WithLocation(7),
                DiscardVerifier.Diagnostic().WithArguments("_________").WithLocation(8),
                DiscardVerifier.Diagnostic().WithArguments("__________").WithLocation(9),
                DiscardVerifier.Diagnostic().WithArguments("___________").WithLocation(10),
                DiscardVerifier.Diagnostic().WithArguments("____________").WithLocation(11),
                DiscardVerifier.Diagnostic().WithArguments("_____________").WithLocation(12),
                DiscardVerifier.Diagnostic().WithArguments("______________").WithLocation(13),
            };

            await DiscardVerifier.VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2631, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2631")]
        public async Task TestDiscardCodeFixNotOfferedBeforeCSharp7Async()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        var {|#0:_|} = string.Empty;
    }
}";

            var expected = DiscardVerifier.Diagnostic().WithArguments("_").WithLocation(0);
            await DiscardVerifier.VerifyCSharpFixAsync(LanguageVersion.CSharp6, testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
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
