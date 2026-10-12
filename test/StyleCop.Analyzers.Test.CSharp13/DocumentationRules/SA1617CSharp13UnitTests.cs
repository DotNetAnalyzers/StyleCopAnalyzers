// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1617VoidReturnValueMustNotBeDocumented>;

    public partial class SA1617CSharp13UnitTests : SA1617CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestVoidMethodWithParamsCollectionParameterAndReturnsDocumentationAsync()
        {
            var testCode = @"
using System.Collections.Generic;

/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""values"">The values.</param>
    /// [|<returns>Nothing.</returns>|]
    public void TestMethod(params IEnumerable<int> values)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
