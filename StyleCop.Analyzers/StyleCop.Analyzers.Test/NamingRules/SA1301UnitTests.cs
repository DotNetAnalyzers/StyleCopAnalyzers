namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains tests for <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/>.
    /// </summary>
    public class SA1301UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// This is a simple test which simply ensure no error is thrown by the analyzer engine during the instantiation
        /// of the <see cref="SA1301ElementMustBeginWithLowerCaseLetter"/> analyzer (e.g. from an incorrect argument to
        /// its <see cref="DiagnosticDescriptor"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSimpleEmptyNamespaceAsync()
        {
            var testCode = @"namespace Test { }";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1301ElementMustBeginWithLowerCaseLetter();
        }
    }
}
