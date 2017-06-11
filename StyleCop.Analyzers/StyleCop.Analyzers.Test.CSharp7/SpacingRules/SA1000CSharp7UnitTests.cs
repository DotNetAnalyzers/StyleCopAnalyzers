// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.SpacingRules;
    using TestHelper;
    using Xunit;

    public class SA1000CSharp7UnitTests : SA1000UnitTests
    {
        [Fact]
        public async Task TestOutVariableDeclarationAsync()
        {
            string statementWithoutSpace = @"int.TryParse(""0"", out@Int32 x);";

            string statementWithSpace = @"int.TryParse(""0"", out @Int32 x);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("out", string.Empty, "followed").WithLocation(12, 31);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestVarKeywordTupleTypeAsync()
        {
            string statementWithoutSpace = @"var(a, b) = (2, 3);";

            string statementWithSpace = @"var (a, b) = (2, 3);";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("var", string.Empty, "followed").WithLocation(12, 13);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRefExpressionAndTypeAsync()
        {
            string statementWithoutSpace = @"
int a = 0;
ref@Int32 b = ref@Call(ref@a);

ref@Int32 Call(ref@Int32 p)
    => ref@p;
";

            string statementWithSpace = @"
int a = 0;
ref @Int32 b = ref @Call(ref @a);

ref @Int32 Call(ref @Int32 p)
    => ref @p;
";

            await this.TestKeywordStatementAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 1),
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 15),
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(14, 24),
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(16, 1),
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(16, 16),
                this.CSharpDiagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(17, 8),
            };

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }
    }
}
