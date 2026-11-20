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
    [Fact]
    public async Task TestMethodNameOrderFromJsonPreservesOverloadsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "static", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public void Bravo() { }
                    public void {|#0:Alpha|}(int value) { }
                    public void Alpha(string value) { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void Alpha(int value) { }
                    public void Alpha(string value) { }
                    public void Bravo() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Bravo'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMethodNameOrderUsesOrdinalComparisonAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public class C
                {
                    public void alpha() { }
                    public void {|#0:Zulu|}() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public void Zulu() { }
                    public void alpha() { }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Zulu' should appear before member 'alpha'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("""{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "static"] } } }""")]
    public async Task TestMethodNameOrderIsOptInAsync(string? settings)
    {
        await new CSharpTest
        {
            Settings = settings,
            TestCode = """
                public class C
                {
                    public void Zulu() { }
                    public void Alpha() { }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMethodNameOrderRespectsConfiguredTraitBoundariesAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "static", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public static void Zulu() { }
                    public void Bravo() { }
                    private void Alpha() { }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMethodNameOrderTreatsMethodsContainingDirectivesAsBarriersAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public class C
                {
                    public void Zulu() { }
                    public void Middle()
                    {
                #if DEBUG
                        System.Console.WriteLine();
                #endif
                    }
                    public void Alpha() { }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }
}
