// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1202ElementsMustBeOrderedByAccess"/>.
    /// </summary>
    public class SA1202UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid access level ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOrderAsync()
        {
            var testCode = @"public class TestClass
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

    public TestClass()
    {
    }

    private TestClass(string a)
    {
    }

    ~TestClass() { }
    
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
    public class TestClass2 { }
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle class access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassOrderingAsync()
        {
            var testCode = @"internal class TestClass1 { }
public class TestClass2 { }
";

            var expected = this.CSharpDiagnostic().WithLocation(2, 14).WithArguments("public", "classes", "internal");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass2 { }
internal class TestClass1 { }
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle interfaces before classes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInternalInterfaceBeforePublicClassAsync()
        {
            var testCode = @"internal interface ITestInterface { }
public class TestClass2 { }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle property access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertiesAsync()
        {
            var testCode = @"public class TestClass
{
    private string TestProperty1 { get; set; }
    protected string TestProperty2 { get; set; }
    protected internal string TestProperty3 { get; set; }
    internal string TestProperty4 { get; set; }
    public string TestProperty5 { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 22).WithArguments("protected", "properties", "private"),
                this.CSharpDiagnostic().WithLocation(5, 31).WithArguments("protected internal", "properties", "protected"),
                this.CSharpDiagnostic().WithLocation(6, 21).WithArguments("internal", "properties", "protected internal"),
                this.CSharpDiagnostic().WithLocation(7, 19).WithArguments("public", "properties", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public string TestProperty5 { get; set; }
    internal string TestProperty4 { get; set; }
    protected internal string TestProperty3 { get; set; }
    protected string TestProperty2 { get; set; }
    private string TestProperty1 { get; set; }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle method access levels.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodsAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    protected void TestMethod2() { }
    protected internal void TestMethod3() { }
    internal void TestMethod4() { }
    public void TestMethod5() { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 20).WithArguments("protected", "methods", "private"),
                this.CSharpDiagnostic().WithLocation(5, 29).WithArguments("protected internal", "methods", "protected"),
                this.CSharpDiagnostic().WithLocation(6, 19).WithArguments("internal", "methods", "protected internal"),
                this.CSharpDiagnostic().WithLocation(7, 17).WithArguments("public", "methods", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public void TestMethod5() { }
    internal void TestMethod4() { }
    protected internal void TestMethod3() { }
    protected void TestMethod2() { }
    private void TestMethod1() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected internal before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedInternalBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    protected internal event System.Action TestEvent1;
    public event System.Action TestEvent2;
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("public", "events", "protected internal");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public event System.Action TestEvent2;
    protected internal event System.Action TestEvent1;
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    protected string TestField1;
    public string TestField2;
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("public", "fields", "protected");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public string TestField2;
    protected string TestField1;
}
";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before public.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforePublicAsync()
        {
            var testCode = @"public class TestClass
{
    private event System.Action TestEvent1 { add { } remove { } }
    public event System.Action TestEvent2 { add { } remove { } }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 32).WithArguments("public", "events", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public event System.Action TestEvent2 { add { } remove { } }
    private event System.Action TestEvent1 { add { } remove { } }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle protected before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProtectedBeforeInternalAsync()
        {
            var testCode = @"public class TestClass
{
    protected event System.Action TestEvent1 { add { } remove { } }
    internal event System.Action TestEvent2 { add { } remove { } }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 34).WithArguments("internal", "events", "protected");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    internal event System.Action TestEvent2 { add { } remove { } }
    protected event System.Action TestEvent1 { add { } remove { } }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforeInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private delegate void TestDelegate1();
    internal delegate void TestDelegate2();
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 28).WithArguments("internal", "delegates", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    internal delegate void TestDelegate2();
    private delegate void TestDelegate1();
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle private before internal.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPrivateBeforeProtectedInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    protected internal void TestMethod2() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 29).WithArguments("protected internal", "methods", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    protected internal void TestMethod2() { }
    private void TestMethod1() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle internal keyword followed by protected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInternalProtectedCountsAsProtectedInternalAsync()
        {
            var testCode = @"public class TestClass
{
    private void TestMethod1() { }
    internal protected void TestMethod2() { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 29).WithArguments("protected internal", "methods", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    internal protected void TestMethod2() { }
    private void TestMethod1() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle unqualified members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMembersWithoutAccessModifiersAsync()
        {
            var testCode = @"public class TestClass
{
    string TestField1;
    public string TestField2;
    string TestField3;
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("public", "fields", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public string TestField2;
    string TestField1;
    string TestField3;
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle unqualified classes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassesWithoutAccessModifiersAsync()
        {
            var testCode = @"class TestClass1 { }
public class TestClass2 { }
";

            var expected = this.CSharpDiagnostic().WithLocation(2, 14).WithArguments("public", "classes", "internal");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass2 { }
class TestClass1 { }
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle multiple violations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOnlyFirstViolationReportedAsync()
        {
            var testCode = @"public class TestClass
{
    private string TestField1;
    public string TestField2;
    public string TestField3;
}
";

            var expected = this.CSharpDiagnostic().WithLocation(4, 19).WithArguments("public", "fields", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public string TestField2;
    public string TestField3;
    private string TestField1;
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle static and instance members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStaticNotComparedToInstanceMembersAsync()
        {
            var testCode = @"public class TestClass
{
    private static void A()
    {
    }

    public void B()
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(7, 17).WithArguments("public", "methods", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public void B()
    {
    }

    private static void A()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle static ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStaticElementOrderingAsync()
        {
            var testCode = @"public class TestClass
{
    private static void TestMethod1() { }
    protected static void TestMethod2() { }
    protected internal static void TestMethod3() { }
    internal static void TestMethod4() { }
    public static void TestMethod5() { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 27).WithArguments("protected", "methods", "private"),
                this.CSharpDiagnostic().WithLocation(5, 36).WithArguments("protected internal", "methods", "protected"),
                this.CSharpDiagnostic().WithLocation(6, 26).WithArguments("internal", "methods", "protected internal"),
                this.CSharpDiagnostic().WithLocation(7, 24).WithArguments("public", "methods", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public static void TestMethod5() { }
    internal static void TestMethod4() { }
    protected internal static void TestMethod3() { }
    protected static void TestMethod2() { }
    private static void TestMethod1() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle const ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstOrderingAsync()
        {
            var testCode = @"public class TestClass
{
    private const int TestConst1 = 1;
    protected const int TestConst2 = 2;
    public int TestField;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 25).WithArguments("protected", "fields", "private"),
                this.CSharpDiagnostic().WithLocation(5, 16).WithArguments("public", "fields", "protected")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public int TestField;
    protected const int TestConst2 = 2;
    private const int TestConst1 = 1;
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
    private class TestClass2 { }
    internal class TestClass3 { }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(5, 20).WithArguments("internal", "classes", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class TestClass
{
    public class TestClass1 { }
    internal class TestClass3 { }
    private class TestClass2 { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
    enum TestEnum1 { }
    public enum TestEnum2 { }
    class TestClass1 { }
    public class TestClass2 { }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 17).WithArguments("public", "enums", "internal"),
                this.CSharpDiagnostic().WithLocation(6, 18).WithArguments("public", "classes", "internal")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"namespace TestNamespace
{
    public enum TestEnum2 { }
    enum TestEnum1 { }
    public class TestClass2 { }
    class TestClass1 { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle static constructors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStaticConstructorsAsync()
        {
            var testCode = @"
class MyClass
{
    static MyClass()
    {
    }

    public MyClass()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle constructors.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConstructorsAsync()
        {
            var testCode = @"
class MyClass
{
    private MyClass()
    {
    }

    public MyClass(int a)
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(8, 12).WithArguments("public", "constructors", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class MyClass
{
    public MyClass(int a)
    {
    }

    private MyClass()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle explicit interface implementations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestExplicitInterfaceImplementationsAsync()
        {
            var testCode = @"using System;
internal interface IA
{
    event EventHandler TestEvent1;

    string T { get; }

    int U { get; }

    string this[string a] { get; }

    void TestMethod1();
}

class Test : IA
{
    private event EventHandler TestEvent2;

    event EventHandler IA.TestEvent1 { add { } remove { } }

    public int Q { get; set; }

    string IA.T
    {
        get { return null; }
    }

    // SA1202 (All public properties must come before all private properties) wrongly reported here.
    public int W { get; set; }

    protected string S { get; set; }

    // SA1202 should be reported here (according to legacy StyleCop), but is not.
    int IA.U
    {
        get { return 0; }
    }

    protected int this[int index] { get { return index; } }

    string IA.this[string a] { get { return a; } }

    protected void TestMethod2() { }

    void IA.TestMethod1() { }
}
";

            DiagnosticResult[] expected =
            {
                // explicit interface events are considered private by StyleCop
                this.CSharpDiagnostic().WithLocation(34, 12).WithArguments("public", "properties", "protected"),
                this.CSharpDiagnostic().WithLocation(41, 15).WithArguments("public", "indexers", "protected"),
                this.CSharpDiagnostic().WithLocation(45, 13).WithArguments("public", "methods", "protected")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"using System;
internal interface IA
{
    event EventHandler TestEvent1;

    string T { get; }

    int U { get; }

    string this[string a] { get; }

    void TestMethod1();
}

class Test : IA
{
    private event EventHandler TestEvent2;

    event EventHandler IA.TestEvent1 { add { } remove { } }

    public int Q { get; set; }

    string IA.T
    {
        get { return null; }
    }

    // SA1202 (All public properties must come before all private properties) wrongly reported here.
    public int W { get; set; }

    // SA1202 should be reported here (according to legacy StyleCop), but is not.
    int IA.U
    {
        get { return 0; }
    }

    protected string S { get; set; }

    string IA.this[string a] { get { return a; } }

    protected int this[int index] { get { return index; } }

    void IA.TestMethod1() { }

    protected void TestMethod2() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
    private static void ExampleMethod()
    {
    }

    void TestInterface.SomeMethod()
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(13, 24).WithArguments("public", "methods", "private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
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

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle incomplete members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncompleteMemberAsync()
        {
            string testCode = @"public class OuterType
{
    private string Test;
    public string
    public string
}
";

            // We don't care about the syntax errors.
            var expected = new[]
            {
                 new DiagnosticResult
                 {
                     Id = "CS1585",
                     Message = "Member modifier 'public' must precede the member type and name",
                     Severity = DiagnosticSeverity.Error,
                     Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 5) }
                 },
                 new DiagnosticResult
                 {
                     Id = "CS1519",
                     Message = "Invalid token '}' in class, struct, or interface member declaration",
                     Severity = DiagnosticSeverity.Error,
                     Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
                 }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1202ElementsMustBeOrderedByAccess();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ElementOrderCodeFixProvider();
        }
    }
}
