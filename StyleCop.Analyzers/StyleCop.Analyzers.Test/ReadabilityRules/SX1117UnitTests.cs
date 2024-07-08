// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SX1117ParametersMustBeOnSameLineOrSeparateLines>;

    public class SX1117UnitTests
    {
        public static IEnumerable<object[]> GetTestDeclarations(string delimiter)
        {
            yield return new object[] { $"public Foo(int a, int b,{delimiter} {{|#0:string s|}}) {{ }}" };
            yield return new object[] { $"public object Bar(int a, int b,{delimiter} {{|#0:string s|}}) => null;" };
            yield return new object[] { $"public object this[int a, int b,{delimiter} {{|#0:string s|}}] => null;" };
            yield return new object[] { $"public delegate void Bar(int a, int b,{delimiter} {{|#0:string s|}});" };
        }

        public static IEnumerable<object[]> GetMultilineTestDeclarations(string delimiter)
        {
            yield return new object[] { $"public Foo(int a,{delimiter} {{|#0:string\r\ns|}}) {{ }}" };
            yield return new object[] { $"public object Bar(int a,{delimiter} {{|#0:string\r\ns|}}) => null;" };
            yield return new object[] { $"public object this[int a,{delimiter} {{|#0:string\r\ns|}}] => null;" };
            yield return new object[] { $"public delegate void Bar(int a,{delimiter} {{|#0:string\r\ns|}});" };
        }

        public static IEnumerable<object[]> GetTestConstructorInitializers(string delimiter)
        {
            yield return new object[] { $"this(42, 43, {delimiter} {{|#0:\"hello\"|}})" };
            yield return new object[] { $"base(42, 43, {delimiter} {{|#0:\"hello\"|}})" };
            yield return new object[] { $"this(42, 43, {delimiter} {{|#0:() => {{ }}|}})" };
            yield return new object[] { $"base(42, 43, {delimiter} {{|#0:() => {{ }}|}})" };
        }

        public static IEnumerable<object[]> GetLeadingMultilineTestConstructorInitializers(string delimiter)
        {
            yield return new object[] { $"this(42\r\n+ 1, {delimiter} {{|#0:43|}}, {delimiter} \"hello\")" };
            yield return new object[] { $"base(42\r\n+ 1, {delimiter} {{|#0:43|}}, {delimiter} \"hello\")" };
        }

        public static IEnumerable<object[]> GetTrailingMultilineTestConstructorInitializers(string delimiter)
        {
            yield return new object[] { $"this(42, {delimiter} 43, {delimiter} {{|#0:() =>\r\n{{\r\n}}|}})" };
            yield return new object[] { $"base(42, {delimiter} 43, {delimiter} {{|#0:() =>\r\n{{\r\n}}|}})" };
        }

        public static IEnumerable<object[]> GetTestExpressions(string delimiter)
        {
            yield return new object[] { $"Bar(1, 2, {delimiter} {{|#0:2|}})" };
            yield return new object[] { $"System.Action<int, int, int> func = (int x, int y, {delimiter} {{|#0:int z|}}) => Bar(x, y, z)" };
            yield return new object[] { $"System.Action<int, int, int> func = delegate(int x, int y, {delimiter} {{|#0:int z|}}) {{ Bar(x, y, z); }}" };
            yield return new object[] { $"new System.DateTime(2015, 9, {delimiter} {{|#0:14|}})" };
            yield return new object[] { $"var arr = new string[2, 2, {delimiter} {{|#0:2|}}];" };
            yield return new object[] { $"char cc = (new char[3, 3, 3])[2, 2,{delimiter} {{|#0:2|}}];" };
            yield return new object[] { $"char? c = (new char[3, 3, 3])?[2, 2,{delimiter} {{|#0:2|}}];" };
            yield return new object[] { $"long ll = this[2, 2,{delimiter} {{|#0:2|}}];" };
            yield return new object[] { $"Buz(() => {{ }}, 2,{delimiter} {{|#0:() => {{ }}|}});" };
        }

        public static IEnumerable<object[]> GetTrailingMultilineTestExpressions(string delimiter)
        {
            yield return new object[] { $"System.Action<int, int, int> func = (int x, {delimiter} int y, {delimiter} {{|#0:int\r\nz|}}) => Bar(x, y, z)" };
            yield return new object[] { $"System.Action<int, int, int> func = delegate(int x, {delimiter} int y, {delimiter} {{|#0:int\r\nz|}}) {{ Bar(x, y, z); }}" };
            yield return new object[] { $"var arr = new string[2, {delimiter} {{|#0:2\r\n+ 2|}}];" };
            yield return new object[] { $"char cc = (new char[3, 3])[2, {delimiter} {{|#0:2\r\n+ 2|}}];" };
            yield return new object[] { $"char? c = (new char[3, 3])?[2, {delimiter} {{|#0:2\r\n+ 2|}}];" };
            yield return new object[] { $"long ll = this[2,{delimiter} 2,{delimiter} {{|#0:2\r\n+ 1|}}];" };
            yield return new object[] { $"var str = string.Join(\r\n\"def\",{delimiter}{{|#0:\"abc\"\r\n + \"cba\"|}});" };
            yield return new object[] { $"Buz(() => {{ }},{delimiter} 2,{delimiter} {{|#0:() =>\r\n{{\r\n}}|}});" };
        }

        public static IEnumerable<object[]> GetLeadingMultilineTestExpressions(string delimiter)
        {
            yield return new object[] { $"var str = string.Join(\r\n\"abc\"\r\n + \"cba\",{delimiter}{{|#0:\"def\"|}});" };
            yield return new object[] { $"Bar(\r\n1\r\n + 2,{delimiter}{{|#0:3|}},\r\n 4);" };
            yield return new object[] { $"Buz(\r\n() =>\r\n{{\r\n}},{delimiter}{{|#0:3|}},\r\n () => {{ }});" };
            yield return new object[] { $"new System.Lazy<int>(\r\n() =>\r\n{{\r\nreturn 1;\r\n}},{delimiter} {{|#0:true|}})" };
        }

        public static IEnumerable<object[]> GetTestAttributes(string delimiter)
        {
            yield return new object[] { $"[MyAttribute(1, {delimiter}2, {{|#0:3|}})]" };
        }

        public static IEnumerable<object[]> GetMultilineTestAttributes(string delimiter)
        {
            yield return new object[] { $"[MyAttribute(1, {delimiter}2, {delimiter}{{|#0:3\r\n+ 5|}})]" };
        }

        public static IEnumerable<object[]> ValidTestExpressions()
        {
            yield return new object[] { $"System.Action func = () => Bar(0, 2, 3)" };
            yield return new object[] { $"System.Action<int> func = x => Bar(x, 2, 3)" };
            yield return new object[] { $"System.Action func = delegate {{ Bar(0, 0, 0); }}" };
            yield return new object[] { "var weird = new int[10][,,,];" };
            yield return new object[] { $"new System.Lazy<int>(() => {{ return 1; }}, true)" };
        }

        public static IEnumerable<object[]> ValidTestDeclarations()
        {
            yield return new object[]
            {
                $@"public Foo(
    int a, int b, string s) {{ }}",
            };
            yield return new object[]
            {
                $@"public Foo(
    int a,
    int b,
    string s) {{ }}",
            };
        }

        public static IEnumerable<object[]> ValidTestAttribute()
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
            yield return new object[] { @"[System.Obsolete]" };
            yield return new object[]
            {
                @"[MyAttribute(
    1, 2, 3)]",
            };
            yield return new object[]
            {
                @"[MyAttribute(
    1,
    2,
    3)]",
            };
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "")]
        [MemberData(nameof(GetMultilineTestDeclarations), "\r\n")]
        [MemberData(nameof(ValidTestDeclarations))]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "\r\n")]
        [MemberData(nameof(GetMultilineTestDeclarations), "")]
        public async Task TestInvalidDeclarationAsync(string declaration)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "")]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "\r\n")]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "\r\n")]
        public async Task TestValidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, int b, string c)
    {{
    }}

    public Base(int d, int e, System.Action f)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int u, int v, string w)
        : base(u, v, w)
    {{
    }}

    public Derived(int x, int y, System.Action z)
        : base(x, y, z)
    {{
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n")]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "")]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "")]
        public async Task TestInvalidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, int b, string c)
    {{
    }}

    public Base(int d, int e, System.Action f)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int u, int v, string w)
        : base(u, v, w)
    {{
    }}

    public Derived(int x, int y, System.Action z)
        : base(x, y, z)
    {{
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "")]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "\r\n")]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "\r\n")]
        [MemberData(nameof(ValidTestExpressions))]
        public async Task TestValidExpressionAsync(string expression)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int d, int e, int f)
    {{
    }}

    public void Buz(System.Action h, int i, System.Action j)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int b, int c] => a + b + c;
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "\r\n")]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "")]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "")]
        public async Task TestInvalidExpressionAsync(string expression)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int d, int e, int f)
    {{
    }}

    public void Buz(System.Action h, int i, System.Action j)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int b, int c] => a + b + c;
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestAttributes), "")]
        [MemberData(nameof(GetMultilineTestAttributes), "\r\n")]
        [MemberData(nameof(ValidTestAttribute))]
        public async Task TestValidAttributeAsync(string attribute)
        {
            var testCode = $@"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{{
    public MyAttribute(int a, int b, int c)
    {{
    }}
}}

{attribute}
class Foo
{{
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestAttributes), "\r\n")]
        [MemberData(nameof(GetMultilineTestAttributes), "")]
        public async Task TestInvalidAttributeAsync(string attribute)
        {
            var testCode = $@"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{{
    public MyAttribute(int a, int b, int c)
    {{
    }}
}}

{attribute}
class Foo
{{
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
