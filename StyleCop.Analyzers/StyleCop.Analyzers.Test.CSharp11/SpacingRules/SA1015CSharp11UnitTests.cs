// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp11UnitTests : SA1015CSharp10UnitTests
    {
        [Fact]
        [WorkItem(3487, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3487")]
        public async Task TestGenericAttributeAsync()
        {
            var testCode = $@"
using System;

public class MyAttribute<T> : Attribute
{{
}}

public class MyClass
{{
    [MyAttribute<int>]
    public double MyDouble {{ get; set; }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
