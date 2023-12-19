// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1207ProtectedMustComeBeforeInternal,
        StyleCop.Analyzers.OrderingRules.SA1207CodeFixProvider>;

    public partial class SA1207CSharp7UnitTests : SA1207UnitTests
    {
        [Fact]
        public async Task TestPrivateProtectedAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        protected private int testField;
    }
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    public class TestClass
    {
        private protected int testField;
    }
}
";

            var expectedDiagnostic = Diagnostic().WithArguments("private", "protected").WithLocation(5, 19);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_2, testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
