// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists,
        StyleCop.Analyzers.ReadabilityRules.SA1100CodeFixProvider>;

    public partial class SA1100CSharp10UnitTests : SA1100CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that <c>base.ToString()</c> is not reported in a record that declares a sealed <c>ToString</c>
        /// override.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestBaseToStringInSealedToStringOverrideAsync()
        {
            var testCode = @"public record A;

public record B : A
{
    public sealed override string ToString() => base.ToString() + ""!"";
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>base.ToString()</c> is reported in a record that derives from a record with a sealed
        /// <c>ToString</c>. The compiler does not synthesize a <c>ToString</c> override in that case, so
        /// <c>this.ToString()</c> calls the same method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestBaseToStringWithSealedBaseToStringAsync()
        {
            var testCode = @"public record A
{
    public sealed override string ToString() => ""A"";
}

public record B : A
{
    public string M() => {|#0:base|}.ToString();
}
";

            var fixedCode = @"public record A
{
    public sealed override string ToString() => ""A"";
}

public record B : A
{
    public string M() => this.ToString();
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>base.ToString()</c> is not reported in a record that inherits a non-sealed
        /// <c>ToString</c>, because the compiler synthesizes a <c>ToString</c> override in the derived record.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3989, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3989")]
        public async Task TestBaseToStringWithSynthesizedToStringAsync()
        {
            var testCode = @"public record A;

public record B : A
{
    public string M() => base.ToString();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
