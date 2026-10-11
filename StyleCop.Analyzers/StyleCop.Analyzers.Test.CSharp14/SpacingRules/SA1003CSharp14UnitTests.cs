// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp14UnitTests : SA1003CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordBinaryExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Prop
    {
        get => field{|#0:+|}field;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Prop
    {
        get => field + field;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("+"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("+"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordBinaryExpressionOnNetFrameworkAsync()
        {
            // The 'field' keyword does not need a newer runtime. This pins .NET Framework reference assemblies so a
            // newer language version targeting an older framework still reports operator spacing around 'field'.
            var testCode = @"
public class TestClass
{
    public int Prop
    {
        get => field{|#0:+|}field;
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Prop
    {
        get => field + field;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("+"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("+"),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.Default,
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        [InlineData("field")]
        [InlineData("this.storage")]
        public async Task TestFieldKeywordAssignmentAsync(string target)
        {
            // Operator spacing next to the 'field' keyword is reported the same way as next to any other expression.
            var testCode = $@"
public class TestClass
{{
    private int storage;

    public int Prop
    {{
        get => {target};
        set {{|#0:=>|}}{target}{{|#1:=|}} value;
    }}
}}
";

            var fixedCode = $@"
public class TestClass
{{
    private int storage;

    public int Prop
    {{
        get => {target};
        set => {target} = value;
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("=>"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(1).WithArguments("="),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
