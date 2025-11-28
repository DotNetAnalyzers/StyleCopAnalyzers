// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
            var test1 = {|#0:..|} {|#1:(|}int)1;
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
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(0).WithArguments(".."),
                    Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(1).WithArguments("(int)"),
                },
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

        [Fact]
        [WorkItem(3008, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3008")]
        public async Task TestIndexAndRangeExpressionsAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            var last = values[^1];
            var slice = values[2..^5];
            var slice2 = values[..];
            Index start = ^5;
            Range middle = 1..4;
        }
    }
}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp31,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3008, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3008")]
        public async Task TestIndexAndRangeExpressionsSpacingViolationsAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    using System;

    public class TestClass
    {
        public void TestMethod(int[] values)
        {
            var last = values[ {|#0:^|}1];
            var compactSlice = values[1 {|#1:..|} ^2];
            var prefixRangeSpaceAfter = values[{|#2:..|} ^2];
            var suffixRangeSpaceBefore = values[1 {|#3:..|}];
            Range missingLeadingSpace = 1 {|#4:..|} 4;
            var missingTrailingSpace = 1{|#5:..|} ^2;
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
        public void TestMethod(int[] values)
        {
            var last = values[^1];
            var compactSlice = values[1..^2];
            var prefixRangeSpaceAfter = values[..^2];
            var suffixRangeSpaceBefore = values[1..];
            Range missingLeadingSpace = 1..4;
            var missingTrailingSpace = 1..^2;
        }
    }
}
";

            var expected = new[]
            {
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(0).WithArguments("^"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(1).WithArguments(".."),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(1).WithArguments(".."),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(2).WithArguments(".."),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(3).WithArguments(".."),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(4).WithArguments(".."),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(4).WithArguments(".."),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(5).WithArguments(".."),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
