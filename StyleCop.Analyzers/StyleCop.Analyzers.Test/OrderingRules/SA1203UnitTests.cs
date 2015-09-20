// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1203UnitTests : CodeFixVerifier
    {
        private bool suppressSA1202 = false;

        [Fact]
        public async Task TestNoDiagnosticAsync()
        {
            var testCode = @"public static class TestClass1 { }
public class TestClass2
{
    public const int TestField1 = 1;
    public static readonly int TestField2 = 1;
    public static int TestField3 = 1;
    public readonly int TestField4 = 1;
    public int TestField5 = 1;
    internal const int TestField6 = 1;
    internal static readonly int TestField7 = 1;
    internal static int TestField8 = 1;
    internal readonly int TestField9 = 1;
    internal int TestField10 = 1;
    protected internal const int TestField11 = 1;
    protected internal static readonly int TestField12 = 1;
    protected internal static int TestField13 = 1;
    protected internal readonly int TestField14 = 1;
    protected internal int TestField15 = 1;
    protected const int TestField16 = 1;
    protected static readonly int TestField17 = 1;
    protected static int TestField18 = 1;
    protected readonly int TestField19 = 1;
    protected int TestField20 = 1;
    private const int TestField21 = 1;
    private static readonly int TestField22 = 1;
    private static int TestField23 = 1;
    private readonly int TestField24 = 1;
    private int TestField25 = 1;
    public TestClass2()
    {
    }
    private TestClass2(string a)
    {
    }
    ~TestClass2() { }
    
    public static int TestProperty1 { get; set; }
    public int TestProperty2 { get; set; }
    internal static int TestProperty3 { get; set; }
    internal int TestProperty4 { get; set; }
    protected internal static int TestProperty5 { get; set; }
    protected internal int TestProperty6 { get; set; }
    protected static int TestProperty7 { get; set; }
    protected int TestProperty8 { get; set; }
    private static int TestProperty9 { get; set; }
    private int TestProperty10 { get; set; }
    
    public static void TestMethod1() { }
    public void TestMethod2() { }
    internal static void TestMethod3() { }
    internal void TestMethod4() { }
    protected internal static void TestMethod5() { }
    protected internal void TestMethod6() { }
    protected static void TestMethod7() { }
    protected void TestMethod8() { }
    private static void TestMethod9() { }
    private void TestMethod10() { }
    public static class TestClass1 { }
    public class TestClass2a { }
    internal static class TestClass3 { }
    internal class TestClass4 { }
    protected internal static class TestClass5 { }
    protected internal class TestClass6 { }
    protected static class TestClass7 { }
    protected class TestClass8 { }
    private static class TestClass9 { }
    private class TestClass10 { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassViolationAsync()
        {
            var testCode = @"
public class Foo
{
    private int Baz = 1;
    private const int Bar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(5, 23).WithArguments("private");
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = @"
public class Foo
{
    private const int Bar = 2;
    private int Baz = 1;
}";
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructViolationAsync()
        {
            var testCode = @"
public struct Foo
{
    private int baz;
    private const int Bar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(5, 23).WithArguments("private");
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = @"
public struct Foo
{
    private const int Bar = 2;
    private int baz;
}";
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSecondConstAfterNonConstAsync()
        {
            var testCode = @"
public class Foo
{
    private const int Bar = 2;
    private int Baz = 1;
    private const int FooBar = 2;
}";
            var firstDiagnostic = this.CSharpDiagnostic().WithLocation(6, 23).WithArguments("private");
            await this.VerifyCSharpDiagnosticAsync(testCode, firstDiagnostic, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = @"
public class Foo
{
    private const int Bar = 2;
    private const int FooBar = 2;
    private int Baz = 1;
}";
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnqualifiedConstAsync()
        {
            var testCode = @"
public class Test
{
    private int Test1 = 1;
    const int Test2 = 2;
    const int Test3 = 3;
}";

            var expected = this.CSharpDiagnostic().WithLocation(5, 15).WithArguments("private");

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var testCodeAfterFix1 = @"
public class Test
{
    const int Test2 = 2;
    private int Test1 = 1;
    const int Test3 = 3;
}";
            expected = this.CSharpDiagnostic().WithLocation(6, 15).WithArguments("private");
            await this.VerifyCSharpDiagnosticAsync(testCodeAfterFix1, expected, CancellationToken.None).ConfigureAwait(false);

            var fixCode = @"
public class Test
{
    const int Test2 = 2;
    const int Test3 = 3;
    private int Test1 = 1;
}";

            await this.VerifyCSharpDiagnosticAsync(fixCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCodeAfterFix1, fixCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will move the non constant fields before the constant ones, only using a single access modifier.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixSingleAccessModifierAsync()
        {
            var testCode = @"
public class Foo
{
    public const string Before1 = ""test"";

    public const string Before2 = ""test"";

    public int field1;

    public const string After1 = ""test"";

    public int between;

    public const string After2 = ""test"";
}
";

            var fixedTestCode = @"
public class Foo
{
    public const string Before1 = ""test"";

    public const string Before2 = ""test"";

    public const string After1 = ""test"";

    public const string After2 = ""test"";

    public int field1;

    public int between;
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 25).WithArguments("public"),
                this.CSharpDiagnostic().WithLocation(14, 25).WithArguments("public")
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will move the non constant fields before the constant ones.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private int field1;

    private const string After1 = ""test"";

    public int between;

    public const string After2 = ""test"";
}
";

            var fixedTestCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    public const string After2 = ""test"";

    private const string After1 = ""test"";

    private int field1;

    public int between;
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 26).WithArguments("private"),
                this.CSharpDiagnostic().WithLocation(13, 25).WithArguments("public")
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will move the non constant fields before the constant ones leaving the comments in the proper place.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixWithCommentsAsync()
        {
            var testCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private int field1;

    //Comment on this field
    private const string After1 = ""test"";

    public int between;

    public const string After2 = ""test"";
}
";

            var fixedTestCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    public const string After2 = ""test"";

    //Comment on this field
    private const string After1 = ""test"";

    private int field1;

    public int between;
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(10, 26).WithArguments("private"),
                this.CSharpDiagnostic().WithLocation(14, 25).WithArguments("public")
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Test comments not being moved.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOnlyLeadingWhitespaceIsMovedAsync()
        {
            var testCode = @"class Foo
{
    /// <summary>
    /// Bar
    /// </summary>
    string bar;
const string foo = ""a"";
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 14).WithArguments("private"),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = @"class Foo
{
    const string foo = ""a"";
    /// <summary>
    /// Bar
    /// </summary>
    string bar;
}
";
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will move the non constant fields before the constant ones with SA1202 suppressed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixWithSA1202SuppressedAsync()
        {
            this.suppressSA1202 = true;

            var testCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private int field1;

    private const string After1 = ""test"";

    public int between;

    public const string After2 = ""test"";
}
";

            var fixedTestCode = @"public class Foo
{
    private const string Before1 = ""test"";

    public const string Before2 = ""test"";

    private const string After1 = ""test"";

    public const string After2 = ""test"";

    private int field1;

    public int between;
}
";

            var diagnosticResults = new[]
            {
                this.CSharpDiagnostic().WithLocation(9, 26).WithArguments("private"),
                this.CSharpDiagnostic().WithLocation(13, 25).WithArguments("public")
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, diagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1203ConstantsMustAppearBeforeFields();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ElementOrderCodeFixProvider();
        }

        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.suppressSA1202)
            {
                yield return SA1202ElementsMustBeOrderedByAccess.DiagnosticId;
            }
        }
    }
}
