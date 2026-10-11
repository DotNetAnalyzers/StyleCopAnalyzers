// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1028CodeMustNotContainTrailingWhitespace,
        StyleCop.Analyzers.SpacingRules.SA1028CodeFixProvider>;

    public partial class SA1028CSharp11UnitTests : SA1028CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that trailing whitespace inside the content of a multi-line raw string literal is not reported,
        /// because it is part of the string value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3993, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3993")]
        public async Task TestTrailingWhitespaceInRawStringLiteralAsync()
        {
            var testCode = "class C\n{\n    string M(int x) => $\"\"\"\n        text  \n        {x}\t\n        \"\"\";\n\n    string N() => \"\"\"\n        text \n        \"\"\";\n}\n";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
