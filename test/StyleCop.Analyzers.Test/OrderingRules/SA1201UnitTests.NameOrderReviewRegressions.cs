// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules;

using System.Threading.Tasks;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;
using AccessOrderVerifier = StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201UnitTests
{
    /// <summary>
    /// Verifies that assembly-level COM visibility applies to a partial declaration without its own public modifier.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionPreservesAssemblyComVisiblePartialInterfaceOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                [assembly: ComVisible(true)]

                public partial interface IApi { }

                partial interface IApi
                {
                    void Zulu();
                    void Alpha();
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that assembly-level COM visibility protects exported interfaces and leaves hidden interfaces sortable.
    /// </summary>
    /// <param name="assemblyAttribute">The attribute that makes public types visible to COM.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Theory]
    [InlineData("[assembly: ComVisible(true)]")]
    [InlineData("[assembly: ClassInterface(ClassInterfaceType.AutoDual)]")]
    public async Task TestReviewRegressionPreservesAssemblyComVisibleInterfaceOrderAsync(string assemblyAttribute)
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = $$"""
                using System.Runtime.InteropServices;

                {{assemblyAttribute}}

                public interface IExported
                {
                    void Zulu();
                    void Alpha();
                }

                [ComVisible(false)]
                public interface IHidden
                {
                    void Zulu();
                    void {|#0:Alpha|}();
                }

                internal interface IInternal
                {
                    void Zulu();
                    void {|#1:Alpha|}();
                }
                """,
            FixedCode = $$"""
                using System.Runtime.InteropServices;

                {{assemblyAttribute}}

                public interface IExported
                {
                    void Zulu();
                    void Alpha();
                }

                [ComVisible(false)]
                public interface IHidden
                {
                    void Alpha();
                    void Zulu();
                }

                internal interface IInternal
                {
                    void Alpha();
                    void Zulu();
                }
                """,
            NumberOfIncrementalIterations = 2,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that an explicit assembly COM opt-out takes precedence over the class-interface setting.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestNameOrderFixHonorsAssemblyComVisibleFalseAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                [assembly: ClassInterface(ClassInterfaceType.AutoDual)]
                [assembly: ComVisible(false)]

                public interface IApi
                {
                    void Zulu();
                    void {|#0:Alpha|}();
                }
                """,
            FixedCode = """
                using System.Runtime.InteropServices;

                [assembly: ClassInterface(ClassInterfaceType.AutoDual)]
                [assembly: ComVisible(false)]

                public interface IApi
                {
                    void Alpha();
                    void Zulu();
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that name ordering uses the effective static or readonly trait of a constant.
    /// </summary>
    /// <param name="trait">The ordering trait that precedes name.</param>
    /// <param name="modifier">The modifier that puts the ordinary field in the same group as the constant.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Theory]
    [InlineData("static", "static")]
    [InlineData("readonly", "readonly")]
    public async Task TestReviewRegressionGroupsConstantsByEffectiveTraitAsync(string trait, string modifier)
    {
        var test = new CSharpTest
        {
            Settings = $$$"""{ "settings": { "orderingRules": { "elementOrder": ["{{{trait}}}", "name"] } } }""",
            TestCode = $$"""
                public class DifferentGroups
                {
                    public const int Zulu = 1;
                    public int Alpha;
                }

                public class SameGroup
                {
                    public const int Zulu = 1;
                    public {{modifier}} int {|#0:Bravo|};
                }
                """,
            FixedCode = $$"""
                public class DifferentGroups
                {
                    public const int Zulu = 1;
                    public int Alpha;
                }

                public class SameGroup
                {
                    public {{modifier}} int Bravo;
                    public const int Zulu = 1;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that Fix All preserves method order where SA1201 is suppressed by a pragma.
    /// </summary>
    /// <param name="warningList">The diagnostic IDs specified in the pragma, if any.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Theory]
    [InlineData(" SA1201")]
    [InlineData("")]
    public async Task TestReviewRegressionFixAllPreservesMethodOrderUnderPragmaAsync(string warningList)
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = $$"""
                public class C
                {
                    public void Tango() { }
                    public void {|#0:Bravo|}() { }
                #pragma warning disable{{warningList}}
                    public void Marker() { }
                    public void Zulu() { }
                    public void Alpha() { }
                #pragma warning restore{{warningList}}
                }
                """,
            FixedCode = $$"""
                public class C
                {
                    public void Bravo() { }
                    public void Tango() { }
                #pragma warning disable{{warningList}}
                    public void Marker() { }
                    public void Zulu() { }
                    public void Alpha() { }
                #pragma warning restore{{warningList}}
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Tango'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that both name-order fixes leave a locally suppressed method in place.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestNameOrderFixPreservesMethodWithSuppressionAttributeAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void Zulu() { }

                    public void {|#0:Bravo|}() { }

                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]
                    public void Alpha() { }
                }
                """,
            FixedCode = """
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                public class C
                {
                    public void Bravo() { }

                    public void Zulu() { }

                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]
                    public void Alpha() { }
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that both name-order fixes honor a targeted global suppression even if the initial order of that member is correct.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestNameOrderFixPreservesMethodWithGlobalSuppressionAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                [assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Scope = "member", Target = "~M:C.Bravo")]

                public class C
                {
                    public void Zulu() { }

                    public void {|#0:Alpha|}() { }

                    public void Bravo() { }
                }
                """,
            FixedCode = """
                #define CODE_ANALYSIS
                using System.Diagnostics.CodeAnalysis;

                [assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Scope = "member", Target = "~M:C.Bravo")]

                public class C
                {
                    public void Alpha() { }

                    public void Zulu() { }

                    public void Bravo() { }
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that an unrelated pragma does not turn an attributed member into a name-order boundary.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionUnrelatedPragmaDoesNotChangeNameFixAllAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Diagnostics.CodeAnalysis;

                #pragma warning disable CS0168
                public class C
                {
                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]
                    public void Zulu() { }

                    public void {|#0:Alpha|}() { }
                }
                """,
            FixedCode = """
                using System.Diagnostics.CodeAnalysis;

                #pragma warning disable CS0168
                public class C
                {
                    public void Alpha() { }

                    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201")]
                    public void Zulu() { }
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that fixing SA1202 preserves sibling name order when SA1201 is absent.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionFixAllForAccessibilityPreservesSiblingNameOrderAsync()
    {
        var test = new AccessOrderVerifier.CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    private void Zulu() { }
                    private void Alpha() { }
                    public void {|#0:Bravo|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void Bravo() { }
                    private void Zulu() { }
                    private void Alpha() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(AccessOrderVerifier.Diagnostic().WithLocation(0).WithArguments("public", "private"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that name-order fixes do not move another tool's generated declarations.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestNameOrderFixPreservesGeneratedMethodAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.CodeDom.Compiler;

                public class C
                {
                    public void Zulu() { }

                    public void {|#0:Bravo|}() { }

                    [GeneratedCode("Tool", "1")]
                    public void Alpha() { }
                }
                """,
            FixedCode = """
                using System.CodeDom.Compiler;

                public class C
                {
                    public void Bravo() { }

                    public void Zulu() { }

                    [GeneratedCode("Tool", "1")]
                    public void Alpha() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that Fix All does not sort members inside a hidden source region.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestNameOrderFixAllPreservesHiddenMemberOrderAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                #line 1
                public class C
                {
                    public void Zulu() { }
                    public void {|#0:Bravo|}() { }
                #line hidden
                    public void Marker() { }
                    public void Zed() { }
                    public void Alpha() { }
                #line default
                }
                """,
            FixedCode = """
                #line 1
                public class C
                {
                    public void Bravo() { }
                    public void Zulu() { }
                #line hidden
                    public void Marker() { }
                    public void Zed() { }
                    public void Alpha() { }
                #line default
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
