// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp10UnitTests : SA1008CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestLambdaAttributeSpacingAsync()
        {
            var testCode = @"
using System;

class TestClass
{
    void M()
    {
        var f = [My]{|#0:(|}) => 0;
        var g = [My][Other]{|#1:(|}) => 1;
        var h = [My] () => 2;
    }
}

class MyAttribute : Attribute
{
}

class OtherAttribute : Attribute
{
}
";

            var fixedCode = @"
using System;

class TestClass
{
    void M()
    {
        var f = [My] () => 0;
        var g = [My][Other] () => 1;
        var h = [My] () => 2;
    }
}

class MyAttribute : Attribute
{
}

class OtherAttribute : Attribute
{
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorPreceded).WithLocation(0),
                    Diagnostic(DescriptorPreceded).WithLocation(1),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestLambdaReturnTypeSpacingAsync()
        {
            var testCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int){|#0:(|}int value) => (value, value);
    }
}
";

            var fixedCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int) (int value) => (value, value);
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorPreceded).WithLocation(0),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionOpeningParenthesisSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int value = 1;
        {|#0:(|} value, int newValue) = (2, 3);
    }
}";

            var fixedCode = @"public class TestClass
{
    public void TestMethod()
    {
        int value = 1;
        (value, int newValue) = (2, 3);
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic(DescriptorNotFollowed).WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task TestLineSpanDirectiveAsync()
        {
            // #line span directives are primarily used in generated code, so their parentheses are not checked
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
#line (1, 1) - (5, 60) 10 ""file.cs""
        int x = 0;
#line ( 2, 3 )-(4,5 ) ""file.cs""
        int y = 0;
#line (3, 4)  -  ( 5, 6) 10 ""file.cs""
        int z = 0;
#line default
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3986, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3986")]
        public async Task TestLambdaWithExplicitReturnTypeAsync()
        {
            var testCode = @"public class TestClass
{
    private int field;

    public void TestMethod()
    {
        var a = int{|#0:(|}int x) => x;
        var b = (int, int){|#1:(|}) => (1, 2);
        var c = ref int () => ref this.field;
        var d = static int (int x) => x;
        var e = System.Func<int> () => () => 1;
        var f = int[] () => new int[0];
        var g = int? () => null;
    }
}";

            var fixedCode = @"public class TestClass
{
    private int field;

    public void TestMethod()
    {
        var a = int (int x) => x;
        var b = (int, int) () => (1, 2);
        var c = ref int () => ref this.field;
        var d = static int (int x) => x;
        var e = System.Func<int> () => () => 1;
        var f = int[] () => new int[0];
        var g = int? () => null;
    }
}";

            var expected = new[]
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorPreceded).WithLocation(1),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3979, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3979")]
        public async Task TestRecordStructPrimaryConstructorAsync()
        {
            var testCode = @"
public record struct Foo {|#0:(|}int X);

public record struct Bar{|#1:(|} int X);
";

            var fixedCode = @"
public record struct Foo(int X);

public record struct Bar(int X);
";

            var expected = new[]
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0),
                Diagnostic(DescriptorNotFollowed).WithLocation(1),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
