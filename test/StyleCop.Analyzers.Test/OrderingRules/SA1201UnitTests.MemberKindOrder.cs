// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.OrderingRules;
using StyleCop.Analyzers.Settings.ObjectModel;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201UnitTests
{
    [Fact]
    public async Task TestConfiguredGroupFixCanStopAfterInitializerAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int P { get; } = 1;
                    public int {|#0:F|};

                    public int {|#1:Q|} { get; } = 2;
                }
                """,
            FixedCode = """
                public class C
                {
                    public int P { get; } = 1;
                    public int Q { get; } = 2;

                    public int F;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'field' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredStaticOrderAcrossGapAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public void Instance() { }
                    public int P { get; }

                    public static void {|#0:Static|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public static void Static() { }

                    public void Instance() { }
                    public int P { get; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1204StaticElementsMustAppearBeforeInstanceElements().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredReadonlyOrderAcrossGapAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int Mutable;
                    public int P { get; }

                    public readonly int {|#0:Readonly|};
                }
                """,
            FixedCode = """
                public class C
                {
                    public readonly int Readonly;

                    public int Mutable;
                    public int P { get; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("""["accessibility", "kind", "static", "readonly"]""", "private int P { get; }")]
    [InlineData("""["kind", "accessibility", "static", "readonly"]""", "public int P { get; }")]
    public async Task TestConfiguredGapOrderingRespectsHigherTraitsAsync(string traits, string property)
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = $$"""{ "settings": { "orderingRules": { "memberKindOrder": ["property"], "elementOrder": {{traits}} } } }""",
            TestCode = $$"""
                public class C
                {
                    public void Instance() { }
                    public int Mutable;
                    {{property}}
                    private static void Static() { }
                    private readonly int Readonly;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredGapOrderingDoesNotCrossPrecedingTraitBoundaryAsync()
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = """
                { "settings": { "orderingRules": {
                    "memberKindOrder": ["property"],
                    "elementOrder": ["accessibility", "kind", "static", "readonly"]
                } } }
                """,
            TestCode = """
                public class C
                {
                    public void Instance() { }
                    public int Mutable;
                    private int P { get; }
                    public static void Static() { }
                    public readonly int Readonly;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredGapOrderingAcceptsCorrectTraitOrderAsync()
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public static void Static() { }
                    public readonly int Readonly;
                    public int P { get; }
                    public void Instance() { }
                    public int Mutable;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredReadonlyGapRespectsStaticPriorityAsync()
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public static int Mutable;
                    public int P { get; }
                    public readonly int Readonly;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("""["static", "readonly", "kind"]""")]
    [InlineData("""["static", "readonly"]""")]
    public async Task TestConfiguredGapOrderingWithKindBelowTraitsAsync(string traits)
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = $$"""{ "settings": { "orderingRules": { "memberKindOrder": ["property"], "elementOrder": {{traits}} } } }""",
            TestCode = """
                public class C
                {
                    public static int Mutable;
                    public int P { get; }
                    public readonly int Readonly;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredGapOrderingResetsAtIncompleteMembersAsync()
    {
        await new ConfiguredTraitOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int Mutable;
                    public void Instance() { }
                    public string
                    public readonly int Readonly;
                    public static void Static() { }
                }
                """,

            // The incomplete declaration deliberately models code being typed.
            CompilerDiagnostics = CompilerDiagnostics.None,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredStaticGapRespectsReadonlyPriorityForConstantsAsync()
    {
        var test = new ConfiguredTraitOrderingTest
        {
            Settings = """
                { "settings": { "orderingRules": {
                    "memberKindOrder": ["property"],
                    "elementOrder": ["kind", "readonly", "static"]
                } } }
                """,
            TestCode = """
                public class C
                {
                    public readonly int Readonly;
                    public int P { get; }

                    public const int {|#0:Constant|} = 0;
                }
                """,
            FixedCode = """
                public class C
                {
                    public const int Constant = 0;

                    public readonly int Readonly;
                    public int P { get; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1204StaticElementsMustAppearBeforeInstanceElements().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredMemberKindOrderAsync()
    {
        var test = new CSharpTest
        {
            Settings = """
                { "settings": { "orderingRules": { "memberKindOrder": ["enum", "field", "property", "constructor", "method"] } } }
                """,
            TestCode = """
                public class C
                {
                    public enum E { }
                    public int F;
                    public C() { }

                    public int {|#0:P|} { get; set; }
                    public void M() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public enum E { }
                    public int F;
                    public int P { get; set; }

                    public C() { }
                    public void M() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a constructor"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("struct")]
    public async Task TestConfiguredOrderInsideTypesAsync(string type)
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["class", "struct", "interface", "enum", "delegate"] } } }""",
            TestCode = $$"""
                public {{type}} C
                {
                    class Nested { }
                    struct S { }
                    interface I { }
                    enum E { }
                    delegate void D();
                }
                """,
        };

        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderInsideInterfaceAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "property"] } } }""",
            TestCode = """
                public interface C
                {
                    void M();
                    int P { get; }
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredEventGroupIncludesBothDeclarationFormsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["event"] } } }""",
            TestCode = """
                public class C
                {
                    public event System.Action First;
                    public int {|#0:P|} { get; set; }
                    public event System.Action {|#1:Second|} { add { } remove { } }
                }
                """,
            FixedCode = """
                public class C
                {
                    public event System.Action First;
                    public event System.Action Second { add { } remove { } }
                    public int P { get; set; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'property' have no configured order, but must not appear between declarations of kind 'event', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestUnspecifiedKindsBetweenNamedGroupsAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["field", "property", "method"] } } }""",
            TestCode = """
                public class C
                {
                    public class First { }
                    ~C() { }
                    public class Second { }
                    public int F;
                    public delegate void D();
                    public int P { get; set; }
                    public event D E { add { } remove { } }
                    public void M() { }
                    public class Last { }
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestUnspecifiedKindInterruptsNamedGroupAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int First { get; set; }
                    ~{|#0:C|}() { }

                    public int {|#1:Second|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public int First { get; set; }
                    public int Second { get; set; }

                    ~C() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'destructor' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestNamedOrderAcrossUnspecifiedKindsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["field", "property"] } } }""",
            TestCode = """
                public class C
                {
                    public int P { get; set; }
                    ~C() { }

                    public int {|#0:F|};
                }
                """,
            FixedCode = """
                public class C
                {
                    public int F;

                    public int P { get; set; }
                    ~C() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A field", "a property"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredFixAllGroupsMembersAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "method"] } } }""",
            TestCode = """
                public class C
                {
                    public void First() { }
                    public int {|#0:P|} { get; set; }
                    public delegate void D();
                    public void Second() { }

                    // Preserve this comment with the property.
                    public int {|#1:Q|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public int P { get; set; }
                    // Preserve this comment with the property.
                    public int Q { get; set; }

                    public void First() { }
                    public void Second() { }
                    public delegate void D();
                }
                """,
            NumberOfFixAllIterations = 1,
            NumberOfIncrementalIterations = 3,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a method"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithArguments("A property", "a method"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredInterruptionAcrossMultipleUnnamedMembersAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int P { get; set; }
                    public delegate void {|#0:D|}();
                    public class Nested { }

                    public int {|#1:Q|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public int P { get; set; }
                    public int Q { get; set; }

                    public delegate void D();
                    public class Nested { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'delegate' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredFixAllClosesMultipleGapsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = """
                public class C
                {
                    public int P { get; set; }
                    public delegate void {|#0:D|}();
                    public int {|#1:Q|} { get; set; }
                    public void {|#2:M|}() { }
                    public int {|#3:R|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public int P { get; set; }
                    public int Q { get; set; }
                    public int R { get; set; }
                    public delegate void D();
                    public void M() { }
                }
                """,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'delegate' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(2).WithLocation(3).WithMessage(
            "Members of kind 'method' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredCodeFixPreservesDirectivesAsync()
    {
        var code = """
            public class C
            {
                public int P { get; set; }
                #region Methods
                public void {|#0:M|}() { }
                #endregion
                public int {|#1:Q|} { get; set; }
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = code,
            FixedCode = code,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'method' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredKindFixPreservesInitializerExecutionOrderAsync()
    {
        var code = """
            public class C
            {
                public static int F = P;
                public static int {|#0:P|} { get; } = 1;
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "field"] } } }""",
            TestCode = code,
            FixedCode = code,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a field"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredGroupFixPreservesInitializerExecutionOrderAsync()
    {
        var code = """
            public class C
            {
                public static int First { get; } = 1;
                public static int {|#0:F|} = Second;
                public static int {|#1:Second|} { get; } = 2;
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property"] } } }""",
            TestCode = code,
            FixedCode = code,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithLocation(1).WithMessage(
            "Members of kind 'field' have no configured order, but must not appear between declarations of kind 'property', which has a configured order"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("public static int F, G = P;", "a field")]
    [InlineData("public static event System.Action E = () => { };", "an event")]
    public async Task TestConfiguredFixPreservesRuntimeInitializersAsync(string declaration, string precedingKind)
    {
        var code = $$"""
            public class C
            {
                {{declaration}}
                public static int {|#0:P|} { get; } = 1;
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "field", "event"] } } }""",
            TestCode = code,
            FixedCode = code,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", precedingKind));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredFixPreservesInstanceInitializersAsync()
    {
        var code = """
            public class C
            {
                public int F = 1;
                public int {|#0:P|} { get; } = 1;
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "field"] } } }""",
            TestCode = code,
            FixedCode = code,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a field"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("public int F = 1;")]
    [InlineData("public static int F;")]
    [InlineData("public const int F = 1;")]
    public async Task TestConfiguredFixCanCrossIndependentInitializationSequencesAsync(string declaration)
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "field"] } } }""",
            TestCode = $$"""
                public class C
                {
                    {{declaration}}

                    public static int {|#0:P|} { get; } = 1;
                }
                """,
            FixedCode = $$"""
                public class C
                {
                    public static int P { get; } = 1;

                    {{declaration}}
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a field"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderSharedAccessibilityFixAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "property"] } } }""",
            TestCode = """
                public class C
                {
                    private void PrivateMethod() { }

                    public void {|#0:PublicMethod|}() { }
                    public int P { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void PublicMethod() { }

                    private void PrivateMethod() { }
                    public int P { get; set; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1202ElementsMustBeOrderedByAccess().SupportedDiagnostics[0])
            .WithLocation(0).WithArguments("public", "private"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderSharedStaticFixAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "field"] } } }""",
            TestCode = """
                public class C
                {
                    public void M() { }
                    public int Instance;

                    public static int {|#0:Static|};
                }
                """,
            FixedCode = """
                public class C
                {
                    public void M() { }
                    public static int Static;

                    public int Instance;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1204StaticElementsMustAppearBeforeInstanceElements().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderSharedReadonlyFixAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "field"] } } }""",
            TestCode = """
                public class C
                {
                    public void M() { }
                    public int Mutable;

                    public readonly int {|#0:Readonly|};
                }
                """,
            FixedCode = """
                public class C
                {
                    public void M() { }
                    public readonly int Readonly;

                    public int Mutable;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderSharedConstantFixAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["method", "field"] } } }""",
            TestCode = """
                public class C
                {
                    public void M() { }
                    public int Field;

                    public const int {|#0:Constant|} = 0;
                }
                """,
            FixedCode = """
                public class C
                {
                    public void M() { }
                    public const int Constant = 0;

                    public int Field;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1203ConstantsMustAppearBeforeFields().SupportedDiagnostics[0]).WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderFixesHigherPriorityTraitAsync()
    {
        var test = new ConfiguredOrderingTest
        {
            Settings = """
                { "settings": { "orderingRules": {
                    "elementOrder": ["accessibility", "kind"],
                    "memberKindOrder": ["method", "property"]
                } } }
                """,
            TestCode = """
                public class C
                {
                    public void M() { }
                    private int P { get; set; }

                    public int {|#0:Q|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void M() { }
                    public int Q { get; set; }

                    private int P { get; set; }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(new SA1202ElementsMustBeOrderedByAccess().SupportedDiagnostics[0])
            .WithLocation(0).WithArguments("public", "private"));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredKindOrderRespectsHigherPriorityTraitsAsync()
    {
        await new CSharpTest
        {
            Settings = """
                { "settings": { "orderingRules": {
                    "elementOrder": ["accessibility", "kind"],
                    "memberKindOrder": ["property", "method"]
                } } }
                """,
            TestCode = """
                public class C
                {
                    public int First { get; set; }
                    public void M() { }
                    private int Second { get; set; }
                    ~C() { }
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderWithIncompleteMembersAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["property", "method"] } } }""",
            TestCode = """
                public class C
                {
                    public void M() { }
                    public string
                    public string
                    public int P { get; set; }
                }
                """,

            // The incomplete declarations deliberately model code being typed in the editor.
            CompilerDiagnostics = CompilerDiagnostics.None,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestConfiguredOrderDoesNotChangeOuterOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "memberKindOrder": ["class", "struct"] } } }""",
            TestCode = """
                namespace N
                {
                    public struct S { }
                    public class C { }
                }
                public struct S { }
                public class C { }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("""{ "memberKindOrder": [] }""")]
    [InlineData("""{ "memberKindOrder": ["method", "property"], "elementOrder": ["accessibility"] }""")]
    public async Task TestEmptyOrDisabledConfiguredKindOrderAsync(string orderingRules)
    {
        await new CSharpTest
        {
            Settings = $$"""{ "settings": { "orderingRules": {{orderingRules}} } }""",
            TestCode = """
                public class C
                {
                    public void M() { }
                    public int P { get; set; }
                    ~C() { }
                    public int Q { get; set; }
                    public int F;
                }
                """,
        }.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Theory]
    [InlineData("field", SyntaxKind.FieldDeclaration)]
    [InlineData("constructor", SyntaxKind.ConstructorDeclaration)]
    [InlineData("destructor", SyntaxKind.DestructorDeclaration)]
    [InlineData("delegate", SyntaxKind.DelegateDeclaration)]
    [InlineData("event", SyntaxKind.EventDeclaration)]
    [InlineData("enum", SyntaxKind.EnumDeclaration)]
    [InlineData("interface", SyntaxKind.InterfaceDeclaration)]
    [InlineData("property", SyntaxKind.PropertyDeclaration)]
    [InlineData("indexer", SyntaxKind.IndexerDeclaration)]
    [InlineData("conversionOperator", SyntaxKind.ConversionOperatorDeclaration)]
    [InlineData("operator", SyntaxKind.OperatorDeclaration)]
    [InlineData("method", SyntaxKind.MethodDeclaration)]
    [InlineData("struct", SyntaxKind.StructDeclaration)]
    [InlineData("class", SyntaxKind.ClassDeclaration)]
    public void TestMemberKindConfigurationNames(string name, SyntaxKind kind)
    {
        var order = new MemberKindOrder(ImmutableArray.Create(name), ImmutableArray.Create(OrderingTrait.Kind));
        Assert.Equal(name, MemberKindOrder.GetKindName(kind));
        Assert.NotNull(order);
    }

    [Theory]
    [InlineData("unknown", "property")]
    [InlineData("record", "property")]
    [InlineData("property", "PROPERTY")]
    public void TestInvalidMemberKindOrder(string first, string second)
    {
        Assert.Throws<InvalidSettingsException>(() => new MemberKindOrder(
            ImmutableArray.Create(first, second),
            ImmutableArray.Create(OrderingTrait.Kind)));
    }

    protected static DiagnosticResult Diagnostic()
    {
        return new DiagnosticResult(new StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder().SupportedDiagnostics[0]);
    }

    private sealed class ConfiguredTraitOrderingTest : CSharpTest
    {
        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            return new DiagnosticAnalyzer[]
            {
                new SA1204StaticElementsMustAppearBeforeInstanceElements(),
                new SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements(),
            };
        }
    }

    private sealed class ConfiguredOrderingTest : CSharpTest
    {
        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        {
            return new DiagnosticAnalyzer[]
            {
                new SA1201ElementsMustAppearInTheCorrectOrder(),
                new SA1202ElementsMustBeOrderedByAccess(),
                new SA1203ConstantsMustAppearBeforeFields(),
                new SA1204StaticElementsMustAppearBeforeInstanceElements(),
                new SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements(),
            };
        }
    }
}
