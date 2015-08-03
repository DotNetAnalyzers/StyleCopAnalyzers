namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements"/>.
    /// </summary>
    public class SA1215UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOrderingAsync()
        {
            var testCode = @"public class TestClass
{
    public readonly int TestField1 = 1;
    public int TestField2;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle static readonly fields.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStaticReadonlyIsIgnoredAsync()
        {
            var testCode = @"public class TestClass
{
    public int TestField1 = 1;
    public static readonly int TestField2;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle readonly fields in classes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestReadonlyOrderingInClassAsync()
        {
            var testCode = @"public class TestClass
{
    public int TestField1;
    public readonly int TestField2 = 1;
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle readonly fields in structs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestReadonlyOrderingInStructAsync()
        {
            var testCode = @"public struct TestStruct
{
    public int TestField1;
    public readonly int TestField2;
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 25);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements();
        }
    }
}
