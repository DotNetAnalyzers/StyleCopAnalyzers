// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1518UseLineEndingsCorrectlyAtEndOfFile,
        StyleCop.Analyzers.LayoutRules.SA1518CodeFixProvider>;

    public partial class SA1518CSharp10UnitTests : SA1518CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task TestMappedLineDirectiveAsLastContentDoesNotTriggerAsync()
        {
            var testCode = @"class TestClass
{
}
#line (10,1)-(10,1) ""Remapped.cs""";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
