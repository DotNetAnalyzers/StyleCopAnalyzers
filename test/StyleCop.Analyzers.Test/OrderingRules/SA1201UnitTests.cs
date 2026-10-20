// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1201UnitTests
    {
        public static IEnumerable<object[]> ValueTypesAndReferenceTypes
        {
            get
            {
                foreach (var valueTypeKeyword in CommonMemberData.ValueTypeDeclarationKeywords)
                {
                    foreach (var referenceTypeKeyword in CommonMemberData.ReferenceTypeDeclarationKeywords)
                    {
                        yield return new object[] { valueTypeKeyword.Single(), referenceTypeKeyword.Single() };
                    }
                }
            }
        }

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

        /// <summary>
        /// Verifies that ordering types preserves region boundaries at compilation-unit and namespace scope.
        /// </summary>
        /// <param name="inNamespace">Whether the types are declared inside a namespace.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [WorkItem(2112, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2112")]
        public async Task TestCodeFixWithRegionsAtOuterScopeAsync(bool inNamespace)
        {
            var testCode = @"
#region Classes
public class Foo { }
#endregion

#region Structs
public struct {|#0:Bar|} { }
#endregion
";
            if (inNamespace)
            {
                testCode = "namespace OuterNamespace\r\n{\r\n" + testCode + "}\r\n";
            }

            var expected = Diagnostic().WithLocation(0).WithArguments("A struct", "a class");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that ordering non-field members preserves region boundaries.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2112, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2112")]
        public async Task TestCodeFixWithRegionsAroundNonFieldMembersAsync()
        {
            var testCode = @"
public class Foo
{
    #region Methods
    public void Method() { }
    #endregion

    #region Properties
    public int {|#0:Property|} { get; set; }
    #endregion
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("A property", "a method");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(4, 22).WithArguments("A delegate", "an enum"),
                Diagnostic().WithLocation(7, 15).WithArguments("A struct", "a class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync("namespace OuterNamespace { " + testCode + " }", expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(ValueTypesAndReferenceTypes))]
        public async Task TestClassBeforeStructAsync(
            string structKeyword,
            string classKeyword)
        {
            string testCode = $@"
public {classKeyword} FooClass {{ }}
public {structKeyword} {{|#0:FooStruct|}} {{ }}
";
            string fixedCode = $@"public {structKeyword} FooStruct {{ }}

public {classKeyword} FooClass {{ }}
";

            var reportedClassKind = classKeyword switch
            {
                "record class" => "a record",
                _ => "a " + classKeyword,
            };

            var reportedStructKind = "A " + structKeyword;

            var expected = new[]
            {
                Diagnostic().WithLocation(0).WithArguments(reportedStructKind, reportedClassKind),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMessageUsesCorrectArticleAsync()
        {
            string testCode = @"public class OuterType
{
    public void Method() { }
    public static bool operator ==(OuterType left, OuterType right) { return true; }
    public static bool operator !=(OuterType left, OuterType right) { return false; }
    public static implicit operator int(OuterType value) { return 0; }
    public enum InnerEnum { A }
}

public class OtherType { }
public enum OtherEnum { A }
";

            var expected = new[]
            {
                Diagnostic().WithLocation(4, 5).WithMessage("An operator should not follow a method"),
                Diagnostic().WithLocation(6, 5).WithMessage("A conversion should not follow an operator"),
                Diagnostic().WithLocation(7, 17).WithMessage("An enum should not follow a conversion"),
                Diagnostic().WithLocation(11, 13).WithMessage("An enum should not follow a class"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

        /// <summary>
        /// Verifies that Fix All sorts all members in a type in a single pass.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1589, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1589")]
        public async Task TestFixAllSortsTypeMembersInSinglePassAsync()
        {
            string testCode = @"namespace ClassLibrary1
{
    public class Class1
    {
        public void Method1()
        {
        }

        public static string {|#0:Property1|} { get; }

        public string Property2 { get; }

        public void Method2()
        {
        }

        public string {|#1:Property3|} { get; }
    }
}
";
            string fixedCode = @"namespace ClassLibrary1
{
    public class Class1
    {
        public static string Property1 { get; }

        public string Property2 { get; }

        public string Property3 { get; }

        public void Method1()
        {
        }

        public void Method2()
        {
        }
    }
}
";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 3,
                NumberOfFixAllIterations = 1,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0).WithArguments("A property", "a method"));
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(1).WithArguments("A property", "a method"));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(5, 12).WithArguments("A constructor", "a destructor"),
                Diagnostic().WithLocation(7, 26).WithArguments("A delegate", "an interface"),
                Diagnostic().WithLocation(11, 5).WithArguments("A conversion", "an operator"),
                Diagnostic().WithLocation(12, 19).WithArguments("A property", "a conversion"),
                Diagnostic().WithLocation(14, 17).WithArguments("A method", "a struct"),
                Diagnostic().WithLocation(16, 19).WithArguments("An indexer", "a class"),
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
                NumberOfFixAllIterations = 1,
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
                Diagnostic().WithLocation(6, 26).WithArguments("A delegate", "an interface"),
                Diagnostic().WithLocation(10, 5).WithArguments("A conversion", "an operator"),
                Diagnostic().WithLocation(11, 19).WithArguments("A property", "a conversion"),
                Diagnostic().WithLocation(13, 17).WithArguments("A method", "a struct"),
                Diagnostic().WithLocation(15, 19).WithArguments("An indexer", "a class"),
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
                NumberOfFixAllIterations = 1,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestTypeMemberOrderWrongOrderInterfaceAsync(string lineEnding)
        {
            string testCode = @"public interface OuterType
{
    string TestProperty { get; set; }
    {|#0:event System.Action TestEvent;|}
    void TestMethod ();
    string {|#1:this|}[string arg] { get; set; }
}
".ReplaceLineEndings(lineEnding);

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("An event", "a property"),
                Diagnostic().WithLocation(1).WithArguments("An indexer", "a method"),
            };

            string fixedCode = @"public interface OuterType
{
    event System.Action TestEvent;
    string TestProperty { get; set; }
    string this[string arg] { get; set; }
    void TestMethod ();
}
".ReplaceLineEndings(lineEnding);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that reordering does not change conditional compilation boundaries.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestConditionalCompilationBoundaryAsync()
        {
            string testCode = @"namespace ClassLibrary1
{
    public class Class1
    {
        public void Method1()
        {
        }

#if true
        public string Property1 { get; }
#endif
    }
}
";

            var expected = Diagnostic().WithLocation(10, 23).WithArguments("A property", "a method");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that directives on preceding members also prevent unsafe reordering.
        /// </summary>
        /// <param name="members">The members preceding the property.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("#if true\r\n    public void Method1() { }\r\n")]
        [InlineData("#if true\r\n    public void Method1() { }\r\n#endif\r\n")]
        [InlineData("    public void Method1() { }\r\n#if true\r\n    public void Method2() { }\r\n#endif\r\n    public void Method3() { }\r\n")]
        [InlineData("    public void Method1()\r\n    {\r\n#if true\r\n#endif\r\n    }\r\n")]
        [InlineData("#region Methods\r\n    public void Method1() { }\r\n#endregion\r\n")]
        [InlineData("#pragma warning disable CS0168\r\n    public void Method1() { }\r\n#pragma warning restore CS0168\r\n")]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestDirectivesOnPrecedingMembersAsync(string members)
        {
            string testCode = "public class Class1\r\n{\r\n"
                + members
                + "    public string Property1 { get; }\r\n"
                + (members.Contains("#if") && !members.Contains("#endif") ? "#endif\r\n" : string.Empty)
                + "}\r\n";

            var expected = Diagnostic().WithLocation(members.Count(c => c == '\n') + 3, 19).WithArguments("A property", "a method");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that directive boundaries are preserved at file and namespace scope.
        /// </summary>
        /// <param name="inNamespace">Whether to place the declarations in a namespace.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestDirectiveBoundaryAtFileOrNamespaceScopeAsync(bool inNamespace)
        {
            string testCode = @"public class Class1 { }
#if true
public enum Enum1 { }
#endif
";
            if (inNamespace)
            {
                testCode = "namespace Namespace1\r\n{\r\n" + testCode + "}\r\n";
            }

            var expected = Diagnostic().WithLocation(inNamespace ? 5 : 3, 13).WithArguments("An enum", "a class");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that unrelated directives do not prevent safe reordering.
        /// </summary>
        /// <param name="condition">The conditional compilation condition.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestReorderWithinDirectiveBoundaryAsync(string condition)
        {
            string testCode = $@"public class Class1
{{
#if {condition}
    public int Field1;
#endif
    public int Field2;
    public void Method1() {{ }}
    public string Property1 {{ get; }}
}}
";
            string fixedCode = $@"public class Class1
{{
#if {condition}
    public int Field1;
#endif
    public int Field2;
    public string Property1 {{ get; }}
    public void Method1() {{ }}
}}
";

            var expected = Diagnostic().WithLocation(8, 19).WithArguments("A property", "a method");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that safe reordering within an active conditional block remains available.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestReorderInsideConditionalBlockAsync()
        {
            string testCode = @"public class Class1
{
#if true
    public int Field1;
    public void Method1() { }
    public string Property1 { get; }
#endif
}
";
            string fixedCode = @"public class Class1
{
#if true
    public int Field1;
    public string Property1 { get; }
    public void Method1() { }
#endif
}
";

            var expected = Diagnostic().WithLocation(6, 19).WithArguments("A property", "a method");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that inactive conditional members do not produce ordering violations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestInactiveConditionalMemberAsync()
        {
            string testCode = @"public class Class1
{
    public void Method1() { }
#if false
    public string Property1 { get; }
#endif
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that Fix All leaves unsafe moves unchanged while applying safe moves.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1588, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1588")]
        public async Task TestFixAllPreservesDirectiveBoundariesAsync()
        {
            string testCode = @"public class Class1
{
    public void Method1() { }
#if true
    public string Property1 { get; }
#endif
}

public class Class2
{
    public void Method2() { }
    public string Property2 { get; }
}
";
            string fixedCode = @"public class Class1
{
    public void Method1() { }
#if true
    public string Property1 { get; }
#endif
}

public class Class2
{
    public string Property2 { get; }
    public void Method2() { }
}
";

            var remaining = Diagnostic().WithLocation(5, 19).WithArguments("A property", "a method");
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            };
            test.ExpectedDiagnostics.Add(remaining);
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(12, 19).WithArguments("A property", "a method"));
            test.RemainingDiagnostics.Add(remaining);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

        /// <summary>
        /// Verifies that explicit interface events use public accessibility when accessibility precedes kind.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1823, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1823")]
        public async Task TestExplicitInterfaceEventWithAccessibilityFirstOrderingAsync()
        {
            var testCode = @"public interface IExample
{
    event System.Action Changed;
}

public class Example : IExample
{
    public void PublicMethod() { }
    event System.Action IExample.{|#0:Changed|} { add { } remove { } }
}
";

            var fixedCode = @"public interface IExample
{
    event System.Action Changed;
}

public class Example : IExample
{
    event System.Action IExample.Changed { add { } remove { } }
    public void PublicMethod() { }
}
";

            var settings = @"{
  ""settings"": {
    ""orderingRules"": {
      ""elementOrder"": [
        ""accessibility"",
        ""kind"",
        ""constant"",
        ""static"",
        ""readonly""
      ]
    }
  }
}";

            await VerifyCSharpFixAsync(
                testCode,
                settings,
                new[] { Diagnostic().WithLocation(0).WithArguments("An event", "a method") },
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}
