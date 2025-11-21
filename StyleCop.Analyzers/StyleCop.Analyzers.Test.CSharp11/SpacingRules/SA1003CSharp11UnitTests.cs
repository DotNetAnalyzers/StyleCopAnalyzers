// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp11UnitTests : SA1003CSharp10UnitTests
    {
        [Fact]
        [WorkItem(3822, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3822")]
        public async Task TestUnsignedRightShiftAssignmentOperatorAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x)
        {
            x{|#0:>>>=|}0;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int x)
        {
            x >>>= 0;
        }
    }
}
";

            var expected = new[]
            {
                    Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments(">>>="),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments(">>>="),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3822, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3822")]
        public async Task TestUnsignedRightShiftOperatorAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int x)
        {
            return x{|#0:>>>|}0;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public int TestMethod(int x)
        {
            return x >>> 0;
        }
    }
}
";

            var expected = new[]
            {
                    Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments(">>>"),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments(">>>"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
