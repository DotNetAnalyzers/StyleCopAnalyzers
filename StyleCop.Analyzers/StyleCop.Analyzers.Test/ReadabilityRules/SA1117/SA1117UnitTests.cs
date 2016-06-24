// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1117UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> GetTestDeclarations(string delimiter)
        {
            yield return new object[] { $"public Foo(int a, int b,{delimiter} string s) {{ }}" };
            yield return new object[] { $"public object Bar(int a, int b,{delimiter} string s) => null;" };
            yield return new object[] { $"public object this[int a, int b,{delimiter} string s] => null;" };
            yield return new object[] { $"public delegate void Bar(int a, int b,{delimiter} string s);" };
        }

        public static IEnumerable<object[]> GetTestConstructorInitializers(string delimiter)
        {
            yield return new object[] { $"this(42, 43, {delimiter} \"hello\")" };
            yield return new object[] { $"base(42, 43, {delimiter} \"hello\")" };
        }

        public static IEnumerable<object[]> GetTestExpressions(string delimiter)
        {
            yield return new object[] { $"Bar(1, 2, {delimiter} 2)", 13 };
            yield return new object[] { $"System.Action<int, int, int> func = (int x, int y, {delimiter} int z) => Bar(x, y, z)", 41 };
            yield return new object[] { $"System.Action<int, int, int> func = delegate(int x, int y, {delimiter} int z) {{ Bar(x, y, z); }}", 49 };
            yield return new object[] { $"new System.DateTime(2015, 9, {delimiter} 14)", 20 };
            yield return new object[] { $"var arr = new string[2, 2, {delimiter} 2];", 30 };
            yield return new object[] { $"char cc = (new char[3, 3, 3])[2, 2,{delimiter} 2];", 36 };
            yield return new object[] { $"char? c = (new char[3, 3, 3])?[2, 2,{delimiter} 2];", 37 };
            yield return new object[] { $"long ll = this[2, 2,{delimiter} 2];", 24 };
        }

        public static IEnumerable<object[]> ValidTestExpressions()
        {
            yield return new object[] { $"System.Action func = () => Bar(0, 2, 3)", 0 };
            yield return new object[] { $"System.Action<int> func = x => Bar(x, 2, 3)", 0 };
            yield return new object[] { $"System.Action func = delegate {{ Bar(0, 0, 0); }}", 0 };
            yield return new object[] { "var weird = new int[10][,,,];", 0 };
        }

        public static IEnumerable<object[]> ValidTestDeclarations()
        {
            yield return new object[]
            {
                $@"public Foo(
    int a, int b, string s) {{ }}"
            };
            yield return new object[]
            {
                $@"public Foo(
    int a,
    int b,
    string s) {{ }}"
            };
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "")]
        [MemberData(nameof(ValidTestDeclarations))]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "\r\n")]
        public async Task TestInvalidDeclarationAsync(string declaration)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "")]
        public async Task TestValidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, int b, string s)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int i, int j, string z)
        : base(i, j, z)
    {{
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n")]
        public async Task TestInvalidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, int b, string s)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int i, int j, string z)
        : base(i, j, z)
    {{
    }}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "")]
        [MemberData(nameof(ValidTestExpressions))]
        public async Task TestValidExpressionAsync(string expression, int column)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int i, int j, int k)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int b, int s] => a + b + s;
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "\r\n")]
        public async Task TestInvalidExpressionAsync(string expression, int column)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int i, int j, int k)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int b, int s] => a + b + s;
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 2);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidAttributeAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b, int c)
    {
    }
}

[MyAttribute(1, 2, 3)]
class Foo
{
}

// This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
[System.Obsolete]
class ObsoleteType
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidAttributeAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b, int c)
    {
    }
}

[MyAttribute(
    1,
    2, 3)]
class Foo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 8);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1117ParametersMustBeOnSameLineOrSeparateLines();
        }
    }
}
