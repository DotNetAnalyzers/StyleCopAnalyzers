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
    public async Task TestMemberNameOrderFixesFieldLikeAndAccessorEventsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public event System.Action Zulu;
                    public event System.Action {|#0:Alpha|};
                    internal event System.Action Yankee { add { } remove { } }
                    internal event System.Action {|#1:Bravo|} { add { } remove { } }
                }
                """,
            FixedCode = """
                public class C
                {
                    public event System.Action Alpha;
                    public event System.Action Zulu;
                    internal event System.Action Bravo { add { } remove { } }
                    internal event System.Action Yankee { add { } remove { } }
                }
                """,
            NumberOfIncrementalIterations = 2,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderPreservesFieldLikeEventsWithSequentialLayoutAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                [StructLayout(LayoutKind.Sequential)]
                public class C
                {
                    public event System.Action Zulu;
                    public event System.Action Alpha;
                    public event System.Action Yankee { add { } remove { } }
                    public event System.Action {|#0:Bravo|} { add { } remove { } }
                }
                """,
            FixedCode = """
                using System.Runtime.InteropServices;

                [StructLayout(LayoutKind.Sequential)]
                public class C
                {
                    public event System.Action Zulu;
                    public event System.Action Alpha;
                    public event System.Action Bravo { add { } remove { } }
                    public event System.Action Yankee { add { } remove { } }
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderFixesInstanceFieldsWithAutoLayoutAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                [StructLayout(LayoutKind.Auto)]
                public class C
                {
                    public int Zulu;
                    public int {|#0:Alpha|};
                }
                """,
            FixedCode = """
                using System.Runtime.InteropServices;

                [StructLayout(LayoutKind.Auto)]
                public class C
                {
                    public int Alpha;
                    public int Zulu;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderPreservesSequentialLayoutFromAnotherPartialDeclarationAsync()
    {
        const string layoutDeclaration = """
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Sequential)]
            public partial class C
            {
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public partial class C
                {
                    public int Zulu;
                    public int Alpha;
                    public void Yankee() { }
                    public void {|#0:Bravo|}() { }
                }
                """,
            FixedCode = """
                public partial class C
                {
                    public int Zulu;
                    public int Alpha;
                    public void Bravo() { }
                    public void Yankee() { }
                }
                """,
        };
        test.TestState.Sources.Add(("Layout.cs", layoutDeclaration));
        test.FixedState.Sources.Add(("Layout.cs", layoutDeclaration));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderPreservesExplicitLayoutFromAnotherPartialDeclarationAsync()
    {
        const string layoutDeclaration = """
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Explicit)]
            public partial class C
            {
            }
            """;
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                public partial class C
                {
                    [FieldOffset(0)]
                    public int Zulu;

                    [FieldOffset(4)]
                    public int Alpha;
                    public void Yankee() { }
                    public void {|#0:Bravo|}() { }
                }
                """,
            FixedCode = """
                using System.Runtime.InteropServices;

                public partial class C
                {
                    [FieldOffset(0)]
                    public int Zulu;

                    [FieldOffset(4)]
                    public int Alpha;
                    public void Bravo() { }
                    public void Yankee() { }
                }
                """,
        };
        test.TestState.Sources.Add(("Layout.cs", layoutDeclaration));
        test.FixedState.Sources.Add(("Layout.cs", layoutDeclaration));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Bravo' should appear before member 'Yankee'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
