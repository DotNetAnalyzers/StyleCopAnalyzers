// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1504AllAccessorsMustBeSingleLineOrMultiLine,
        StyleCop.Analyzers.LayoutRules.SA1504CodeFixProvider>;

    public partial class SA1504CSharp9UnitTests : SA1504CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestGetSingleLineInitMultiLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Value
    {
        {|#0:get|} { return this.value; }
        init
        {
            this.value = value;
        }
    }

    private int value;
}
";

            var fixedSingleLine = @"
public class TestClass
{
    public int Value
    {
        get { return this.value; }
        init { this.value = value; }
    }

    private int value;
}
";

            var fixedMultiLine = @"
public class TestClass
{
    public int Value
    {
        get
        {
            return this.value;
        }

        init
        {
            this.value = value;
        }
    }

    private int value;
}
";

            var expected = Diagnostic().WithLocation(0);

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedSingleLine,
                ExpectedDiagnostics = { expected },
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedMultiLine,
                ExpectedDiagnostics = { expected },
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
