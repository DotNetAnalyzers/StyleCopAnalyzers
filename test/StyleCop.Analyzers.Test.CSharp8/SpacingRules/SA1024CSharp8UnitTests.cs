// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1024CSharp8UnitTests : SA1024CSharp7UnitTests
    {
        /// <summary>
        /// Verifies the behavior of colons in case patterns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1013CSharp8UnitTests.TestSpacingAroundClosingBraceInPatternAsync"/>
        [Fact]
        [WorkItem(3053, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3053")]
        public async Task TestColonAfterClosingBraceInPatternAsync()
        {
            string testCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        case Exception { Message: { } message } {|#0::|}
            break;
        }
    }
}";
            string fixedCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        case Exception { Message: { } message }:
            break;
        }
    }
}";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternColonSpacingAsync()
        {
            string testCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value is { X {|#0::|}1, Y{|#1::|}2 };
}

public class SomeType
{
    public int X { get; set; }
    public int Y { get; set; }
}
";

            string fixedCode = @"
public class TestClass
{
    public bool Test(SomeType value) => value is { X: 1, Y: 2 };
}

public class SomeType
{
    public int X { get; set; }
    public int Y { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorFollowed).WithLocation(0),
                Diagnostic(DescriptorFollowed).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
