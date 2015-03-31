namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1201UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOuterOrderCorrectOrder()
        {
            string testCode = @"namespace Foo { }
public delegate void bar();
public enum TestEnum { }
public interface IFoo { }
public struct FooStruct { }
public class FooClass { }
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOuterOrderWrongOrder()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None);
        }
        
        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderClass()
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
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderStruct()
        {
            string testCode = @"public struct OuterType
{
    public string TestField;
    public OuterType() { TestField = ""foo""; TestProperty = """"; }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    public string TestProperty { get; set; }
    public string this[string arg] { get { return ""foo""; } set { } }
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeMemberOrderCorrectOrderInterface()
        {
            string testCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    string this[string arg] { get; set; }
    void TestMethod ();
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderClass()
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
    public string TestProperty { get; set; }
    public struct TestStruct { }
    public void TestMethod () { }
    public string this[string arg] { get { return ""foo""; } set { } }
    public class TestClass { }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 12).WithArguments("constructor", "destructor"),
                this.CSharpDiagnostic().WithLocation(7, 26).WithArguments("delegate", "interface"),
                this.CSharpDiagnostic().WithLocation(12, 17).WithArguments("method", "struct"),
                this.CSharpDiagnostic().WithLocation(13, 19).WithArguments("indexer", "method")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderStruct()
        {
            // Constructor and destructors might not be valid for interfaces/structs, but the code does not have to be valid to test the diagnostic
            string testCode = @"public struct OuterType
{
    public string TestField;
    public OuterType() { TestField = ""foo""; TestProperty = ""bar""; }
    public interface ITest { }
    public delegate void TestDelegate();
    public event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public string TestProperty { get; set; }
    public struct TestStruct { }
    public void TestMethod () { }
    public string this[string arg] { get { return ""foo""; } set { } }
    public class TestClass { }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(6, 26).WithArguments("delegate", "interface"),
                this.CSharpDiagnostic().WithLocation(11, 17).WithArguments("method", "struct"),
                this.CSharpDiagnostic().WithLocation(12, 19).WithArguments("indexer", "method")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestTypeMemberOrderWrongOrderInterface()
        {
            // Constructor and destructors might not be valid for interfaces/structs, but the code does not have to be valid to test the diagnostic
            string testCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    void TestMethod ();
    string this[string arg] { get; set; }
}
";
            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(6, 12).WithArguments("indexer", "method")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1201ElementsMustAppearInTheCorrectOrder();
        }
    }
}
