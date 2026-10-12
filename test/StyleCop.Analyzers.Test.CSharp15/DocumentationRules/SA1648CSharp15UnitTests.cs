// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1648InheritDocMustBeUsedWithInheritingClass>;

    public partial class SA1648CSharp15UnitTests : SA1648CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that <c>&lt;inheritdoc/&gt;</c> on a union without a base list is reported exactly once, and that it is
        /// allowed on a union with a base list.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
/// {|#0:<inheritdoc/>|}
public union Pet(int, string);

/// <inheritdoc/>
public union Shape(int, string) : System.IDisposable
{
    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
