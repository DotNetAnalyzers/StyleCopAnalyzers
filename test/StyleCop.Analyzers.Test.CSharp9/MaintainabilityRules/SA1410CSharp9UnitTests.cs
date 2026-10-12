// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1410RemoveDelegateParenthesisWhenPossible,
        StyleCop.Analyzers.MaintainabilityRules.SA1410SA1411CodeFixProvider>;

    public partial class SA1410CSharp9UnitTests : SA1410CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticAnonymousMethodAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int> getValue = static delegate{|#0:()|} { return 3; };
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int> getValue = static delegate { return 3; };
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
