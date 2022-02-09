// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public class SA1413CSharp9UnitTests : SA1413CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3240, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3240")]
        public async Task VerifyWithInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public record R
    {
        public int A { get; set; }

        public int B { get; set; }

        void M()
        {
            _ = this with { A = 1, B = 2 };
            _ = this with { A = 1, B = 2, };
            _ = this with
            {
                A = 1,
                [|B = 2|]
            };
        }
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    public record R
    {
        public int A { get; set; }

        public int B { get; set; }

        void M()
        {
            _ = this with { A = 1, B = 2 };
            _ = this with { A = 1, B = 2, };
            _ = this with
            {
                A = 1,
                B = 2,
            };
        }
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp9, testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
