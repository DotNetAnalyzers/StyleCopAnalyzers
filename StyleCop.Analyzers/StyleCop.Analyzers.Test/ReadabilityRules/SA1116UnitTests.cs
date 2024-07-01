// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

// Several test methods in this file use the same member data, but in some cases the test does not use all of the
// supported parameters. See https://github.com/xunit/xunit/issues/1556.
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public class SA1116UnitTests
    {
        public static IEnumerable<object[]> GetTestDeclarations(string delimiter, string fixDelimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"public Foo(int a,{delimiter} string s) {{ }}", $"public Foo({fixDelimiter}int a,{delimiter} string s) {{ }}", 16, treatMultilineParametersAsSplit };
            yield return new object[] { $"public object Bar(int a,{delimiter} string s) => null;", $"public object Bar({fixDelimiter}int a,{delimiter} string s) => null;", 23, treatMultilineParametersAsSplit };
            yield return new object[] { $"public static Foo operator + (Foo a,{delimiter} Foo b) => null;", $"public static Foo operator + ({fixDelimiter}Foo a,{delimiter} Foo b) => null;", 35, treatMultilineParametersAsSplit };
            yield return new object[] { $"public object this[int a,{delimiter} string s] => null;", $"public object this[{fixDelimiter}int a,{delimiter} string s] => null;", 24, treatMultilineParametersAsSplit };
            yield return new object[] { $"public delegate void Bar(int a,{delimiter} string s);", $"public delegate void Bar({fixDelimiter}int a,{delimiter} string s);", 30, treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTestConstructorInitializers(string delimiter, string fixDelimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"this(42,{delimiter} \"hello\")", $"this({fixDelimiter}42,{delimiter} \"hello\")", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42,{delimiter} \"hello\")", $"base({fixDelimiter}42,{delimiter} \"hello\")", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTrailingMultilineTestConstructorInitializers(string delimiter, string fixDelimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"this(42,{delimiter} () =>\r\n{{\r\n}})", $"this({fixDelimiter}42,{delimiter} () =>\r\n{{\r\n}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42,{delimiter} () =>\r\n{{\r\n}})", $"base({fixDelimiter}42,{delimiter} () =>\r\n{{\r\n}})", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTestExpressions(string delimiter, string fixDelimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"Bar(1,{delimiter} 2)", $"Bar({fixDelimiter}1,{delimiter} 2)", 13, treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int, int> func = (int x,{delimiter} int y) => Bar(x, y)", $"System.Action<int, int> func = ({fixDelimiter}int x,{delimiter} int y) => Bar(x, y)", 41, treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int, int> func = delegate(int x,{delimiter} int y) {{ Bar(x, y); }}", $"System.Action<int, int> func = delegate({fixDelimiter}int x,{delimiter} int y) {{ Bar(x, y); }}", 49, treatMultilineParametersAsSplit };
            yield return new object[] { $"new string('a',{delimiter} 2)", $"new string({fixDelimiter}'a',{delimiter} 2)", 20, treatMultilineParametersAsSplit };
            yield return new object[] { $"var arr = new string[2,{delimiter} 2];", $"var arr = new string[{fixDelimiter}2,{delimiter} 2];", 30, treatMultilineParametersAsSplit };
            yield return new object[] { $"char cc = (new char[3, 3])[2,{delimiter} 2];", $"char cc = (new char[3, 3])[{fixDelimiter}2,{delimiter} 2];", 36, treatMultilineParametersAsSplit };
            yield return new object[] { $"char? c = (new char[3, 3])?[2,{delimiter} 2];", $"char? c = (new char[3, 3])?[{fixDelimiter}2,{delimiter} 2];", 37, treatMultilineParametersAsSplit };
            yield return new object[] { $"long ll = this[2,{delimiter} 2];", $"long ll = this[{fixDelimiter}2,{delimiter} 2];", 24, treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetLambdaTestExpressions(string delimiter, string fixDelimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"Buz(() => {{ }}, () =>{delimiter} {{{delimiter} }})", $"Buz({fixDelimiter}() => {{ }}, () =>{delimiter} {{{delimiter} }})", 13, treatMultilineParametersAsSplit };
            yield return new object[] { $"new System.Lazy<int>(() =>{delimiter} {{{delimiter} return 1;{delimiter} }})", $"new System.Lazy<int>({fixDelimiter}() =>{delimiter} {{{delimiter} return 1;{delimiter} }})", 30, treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> ValidTestExpressions(bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"System.Action func = () => Bar(0, 3)", null, 0, treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int> func = x => Bar(x, 3)", null, 0, treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action func = delegate {{ Bar(0, 0); }}", null, 0, treatMultilineParametersAsSplit };
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "", "", false)]
        [MemberData(nameof(GetTestDeclarations), "", "", true)]
        public async Task TestValidDeclarationAsync(string declaration, string fixedDeclaration, int column, bool treatMultilineParametersAsSplit)
        {
            // Not needed for this test
            _ = fixedDeclaration;
            _ = column;

            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "\r\n", "\r\n        ", false)]
        [MemberData(nameof(GetTestDeclarations), "\r\n", "\r\n        ", true)]
        public async Task TestInvalidDeclarationAsync(string declaration, string fixedDeclaration, int column, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            var fixedCode = $@"
class Foo
{{
    {fixedDeclaration}
}}";
            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(4, column) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "", "", false)]
        [MemberData(nameof(GetTestConstructorInitializers), "", "", true)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "", "", false)]
        public async Task TestValidConstructorInitializerAsync(string initializer, string fixedInitializer, bool treatMultilineParametersAsSplit)
        {
            // Not needed for this test
            _ = fixedInitializer;

            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Base
{{
    public Base(int a, string b)
    {{
    }}

    public Base(int c, System.Action d)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int w, string x)
        : base(w, x)
    {{
    }}

    public Derived(int y, System.Action z)
        : base(y, z)
    {{
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n", "\r\n            ", false)]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n", "\r\n            ", true)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "", "\r\n            ", true)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "\r\n", "\r\n            ", false)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "\r\n", "\r\n            ", true)]
        public async Task TestInvalidConstructorInitializerAsync(string initializer, string fixedInitializer, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Base
{{
    public Base(int a, string b)
    {{
    }}

    public Base(int c, System.Action d)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int w, string x)
        : base(w, x)
    {{
    }}

    public Derived(int y, System.Action z)
        : base(y, z)
    {{
    }}
}}";
            var fixedCode = $@"
