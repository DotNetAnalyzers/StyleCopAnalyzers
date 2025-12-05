// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public partial class SA1116CSharp9UnitTests : SA1116CSharp8UnitTests
    {
        [Fact]
        public async Task TestTargetTypedNewExpressionnAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(int a, int b)
    {
    }

    public void Method()
    {
        Foo x = new(1,
            2);
    }
}";

            var fixedCode = @"
class Foo
{
    public Foo(int a, int b)
    {
    }

    public void Method()
    {
        Foo x = new(
            1,
            2);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 21);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticAnonymousFunctionFirstParameterOnSameLineAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int, int> func = static ({|#0:int first|},
            int second) => first + second;
    }
}
";

            var fixedCode = @"
using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int, int> func = static (
            int first,
            int second) => first + second;
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
