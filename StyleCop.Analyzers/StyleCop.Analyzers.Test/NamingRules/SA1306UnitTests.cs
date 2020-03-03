// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1306FieldNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public class SA1306UnitTests
    {
        [Theory]
        [InlineData("const")]
        [InlineData("private const")]
        [InlineData("internal const")]
        [InlineData("protected const")]
        [InlineData("protected internal const")]
        [InlineData("internal readonly")]
        [InlineData("public const")]
        [InlineData("protected readonly")]
        [InlineData("protected internal readonly")]
        [InlineData("public readonly")]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        [InlineData("public static")]
        [InlineData("internal static")]
        [InlineData("protected internal static")]
        [InlineData("public static readonly")]
        [InlineData("internal static readonly")]
        [InlineData("protected internal static readonly")]
        [InlineData("protected static readonly")]
        [InlineData("private static readonly")]
        public async Task TestThatDiagnosticIsNotReportedAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar = """", car = """", Dar = """";
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForEventFieldsAsync()
        {
            var testCode = @"using System;
public class TypeName
{
    static event EventHandler bar;
    static event EventHandler Bar;
    event EventHandler car;
    event EventHandler Car;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForParametersAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string bar, string Car)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForVariablesAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName()
    {
        const string bar = nameof(bar);
        const string Bar = nameof(Bar);
        string car = nameof(car);
        string Car = nameof(Car);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This test ensures the implementation of <see cref="SA1306FieldNamesMustBeginWithLowerCaseLetter"/> is
        /// correct with respect to the documented behavior for parameters and local variables (including local
        /// constants).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForParametersAndLocalsAsync()
        {
            var testCode = @"public class TypeName
{
    public void MethodName(string Parameter1, string parameter2)
    {
        const int Constant = 1;
        const int constant = 1;
        int Variable;
        int variable;
        int Variable1, Variable2;
        int variable1, variable2;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]
        [InlineData("private readonly")]
        [InlineData("static")]
        [InlineData("private static")]
        [InlineData("protected static")]
        public async Task TestThatDiagnosticIsReported_SingleFieldAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar;
{0}
string car;
{0}
string Dar;
{0}
string _ear;
{0}
string _Far;
{0}
string __gar;
{0}
string __Har;
{0}
string ___iar;
{0}
string ___Jar;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments("Bar").WithLocation(4, 8),
                    Diagnostic().WithArguments("Dar").WithLocation(8, 8),
                    Diagnostic().WithArguments("_Far").WithLocation(12, 8),
                    Diagnostic().WithArguments("__Har").WithLocation(16, 8),
                    Diagnostic().WithArguments("___Jar").WithLocation(20, 8),
                };

            var fixedCode = @"public class Foo
{{
{0}
string bar;
{0}
string car;
{0}
string dar;
{0}
string _ear;
{0}
string _far;
{0}
string __gar;
{0}
string __har;
{0}
string ___iar;
{0}
string ___jar;
}}";

            var test = new CSharpTest
            {
                TestCode = string.Format(testCode, modifiers),
                FixedCode = string.Format(fixedCode, modifiers),
                NumberOfFixAllIterations = 4,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]
        [InlineData("private readonly")]
        [InlineData("static")]
        [InlineData("private static")]
        [InlineData("protected static")]
        public async Task TestThatDiagnosticIsReported_MultipleFieldsAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar, car, Dar;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments("Bar").WithLocation(4, 8),
                    Diagnostic().WithArguments("Dar").WithLocation(4, 18),
                };

            var fixedCode = @"public class Foo
{{
{0}
string bar, car, dar;
}}";

            await VerifyCSharpFixAsync(string.Format(testCode, modifiers), expected, string.Format(fixedCode, modifiers), CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetterAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithAllUnderscoresAsync()
        {
            var testCode = @"public class Foo
{
    private string _ = ""bar"";
    private string __ = ""baz"";
    private string ___ = ""qux"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithTrailingUnderscoreAsync()
        {
            var testCode = @"public class Foo
{
    private string someVar_ = ""bar"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldWithCodefixRenameConflictAsync()
        {
            var testCode = @"public class Foo
{
    private string _test = ""test1"";
    private string _Test = ""test2"";
}";

            var fixedTestCode = @"public class Foo
{
    private string _test = ""test1"";
    private string _test1 = ""test2"";
}";

            var expected = Diagnostic().WithArguments("_Test").WithLocation(4, 20);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    string Bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
