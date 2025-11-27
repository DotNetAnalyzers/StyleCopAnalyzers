// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.NamingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SX1309FieldNamesMustBeginWithUnderscore,
        StyleCop.Analyzers.NamingRules.SX1309CodeFixProvider>;

    public partial class SX1309CSharp8UnitTests : SX1309CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestDiscardDesignationInPropertyPatternAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(SomeType value)
    {
        if (value is { Number: _ })
        {
            return 1;
        }

        return 0;
    }
}

public class SomeType
{
    public int Number { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestDiscardDesignationInSwitchExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(SomeType value) =>
        value switch
        {
            { Number: _ } => 1,
            _ => 0,
        };
}

public class SomeType
{
    public int Number { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
