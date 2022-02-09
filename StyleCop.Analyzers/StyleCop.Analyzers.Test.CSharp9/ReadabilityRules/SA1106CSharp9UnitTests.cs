// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public class SA1106CSharp9UnitTests : SA1106CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3267, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3267")]
        public async Task TestNoDiagnosticForEmptyRecordDeclarationAsync()
        {
            var testCode = @"public record Result(int Value);";

            await new CSharpTest(LanguageVersion.CSharp9)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
