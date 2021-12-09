// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public class SA1003CSharp8UnitTests : SA1003CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that spacing around a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to surround the range expression with spaces.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3386, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3386")]
        public async Task TestRangeExpressionAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = .. {|#0:(|}int)1;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public void TestMethod()
        {
            var test1 = ..(int)1;
        }
    }
}
";

            await new CSharpTest(LanguageVersion.CSharp8)
            {
                ReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp31,
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(0).WithArguments("(int)") },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
