// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for expressions part of <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>.
    /// </summary>
    public partial class SA1117UnitTests
    {
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
    }
}
