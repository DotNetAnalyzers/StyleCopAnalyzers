// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1129DoNotUseDefaultValueTypeConstructor,
        Analyzers.ReadabilityRules.SA1129CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1129DoNotUseDefaultValueTypeConstructor"/> class.
    /// </summary>
    public class SA1129UnitTests
    {
        /// <summary>
        /// Verifies that new expressions for reference types will not generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyReferenceTypeCreationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = new TestClass2();
        var v2 = new TestClass2(1);
    }

    private class TestClass2
    {
        public TestClass2()
        {
        }

        public TestClass2(int x)
        {
        }
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that new expressions for value types with parameters will not generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyValueTypeWithParametersCreationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = new TestStruct(1);
        var v2 = new TestStruct(1) { TestProperty = 2 };
        var v3 = new TestStruct() { TestProperty = 2 };
        var v4 = new TestStruct { TestProperty = 2 };
    }

    private struct TestStruct
    {
        public TestStruct(int x)
        {
            TestProperty = x;
        }

        public int TestProperty { get; set; }
    }
}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that new expressions for value types without parameters will generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyInvalidValueTypeCreationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = new TestStruct();

        System.Console.WriteLine(new TestStruct());
    }

    private struct TestStruct
    {
        public int TestProperty { get; set; }
    }
}
";

            var fixedTestCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = default(TestStruct);

        System.Console.WriteLine(default(TestStruct));
    }

    private struct TestStruct
    {
        public int TestProperty { get; set; }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 18),
                Diagnostic().WithLocation(7, 34),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix will preserve surrounding trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyTriviaPreservationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = /* c1 */ new TestStruct(); // c2

        var v2 =
#if true
            new TestStruct();
#else
            new TestStruct() { TestProperty = 3 };
