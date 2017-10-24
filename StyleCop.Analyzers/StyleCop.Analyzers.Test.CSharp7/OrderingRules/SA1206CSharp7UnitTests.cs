// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1206CSharp7UnitTests : SA1206UnitTests
    {
        [Fact(Skip = "The version of the compiler used in these tests does not yet support this feature.")]
        public async Task TestRefKeywordInStructDeclarationAsync()
        {
            var testCode = @"private ref struct BitHelper
{
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact(Skip = "The version of the compiler used in these tests does not yet support this feature.")]
        public async Task TestRefKeywordInStructDeclarationWrongOrderAsync()
        {
            var testCode = @"ref private struct BitHelper
{
}
";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(1, 13).WithArguments("ref", "private"),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
