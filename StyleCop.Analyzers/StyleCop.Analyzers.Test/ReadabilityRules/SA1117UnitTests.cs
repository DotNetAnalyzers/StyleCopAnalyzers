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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public class SA1117UnitTests
    {
        public static IEnumerable<object[]> GetTestDeclarations(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"public Foo(int a, int b,{delimiter} {{|#0:string s|}}) {{ }}", treatMultilineParametersAsSplit };
            yield return new object[] { $"public object Bar(int a, int b,{delimiter} {{|#0:string s|}}) => null;", treatMultilineParametersAsSplit };
            yield return new object[] { $"public object this[int a, int b,{delimiter} {{|#0:string s|}}] => null;", treatMultilineParametersAsSplit };
            yield return new object[] { $"public delegate void Bar(int a, int b,{delimiter} {{|#0:string s|}});", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetMultilineTestDeclarations(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"public Foo(int a,{delimiter} {{|#0:string\r\ns|}}) {{ }}", treatMultilineParametersAsSplit };
            yield return new object[] { $"public object Bar(int a,{delimiter} {{|#0:string\r\ns|}}) => null;", treatMultilineParametersAsSplit };
            yield return new object[] { $"public object this[int a,{delimiter} {{|#0:string\r\ns|}}] => null;", treatMultilineParametersAsSplit };
            yield return new object[] { $"public delegate void Bar(int a,{delimiter} {{|#0:string\r\ns|}});", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTestConstructorInitializers(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"this(42, 43, {delimiter} {{|#0:\"hello\"|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42, 43, {delimiter} {{|#0:\"hello\"|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"this(42, 43, {delimiter} {{|#0:() => {{ }}|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42, 43, {delimiter} {{|#0:() => {{ }}|}})", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetLeadingMultilineTestConstructorInitializers(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"this(42\r\n+ 1, {delimiter} {{|#0:43|}}, {delimiter} \"hello\")", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42\r\n+ 1, {delimiter} {{|#0:43|}}, {delimiter} \"hello\")", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTrailingMultilineTestConstructorInitializers(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"this(42, {delimiter} 43, {delimiter} {{|#0:() =>\r\n{{\r\n}}|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"base(42, {delimiter} 43, {delimiter} {{|#0:() =>\r\n{{\r\n}}|}})", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTestExpressions(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"Bar(1, 2, {delimiter} {{|#0:2|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int, int, int> func = (int x, int y, {delimiter} {{|#0:int z|}}) => Bar(x, y, z)", treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int, int, int> func = delegate(int x, int y, {delimiter} {{|#0:int z|}}) {{ Bar(x, y, z); }}", treatMultilineParametersAsSplit };
            yield return new object[] { $"new System.DateTime(2015, 9, {delimiter} {{|#0:14|}})", treatMultilineParametersAsSplit };
            yield return new object[] { $"var arr = new string[2, 2, {delimiter} {{|#0:2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"char cc = (new char[3, 3, 3])[2, 2,{delimiter} {{|#0:2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"char? c = (new char[3, 3, 3])?[2, 2,{delimiter} {{|#0:2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"long ll = this[2, 2,{delimiter} {{|#0:2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"Buz(() => {{ }}, 2,{delimiter} {{|#0:() => {{ }}|}});", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTrailingMultilineTestExpressions(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"System.Action<int, int, int> func = (int x, {delimiter} int y, {delimiter} {{|#0:int\r\nz|}}) => Bar(x, y, z)", treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int, int, int> func = delegate(int x, {delimiter} int y, {delimiter} {{|#0:int\r\nz|}}) {{ Bar(x, y, z); }}", treatMultilineParametersAsSplit };
            yield return new object[] { $"var arr = new string[2, {delimiter} {{|#0:2\r\n+ 2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"char cc = (new char[3, 3])[2, {delimiter} {{|#0:2\r\n+ 2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"char? c = (new char[3, 3])?[2, {delimiter} {{|#0:2\r\n+ 2|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"long ll = this[2,{delimiter} 2,{delimiter} {{|#0:2\r\n+ 1|}}];", treatMultilineParametersAsSplit };
            yield return new object[] { $"var str = string.Join(\r\n\"def\",{delimiter}{{|#0:\"abc\"\r\n + \"cba\"|}});", treatMultilineParametersAsSplit };
            yield return new object[] { $"Buz(() => {{ }},{delimiter} 2,{delimiter} {{|#0:() =>\r\n{{\r\n}}|}});", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetLeadingMultilineTestExpressions(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"var str = string.Join(\r\n\"abc\"\r\n + \"cba\",{delimiter}{{|#0:\"def\"|}});", treatMultilineParametersAsSplit };
            yield return new object[] { $"Bar(\r\n1\r\n + 2,{delimiter}{{|#0:3|}},\r\n 4);", treatMultilineParametersAsSplit };
            yield return new object[] { $"Buz(\r\n() =>\r\n{{\r\n}},{delimiter}{{|#0:3|}},\r\n () => {{ }});", treatMultilineParametersAsSplit };
            yield return new object[] { $"new System.Lazy<int>(\r\n() =>\r\n{{\r\nreturn 1;\r\n}},{delimiter} {{|#0:true|}})", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetTestAttributes(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"[MyAttribute(1, {delimiter}2, {{|#0:3|}})]", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> GetMultilineTestAttributes(string delimiter, bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"[MyAttribute(1, {delimiter}2, {delimiter}{{|#0:3\r\n+ 5|}})]", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> ValidTestExpressions(bool treatMultilineParametersAsSplit)
        {
            yield return new object[] { $"System.Action func = () => Bar(0, 2, 3)", treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action<int> func = x => Bar(x, 2, 3)", treatMultilineParametersAsSplit };
            yield return new object[] { $"System.Action func = delegate {{ Bar(0, 0, 0); }}", treatMultilineParametersAsSplit };
            yield return new object[] { "var weird = new int[10][,,,];", treatMultilineParametersAsSplit };
            yield return new object[] { $"new System.Lazy<int>(() => {{ return 1; }}, true)", treatMultilineParametersAsSplit };
        }

        public static IEnumerable<object[]> ValidTestDeclarations(bool treatMultilineParametersAsSplit)
        {
            yield return new object[]
            {
                $@"public Foo(
    int a, int b, string s) {{ }}",
                treatMultilineParametersAsSplit,
            };
            yield return new object[]
            {
                $@"public Foo(
    int a,
    int b,
    string s) {{ }}",
                treatMultilineParametersAsSplit,
            };
        }

        public static IEnumerable<object[]> ValidTestAttribute(bool treatMultilineParametersAsSplit)
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
            yield return new object[] { @"[System.Obsolete]", treatMultilineParametersAsSplit };
            yield return new object[]
            {
                @"[MyAttribute(
    1, 2, 3)]",
                treatMultilineParametersAsSplit,
            };
            yield return new object[]
            {
                @"[MyAttribute(
    1,
    2,
    3)]",
                treatMultilineParametersAsSplit,
            };
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "", false)]
        [MemberData(nameof(GetMultilineTestDeclarations), "\r\n", false)]
        [MemberData(nameof(GetMultilineTestDeclarations), "", false)]
        [MemberData(nameof(ValidTestDeclarations), false)]
        [MemberData(nameof(GetTestDeclarations), "", true)]
        [MemberData(nameof(GetMultilineTestDeclarations), "\r\n", true)]
        [MemberData(nameof(ValidTestDeclarations), true)]
        public async Task TestValidDeclarationAsync(string declaration, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "\r\n", false)]
        [MemberData(nameof(GetTestDeclarations), "\r\n", true)]
        [MemberData(nameof(GetMultilineTestDeclarations), "", true)]
        public async Task TestInvalidDeclarationAsync(string declaration, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = $@"
class Foo
{{
    {declaration}
}}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(0) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "", false)]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "\r\n", false)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "", false)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "\r\n", false)]
        [MemberData(nameof(GetTestConstructorInitializers), "", true)]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "\r\n", true)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "\r\n", true)]
        public async Task TestValidConstructorInitializerAsync(string initializer, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n", false)]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "", false)]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n", true)]
        [MemberData(nameof(GetLeadingMultilineTestConstructorInitializers), "", true)]
        [MemberData(nameof(GetTrailingMultilineTestConstructorInitializers), "", true)]
        public async Task TestInvalidConstructorInitializerAsync(string initializer, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(0) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "", false)]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "\r\n", false)]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "\r\n", false)]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "", false)]
        [MemberData(nameof(ValidTestExpressions), false)]
        [MemberData(nameof(GetTestExpressions), "", true)]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "\r\n", true)]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "\r\n", true)]
        [MemberData(nameof(ValidTestExpressions), true)]
        public async Task TestValidExpressionAsync(string expression, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "\r\n", false)]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "", false)]
        [MemberData(nameof(GetTestExpressions), "\r\n", true)]
        [MemberData(nameof(GetLeadingMultilineTestExpressions), "", true)]
        [MemberData(nameof(GetTrailingMultilineTestExpressions), "", true)]
        public async Task TestInvalidExpressionAsync(string expression, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(0) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestAttributes), "", false)]
        [MemberData(nameof(GetMultilineTestAttributes), "\r\n", false)]
        [MemberData(nameof(GetMultilineTestAttributes), "", false)]
        [MemberData(nameof(ValidTestAttribute), false)]
        [MemberData(nameof(GetTestAttributes), "", true)]
        [MemberData(nameof(GetMultilineTestAttributes), "\r\n", true)]
        [MemberData(nameof(ValidTestAttribute), true)]
        public async Task TestValidAttributeAsync(string attribute, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestAttributes), "\r\n", false)]
        [MemberData(nameof(GetTestAttributes), "\r\n", true)]
        [MemberData(nameof(GetMultilineTestAttributes), "", true)]
        public async Task TestInvalidAttributeAsync(string attribute, bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

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

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(0) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
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
