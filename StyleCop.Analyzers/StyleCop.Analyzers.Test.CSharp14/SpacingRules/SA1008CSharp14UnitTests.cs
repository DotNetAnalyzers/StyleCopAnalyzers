// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp14UnitTests : SA1008CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordInvocationAsync()
        {
            var testCode = @"
public class TestClass
{
    private int result;

    public System.Func<int, int> Handler
    {
        get => field;
        set
        {
            field = value;
            this.result = field {|#0:(|}9);
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int result;

    public System.Func<int, int> Handler
    {
        get => field;
        set
        {
            field = value;
            this.result = field(9);
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4027, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4027")]
        public async Task TestSimpleLambdaParameterWithRefModifierAsync()
        {
            var testCode = @"
public delegate void RefIntAction(ref int value);

public class TestClass
{
    public void Method()
    {
        RefIntAction action = {|#0:(|} ref @x) => { x = 1; };
    }
}
";

            var fixedCode = @"
public delegate void RefIntAction(ref int value);

public class TestClass
{
    public void Method()
    {
        RefIntAction action = (ref @x) => { x = 1; };
    }
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestInstanceIncrementOperatorDeclarationWithSpaceBeforeParenthesisAsync()
        {
            var testCode = @"
public class TestClass
{
    private int value;

    public void operator ++ {|#0:(|})
    {
        this.value++;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private int value;

    public void operator ++()
    {
        this.value++;
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithSpaceBeforeParenthesisAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension {|#0:(|}string source)
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithSpaceAfterParenthesisAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension{|#0:(|} string source)
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
    }
}
";

            var expected = Diagnostic(DescriptorNotFollowed).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
