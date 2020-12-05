// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1200UsingDirectivesMustBePlacedCorrectly,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1200CSharp9OutsideNamespaceUnitTests : SA1200CSharp8OutsideNamespaceUnitTests
    {
        /// <summary>
        /// Verifies that having using statements in the compilation unit will not produce diagnostics for top-level
        /// programs.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3243, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3243")]
        public async Task TestValidUsingStatementsInTopLevelProgramAsync()
        {
            var testCode = @"using System;
using System.Threading;

return 0;
";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                Settings = TestSettings,
                SolutionTransforms =
                {
                    (solution, projectId) =>
                    {
                        var project = solution.GetProject(projectId);
                        var options = project.CompilationOptions;
                        return solution.WithProjectCompilationOptions(projectId, options.WithOutputKind(OutputKind.ConsoleApplication));
                    },
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
