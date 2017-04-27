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
    /// This class contains unit tests for SA1139.
    /// </summary>
    /// <seealso cref="SA1139UseLiteralSuffixNotationInsteadOfCasting" />
    /// <seealso cref="SA1139CodeFixProvider" />
    public class SA1139UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that using literals does not produce diagnostic.
        /// </summary>
        /// <param name="literalType">The type which is checked.</param>
        /// <param name="literalSuffix">The literal's suffix.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("int", "")]
        [InlineData("long", "L")]
        [InlineData("long", "l")]
        [InlineData("ulong", "UL")]
        [InlineData("ulong", "Ul")]
        [InlineData("ulong", "uL")]
        [InlineData("ulong", "ul")]
        [InlineData("ulong", "LU")]
        [InlineData("ulong", "lU")]
        [InlineData("ulong", "Lu")]
        [InlineData("ulong", "lu")]
        [InlineData("uint", "U")]
        [InlineData("uint", "u")]
        [InlineData("float", "F")]
        [InlineData("float", "f")]
        [InlineData("double", "D")]
        [InlineData("double", "d")]
        [InlineData("decimal", "M")]
        [InlineData("decimal", "m")]
        public async Task TestUsingLiteralsDoesNotProduceDiagnosticAsync(string literalType, string literalSuffix)
        {
            var testCode = $@"
class ClassName
{{
    {literalType} x = 1{literalSuffix};

    public void Method()
    {{
        var x = 1{literalSuffix};
    }}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that using casts produces diagnostic and correct code fix.
        /// </summary>
        /// <param name="literalType">The type which is checked.</param>
        /// <param name="castedLiteral">The literal that is casted.</param>
        /// <param name="correctLiteral">A literal representing result of a cast.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("long", "1", "1L")]
        [InlineData("long", "+1", "+1L")]
        [InlineData("long", "-1", "-1L")]
        [InlineData("ulong", "1", "1UL")]
        [InlineData("ulong", "+1", "+1UL")]
        [InlineData("uint", "1", "1U")]
        [InlineData("uint", "+1", "+1U")]
        [InlineData("float", "1", "1F")]
        [InlineData("float", "+1", "+1F")]
        [InlineData("float", "-1", "-1F")]
        [InlineData("float", "1.0", "1.0F")]
        [InlineData("float", "-1e-10", "-1e-10F")]
        [InlineData("double", "1", "1D")]
        [InlineData("double", "+1", "+1D")]
        [InlineData("double", "-1", "-1D")]
        [InlineData("decimal", "1", "1M")]
        [InlineData("decimal", "+1", "+1M")]
        [InlineData("decimal", "-1", "-1M")]
        [InlineData("ulong", "1L", "1UL")]
        [InlineData("ulong", "+1L", "+1UL")]
        [InlineData("ulong", "1l", "1UL")]
        [InlineData("ulong", "1U", "1UL")]
        [InlineData("ulong", "1u", "1UL")]
        public async Task TestUsingCastsProducesDiagnosticAndCorrectCodefixAsync(string literalType, string castedLiteral, string correctLiteral)
        {
            var testCode = $@"
class ClassName
{{
    {literalType} x = ({literalType}){castedLiteral};

    public object Method()
    {{
        var y = ({literalType}){castedLiteral};
        return y;
    }}
}}
";

            var fixedCode = $@"
class ClassName
{{
    {literalType} x = {correctLiteral};

    public object Method()
    {{
        var y = {correctLiteral};
        return y;
    }}
}}
";
            DiagnosticResult[] expectedDiagnosticResult =
            {
                this.CSharpDiagnostic().WithLocation(4, 10 + literalType.Length),
                this.CSharpDiagnostic().WithLocation(8, 17),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnosticResult, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that redundant cast does not trigger diagnostic.
        /// </summary>
        /// <param name="literal">A literal that is casted</param>
        /// <param name="type">A type that literal is casted on</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("1", "int")]
        [InlineData("1L", "long")]
        [InlineData("1l", "long")]
        [InlineData("1UL", "ulong")]
        [InlineData("1Ul", "ulong")]
        [InlineData("1uL", "ulong")]
        [InlineData("1ul", "ulong")]
        [InlineData("1LU", "ulong")]
        [InlineData("1Lu", "ulong")]
        [InlineData("1lU", "ulong")]
        [InlineData("1lu", "ulong")]
        [InlineData("1U", "uint")]
        [InlineData("1u", "uint")]
        [InlineData("1F", "float")]
        [InlineData("1f", "float")]
        [InlineData("1D", "double")]
        [InlineData("1d", "double")]
        [InlineData("1M", "decimal")]
        [InlineData("1m", "decimal")]
        public async Task TestDoNotRaportDiagnositcOnRedundantCastAsync(string literal, string type)
        {
            var testCode = $@"
class ClassName
{{
    public void Method()
    {{
        var x = ({type}){literal};
    }}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that other types of casts should not produces diagnostics.
        /// </summary>
        /// <param name="correctCastExpression">A legal cast that should not trigger diagnostic</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("(long)~1")]
        [InlineData("(bool)true")]
        [InlineData("(bool)(false)")]
        [InlineData("(long)~+1")]
        public async Task TestOtherTypesOfCastsShouldNotTriggerDiagnosticAsync(string correctCastExpression)
        {
            var testCode = $@"
class ClassName
{{
    public void Method()
    {{
        var x = {correctCastExpression};
    }}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics is not produced when error CS0221 is reported.
        /// </summary>
        /// <param name="type">A type that a literal is casted on</param>
        /// <param name="castedLiteral">A literal that is casted</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("ulong", "-1")]
        public async Task TestCodeTriggeringCs0221ShouldNotTriggerDiagnosticAsync(string type, string castedLiteral)
        {
            var testCode = $@"
class ClassName
{{
    public void Method()
    {{
        var x = ({type}){castedLiteral};
    }}
}}
";

            DiagnosticResult[] expectedDiagnosticResult =
            {
                this.CSharpCompilerError("CS0221")
                    .WithMessage($"Constant value '{castedLiteral}' cannot be converted to a '{type}' (use 'unchecked' syntax to override)")
                    .WithLocation(6, 17),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnosticResult, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that using casts in unchecked enviroment produces diagnostics with a correct codefix.
        /// </summary>
        /// <param name="castExpression">A cast which can be performend in unchecked enviroment</param>
        /// <param name="correctLiteral">The corresponding literal with suffix</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("(ulong)-1L", "18446744073709551615UL")]
        [InlineData("(int)1000000000000000000L", "-1486618624")]
        [InlineData("(int)0xFFFFFFFFFFFFFFFFL", "-1")]
        [InlineData("(uint)0xFFFFFFFFFFFFFFFFL", "4294967295U")]
        public async Task TestCastsInUncheckedEnviromentShouldPreserveValueAsync(string castExpression, string correctLiteral)
        {
            var testCode = $@"
class ClassName
{{
    public void Method()
    {{
        unchecked
        {{
            var x = {castExpression};
        }}
        var y = unchecked({castExpression});
    }}
}}
";
            var fixedCode = $@"
class ClassName
{{
    public void Method()
    {{
        unchecked
        {{
            var x = {correctLiteral};
        }}
        var y = unchecked({correctLiteral});
    }}
}}
";
            DiagnosticResult[] expectedDiagnosticResult =
            {
                this.CSharpDiagnostic().WithLocation(8, 21),
                this.CSharpDiagnostic().WithLocation(10, 27),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnosticResult, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1139CodeFixProvider();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1139UseLiteralSuffixNotationInsteadOfCasting();
        }
    }
}
