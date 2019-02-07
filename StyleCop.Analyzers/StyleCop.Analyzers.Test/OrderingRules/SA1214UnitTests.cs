﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public class SA1214UnitTests
    {
        [Fact]
        public async Task TestValidOrderingAsync()
        {
            var testCode = @"public static class TestClass1 { }
public class TestClass2
{
    public const int TestField1 = 1;
    public static readonly int TestField2 = 1;
    public static int TestField3 = 1;
    public readonly int TestField4 = 1;
    public int TestField5 = 1;
    internal const int TestField6 = 1;
    internal static readonly int TestField7 = 1;
    internal static int TestField8 = 1;
    internal readonly int TestField9 = 1;
    internal int TestField10 = 1;
    protected internal const int TestField11 = 1;
    protected internal static readonly int TestField12 = 1;
    protected internal static int TestField13 = 1;
    protected internal readonly int TestField14 = 1;
    protected internal int TestField15 = 1;
    protected const int TestField16 = 1;
    protected static readonly int TestField17 = 1;
    protected static int TestField18 = 1;
    protected readonly int TestField19 = 1;
    protected int TestField20 = 1;
    private const int TestField21 = 1;
    private static readonly int TestField22 = 1;
    private static int TestField23 = 1;
    private readonly int TestField24 = 1;
    private int TestField25 = 1;

    public TestClass2()
    {
    }

    private TestClass2(string a)
    {
    }

    ~TestClass2() { }
    
    public static int TestProperty1 { get; set; }
    public int TestProperty2 { get; set; }
    internal static int TestProperty3 { get; set; }
    internal int TestProperty4 { get; set; }
    protected internal static int TestProperty5 { get; set; }
    protected internal int TestProperty6 { get; set; }
    protected static int TestProperty7 { get; set; }
    protected int TestProperty8 { get; set; }
    private static int TestProperty9 { get; set; }
    private int TestProperty10 { get; set; }
    
    public static void TestMethod1() { }
    public void TestMethod2() { }
    internal static void TestMethod3() { }
    internal void TestMethod4() { }
    protected internal static void TestMethod5() { }
    protected internal void TestMethod6() { }
    protected static void TestMethod7() { }
    protected void TestMethod8() { }
    private static void TestMethod9() { }
    private void TestMethod10() { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(5, 33),
            };

            var fixTestCode = @"
public class Foo
{
    private static readonly int j = 0;
    private static int i = 0;
}";
            await VerifyCSharpFixAsync(testCode, expected, fixTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsAllStaticReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static readonly int i = 0;
    private static readonly int j = 0;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedAfterStaticNonReadonlyOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    private static int i = 0;private static readonly int j = 0;
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(4, 58),
            };

            var fixTestCode = @"
public class Foo
{
    private static readonly int j = 0;
    private static int i = 0;}";
            await VerifyCSharpFixAsync(testCode, expected, fixTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassStaticReadonlyFieldPlacedBeforeStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private static readonly int i = 0;
    private static int j = 0;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInClassNonStaticReadonlyFieldPlacedAfterNonStaticNonReadonlyAsync()
        {
            var testCode = @"
public class Foo
{
    private int i = 0;
    private readonly int j = 0;
}";
            var fixedCode = @"
public class Foo
{
    private readonly int j = 0;
    private int i = 0;
}";

            var expected = Diagnostic().WithLocation(5, 26);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoFieldsInStructStaticReadonlyFieldPlacedAfterStaticNonReadonlyAsync()
        {
            var testCode = @"
public struct Foo
{
    private static int i = 0;
    private static readonly int j = 0;
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(5, 33),
            };

            var fixTestCode = @"
public struct Foo
{
    private static readonly int j = 0;
    private static int i = 0;
}";
            await VerifyCSharpFixAsync(testCode, expected, fixTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task ComplexExampleAsync()
        {
            var testCode = @"
public class Foo
{
    public string s = ""qwe"";
    private static readonly int i = 0;

    public void Ff() {}

    public static string s2 = ""qwe"";

    public static readonly int  u = 5;

    public class FooInner 
    {
        private int aa = 0;
        public static readonly int t = 2;
        private static int z = 999;
        private static readonly int e = 1;
    }

    public static readonly int j = 0;
}";

            var expected = new[]
            {
                Diagnostic().WithLocation(11, 33),
                Diagnostic().WithLocation(18, 37),

                // line 21 should be reported by SA1201
            };

            var fixTestCode = @"
public class Foo
{
    public static readonly int  u = 5;

    public string s = ""qwe"";
    private static readonly int i = 0;

    public void Ff() {}

    public static string s2 = ""qwe"";

    public class FooInner 
    {
        private int aa = 0;
        public static readonly int t = 2;
        private static readonly int e = 1;
        private static int z = 999;
    }

    public static readonly int j = 0;
}";

            await VerifyCSharpFixAsync(testCode, expected, fixTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticReadonlyPrecededByClassAsync()
        {
            var testCode = @"
public class Foo
{
    public static readonly int  u = 5;
    public string s = ""qwe"";
    private static readonly int i = 0;

    public void Ff() {}

    public static string s2 = ""qwe"";

    public class FooInner 
    {
        private int aa = 0;
        public static readonly int t = 2;
        private static readonly int e = 1;
        private static int z = 999;
    }

    public static readonly int j = 0;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFollowedByReadOnlyAtDifferentAccessLevelAsync()
        {
            var testCode = @"class TestClass
{
    public static int TestField1;
    internal static readonly int TestField2;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
