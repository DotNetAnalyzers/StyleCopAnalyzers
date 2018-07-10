// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1129DoNotUseDefaultValueTypeConstructor"/> class.
    /// </summary>
    public class SA1129UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 18),
                this.CSharpDiagnostic().WithLocation(7, 34),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 27),
                this.CSharpDiagnostic().WithLocation(9, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 16),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCancellationTokenFixUsesNoneSyntaxAsync()
        {
            var testCode = @"
using System.Threading;

public class TestClass
{
    public void TestMethod()
    {
        var ct = new CancellationToken();
    }
}
";

            var fixedTestCode = @"
using System.Threading;

public class TestClass
{
    public void TestMethod()
    {
        var ct = CancellationToken.None;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix will preserve trivia surrounding <c>new CancellationToken()</c>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCancellationTokenTriviaPreservationAsync()
        {
            var testCode = @"
using System.Threading;

public class TestClass
{
    public void TestMethod()
    {
        var ct1 = /* c1 */ new CancellationToken(); // c2
    }
}
";

            var fixedTestCode = @"
using System.Threading;

public class TestClass
{
    public void TestMethod()
    {
        var ct1 = /* c1 */ CancellationToken.None; // c2
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 28),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>,
        /// and a fully-qualified name is preserved correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyQualifiedCancellationTokenFixUsesNoneSyntaxAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var ct = new System.Threading.CancellationToken();
    }
}
";

            var fixedTestCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var ct = System.Threading.CancellationToken.None;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is <b>not</b> replaced by <c>CancellationToken.None</c>
        /// if the qualified name is not exactly <c>System.Threading.CancellationToken</c>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCustomCancellationTokenClassIsNotReplacedAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var ct = new CancellationToken();
    }

    private struct CancellationToken
    {
        public int TestProperty { get; set; }
    }
}
";

            var fixedTestCode = @"public class TestClass
{
    public void TestMethod()
    {
        var ct = default(CancellationToken);
    }

    private struct CancellationToken
    {
        public int TestProperty { get; set; }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>CancellationToken.None</c>,
        /// even when aliased by a <c>using</c> statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyAliasedCancellationTokenUsesNoneSyntaxAsync()
        {
            var testCode = @"
using SystemToken = System.Threading.CancellationToken;

public class TestClass
{
    private SystemToken ct = new SystemToken();
}
";

            var fixedTestCode = @"
using SystemToken = System.Threading.CancellationToken;

public class TestClass
{
    private SystemToken ct = SystemToken.None;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 30),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(5, 18),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that <c>new CancellationToken()</c> is replaced by <c>default(CancellationToken)</c> when its used for a default parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2740, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2740")]
        public async Task VerifyCancellationTokenDefaultParameterAsync()
        {
            var testCode = @"using System.Threading;

public class TestClass
{
    public TestClass(CancellationToken cancellationToken = new CancellationToken())
    {
    }

    public void TestMethod(CancellationToken cancellationToken = new CancellationToken())
    {
    }
}
";

            var fixedTestCode = @"using System.Threading;

public class TestClass
{
    public TestClass(CancellationToken cancellationToken = default(CancellationToken))
    {
    }

    public void TestMethod(CancellationToken cancellationToken = default(CancellationToken))
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 60),
                this.CSharpDiagnostic().WithLocation(9, 66),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1129DoNotUseDefaultValueTypeConstructor();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1129CodeFixProvider();
        }
    }
}