#endif
    }

    private struct TestStruct
    {
        public int TestProperty { get; set; }
    }
}
";

            var fixedTestCode = @"public class TestClass
{
    public void TestMethod()
    {
        var v1 = /* c1 */ default(TestStruct); // c2

        var v2 =
#if true
            default(TestStruct);
#else
            new TestStruct() { TestProperty = 3 };
#endif
    }

    private struct TestStruct
    {
        public int TestProperty { get; set; }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 27),
                Diagnostic().WithLocation(9, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that new expressions for value types through generics will generate diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyGenericsTypeCreationAsync()
        {
            var testCode = @"public class TestClass
{
    public T TestMethod1<T>()
        where T : struct
    {
        return new T();
    }

    public T TestMethod2<T>()
        where T : new()
    {
        return new T();
    }
}
";

            var fixedTestCode = @"public class TestClass
{
    public T TestMethod1<T>()
        where T : struct
    {
        return default(T);
    }

    public T TestMethod2<T>()
        where T : new()
    {
        return new T();
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 16),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>.
        /// </summary>
        /// <param name="typeNamespace">The namespace of the special type.</param>
        /// <param name="typeName">The name of the special type.</param>
        /// <param name="fieldName">The name of the field providing the default value for the type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.Threading", nameof(CancellationToken), nameof(CancellationToken.None))]
        [InlineData("System", nameof(IntPtr), nameof(IntPtr.Zero))]
        [InlineData("System", nameof(UIntPtr), nameof(UIntPtr.Zero))]
        [InlineData("System", nameof(Guid), nameof(Guid.Empty))]
        public async Task VerifySpecialTypeFixUsesSpecialSyntaxAsync(string typeNamespace, string typeName, string fieldName)
        {
            var testCode = $@"
using {typeNamespace};

public class TestClass
{{
    public void TestMethod()
    {{
        var value = [|new {typeName}()|];
    }}
}}
";

            var fixedTestCode = $@"
using {typeNamespace};

public class TestClass
{{
    public void TestMethod()
    {{
        var value = {typeName}.{fieldName};
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix will preserve trivia surrounding <c>new CancellationToken()</c>.
        /// </summary>
        /// <param name="typeNamespace">The namespace of the special type.</param>
        /// <param name="typeName">The name of the special type.</param>
        /// <param name="fieldName">The name of the field providing the default value for the type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.Threading", nameof(CancellationToken), nameof(CancellationToken.None))]
        [InlineData("System", nameof(IntPtr), nameof(IntPtr.Zero))]
        [InlineData("System", nameof(UIntPtr), nameof(UIntPtr.Zero))]
        [InlineData("System", nameof(Guid), nameof(Guid.Empty))]
        public async Task VerifySpecialTypeTriviaPreservationAsync(string typeNamespace, string typeName, string fieldName)
        {
            var testCode = $@"
using {typeNamespace};

public class TestClass
{{
    public void TestMethod()
    {{
        var value = /* c1 */ [|new {typeName}()|]; // c2
    }}
}}
";

            var fixedTestCode = $@"
using {typeNamespace};

public class TestClass
{{
    public void TestMethod()
    {{
        var value = /* c1 */ {typeName}.{fieldName}; // c2
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>,
        /// and a fully-qualified name is preserved correctly.
        /// </summary>
        /// <param name="typeNamespace">The namespace of the special type.</param>
        /// <param name="typeName">The name of the special type.</param>
        /// <param name="fieldName">The name of the field providing the default value for the type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.Threading", nameof(CancellationToken), nameof(CancellationToken.None))]
        [InlineData("System", nameof(IntPtr), nameof(IntPtr.Zero))]
        [InlineData("System", nameof(UIntPtr), nameof(UIntPtr.Zero))]
        [InlineData("System", nameof(Guid), nameof(Guid.Empty))]
        public async Task VerifyQualifiedSpecialTypeFixUsesFieldSyntaxAsync(string typeNamespace, string typeName, string fieldName)
        {
            var testCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var value = [|new {typeNamespace}.{typeName}()|];
    }}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    public void TestMethod()
    {{
        var value = {typeNamespace}.{typeName}.{fieldName};
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is <b>not</b> replaced by <c>CancellationToken.None</c>
        /// if the qualified name is not exactly <c>System.Threading.CancellationToken</c>.
        /// </summary>
        /// <param name="typeName">The name of the special type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(nameof(CancellationToken))]
        [InlineData(nameof(IntPtr))]
        [InlineData(nameof(UIntPtr))]
        [InlineData(nameof(Guid))]
        public async Task VerifyCustomSpecialTypeStructIsNotReplacedAsync(string typeName)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var value = [|new {typeName}()|];
    }}

    private struct {typeName}
    {{
        public int TestProperty {{ get; set; }}
    }}
}}
";

            var fixedTestCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var value = default({typeName});
    }}

    private struct {typeName}
    {{
        public int TestProperty {{ get; set; }}
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>,
        /// even when aliased by a <c>using</c> statement.
        /// </summary>
        /// <param name="typeNamespace">The namespace of the special type.</param>
        /// <param name="typeName">The name of the special type.</param>
        /// <param name="fieldName">The name of the field providing the default value for the type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.Threading", nameof(CancellationToken), nameof(CancellationToken.None))]
        [InlineData("System", nameof(IntPtr), nameof(IntPtr.Zero))]
        [InlineData("System", nameof(UIntPtr), nameof(UIntPtr.Zero))]
        [InlineData("System", nameof(Guid), nameof(Guid.Empty))]
        public async Task VerifyAliasedSpecialTypeUsesFieldSyntaxAsync(string typeNamespace, string typeName, string fieldName)
        {
            var testCode = $@"
using SystemType = {typeNamespace}.{typeName};

public class TestClass
{{
    private SystemType value = [|new SystemType()|];
}}
";

            var fixedTestCode = $@"
using SystemType = {typeNamespace}.{typeName};

public class TestClass
{{
    private SystemType value = SystemType.{fieldName};
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new MyEnum()</c> is replaced by <c>MyEnum.Member</c>
        /// iff there is one member in <c>MyEnum</c> with a value of <c>0</c>.
        /// </summary>
        /// <param name="declarationBody">The injected <c>enum</c> declaration body.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Member")]
        [InlineData("Member = 0")]
        [InlineData("Member = 0, Another = 1")]
        [InlineData("Another = 1, Member = 0")]
        [InlineData("Member, Another")]
        public async Task VerifyEnumMemberReplacementBehaviorAsync(string declarationBody)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var v1 = new MyEnum();
    }}

    private enum MyEnum {{ {declarationBody} }}
}}";

            var fixedTestCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var v1 = MyEnum.Member;
    }}

    private enum MyEnum {{ {declarationBody} }}
}}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new MyEnum()</c> is <b>not</b> replaced by <c>MyEnum.Member</c>
        /// if there is no member with a value of <c>0</c>, but instead uses the default replacement behavior.
        /// </summary>
        /// <param name="declarationBody">The injected <c>enum</c> declaration body.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("Member = 1")]
        [InlineData("Member = 1, Another = 2")]
        [InlineData("FooMember = 0, BarMember = 0")]
        [InlineData("FooMember, BarMember = 0")]
        public async Task VerifyEnumMemberDefaultBehaviorAsync(string declarationBody)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var v1 = new MyEnum();
    }}

    private enum MyEnum {{ {declarationBody} }}
}}";

            var fixedTestCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        var v1 = default(MyEnum);
    }}

    private enum MyEnum {{ {declarationBody} }}
}}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>default(CancellationToken)</c> when its used for a default parameter.
        /// </summary>
        /// <param name="typeNamespace">The namespace of the special type.</param>
        /// <param name="typeName">The name of the special type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("System.Threading", nameof(CancellationToken))]
        [InlineData("System", nameof(IntPtr))]
        [InlineData("System", nameof(UIntPtr))]
        [InlineData("System", nameof(Guid))]
        [WorkItem(2740, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2740")]
        public async Task VerifySpecialTypeDefaultParameterAsync(string typeNamespace, string typeName)
        {
            var testCode = $@"using {typeNamespace};

public class TestClass
{{
    public TestClass({typeName} cancellationToken = [|new {typeName}()|])
    {{
    }}

    public void TestMethod({typeName} cancellationToken = [|new {typeName}()|])
    {{
    }}
}}
";

            var fixedTestCode = $@"using {typeNamespace};

public class TestClass
{{
    public TestClass({typeName} cancellationToken = default({typeName}))
    {{
    }}

    public void TestMethod({typeName} cancellationToken = default({typeName}))
    {{
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
