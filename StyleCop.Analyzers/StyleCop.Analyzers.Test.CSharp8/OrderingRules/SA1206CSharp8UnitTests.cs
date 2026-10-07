// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CSharp8UnitTests : SA1206CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3005")]
        public async Task TestStaticLocalFunctionIncorrectOrderAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        async {|#0:static|} void Local()
        {
        }
    }
}";
            var fixedCode = @"public class TestClass
{
    public void Method()
    {
        static async void Local()
        {
        }
    }
}";

            var expected = Diagnostic().WithArguments("static", "async").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3005")]
        public async Task TestStaticUnsafeLocalFunctionIncorrectOrderAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        unsafe {|#0:static|} void Local()
        {
        }
    }
}";
            var fixedCode = @"public class TestClass
{
    public void Method()
    {
        static unsafe void Local()
        {
        }
    }
}";

            var expected = Diagnostic().WithArguments("static", "unsafe").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
