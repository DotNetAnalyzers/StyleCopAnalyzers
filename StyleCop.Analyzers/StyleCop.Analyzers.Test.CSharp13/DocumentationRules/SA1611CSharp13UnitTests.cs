// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    public partial class SA1611CSharp13UnitTests : SA1611CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestParamsCollectionParameterMissingDocumentationAsync()
        {
            var testCode = @"
using System;

/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    public void TestMethod(params ReadOnlySpan<int> {|#0:values|})
    {
    }
}";

            var expected = new[] { Diagnostic().WithLocation(0).WithArguments("values") };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4013, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4013")]
        public async Task TestParamsCollectionParameterWithDocumentationAsync()
        {
            var testCode = @"
using System;

/// <summary>
/// Foo
/// </summary>
public class ClassName
{
    /// <summary>
    /// Foo
    /// </summary>
    /// <param name=""values"">The values.</param>
    public void TestMethod(params ReadOnlySpan<int> values)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
