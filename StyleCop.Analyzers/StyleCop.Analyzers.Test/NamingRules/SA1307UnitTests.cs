// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.NamingRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1307UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithArguments("bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("dar").WithLocation(8, 8),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string Bar;
{0}
string Car;
{0}
string Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithArguments("bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("dar").WithLocation(4, 18),
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string Bar, Car, Dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1307 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1307AccessibleFieldsMustBeginWithUpperCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToUpperCaseCodeFixProvider();
        }
    }
}
