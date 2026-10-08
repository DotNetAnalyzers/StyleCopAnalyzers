// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1648InheritDocMustBeUsedWithInheritingClass>;

    public partial class SA1648CSharp10UnitTests : SA1648CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that <c>&lt;inheritdoc/&gt;</c> is allowed on a sealed <c>ToString</c> override in a record.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestInheritDocOnSealedToStringInRecordAsync()
        {
            var testCode = @"/// <summary>A record.</summary>
public record R
{
    /// <inheritdoc/>
    public sealed override string ToString() => ""R"";
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
