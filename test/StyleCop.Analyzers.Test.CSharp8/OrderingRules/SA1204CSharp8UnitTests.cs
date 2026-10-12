// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1204StaticElementsMustAppearBeforeInstanceElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1204CSharp8UnitTests : SA1204CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3002")]
        public async Task TestStaticMemberAfterDefaultInterfaceMethodAsync()
        {
            var testCode = @"public interface ITest
{
    void InstanceMethod()
    {
    }

    static void [|StaticMethod|]()
    {
    }
}
";

            var fixedCode = @"public interface ITest
{
    static void StaticMethod()
    {
    }

    void InstanceMethod()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
