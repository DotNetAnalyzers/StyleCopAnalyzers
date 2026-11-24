// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules;

using System.Threading.Tasks;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201UnitTests
{
    /// <summary>
    /// Verifies that Fix All groups and alphabetizes configured methods within their accessibility group.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestFixAllOrdersMethodsAfterGroupingByConfiguredKindAsync()
    {
        var test = new CSharpTest
        {
            Settings = """
                { "settings": { "orderingRules": {
                    "elementOrder": ["accessibility", "kind", "name"],
                    "memberKindOrder": ["method"]
                } } }
                """,
            TestCode = """
                public class C
                {
                    public void Yankee() { }
                    private void Zulu() { }
                    private int {|#0:Property|} { get; set; }
                    private void {|#1:Alpha|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void Yankee() { }
                    private void Alpha() { }
                    private void Zulu() { }
                    private int Property { get; set; }
                }
                """,
            NumberOfIncrementalIterations = 2,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'property' have no configured order, but must not appear between declarations of kind 'method', which has a configured order"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNameOrderPreservesFieldWithLowerAccessibilityDependencyAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    private static int Dependency;
                    public static int Zulu = Dependency;
                    public static int Alpha;
                    public static int Yankee;
                    public static int {|#0:Bravo|};
                }
                """,
            FixedCode = """
                public class C
                {
                    private static int Dependency;
                    public static int Zulu = Dependency;
                    public static int Alpha;
                    public static int Bravo;
                    public static int Yankee;
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNameOrderCanCrossInitializerWithSameAccessibilityDependencyAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public static int Dependency;
                    public static int Zulu = Dependency;
                    public static int {|#0:Alpha|};
                }
                """,
            FixedCode = """
                public class C
                {
                    public static int Alpha;
                    public static int Dependency;
                    public static int Zulu = Dependency;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("kind", "public void Zzz() { }", "", "", """[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]""")]
    [InlineData("kind", "public void Zzz() { }", "", "", """[GeneratedCode("Tool", "1")]""")]
    [InlineData("constant", "public int Earlier;", "const ", " = 1", """[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203")]""")]
    [InlineData("readonly", "public int Earlier;", "readonly ", "", """[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1214")]""")]
    public async Task TestNameOrderDoesNotExposeEarlierOrderingViolationAsync(string trait, string previousMember, string modifier, string initializer, string attribute)
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = $$$"""{"settings":{"orderingRules":{"elementOrder":["{{{trait}}}","name"]} } }""",
            TestCode = $$"""
                #define CODE_ANALYSIS
                using System.CodeDom.Compiler;
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void ZuluMethod() { }
                    public void {|#0:AlphaMethod|}() { }
                    {{previousMember}}
                    {{attribute}}
                    public {{modifier}}int Zulu{{initializer}};
                    public {{modifier}}int Alpha{{initializer}};
                }
                """,
            FixedCode = $$"""
                #define CODE_ANALYSIS
                using System.CodeDom.Compiler;
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void AlphaMethod() { }
                    public void ZuluMethod() { }
                    {{previousMember}}
                    {{attribute}}
                    public {{modifier}}int Zulu{{initializer}};
                    public {{modifier}}int Alpha{{initializer}};
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'AlphaMethod' should appear before member 'ZuluMethod'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNameOrderPreservesFieldDependencyAcrossUnrankedKindGroupAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "name"], "memberKindOrder": ["method"] } } }""",
            TestCode = """
                public class C
                {
                    private static int Dependency;
                    public static void M() { }
                    public static int Zulu = Dependency;
                    public static int Alpha;
                    public static int Yankee;
                    public static int {|#0:Bravo|};
                }
                """,
            FixedCode = """
                public class C
                {
                    private static int Dependency;
                    public static void M() { }
                    public static int Zulu = Dependency;
                    public static int Alpha;
                    public static int Bravo;
                    public static int Yankee;
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("""[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]""")]
    [InlineData("""[GeneratedCode("Tool", "1")]""")]
    public async Task TestNameOrderDoesNotExposeEarlierConfiguredKindInterruptionAsync(string attribute)
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "name"], "memberKindOrder": ["method"] } } }""",
            TestCode = $$"""
                #define CODE_ANALYSIS
                using System.CodeDom.Compiler;
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void Yankee() { }
                    public void {|#0:Bravo|}() { }
                    {{attribute}}
                    public int Zulu;
                    public int Alpha;
                    public void Zzz() { }
                }
                """,
            FixedCode = $$"""
                #define CODE_ANALYSIS
                using System.CodeDom.Compiler;
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void Bravo() { }
                    public void Yankee() { }
                    {{attribute}}
                    public int Zulu;
                    public int Alpha;
                    public void Zzz() { }
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("constant", "const ", " = 1")]
    [InlineData("readonly", "readonly ", "")]
    public async Task TestNameOrderCanStartFieldGroupAfterMethodAsync(string trait, string modifier, string initializer)
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = $$$"""{"settings":{"orderingRules":{"elementOrder":["{{{trait}}}","name"]} } }""",
            TestCode = $$"""
                public class C
                {
                    public void M() { }
                    public {{modifier}}int Zulu{{initializer}};
                    public {{modifier}}int {|#0:Alpha|}{{initializer}};
                }
                """,
            FixedCode = $$"""
                public class C
                {
                    public void M() { }
                    public {{modifier}}int Alpha{{initializer}};
                    public {{modifier}}int Zulu{{initializer}};
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("""{ "settings": { "orderingRules": { "elementOrder": ["constant", "name"] } } }""", "SA1203", "const ", " = 1")]
    [InlineData("""{ "settings": { "orderingRules": { "elementOrder": ["kind", "readonly", "name"], "memberKindOrder": ["method"] } } }""", "SA1214", "readonly ", "")]
    [InlineData("""{ "settings": { "orderingRules": { "elementOrder": ["kind", "static", "name"], "memberKindOrder": ["method"] } } }""", "SA1204", "static ", "")]
    public async Task TestNameOrderDoesNotExposeEarlierFieldTraitViolationAcrossMethodAsync(string settings, string rule, string modifier, string initializer)
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = settings,
            TestCode = $$"""
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public int Earlier;
                    public void Yankee() { }
                    public void {|#0:Bravo|}() { }
                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "{{rule}}")]
                    public {{modifier}}int Zulu{{initializer}};
                    public {{modifier}}int Alpha{{initializer}};
                }
                """,
            FixedCode = $$"""
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public int Earlier;
                    public void Bravo() { }
                    public void Yankee() { }
                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "{{rule}}")]
                    public {{modifier}}int Zulu{{initializer}};
                    public {{modifier}}int Alpha{{initializer}};
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNameOrderRespectsExplicitStaticInAccessibilityComparisonAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["static", "accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    private static int Dependency;
                    public const int Bravo = 1;
                    public static int Zulu = Dependency;
                    public static int Charlie;
                    public static int Yankee;
                    public static int {|#0:Delta|};
                }
                """,
            FixedCode = """
                public class C
                {
                    private static int Dependency;
                    public const int Bravo = 1;
                    public static int Zulu = Dependency;
                    public static int Charlie;
                    public static int Delta;
                    public static int Yankee;
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Delta' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
