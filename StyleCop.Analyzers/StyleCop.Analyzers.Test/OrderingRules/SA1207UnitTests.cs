namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
}
";

        public static IEnumerable<object[]> ValidDeclarations
        {
            get
            {
                return
                    from accessModifier in ValidAccessModifiers
                    from declaration in DeclarationsWithoutAccessModifier
                    select new object[] { accessModifier + " " + declaration };
            }
        }

        public static IEnumerable<object[]> InvalidDeclarations
        {
            get
            {
                return
                    from accessModifier in InvalidAccessModifiers
                    from declarationWithoutAccessModifier in DeclarationsWithoutAccessModifier
                    select new object[] { accessModifier + " " + declarationWithoutAccessModifier };
            }
        }

        private static IEnumerable<string> ValidAccessModifiers
        {
            get
            {
                yield return string.Empty;
                yield return "protected";
                yield return "internal";
                yield return "protected internal";
            }
        }

        private static IEnumerable<string> InvalidAccessModifiers
        {
            get
            {
                yield return "internal protected";
            }
        }

        private static IEnumerable<string> DeclarationsWithoutAccessModifier
        {
            get
            {
                yield return "class Bar {}";
                yield return "delegate void Bar();";
                yield return "event System.Action Bar { add {} remove {} }";
                yield return "event System.Action Bar;";
                yield return "int Bar;";
                yield return "int this[int index] { get { return 0; } }";
                yield return "interface Bar {}";
                yield return "void Bar() {}";
                yield return "int Bar { get; set; }";
                yield return "struct Bar {}";
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

        /// <summary>
        /// Verifies that an invalid type declaration will produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(InvalidDeclarations))]
        public async Task TestInvalidDeclarationAsync(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            ////var fixedTestCode = FixedTestCodeTemplate.Replace("##", "internal").Replace("$$", declaration);

            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(4, 14), CancellationToken.None).ConfigureAwait(false);
            ////await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1207ProtectedMustComeBeforeInternal();
        }
    }
}
