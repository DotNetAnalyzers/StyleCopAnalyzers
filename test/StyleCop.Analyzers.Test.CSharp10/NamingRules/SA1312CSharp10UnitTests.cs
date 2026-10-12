// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.NamingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1312CSharp10UnitTests : SA1312CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionAssignmentAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int existing = 0;
        (existing, int {|#0:NewValue|}) = (1, 2);
        existing = existing + NewValue;
    }
}";

            var fixedCode = @"public class TestClass
{
    public void TestMethod()
    {
        int existing = 0;
        (existing, int newValue) = (1, 2);
        existing = existing + newValue;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("NewValue").WithLocation(0),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
