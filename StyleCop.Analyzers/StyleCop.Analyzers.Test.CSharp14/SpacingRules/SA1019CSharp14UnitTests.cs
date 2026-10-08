// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1019CSharp14UnitTests : SA1019CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldMemberAccessAsync()
        {
            var testCode = @"
public class TestClass
{
    private string result;

    public string Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = field {|#0:.|}ToUpperInvariant();
        }
    }
}
";

            var fixedCode = @"
public class TestClass
{
    private string result;

    public string Prop
    {
        get => field;
        set
        {
            field = value;
            this.result = field.ToUpperInvariant();
        }
    }
}
";

            var expected = Diagnostic(DescriptorNotPreceded).WithLocation(0).WithArguments(".");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4024, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4024")]
        public async Task TestNullConditionalAssignmentAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Value { get; set; }

    public TestClass Next { get; set; }

    public static void TestMethod(TestClass c)
    {
        c?.Value = 1;
        c?.Next?.Value += 2;
        c {|#0:?|}.Value = 3;
        c?{|#1:.|} Value = 4;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Value { get; set; }

    public TestClass Next { get; set; }

    public static void TestMethod(TestClass c)
    {
        c?.Value = 1;
        c?.Next?.Value += 2;
        c?.Value = 3;
        c?.Value = 4;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly.DescriptorNotPreceded).WithLocation(0).WithArguments("?"),
                Diagnostic(StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly.DescriptorNotFollowed).WithLocation(1).WithArguments("."),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