class Base
{{
    public Base(int a, string b)
    {{
    }}

    public Base(int c, System.Action d)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {fixedInitializer}
    {{
    }}

    public Derived(int w, string x)
        : base(w, x)
    {{
    }}

    public Derived(int y, System.Action z)
        : base(y, z)
    {{
    }}
}}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(16, 16) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "", "", false)]
        [MemberData(nameof(GetTestExpressions), "", "", true)]
        [MemberData(nameof(GetLambdaTestExpressions), "", "", false)]
        [MemberData(nameof(GetLambdaTestExpressions), "", "", true)]
        [MemberData(nameof(GetLambdaTestExpressions), "\r\n", "", false)]
        [MemberData(nameof(ValidTestExpressions), false)]
        [MemberData(nameof(ValidTestExpressions), true)]
        public async Task TestValidExpressionAsync(string expression, string fixedExpression, int column, bool treatMultilineParametersAsSplit)
        {
            // Not needed for this test
            _ = fixedExpression;
            _ = column;

            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    public void Bar(int a, int b)
    {{
    }}

    public void Buz(System.Action c, System.Action d)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int e, int f] => e + f;
}}";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "\r\n", "\r\n            ", false)]
        [MemberData(nameof(GetTestExpressions), "\r\n", "\r\n            ", true)]
        [MemberData(nameof(GetLambdaTestExpressions), "\r\n", "\r\n            ", true)]
        public async Task TestInvalidExpressionAsync(string expression, string fixedExpression, int column, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    public void Bar(int a, int b)
    {{
    }}

    public void Buz(System.Action c, System.Action d)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int e, int f] => e + f;
}}";
            var fixedCode = $@"
class Foo
{{
    public void Bar(int a, int b)
    {{
    }}

    public void Buz(System.Action c, System.Action d)
    {{
    }}

    public void Baz()
    {{
        {fixedExpression};
    }}

    public long this[int e, int f] => e + f;
}}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(14, column) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestImplicitElementAccessAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    private System.Collections.Generic.Dictionary<int, string> dict = new System.Collections.Generic.Dictionary<int, string>
    {
        [1] = ""foo""
    };
}";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestValidAttributeAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(1, 2)]
class Foo
{
}

// This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
[System.Obsolete]
class ObsoleteType
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInvalidAttributeAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(1,
      2)]
class Foo
{
}";
            var fixedCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(
    1,
      2)]
class Foo
{
}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(10, 14) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        protected static string GetSettings(bool treatMultilineParametersAsSplit = false)
        {
            return $@"
{{
    ""settings"": {{
        ""readabilityRules"": {{
            ""treatMultilineParametersAsSplit"" : {treatMultilineParametersAsSplit.ToString().ToLowerInvariant()}
        }}
    }}
}}
";
        }
    }
}
