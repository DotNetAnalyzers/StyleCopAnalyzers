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
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1300CSharp8UnitTests : SA1300CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3005")]
        public async Task TestLowerCaseStaticLocalFunctionAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        static void {|#0:localFunction|}()
        {
        }
    }
}";
            var fixedCode = @"public class TestClass
{
    public void Method()
    {
        static void LocalFunction()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithArguments("localFunction").WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
