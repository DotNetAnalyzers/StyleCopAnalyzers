namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1205PartialElementsMustDeclareAccess"/> class.
    /// </summary>
    public class SA1205UnitTests : CodeFixVerifier
    {
        private const string TestCodeTemplate = @"$$ Foo
{
}
";

        private const string FixedTestCodeTemplate = @"## $$ Foo
{
}
";
        
        public static IEnumerable<object[]> ValidDeclarations
        {
            get
            {
                yield return new object[] { "public partial class" };
                yield return new object[] { "internal partial class" };
                yield return new object[] { "public static partial class" };
                yield return new object[] { "internal static partial class" };
                yield return new object[] { "public sealed partial class" };
                yield return new object[] { "internal sealed partial class" };
                yield return new object[] { "public partial struct" };
                yield return new object[] { "internal partial struct" };
                yield return new object[] { "public partial interface" };
                yield return new object[] { "internal partial interface" };
                yield return new object[] { "class" };
                yield return new object[] { "struct" };
                yield return new object[] { "interface" };
            }
        }

        public static IEnumerable<object[]> InvalidDeclarations
        {
            get
            {
                yield return new object[] { "partial class" };
                yield return new object[] { "sealed partial class" };
                yield return new object[] { "static partial class" };
                yield return new object[] { "partial struct" };
                yield return new object[] { "partial interface" };
            }
        }

        /// <summary>
        /// Verify that the analyzer accepts an empty source.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a valid declaration (with an access modifier or not a partial type) will not produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        [Theory]
        [MemberData(nameof(ValidDeclarations))]
        public async Task TestValidDeclaration(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an invalid type declaration will produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        [Theory]
        [MemberData(nameof(InvalidDeclarations))]
        public async Task TestInvalidDeclaration(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            var publicFixedTestCode = FixedTestCodeTemplate.Replace("##", "public").Replace("$$", declaration);
            var internalFixedTestCode = FixedTestCodeTemplate.Replace("##", "internal").Replace("$$", declaration);

            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(1, 1), CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(publicFixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(internalFixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, publicFixedTestCode, 0);
            await this.VerifyCSharpFixAsync(testCode, internalFixedTestCode, 1);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1205PartialElementsMustDeclareAccess();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1205CodeFixProvider();
        }
    }
}
