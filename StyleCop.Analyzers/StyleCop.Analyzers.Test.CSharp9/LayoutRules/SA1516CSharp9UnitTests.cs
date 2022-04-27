// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public class SA1516CSharp9UnitTests : SA1516CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that SA1516 is reported at the correct location in top-level programs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3242, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3242")]
        public async Task TestUsingAndGlobalStatementSpacingInTopLevelProgramAsync()
        {
            var testCode = @"using System;
using System.Threading;
{|#0:return|} 0;
";
            var fixedCode = @"using System;
using System.Threading;

return 0;
";

            var test = new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                },
                FixedCode = fixedCode,
            };
            var expectedDiagnostics = this.GetExpectedResultTestUsingAndGlobalStatementSpacingInTopLevelProgram();
            test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is not reported between global statement in top-level programs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3351, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3351")]
        public async Task TestGlobalStatementSpacingInTopLevelProgramAsync()
        {
            var testCode = @"int i = 0;
return i;
";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that SA1516 is reported between global statement and record declaration in top-level programs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestGlobalStatementAndRecordSpacingInTopLevelProgramAsync()
        {
            var testCode = @"return 0;
{|#0:record|} A();
";

            var fixedCode = @"return 0;

record A();
";

            var test = new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                },
                FixedCode = fixedCode,
            };
            var expectedDiagnostics = this.GetExpectedResultTestGlobalStatementAndRecordSpacingInTopLevelProgram();
            test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestUsingAndGlobalStatementSpacingInTopLevelProgram()
        {
            // NOTE: Seems like a Roslyn bug made diagnostics be reported twice. Fixed in a later version.
            return new[]
            {
                // /0/Test0.cs(3,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),

                // /0/Test0.cs(3,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestGlobalStatementAndRecordSpacingInTopLevelProgram()
        {
            // NOTE: Seems like a Roslyn bug made diagnostics be reported twice. Fixed in a later version.
            return new[]
            {
                // /0/Test0.cs(2,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),

                // /0/Test0.cs(2,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),
            };
        }
    }
}
