// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp8UnitTests : SA1003CSharp7UnitTests
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

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp31,
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(0).WithArguments("(int)") },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3822, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3822")]
        public async Task TestNullCoalescingAssignmentOperatorAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int? x)
        {
            x{|#0:??=|}0;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int? x)
        {
            x ??= 0;
        }
    }
}
";

            var expected = new[]
            {
                    Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("??="),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("??="),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionArrowsAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(object value)
        {
            return value switch
            {
                0{|#0:=>|}""zero"",
                { }{|#1:=>|}""object"",
            };
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(object value)
        {
            return value switch
            {
                0 => ""zero"",
                { } => ""object"",
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("=>"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(1).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(1).WithArguments("=>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionArrowWithWrappedArmAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(int[] values)
        {
            return values switch
            {
                { Length: 1 }{|#0:=>|}
                    ""single"",
                _ => ""other"",
            };
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(int[] values)
        {
            return values switch
            {
                { Length: 1 } =>
                    ""single"",
                _ => ""other"",
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("=>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3822, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3822")]
        public async Task TestNullForgivingOperatorAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(string? x)
        {
            return x {|#0:!|} ;
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public string TestMethod(string? x)
        {
            return x!;
        }
    }
}
";

            var expected = new[]
            {
                    Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(0).WithArguments("!"),
                    Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(0).WithArguments("!"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
