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
    /// Verifies that member name ordering preserves the order of nested types containing module initializers.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionMemberNameOrderPreservesNestedModuleInitializerOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.CompilerServices;

                internal static class Initializers
                {
                    internal static class Zulu
                    {
                        [ModuleInitializer]
                        internal static void Initialize() { }
                    }

                    internal static class Alpha
                    {
                        [ModuleInitializer]
                        internal static void Initialize() { }
                    }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the code fix sorts ordinary nested types without crossing a nested module initializer.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionMemberNameOrderFixPreservesNestedModuleInitializerBoundaryAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.CompilerServices;

                internal static class Initializers
                {
                    internal static class Bravo
                    {
                        [ModuleInitializer]
                        internal static void Initialize() { }
                    }

                    internal static class Tango { }

                    internal static class {|#0:Alpha|} { }
                }
                """,
            FixedCode = """
                using System.Runtime.CompilerServices;

                internal static class Initializers
                {
                    internal static class Bravo
                    {
                        [ModuleInitializer]
                        internal static void Initialize() { }
                    }

                    internal static class Alpha { }

                    internal static class Tango { }
                }
                """,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Tango'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that member name ordering recognizes module initializer attributes on partial method implementations.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionMemberNameOrderPreservesPartialModuleInitializerDefinitionsAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.CompilerServices;

                internal static partial class Initializers
                {
                    internal static partial void Zulu();
                    internal static partial void Alpha();
                }

                internal static partial class Initializers
                {
                    [ModuleInitializer]
                    internal static partial void Zulu() { }

                    [ModuleInitializer]
                    internal static partial void Alpha() { }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the code fix sorts ordinary methods without crossing a partial module initializer definition.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestReviewRegressionMemberNameOrderFixPreservesPartialModuleInitializerBoundaryAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.CompilerServices;

                internal static partial class Initializers
                {
                    internal static partial void Bravo();
                    internal static void Tango() { }
                    internal static void {|#0:Alpha|}() { }
                }

                internal static partial class Initializers
                {
                    [ModuleInitializer]
                    internal static partial void Bravo() { }
                }
                """,
            FixedCode = """
                using System.Runtime.CompilerServices;

                internal static partial class Initializers
                {
                    internal static partial void Bravo();
                    internal static void Alpha() { }
                    internal static void Tango() { }
                }

                internal static partial class Initializers
                {
                    [ModuleInitializer]
                    internal static partial void Bravo() { }
                }
                """,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Tango'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
