// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp10UnitTests : SA1101CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3472")]
        public async Task TestExtendedPropertyPatternAsync()
        {
            var testCode = @"public class Test
{
    public Test Inner;
    public string Value;

    public bool Method(Test arg)
    {
        return arg is { Inner.Value: """" };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3984, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3984")]
        public async Task TestExtendedPropertyPatternInSwitchAsync()
        {
            var testCode = @"public class Test
{
    public Test Child { get; init; }
    public int Value { get; init; }

    public int Evaluate(Test other)
    {
        return other switch
        {
            { Child.Value: > 0 } and not { Value: 0 } => 1,
            { Child.Value: <= 0 } or ({ Value: 0 } and { Child.Value: 0 }) => 0,
            _ => -1,
        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3984, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3984")]
        public async Task TestExtendedPropertyPatternWithInstanceMemberAccessAsync()
        {
            var testCode = @"public class Test
{
    private int value;

    public Test Child { get; }
    public int Value { get; }

    public bool Evaluate(Test other)
    {
        return other is { Child.Value: > 0 } && {|#0:value|} > 0;
    }
}";

            var fixedCode = @"public class Test
{
    private int value;

    public Test Child { get; }
    public int Value { get; }

    public bool Evaluate(Test other)
    {
        return other is { Child.Value: > 0 } && this.value > 0;
    }
}";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
