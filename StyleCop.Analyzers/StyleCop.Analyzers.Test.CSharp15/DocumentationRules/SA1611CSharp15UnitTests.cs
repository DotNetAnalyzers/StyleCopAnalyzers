// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1611ElementParametersMustBeDocumented>;

    public partial class SA1611CSharp15UnitTests : SA1611CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the case types of a union are not reported as undocumented parameters, because they have no
        /// names to document.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionCaseTypesAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string);

/// <summary>
/// A union with a body.
/// </summary>
/// <typeparam name=""T"">The value type.</typeparam>
public union Result<T>(T, string)
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
