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
    public async Task TestMemberNameOrderFixesPropertiesConstantsAndFieldsAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "constant", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public const int ZuluConstant = 1;
                    public const int {|#0:AlphaConstant|} = 2;
                    public int ZuluField;
                    public int {|#1:AlphaField|};
                    public int ZuluProperty { get; set; }
                    public int {|#2:AlphaProperty|} { get; set; }
                }
                """,
            FixedCode = """
                public class C
                {
                    public const int AlphaConstant = 2;
                    public const int ZuluConstant = 1;
                    public int AlphaField;
                    public int ZuluField;
                    public int AlphaProperty { get; set; }
                    public int ZuluProperty { get; set; }
                }
                """,
            NumberOfIncrementalIterations = 3,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'AlphaConstant' should appear before member 'ZuluConstant'"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithMessage("Member 'AlphaField' should appear before member 'ZuluField'"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(2).WithMessage("Member 'AlphaProperty' should appear before member 'ZuluProperty'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderMovesInitializedFieldAcrossUninitializedFieldAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public class C
                {
                    public static int Zulu;
                    public static int {|#0:Alpha|} = GetValue();

                    private static int GetValue() => 1;
                }
                """,
            FixedCode = """
                public class C
                {
                    public static int Alpha = GetValue();
                    public static int Zulu;

                    private static int GetValue() => 1;
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Alpha' should appear before member 'Zulu'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderPreservesRuntimeInitializerOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public class C
                {
                    public static int Zulu = GetValue();
                    public static int Alpha = Zulu + 1;

                    private static int GetValue() => 1;
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderFixPreservesRuntimeInitializerOrderAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                public class C
                {
                    public static int Zulu = GetValue();
                    public static int {|#0:Tango|};
                    public static int {|#1:Alpha|} = GetValue();

                    private static int GetValue() => 1;
                }
                """,
            FixedCode = """
                public class C
                {
                    public static int Tango;
                    public static int Zulu = GetValue();
                    public static int Alpha = GetValue();

                    private static int GetValue() => 1;
                }
                """,
            NumberOfIncrementalIterations = 1,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Tango' should appear before member 'Zulu'"));
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithMessage("Member 'Alpha' should appear before member 'Tango'"));
        await test.RunAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMemberNameOrderPreservesInstanceStorageLayoutAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                public struct S
                {
                    public int ZuluField;
                    public int AlphaField;

                    public int ZuluProperty { get; set; }

                    public int AlphaProperty { get; set; }
                }

                [StructLayout(LayoutKind.Sequential)]
                public class C
                {
                    public int ZuluField;
                    public int AlphaField;

                    public int ZuluProperty { get; set; }

                    public int AlphaProperty { get; set; }
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that Fix All sorts methods that become consecutive after a kind-order fix.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestFixAllOrdersMethodsAfterGroupingByKindAsync()
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["kind", "accessibility", "name"] } } }""",
            TestCode = """
                public class C
                {
                    public void Zulu() { }
                    public int {|#0:Property|} { get; set; }
                    public void Alpha() { }
                }
                """,
            FixedCode = """
                public class C
                {
                    public int Property { get; set; }
                    public void Alpha() { }
                    public void Zulu() { }
                }
                """,
            NumberOfIncrementalIterations = 2,
            NumberOfFixAllIterations = 1,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a method"));
        await test.RunAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that member name ordering preserves COM interface method order.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Fact]
    public async Task TestMemberNameOrderPreservesComInterfaceMethodOrderAsync()
    {
        await new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = """
                using System.Runtime.InteropServices;

                [ComImport]
                [Guid("00000000-0000-0000-0000-000000000001")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                public interface IImported
                {
                    void Zulu();
                    void Alpha();
                }
                """,
        }.RunAsync().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("decimal")]
    [InlineData("DecimalAlias")]
    public async Task TestMemberNameOrderPreservesDecimalConstantInitializationAsync(string constantType)
    {
        var test = new CSharpTest
        {
            Settings = """{ "settings": { "orderingRules": { "elementOrder": ["name"] } } }""",
            TestCode = $$"""
                using DecimalAlias = System.Decimal;

                public class C
                {
                    public const {{constantType}} Zulu = 3.5m;
                    public static decimal Alpha = (decimal)typeof(C).GetField("Zulu").GetValue(null);
                    public static decimal {|#0:Aardvark|};
                }
                """,
            FixedCode = $$"""
                using DecimalAlias = System.Decimal;

                public class C
                {
                    public static decimal Aardvark;
                    public const {{constantType}} Zulu = 3.5m;
                    public static decimal Alpha = (decimal)typeof(C).GetField("Zulu").GetValue(null);
                }
                """,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithMessage("Member 'Aardvark' should appear before member 'Alpha'"));
        await test.RunAsync().ConfigureAwait(false);
    }
}
