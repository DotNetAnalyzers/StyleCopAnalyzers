// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1131UseReadableConditions,
        StyleCop.Analyzers.ReadabilityRules.SA1131CodeFixProvider>;

    public partial class SA1131CSharp8UnitTests : SA1131CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3006")]
        public async Task TestPatternMatchingNullChecksAsync()
        {
            var testCode = @"#nullable enable
public class TestClass
{
    public bool IsNull(string? value)
    {
        return value is null;
    }

    public bool IsNotNull(string? value)
    {
        return value is { };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
