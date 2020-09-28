// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1414TupleTypesInSignaturesShouldHaveElementNames>;

    /// <summary>
    /// This class contains the CSharp 7.x unit tests for SA1414.
    /// </summary>
    /// <seealso cref="SA1414TupleTypesInSignaturesShouldHaveElementNames"/>
    public class SA1414CSharp7UnitTests
    {
        [Theory]
        [InlineData("(int valueA, int valueB)")]
        [InlineData("(int valueA, (long valueB, float valueC, string valueD) valueE)")]
        [InlineData("System.Collections.Generic.List<(int valueA, long valueB)>")]
        public async Task ValidateTuplesWithElementNamesAsync(string typeExpression)
        {
            var testCode = $@"using System;

public delegate {typeExpression} TestDelegate({typeExpression} p1);

public class TestClass
{{
    public TestClass({typeExpression} p1)
    {{
    }}

    public {typeExpression} TestMethod({typeExpression} p1)
    {{
        throw new NotImplementedException();
    }}

    public {typeExpression} TestProperty {{ get; set; }}
    public {typeExpression} this[int index] 
    {{
        get 
        {{
            throw new NotImplementedException();
        }}
    }}
  
    public static explicit operator TestClass({typeExpression} p1)
    {{
        throw new NotImplementedException();
    }}

    public static implicit operator {typeExpression}(TestClass p1)
    {{
        throw new NotImplementedException();
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("([|int|], [|int|])")]
        [InlineData("(int valueA, [|int|])")]
        [InlineData("([|int|], int valueB)")]
        [InlineData("([|int|], [|([|long|], [|float|], [|string|])|])")]
        [InlineData("([|int|], (long valueB, [|float|], string valueD) valueE)")]
        [InlineData("(int valueA, ([|long|], float valueC, [|string|]) valueE)")]
        [InlineData("System.Collections.Generic.List<([|int|], [|long|])>")]
        public async Task ValidateTuplesWithoutElementNamesAsync(string typeExpression)
        {
            var testCode = $@"using System;

public delegate {typeExpression} TestDelegate({typeExpression} p1);

public class TestClass
{{
    public TestClass({typeExpression} p1)
    {{
    }}

    public {typeExpression} TestMethod({typeExpression} p1)
    {{
        throw new NotImplementedException();
    }}

    public {typeExpression} TestProperty {{ get; set; }}
    public {typeExpression} this[int index] 
    {{
        get 
        {{
            throw new NotImplementedException();
        }}
    }}
  
    public static explicit operator TestClass({typeExpression} p1)
    {{
        throw new NotImplementedException();
    }}

    public static implicit operator {typeExpression}(TestClass p1)
    {{
        throw new NotImplementedException();
    }}
}}
";

            DiagnosticResult[] expected =
            {
                // The actual locations have been specified inline.
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
