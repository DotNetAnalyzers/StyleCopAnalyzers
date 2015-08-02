namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1204UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid ordering.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidOrderingAsync()
        {
            var testCode = @"public static class TestClass1 { }
public class TestClass2 {
    public static int TestField1;
    public int TestField2;
    public static int TestProperty1 { get; set; }
    public int TestProperty2 { get; set; }
    public static void TestMethod1() { }
    public void TestMethod2() { }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static classes before static.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNonStaticClassBeforeStaticAsync()
        {
            var testCode = @"public class TestClass1 { }
public static class TestClass2 { }
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(2, 21).WithArguments("classes")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static elements before static in a class.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOrderingInClassAsync()
        {
            var testCode = @"public class TestClass
{
    public int TestField1;
    public static int TestField2;
    public int TestProperty1 { get; set; }
    public static int TestProperty2 { get; set; }
    public void TestMethod1() { }
    public static void TestMethod2() { }
    
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 23).WithArguments("fields"),
                this.CSharpDiagnostic().WithLocation(6, 23).WithArguments("properties"),
                this.CSharpDiagnostic().WithLocation(8, 24).WithArguments("methods")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle non-static elements before static in a struct.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOrderingInStructAsync()
        {
            var testCode = @"public struct TestStruct
{
    public int TestField1;
    public static int TestField2;
    public int TestProperty1 { get; set; }
    public static int TestProperty2 { get; set; }
    public void TestMethod1() { }
    public static void TestMethod2() { }
    
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 23).WithArguments("fields"),
                this.CSharpDiagnostic().WithLocation(6, 23).WithArguments("properties"),
                this.CSharpDiagnostic().WithLocation(8, 24).WithArguments("methods")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle events.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventOrderingAsync()
        {
            var testCode = @"public class TestClass
{
    event System.Action TestEvent;
    public event System.Action TestEvent2 { add { } remove { } }
    static event System.Action TestEvent3;
    public static event System.Action TestEvent4 { add { } remove { } }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("events")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly incomplete members.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIncompleteMemberAsync()
        {
            string testCode = @"public class TestClass
{
    public string Test;
    public static string
    public static string
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

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1204StaticElementsMustAppearBeforeInstanceElements();
        }
    }
}
