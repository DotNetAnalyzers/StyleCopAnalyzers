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
                this.CSharpDiagnostic().WithLocation(4, 22),
                this.CSharpDiagnostic().WithLocation(7, 15),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeMemberOrderCorrectOrder(string type)
        {
            // Constructor and destructors might not be valid for interfaces/structs, but the code does not have to be valid to test the diagnostic
            string testCode = @"public %type% OuterType
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
    public void TestMethod () { }
    public struct TestStruct { }
    public class TestClass { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%type%", type), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeMemberOrderWrongOrder(string type)
        {
            // Constructor and destructors might not be valid for interfaces/structs, but the code does not have to be valid to test the diagnostic
            string testCode = @"public %type% OuterType
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
                this.CSharpDiagnostic().WithLocation(5, 12),
                this.CSharpDiagnostic().WithLocation(7, 26),
                this.CSharpDiagnostic().WithLocation(8, 31),
                this.CSharpDiagnostic().WithLocation(9, 17),
                this.CSharpDiagnostic().WithLocation(12, 17),
                this.CSharpDiagnostic().WithLocation(13, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%type%", type), expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1201ElementsMustAppearInTheCorrectOrder();
        }
    }
}
