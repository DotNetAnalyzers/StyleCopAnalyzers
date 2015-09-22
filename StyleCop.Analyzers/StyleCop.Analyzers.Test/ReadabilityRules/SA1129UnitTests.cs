// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.ReadabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
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
                this.CSharpDiagnostic().WithLocation(9, 13)
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
                this.CSharpDiagnostic().WithLocation(6, 16)
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
