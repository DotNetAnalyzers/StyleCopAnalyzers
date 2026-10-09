// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp14UnitTests : SA1000CSharp13UnitTests
    {
        [Theory]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        [InlineData("")]
        [InlineData(" ")]
        public async Task TestExtensionBlockDeclarationAsync(string spaces)
        {
            var testCode = $@"
public static class TestClass
{{
    extension{spaces}(string source)
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4028, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4028")]
        public async Task TestFieldKeywordIsSpacedLikeAnIdentifierAsync()
        {
            // SA1000 only governs the spacing after specific keywords (such as 'new', 'return', or 'checked'). The
            // contextual 'field' keyword is used like an identifier, so SA1000 neither requires nor forbids a space
            // after it. Spacing of the operators and punctuation around 'field' is still enforced by the other spacing
            // rules (see SA1003CSharp14UnitTests, SA1008CSharp14UnitTests, etc.).
            var testCode = @"
public class TestClass
{
    public string Prop
    {
        get => field;
        set => field = value?.Trim();
    }

    public int Count
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
            field++;
            field = field.CompareTo(0) + (field);
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestSimpleLambdaParameterWithScopedModifierAsync()
        {
            var testCode = @"
public delegate void ScopedSpanAction(scoped System.Span<int> value);

public class TestClass
{
    public void Method()
    {
        ScopedSpanAction action = ({|#0:scoped|}@x) => { };
    }
}
";

            var fixedCode = @"
public delegate void ScopedSpanAction(scoped System.Span<int> value);

public class TestClass
{
    public void Method()
    {
        ScopedSpanAction action = (scoped @x) => { };
    }
}
";

            var expected = Diagnostic().WithArguments("scoped", string.Empty, "followed").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4027, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4027")]
        public async Task TestSimpleLambdaParameterWithRefModifierAsync()
        {
            var testCode = @"
public delegate void RefIntAction(ref int value);

public class TestClass
{
    public void Method()
    {
        RefIntAction action = ({|#0:ref|}@x) => { x = 1; };
    }
}
";

            var fixedCode = @"
public delegate void RefIntAction(ref int value);

public class TestClass
{
    public void Method()
    {
        RefIntAction action = (ref @x) => { x = 1; };
    }
}
";

            var expected = Diagnostic().WithArguments("ref", string.Empty, "followed").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestCheckedCompoundAssignmentAndIncrementOperatorDeclarationAsync()
        {
            // NOTE: A checked operator requires a non-checked operator as well
            var testCode = @"
public class MyClass
{
    private int value;

    public void operator {|#0:checked|}+=(int x) => this.value = checked(this.value + x);
    public void operator +=(int x) => this.value += x;

    public void operator {|#1:checked|}++() => this.value = checked(this.value + 1);
    public void operator ++() => this.value++;
}";

            var fixedCode = @"
public class MyClass
{
    private int value;

    public void operator checked +=(int x) => this.value = checked(this.value + x);
    public void operator +=(int x) => this.value += x;

    public void operator checked ++() => this.value = checked(this.value + 1);
    public void operator ++() => this.value++;
}";

            var expected = new[]
            {
                Diagnostic().WithArguments("checked", string.Empty, "followed").WithLocation(0),
                Diagnostic().WithArguments("checked", string.Empty, "followed").WithLocation(1),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
