// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp9UnitTests : SA1008CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3230, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3230")]
        public async Task TestParenthesizedPatternAsync()
        {
            const string testCode = @"
class C
{
    void Method(int b)
    {
        _ = b is{|#0:(|} >= 0 and <= 31) or 127;
        _ = b is{|#1:(|}>= 0 and <= 31) or 127;
        _ = b is {|#2:(|} >= 0 and <= 31) or 127;
    }
}";
            const string fixedCode = @"
class C
{
    void Method(int b)
    {
        _ = b is (>= 0 and <= 31) or 127;
        _ = b is (>= 0 and <= 31) or 127;
        _ = b is (>= 0 and <= 31) or 127;
    }
}";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                ExpectedDiagnostics =
                {
                    // /0/Test0.cs(6,17): warning SA1008: Opening parenthesis should be preceded by a space.
                    Diagnostic(DescriptorPreceded).WithLocation(0),

                    // /0/Test0.cs(6,17): warning SA1008: Opening parenthesis should not be followed by a space.
                    Diagnostic(DescriptorNotFollowed).WithLocation(0),

                    // /0/Test0.cs(7,17): warning SA1008: Opening parenthesis should be preceded by a space.
                    Diagnostic(DescriptorPreceded).WithLocation(1),

                    // /0/Test0.cs(8,18): warning SA1008: Opening parenthesis should not be followed by a space.
                    Diagnostic(DescriptorNotFollowed).WithLocation(2),
                },
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3476, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3476")]
        public async Task TestLogicalTuplePatternAsync()
        {
            const string testCode = @"
class C
{
    void Method((int, int) c)
    {
        _ = c is (1, 1) or (2, 2);
        _ = c is (1, 1) and (1, 1);
        _ = c is not (2, 2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\n")]
        [InlineData("\n ")]
        [WorkItem(2354, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2354")]
        public async Task TestDeconstructionInTopLevelProgramAsync(string prefix)
        {
            var testCode = $@"{prefix}{{|#0:(|}} var a, var b) = (1, 2);";
            var fixedCode = $@"{prefix}(var a, var b) = (1, 2);";

            await new CSharpTest()
            {
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                },
                ExpectedDiagnostics =
                {
                    // /0/Test0.cs(1,1): warning SA1008: Opening parenthesis should not be followed by a space.
                    Diagnostic(DescriptorNotFollowed).WithLocation(0),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestLogicalPatternsWithParenthesesAsync()
        {
            const string testCode = @"
class C
{
    void M(int value)
    {
        _ = value is not{|#0:(|}> 0);
        _ = value is > 0 and{|#1:(|}< 5);
        _ = value is > 0 and {|#2:(|} < 5);
        _ = value is > 10 or{|#3:(|}< 5);
        _ = value is > 10 or {|#4:(|} < 5);
    }
}";

            const string fixedCode = @"
class C
{
    void M(int value)
    {
        _ = value is not (> 0);
        _ = value is > 0 and (< 5);
        _ = value is > 0 and (< 5);
        _ = value is > 10 or (< 5);
        _ = value is > 10 or (< 5);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorPreceded).WithLocation(1),
                Diagnostic(DescriptorNotFollowed).WithLocation(2),
                Diagnostic(DescriptorPreceded).WithLocation(3),
                Diagnostic(DescriptorNotFollowed).WithLocation(4),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3973, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3973")]
        public async Task TestStaticLambdaSpacingAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int> identity = static{|#0:(|}int value) => value;
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        Func<int, int> identity = static (int value) => value;
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic(DescriptorPreceded).WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
