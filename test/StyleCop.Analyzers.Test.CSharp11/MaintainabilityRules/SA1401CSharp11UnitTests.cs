// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.MaintainabilityRules.SA1401FieldsMustBePrivate>;

    public partial class SA1401CSharp11UnitTests : SA1401CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that a required field is still reported when it is not private.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4000, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4000")]
        public async Task TestRequiredFieldAsync()
        {
            var testCode = @"public class Person
{
    public required string {|#0:Name|};

    public required string Last { get; init; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
