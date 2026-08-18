// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public partial class SA1117CSharp13UnitTests : SA1117CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestParamsCollectionParameterOnSeparateLineAsync()
        {
            var testCode = @"
using System;

class Foo
{
    public Foo(int a, int b,
        [|params ReadOnlySpan<int> s|]) { }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
