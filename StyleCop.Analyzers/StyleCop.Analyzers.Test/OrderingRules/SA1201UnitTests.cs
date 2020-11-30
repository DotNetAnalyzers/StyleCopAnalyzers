// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public class SA1201UnitTests
    {
        [Fact]
        public async Task TestOuterOrderCorrectOrderAsync()
        {
            string testCode = @"namespace Foo { }
public delegate void bar();
public enum TestEnum { }
public interface IFoo { }
public struct FooStruct { }
public class FooClass { }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOuterOrderWrongOrderAsync()
        {
            string testCode = @"
namespace Foo { }
public enum TestEnum { }
public delegate void bar();
public interface IFoo { }
public class FooClass { }
public struct FooStruct { }
";
            var expected = new[]
            {
                Diagnostic().WithLocation(4, 22).WithArguments("delegate", "enum"),
                Diagnostic().WithLocation(7, 15).WithArguments("struct", "class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderClassAsync()
        {
            string testCode = @"public class OuterType
{
    public string TestField;
    public OuterType() { TestField = ""foo""; TestProperty = """"; }
    ~OuterType() { }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    public string TestProperty { get; set; }
    public string this[string arg] { get { return ""foo""; } set { } }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderStructAsync()
        {
            string testCode = @"public struct OuterType
{
    public string TestField;
    public OuterType(int argument) { TestField = ""foo""; TestProperty = """"; }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    public string TestProperty { get; set; }
    public string this[string arg] { get { return ""foo""; } set { } }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderInterfaceAsync()
        {
            string testCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    string this[string arg] { get; set; }
    void TestMethod ();
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderClassAsync()
        {
            string testCode = @"public class OuterType
{
    public string TestField;
    ~OuterType() { }
    public OuterType() { }
    public interface ITest { }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public string TestProperty { get; set; }
    public struct TestStruct { }
    public void TestMethod () { }
    public class TestClass { }
    public string this[string arg] { get { return ""foo""; } set { } }
}
";
            var expected = new[]
            {
                Diagnostic().WithLocation(5, 12).WithArguments("constructor", "destructor"),
                Diagnostic().WithLocation(7, 26).WithArguments("delegate", "interface"),
                Diagnostic().WithLocation(11, 5).WithArguments("conversion", "operator"),
                Diagnostic().WithLocation(12, 19).WithArguments("property", "conversion"),
                Diagnostic().WithLocation(14, 17).WithArguments("method", "struct"),
                Diagnostic().WithLocation(16, 19).WithArguments("indexer", "class"),
            };

            string fixedCode = @"public class OuterType
{
    public string TestField;
    public OuterType() { }
    ~OuterType() { }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    public string TestProperty { get; set; }
    public string this[string arg] { get { return ""foo""; } set { } }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 8,
                NumberOfFixAllIterations = 3,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderStructAsync()
        {
            string testCode = @"public struct OuterType
{
    public string TestField;
    public OuterType(int argument) { TestField = ""foo""; TestProperty = ""bar""; }
    public interface ITest { }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public string TestProperty { get; set; }
    public struct TestStruct { }
    public void TestMethod () { }
    public class TestClass { }
    public string this[string arg] { get { return ""foo""; } set { } }
}
";
            var expected = new[]
            {
                Diagnostic().WithLocation(6, 26).WithArguments("delegate", "interface"),
                Diagnostic().WithLocation(10, 5).WithArguments("conversion", "operator"),
                Diagnostic().WithLocation(11, 19).WithArguments("property", "conversion"),
                Diagnostic().WithLocation(13, 17).WithArguments("method", "struct"),
                Diagnostic().WithLocation(15, 19).WithArguments("indexer", "class"),
            };

            string fixedCode = @"public struct OuterType
{
    public string TestField;
    public OuterType(int argument) { TestField = ""foo""; TestProperty = ""bar""; }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    public string TestProperty { get; set; }
    public string this[string arg] { get { return ""foo""; } set { } }
    public static explicit operator bool(OuterType t1) { return t1.TestField != null; }
    public static OuterType operator +(OuterType t1, OuterType t2) { return t1; }
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 7,
                NumberOfFixAllIterations = 3,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderInterfaceAsync()
        {
            string testCode = @"public interface OuterType
{
    string TestProperty { get; set; }
    event System.Action TestEvent;
    void TestMethod ();
    string this[string arg] { get; set; }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 5).WithArguments("event", "property"),
                Diagnostic().WithLocation(6, 12).WithArguments("indexer", "method"),
            };

            string fixedCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    string this[string arg] { get; set; }
    void TestMethod ();
}
";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIncompleteMemberAsync()
        {
            // Tests that the analyzer does not crash on incomplete members
            string testCode = @"public interface OuterType
{
    event System.Action TestEvent;
    public string
    public string
}
";

            // We don't care about the syntax errors.
            var expected = new[]
            {
                // /0/Test0.cs(5,5): error CS1585: Member modifier 'public' must precede the member type and name
                DiagnosticResult.CompilerError("CS1585").WithLocation(5, 5).WithArguments("public"),

                // /0/Test0.cs(6,1): error CS1519: Invalid token '}' in class, record, struct, or interface member declaration
                DiagnosticResult.CompilerError("CS1519").WithLocation(6, 1).WithArguments("}"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldsAsync()
        {
            // Tests that the analyzer handles event fields as if they were events
            string testCode = @"public class OuterType
{
    event System.Action TestEvent;
    public event System.Action TestEvent2 { add { } remove { } }
    event System.Action TestEvent3;
    public event System.Action TestEvent4 { add { } remove { } }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
