// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        public static IEnumerable<object[]> GeneratedValidDeclarations
        {
            get
            {
                return
                    from accessModifier in ValidAccessModifiers
                    from declaration in DeclarationsWithoutAccessModifier
                    select new object[] { accessModifier + " " + declaration };
            }
        }

        public static IEnumerable<object[]> GeneratedInvalidDeclarations
        {
            get
            {
                return
                    from invalidAndFixedAccessModifier in InvalidAndFixedAccessModifiers
                    from declarationWithoutAccessModifier in DeclarationsWithoutAccessModifier
                    select new object[]
                    {
                        invalidAndFixedAccessModifier.Item1 + " " + declarationWithoutAccessModifier,
                        invalidAndFixedAccessModifier.Item2,
                        invalidAndFixedAccessModifier.Item3 + " " + declarationWithoutAccessModifier
                    };
            }
        }

        public static IEnumerable<object[]> ManualInvalidDeclarations
        {
            get
            {
                yield return new object[] { "internal static protected int Bar;", 17, "protected static internal int Bar;" };
                yield return new object[] { "abstract class Qux { internal protected abstract void Bar(); }", 31, "abstract class Qux { protected internal abstract void Bar(); }" };
                yield return new object[] { "/*comment*/internal /* 2 */ protected /*3*/ int Bar;", 29, "/*comment*/protected /* 2 */ internal /*3*/ int Bar;" };
                yield return new object[] { "internal protected class Bar { protected internal int Qux; }", 10, "protected internal class Bar { protected internal int Qux; }" };
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

        private static IEnumerable<Tuple<string, int, string>> InvalidAndFixedAccessModifiers
        {
            get
            {
                yield return Tuple.Create("internal protected", 10, "protected internal");
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
        /// Verifies that a valid declaration will not produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(GeneratedValidDeclarations))]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid type declaration will produce a diagnostic and that the code fix will solve it.
        /// </summary>
        /// <param name="invalidDeclaration">The declaration to verify.</param>
        /// <param name="diagnosticColumn">The column in parameter <paramref name="invalidDeclaration"/> at which the diagnostic is expected to occur.</param>
        /// <param name="fixedDeclaration">The declaration as fixed by the code fix provider.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(GeneratedInvalidDeclarations))]
        [MemberData(nameof(ManualInvalidDeclarations))]
        public async Task TestInvalidDeclarationAsync(string invalidDeclaration, int diagnosticColumn, string fixedDeclaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", invalidDeclaration);
            var fixedTestCode = TestCodeTemplate.Replace("$$", fixedDeclaration);

            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(4, 4 + diagnosticColumn), CancellationToken.None).ConfigureAwait(false);
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
