// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1513ClosingBraceMustBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1513CodeFixProvider>;

    public partial class SA1513CSharp8UnitTests : SA1513CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionArmPropertyPatternNoBlankLineRequiredAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Point value) =>
        value switch
        {
            { X: 1, Y: 2 } => 1,
            { X: 3, Y: 4 } => 2,
            _ => 0,
        };
}

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternInsideSwitchExpressionArmNoBlankLineRequiredAsync()
        {
            var testCode = @"
public class TestClass
{
    public string Test(Wrapper wrapper) =>
        wrapper.Value switch
        {
            { Length: 0 } => ""empty"",
            { Length: var length } when length > 0 => ""nonempty"",
            _ => ""unknown"",
        };
}

public class Wrapper
{
    public string Value { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3288, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3288")]
        public async Task TestNestedPropertyPatternWithContinuationTokensAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Container value) =>
        value switch
        {
            {
                Inner:
                {
                    Count: 1,
                },
            } => 1,
            _ => 0,
        };
}

public class Container
{
    public Item Inner { get; set; }
}

public class Item
{
    public int Count { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3288, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3288")]
        public async Task TestPropertyPatternFollowedByCommaAndCloseParenAsync()
        {
            var testCode = @"
public class TestClass
{
    public (bool, int) Test(Wrapper wrapper)
    {
        return (
            wrapper.Value is
            {
                Length: var length,
            },
            1);
    }
}

public class Wrapper
{
    public string Value { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3319, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3319")]
        public async Task TestRecursivePatternDesignationInSwitchLabelAsync()
        {
            var testCode = @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class TestClass
{
    public void Test(SyntaxNode syntaxNode)
    {
        switch (syntaxNode)
        {
            case MethodDeclarationSyntax
            {
                Parent: ClassDeclarationSyntax { Identifier: { ValueText: ""ResultExtensions"" } },
                Modifiers: var modifiers,
                ParameterList: { Parameters: var parameters },
            } methodDeclarationSyntax:
                _ = modifiers;
                break;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3319, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3319")]
        public async Task TestRecursivePatternDesignationInIsExpressionAsync()
        {
            var testCode = @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class TestClass
{
    public bool Test(SyntaxNode syntaxNode)
    {
        if (syntaxNode is MethodDeclarationSyntax
            {
                Parent: ClassDeclarationSyntax { Identifier: { ValueText: ""ResultExtensions"" } },
                Modifiers: var modifiers,
                ParameterList: { Parameters: var parameters },
            } methodDeclarationSyntax)
        {
            return methodDeclarationSyntax is object;
        }

        return false;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3320, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3320")]
        public async Task TestLargePropertyPatternInSwitchExpressionAsync()
        {
            var testCode = @"
public class TestClass
{
    public static int DummyMethodUsingSwitch(Outer foo) =>
        foo switch
        {
            {
                Foo: { Bar: ""foo bar"", Baz: true },
                Supercalifragilisticexpialidocious: { Something: int smth, SomethingElse: int smthElse }
            } => smth + smthElse,
            _ => 0,
        };
}

public class Outer
{
    public Inner Foo { get; set; }

    public Nested Supercalifragilisticexpialidocious { get; set; }
}

public class Inner
{
    public string Bar { get; set; }

    public bool Baz { get; set; }
}

public class Nested
{
    public int Something { get; set; }

    public int SomethingElse { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3692, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3692")]
        public async Task TestSwitchExpressionWithMultilinePropertyPatternArmAsync()
        {
            var testCode = @"
public class TestClass
{
    public string Test(TestClass value) =>
        value switch
        {
            {
                thisIsAReallyLongMemberNameAndItIsReallyReallyLongEvenLongerThanYouMayThink:
                42
            } => ""extremely unusual result"",
            _ => ""expected result"",
        };

    public int HighlyUnexpectedLargerThanLifeInteger { get; }

    public int thisIsAReallyLongMemberNameAndItIsReallyReallyLongEvenLongerThanYouMayThink { get; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3692, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3692")]
        public async Task TestIsPatternWithMultilinePropertyPatternAsync()
        {
            var testCode = @"
public class TestClass
{
    public string Test(TestClass value)
    {
        if (value is
            {
                thisIsAReallyLongMemberNameAndItIsReallyReallyLongEvenLongerThanYouMayThink:
                42
            } && 1 == 1)
        {
            return ""expected result"";
        }

        return ""other"";
    }

    public int HighlyUnexpectedLargerThanLifeInteger { get; }

    public int thisIsAReallyLongMemberNameAndItIsReallyReallyLongEvenLongerThanYouMayThink { get; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
