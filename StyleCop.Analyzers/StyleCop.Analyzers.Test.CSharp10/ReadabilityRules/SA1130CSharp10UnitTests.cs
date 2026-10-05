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
        StyleCop.Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public partial class SA1130CSharp10UnitTests : SA1130CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the parameter types are kept when there is no target delegate type to infer them from, since
        /// the lambda would otherwise have no natural type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestAnonymousMethodWithNaturalTypeAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        var a = [|delegate|](int x) { return x; };
        var b = [|delegate|](int x, int y) { return x + y; };
        var c = [|delegate|]() { };
        object d = [|delegate|](int x) { return x; };
        Delegate e = [|delegate|](int x) { return x; };
    }
}";

            var fixedCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        var a = (int x) => { return x; };
        var b = (int x, int y) => { return x + y; };
        var c = () => { };
        object d = (int x) => { return x; };
        Delegate e = (int x) => { return x; };
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
