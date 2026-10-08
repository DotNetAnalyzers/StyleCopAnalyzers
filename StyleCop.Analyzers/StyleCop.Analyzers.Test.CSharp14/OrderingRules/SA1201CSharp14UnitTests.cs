// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1201CSharp14UnitTests : SA1201CSharp13UnitTests
    {
        [Fact]
        [WorkItem(3955, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3955")]
        public async Task TestExtensionBlockAfterFieldAsync()
        {
            var testCode = @"
public class MyClass { }

public static class MyClassExtensions
{
    private const string SomeField = ""hi"";

    extension(MyClass self)
    {
        public void Hey() => self.ToString();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3955, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3955")]
        public async Task TestTypeMemberOrderCorrectOrderWithExtensionBlocksAsync()
        {
            var testCode = @"
public static class TestClass
{
    public static string TestField;
    public delegate void TestDelegate();
    public static event TestDelegate TestEvent { add { } remove { } }
    public enum TestEnum { }
    public interface ITest { }
    extension(string source) { public int Length1 => source.Length; }
    extension(object source) { public string Text => source.ToString(); }
    public static string TestProperty { get; set; }
    public static void TestMethod() { }
    public struct TestStruct { }
    public class TestClass1 { }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3955, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3955")]
        public async Task TestExtensionBlockAfterPropertyAsync()
        {
            var testCode = @"
public static class TestClass
{
    public static int TestProperty => 0;

    {|#0:extension|}(string source)
    {
        public int MyLength => source.Length;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int MyLength => source.Length;
    }

    public static int TestProperty => 0;
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("An extension", "a property");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3955, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3955")]
        public async Task TestFieldAfterExtensionBlockAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int MyLength => source.Length;
    }

    private static int {|#0:testField|};
}
";

            var fixedCode = @"
public static class TestClass
{
    private static int testField;

    extension(string source)
    {
        public int MyLength => source.Length;
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("A field", "an extension");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestInstanceCompoundAssignmentOperatorAfterStaticOperatorAsync()
        {
            string testCode = @"
public class TestClass
{
    public int Value;

    public static TestClass operator +(TestClass a, int b)
    {
        return a;
    }

    public void operator +=(int x)
    {
        this.Value += x;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4030, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4030")]
        public async Task TestInstanceCompoundAssignmentOperatorAfterMethodAsync()
        {
            string testCode = @"
public class TestClass
{
    public int Value;

    public void TestMethod()
    {
    }

    {|#0:public void operator +=(int x)
    {
        this.Value += x;
    }|}
}
";

            string fixedCode = @"
public class TestClass
{
    public int Value;

    public void operator +=(int x)
    {
        this.Value += x;
    }

    public void TestMethod()
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("An operator", "a method");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
