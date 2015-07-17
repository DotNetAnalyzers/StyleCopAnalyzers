namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1016OpeningAttributeBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1016UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly valid bracket placement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBracketsAsync()
        {
            var testCode = @"
[System.Obsolete]
class ClassName
{
}

[
System.Obsolete]
class ClassName2
{
}

[ // Comment
System.Obsolete]
class ClassName3
{
}

class ClassName4<[MyAttribute] T>
{
}

class ClassName5<[
    MyAttribute] T>
{
    [return: MyAttribute]
    int [ ] MethodName([MyAttribute] int x) { return new int [ 3 ]; }
}

[System.AttributeUsage(System.AttributeTargets.All)]
sealed class MyAttribute : System.Attribute { }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly report invalid bracket placements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBracketsAsync()
        {
            var testCode = @"
[ System.Obsolete]
class ClassName
{
}

[  System.Obsolete]
class ClassName2
{
}
";
            var fixedCode = @"
[System.Obsolete]
class ClassName
{
}

[System.Obsolete]
class ClassName2
{
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1016OpeningAttributeBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1016CodeFixProvider();
        }
    }
}
