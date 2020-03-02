// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1204StaticElementsMustAppearBeforeInstanceElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1204StaticElementsMustAppearBeforeInstanceElements"/>.
    /// </summary>
    public class SA1204UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
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

    public static class TestClass1 { }
    public class TestClass2a { }
    internal static class TestClass3 { }
    internal class TestClass4 { }
    protected internal static class TestClass5 { }
    protected internal class TestClass6 { }
    protected static class TestClass7 { }
    protected class TestClass8 { }
    private static class TestClass9 { }
    private class TestClass10 { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static classes before static.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNonStaticClassBeforeStaticAsync()
        {
            var testCode = @"public class TestClass1 { }
public static class TestClass2 { }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(2, 21),
            };

            var fixedCode = @"public static class TestClass2 { }
public class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static elements before static in a class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOrderingInClassAsync()
        {
            var testCode = @"public class TestClass
{
    public int TestField1;
    public static int TestField2;
    public int TestProperty1 { get; set; }
    public static int TestProperty2 { get; set; }
    public void TestMethod1() { }
    public static void TestMethod2() { }
    
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 23),
                Diagnostic().WithLocation(6, 23),
                Diagnostic().WithLocation(8, 24),
            };

            var fixedCode = @"public class TestClass
{
    public static int TestField2;
    public int TestField1;
    public static int TestProperty2 { get; set; }
    public int TestProperty1 { get; set; }
    public static void TestMethod2() { }
    public void TestMethod1() { }
    
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static elements before static in a struct.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOrderingInStructAsync()
        {
            var testCode = @"public struct TestStruct
{
    public int TestField1;
    public static int TestField2;
    public int TestProperty1 { get; set; }
    public static int TestProperty2 { get; set; }
    public void TestMethod1() { }
    public static void TestMethod2() { }
    
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 23),
                Diagnostic().WithLocation(6, 23),
                Diagnostic().WithLocation(8, 24),
            };

            var fixedCode = @"public struct TestStruct
{
    public static int TestField2;
    public int TestField1;
    public static int TestProperty2 { get; set; }
    public int TestProperty1 { get; set; }
    public static void TestMethod2() { }
    public void TestMethod1() { }
    
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle events.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventOrderingAsync()
        {
            var testCode = @"public class TestClass
{
    public event System.Action TestEvent;
    public event System.Action TestEvent2 { add { } remove { } }
    public static event System.Action TestEvent3;
    public static event System.Action TestEvent4 { add { } remove { } }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 5),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle instance members before const.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstNotReportedBeforeInstanceMembersAsync()
        {
            var testCode = @"public class TestClass {
    public int TestField1;
    public const int TestField2 = 1;
}
";

            // should be reported by SA1203
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle nested class ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNestedClassesAsync()
        {
            var testCode = @"public class TestClass
{
    public class TestClass1 { }
    internal class TestClass2 { }
    internal static class TestClass3 { }
}
";

            var expected = Diagnostic().WithLocation(5, 27);

            var fixedCode = @"public class TestClass
{
    public class TestClass1 { }
    internal static class TestClass3 { }
    internal class TestClass2 { }
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle class ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassesAsync()
        {
            var testCode = @"public class TestClass1 { }
public static class TestClass2 { }
internal class TestClass3 { }
internal static class TestClass4 { }
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(2, 21),
                Diagnostic().WithLocation(4, 23),
            };

            var fixedCode = @"public static class TestClass2 { }
public class TestClass1 { }
internal static class TestClass4 { }
internal class TestClass3 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle unqualified members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMembersWithoutAccessModifiersAsync()
        {
            var testCode = @"class TestClass
{
    string TestField1;
    static string TestField2;
    string TestField3;
}
";

            var expected = Diagnostic().WithLocation(4, 19);

            var fixedCode = @"class TestClass
{
    static string TestField2;
    string TestField1;
    string TestField3;
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle unqualified classes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassesWithoutAccessModifiersAsync()
        {
            var testCode = @"
class TestClass1 { }
static class TestClass2 { }
";

            var expected = Diagnostic().WithLocation(3, 14);

            var fixedCode = @"static class TestClass2 { }

class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle wrongly ordered classes with a file header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWronglyOrderedClassesWithFileHeaderAsync()
        {
            var testCode = @"// Test header

public class TestClass1 { }
public static class TestClass2 { }
";

            var expected = Diagnostic().WithLocation(4, 21);

            var fixedCode = @"// Test header

public static class TestClass2 { }
public class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle wrongly ordered classes with a file header and XMN comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWronglyOrderedClassesWithFileHeaderAndXmlCommentsAsync()
        {
            var testCode = @"// Test header

/// <summary>
/// Test comment 1
/// </summary>
public class TestClass1 { }

/// <summary>
/// Test comment 2
/// </summary>
public static class TestClass2 { }
";

            var expected = Diagnostic().WithLocation(11, 21);

            var fixedCode = @"// Test header

/// <summary>
/// Test comment 2
/// </summary>
public static class TestClass2 { }

/// <summary>
/// Test comment 1
/// </summary>
public class TestClass1 { }
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle ordering within a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceClassesAsync()
        {
            var testCode = @"namespace TestNamespace
{
    class TestClass1 { }
    static class TestClass2 { }
}
";

            var expected = Diagnostic().WithLocation(4, 18);

            var fixedCode = @"namespace TestNamespace
{
    static class TestClass2 { }
    class TestClass1 { }
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle static constructors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStaticConstructorsAsync()
        {
            var testCode = @"
class MyClass1
{
    public MyClass1()
    {
    }

    static MyClass1()
    {
    }
}

class MyClass2
{
    static MyClass2()
    {
    }

    public MyClass2()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(8, 12);

            var fixedCode = @"
class MyClass1
{
    static MyClass1()
    {
    }

    public MyClass1()
    {
    }
}

class MyClass2
{
    static MyClass2()
    {
    }

    public MyClass2()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceFollowedByPrivateStaticAsync()
        {
            var testCode = @"
public interface TestInterface
{
    void SomeMethod();
}

public class TestClass : TestInterface
{
    void TestInterface.SomeMethod()
    {
    }

    private static void ExampleMethod()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly incomplete members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncompleteMemberAsync()
        {
            string testCode = @"public class TestClass
{
    public string Test;
    public static string
    public static string
}
";

            // We don't care about the syntax errors.
            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1585").WithMessage("Member modifier 'public' must precede the member type and name").WithLocation(5, 5),
                DiagnosticResult.CompilerError("CS1519").WithMessage("Invalid token '}' in class, struct, or interface member declaration").WithLocation(6, 1),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
