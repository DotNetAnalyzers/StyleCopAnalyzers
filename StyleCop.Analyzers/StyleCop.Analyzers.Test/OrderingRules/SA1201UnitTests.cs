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

    public class SA1201UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 22).WithArguments("delegate", "enum"),
                this.CSharpDiagnostic().WithLocation(7, 15).WithArguments("struct", "class"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 12).WithArguments("constructor", "destructor"),
                this.CSharpDiagnostic().WithLocation(7, 26).WithArguments("delegate", "interface"),
                this.CSharpDiagnostic().WithLocation(11, 5).WithArguments("conversion", "operator"),
                this.CSharpDiagnostic().WithLocation(12, 19).WithArguments("property", "conversion"),
                this.CSharpDiagnostic().WithLocation(14, 17).WithArguments("method", "struct"),
                this.CSharpDiagnostic().WithLocation(16, 19).WithArguments("indexer", "class")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 26).WithArguments("delegate", "interface"),
                this.CSharpDiagnostic().WithLocation(10, 5).WithArguments("conversion", "operator"),
                this.CSharpDiagnostic().WithLocation(11, 19).WithArguments("property", "conversion"),
                this.CSharpDiagnostic().WithLocation(13, 17).WithArguments("method", "struct"),
                this.CSharpDiagnostic().WithLocation(15, 19).WithArguments("indexer", "class")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(4, 5).WithArguments("event", "property"),
                this.CSharpDiagnostic().WithLocation(6, 12).WithArguments("indexer", "method")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            string fixedCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    string this[string arg] { get; set; }
    void TestMethod ();
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1201ElementsMustAppearInTheCorrectOrder();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ElementOrderCodeFixProvider();
        }
    }
}
