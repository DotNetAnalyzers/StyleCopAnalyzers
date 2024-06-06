// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SX1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SX1116CodeFixProvider>;

    public partial class SX1116CSharp9UnitTests : SX1116CSharp8UnitTests
    {
        public static IEnumerable<object[]> GetTargetTypedNewTestExpressions(string delimiter, string fixDelimiter)
        {
            yield return new object[] { $"Foo x = new(1,{delimiter} 2)", $"Foo x = new({fixDelimiter}1,{delimiter} 2)", 21 };
            yield return new object[]
            {
                $"System.Lazy<int> x = new(() =>{delimiter} {{{delimiter} return 1;{delimiter} }},{delimiter} true)",
                $"System.Lazy<int> x = new({fixDelimiter}() =>{delimiter} {{{delimiter} return 1;{delimiter} }},{delimiter} true)",
                34,
            };
        }

        [Theory]
        [MemberData(nameof(GetTargetTypedNewTestExpressions), "\r\n", "\r\n            ")]
        public async Task TestTargetTypedNewExpressionAsync(string expression, string fixedExpression, int column)
        {
            var testCode = $@"
class Foo
{{
    public Foo(int a, int b)
    {{
    }}

    public void Method()
    {{
        {expression};
    }}
}}";

            var fixedCode = $@"
class Foo
{{
    public Foo(int a, int b)
    {{
    }}

    public void Method()
    {{
        {fixedExpression};
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, column);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
