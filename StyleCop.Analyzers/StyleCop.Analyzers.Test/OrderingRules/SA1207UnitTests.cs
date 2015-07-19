namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1207ProtectedMustComeBeforeInternal"/> class.
    /// </summary>
    public class SA1207UnitTests : CodeFixVerifier
    {
        private const string TestCodeTemplate = @"
public class Foo
{
    $$
    {
    }
}
";

        public static IEnumerable<object[]> ValidDeclarations
        {
            get
            {
                yield return new object[] { "protected internal void Bar()" };
                yield return new object[] { "protected internal class Bar" };
                yield return new object[] { "protected internal struct Bar" };
            }
        }

        /// <summary>
        /// Verify that the analyzer accepts an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid declaration will not produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(ValidDeclarations))]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1207ProtectedMustComeBeforeInternal();
        }
    }
}
