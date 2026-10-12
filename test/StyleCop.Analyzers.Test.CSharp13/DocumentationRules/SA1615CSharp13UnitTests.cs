// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1615ElementReturnValueMustBeDocumented>;

    public partial class SA1615CSharp13UnitTests : SA1615CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestMethodWithParamsCollectionParameterMissingReturnsDocumentationAsync()
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
    public [|int|] TestMethod(params IEnumerable<int> values) { return 0; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
