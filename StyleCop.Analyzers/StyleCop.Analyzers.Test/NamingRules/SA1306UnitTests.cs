// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;

    public class SA1306UnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments("Bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("Dar").WithLocation(8, 8)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string bar;
{0}
string car;
{0}
string dar;
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(fixedCode, modifiers), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithArguments("Bar").WithLocation(4, 8),
                    this.CSharpDiagnostic().WithArguments("Dar").WithLocation(4, 18)
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, modifiers), expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{{
{0}
string bar, car, dar;
}}";

            await this.VerifyCSharpFixAsync(string.Format(testCode, modifiers), string.Format(fixedCode, modifiers)).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithAnUnderscoreAsync()
        {
            // Makes sure SA1306 is not reported for fields starting with an underscore
            var testCode = @"public class Foo
{
    public string _bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldStartingWithLetterAsync()
        {
            var testCode = @"public class Foo
{
    public string bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldPlacedInsideNativeMethodsClassAsync()
        {
            var testCode = @"public class FooNativeMethods
{
    string Bar = ""baz"";
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1306FieldNamesMustBeginWithLowerCaseLetter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToLowerCaseCodeFixProvider();
        }
    }
}
