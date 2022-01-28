// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1204StaticElementsMustAppearBeforeInstanceElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public class SA1204CSharp10UnitTests : SA1204CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle ordering within a file-scoped namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileScopedNamespaceClassesAsync()
        {
            var testCode = @"namespace TestNamespace;

class TestClass1 { }
static class {|#0:TestClass2|} { }
";

            var expected = Diagnostic().WithLocation(0);

            var fixedCode = @"namespace TestNamespace;
static class TestClass2 { }

class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
