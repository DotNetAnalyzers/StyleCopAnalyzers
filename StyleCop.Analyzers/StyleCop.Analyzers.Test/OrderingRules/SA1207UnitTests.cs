namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                    from invalidAndFixedAccessModifier in InvalidAndFixedAccessModifiers
                    from declarationWithoutAccessModifier in DeclarationsWithoutAccessModifier
                    select new object[] {
                        invalidAndFixedAccessModifier.Item1 + " " + declarationWithoutAccessModifier,
                        invalidAndFixedAccessModifier.Item2 + " " + declarationWithoutAccessModifier
                    };
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

        private static IEnumerable<Tuple<string, string>> InvalidAndFixedAccessModifiers
        {
            get
            {
                yield return Tuple.Create("internal protected", "protected internal");
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
        /// Verifies that an invalid type declaration will produce a diagnostic and that the code fix will solve it.
        /// </summary>
        /// <param name="invalidDeclaration">The declaration to verify.</param>
        /// <param name="fixedDeclaration">The declaration as fixed by the code fix provider.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(InvalidDeclarations))]
        public async Task TestInvalidDeclarationAsync(string invalidDeclaration, string fixedDeclaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", invalidDeclaration);
            var fixedTestCode = TestCodeTemplate.Replace("$$", fixedDeclaration);

            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(4, 14), CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1207ProtectedMustComeBeforeInternal();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1207CodeFixProvider();
        }
    }
}
