// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1307AccessibleFieldsMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public class SA1307UnitTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("readonly")]
        [InlineData("private")]

        [InlineData("const")]
        [InlineData("private const")]
        [InlineData("protected const")]
        [InlineData("protected internal const")]

        [InlineData("private readonly")]
        [InlineData("protected readonly")]
        public async Task TestThatDiagnosticIsNotReportedAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string Bar = """", car = """", Dar = """";
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]

        [InlineData("public readonly")]
        [InlineData("protected internal readonly")]
        public async Task TestThatDiagnosticIsReported_SingleFieldAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar;
{0}
string Car;
{0}
string dar;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments("bar").WithLocation(4, 8),
                    Diagnostic().WithArguments("dar").WithLocation(8, 8),
                };

            var fixedCode = @"public class Foo
{{
{0}
string Bar;
{0}
string Car;
{0}
string Dar;
}}";

            await VerifyCSharpFixAsync(string.Format(testCode, modifiers), expected, string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        public async Task TestThatDiagnosticIsReported_MultipleFieldsAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar, Car, dar;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments("bar").WithLocation(4, 8),
                    Diagnostic().WithArguments("dar").WithLocation(4, 18),
                };

            var fixedCode = @"public class Foo
{{
{0}
string Bar, Car, Dar;
}}";

            await VerifyCSharpFixAsync(string.Format(testCode, modifiers), expected, string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("protected internal")]
        public async Task TestThatDiagnosticIsReported_MultipleFieldsWithConflictAsync(string modifiers)
        {
            var testCode = @"public class Foo
{{
{0}
string bar, Bar, barValue;
{0}
string carValue, Car, car;
}}";
            var fixedCode = @"public class Foo
{{
{0}
string BarValue, Bar, BarValueValue;
{0}
string CarValue, Car, Car1;
}}";
            var batchFixedCode = @"public class Foo
{{
{0}
string BarValue, Bar, BarValueValue;
{0}
string CarValueValue, Car, CarValue;
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments("bar").WithLocation(4, 8),
                    Diagnostic().WithArguments("barValue").WithLocation(4, 18),
                    Diagnostic().WithArguments("carValue").WithLocation(6, 8),
                    Diagnostic().WithArguments("car").WithLocation(6, 23),
                };

            var test = new CSharpTest
            {
                TestCode = string.Format(testCode, modifiers),
                FixedCode = string.Format(fixedCode, modifiers),
                BatchFixedCode = string.Format(batchFixedCode, modifiers),
                NumberOfFixAllIterations = 2,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1307 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public string bar = ""baz"";
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
