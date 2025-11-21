// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1201CSharp9UnitTests : SA1201CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3236, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3236")]
        public async Task TestOuterOrderWithRecordCorrectOrderAsync()
        {
            string testCode = @"namespace Foo { }
public delegate void bar();
public enum TestEnum { }
public interface IFoo { }
public struct FooStruct { }
public record FooClass { }
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3236, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3236")]
        public async Task TestOuterOrderWithRecordWrongOrderAsync()
        {
            string testCode = @"
namespace Foo { }
public enum TestEnum { }
public delegate void {|#0:bar|}();
public interface IFoo { }
public record FooClass { }
public struct {|#1:FooStruct|} { }
";
            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments("delegate", "enum"),
                Diagnostic().WithLocation(1).WithArguments("struct", "record"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderRecordAsync()
        {
            string testCode = @"public record OuterType
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
    public class TestClass1 { }
    public record TestRecord1 { }
    public class TestClass2 { }
    public record TestRecord2 { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderRecordAsync()
        {
            string testCode = @"public record OuterType
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

            string fixedCode = @"public record OuterType
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
    }
}
