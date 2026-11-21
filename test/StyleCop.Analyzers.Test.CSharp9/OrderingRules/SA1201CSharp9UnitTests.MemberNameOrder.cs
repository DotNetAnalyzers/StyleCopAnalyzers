// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules;

using System.Threading.Tasks;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
    StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

public partial class SA1201CSharp9UnitTests
{
    /// <summary>
    /// Verifies that member name ordering preserves module initializer order.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestMemberNameOrderPreservesModuleInitializerOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.CompilerServices;

                internal static class Initializers
                {
                    [ModuleInitializer]
                    internal static void Zulu() { }

                    [ModuleInitializer]
                    internal static void Alpha() { }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that member name ordering preserves the order of instance properties in a record.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestMemberNameOrderPreservesRecordPropertyOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public record C
                {
                    public int Zulu { get; set; }
                    public int Alpha { get; set; }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that sorting ordinary methods cannot move them across an aliased module initializer.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestMemberNameOrderFixKeepsModuleInitializersAsBoundariesAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using Startup = System.Runtime.CompilerServices.ModuleInitializerAttribute;

                internal static class Initializers
                {
                    [Startup]
                    internal static void Zulu() { }
                    [Startup]
                    internal static void Yankee() { }
                    internal static void Tango() { }
                    internal static void {|#0:Alpha|}() { }
                }
                """,
            FixedCode = """
                using Startup = System.Runtime.CompilerServices.ModuleInitializerAttribute;

                internal static class Initializers
                {
                    [Startup]
                    internal static void Zulu() { }
                    [Startup]
                    internal static void Yankee() { }
                    internal static void Alpha() { }
                    internal static void Tango() { }
                }
                """,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Tango'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
