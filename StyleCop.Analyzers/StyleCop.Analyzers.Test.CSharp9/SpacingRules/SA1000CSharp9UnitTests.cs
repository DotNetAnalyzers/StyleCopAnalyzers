// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;

    public class SA1000CSharp9UnitTests : SA1000CSharp8UnitTests
    {
        [Fact]
        public async Task TestTargetTypedNewAsync()
        {
            string statementWithoutSpace = "int a = new();";

            await this.TestKeywordStatementAsync(statementWithoutSpace, DiagnosticResult.EmptyDiagnosticResults, statementWithoutSpace, languageVersion: LanguageVersion.CSharp9).ConfigureAwait(false);
        }
    }
}
