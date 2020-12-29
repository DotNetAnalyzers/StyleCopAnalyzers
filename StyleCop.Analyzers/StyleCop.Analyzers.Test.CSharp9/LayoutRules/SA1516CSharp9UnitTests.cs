// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        public async Task TestStatementSpacingInTopLevelProgramAsync()
        {
            var testCode = @"using System;
using System.Threading;
{|#0:return|} 0;
";
            var fixedCode = @"using System;
using System.Threading;

return 0;
";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                    ExpectedDiagnostics =
                    {
                        // /0/Test0.cs(3,1): warning SA1516: Elements should be separated by blank line
                        Diagnostic().WithLocation(0),

                        // /0/Test0.cs(3,1): warning SA1516: Elements should be separated by blank line
                        Diagnostic().WithLocation(0),
                    },
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
