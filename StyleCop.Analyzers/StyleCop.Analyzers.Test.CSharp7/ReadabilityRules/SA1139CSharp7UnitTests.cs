// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1139UseLiteralSuffixNotationInsteadOfCasting,
        StyleCop.Analyzers.ReadabilityRules.SA1139CodeFixProvider>;

    public partial class SA1139CSharp7UnitTests : SA1139UnitTests
    {
        /// <summary>
        /// Verifies that using literals with digit separators does not produce diagnostic.
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
        public async Task TestUsingLiteralsWithSeparatorsDoesNotProduceDiagnosticAsync(string literalType, string literalSuffix)
        {
            var testCode = $@"
class ClassName
{{
    {literalType} x = 0_1{literalSuffix};

    public void Method()
    {{
        var x = 1{literalSuffix};
    }}
}}
";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that using casts on literals with digit separators produces diagnostic and correct code fix.
        /// </summary>
        /// <param name="literalType">The type which is checked.</param>
        /// <param name="castedLiteral">The literal that is casted.</param>
        /// <param name="correctLiteral">A literal representing result of a cast.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("long", "0_1", "0_1L")]
        [InlineData("long", "+0_1", "+0_1L")]
        [InlineData("long", "-0_1", "-0_1L")]
        [InlineData("ulong", "0_1", "0_1UL")]
        [InlineData("ulong", "+0_1", "+0_1UL")]
        [InlineData("uint", "0_1", "0_1U")]
        [InlineData("uint", "+0_1", "+0_1U")]
        [InlineData("float", "0_1", "0_1F")]
        [InlineData("float", "+0_1", "+0_1F")]
        [InlineData("float", "-0_1", "-0_1F")]
        [InlineData("float", "0_1.0", "0_1.0F")]
        [InlineData("float", "-0_1e-10", "-0_1e-10F")]
        [InlineData("double", "0_1", "0_1D")]
        [InlineData("double", "+0_1", "+0_1D")]
        [InlineData("double", "-0_1", "-0_1D")]
        [InlineData("decimal", "0_1", "0_1M")]
        [InlineData("decimal", "+0_1", "+0_1M")]
        [InlineData("decimal", "-0_1", "-0_1M")]
        [InlineData("ulong", "0_1L", "0_1UL")]
        [InlineData("ulong", "+0_1L", "+0_1UL")]
        [InlineData("ulong", "0_1l", "0_1UL")]
        [InlineData("ulong", "0_1U", "0_1UL")]
        [InlineData("ulong", "0_1u", "0_1UL")]
        public async Task TestUsingCastsWithSeparatorsProducesDiagnosticAndCorrectCodefixAsync(string literalType, string castedLiteral, string correctLiteral)
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
                Diagnostic().WithLocation(4, 10 + literalType.Length),
                Diagnostic().WithLocation(8, 17),
            };
            await VerifyCSharpFixAsync(testCode, expectedDiagnosticResult, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that redundant cast on literal with digit separators does not trigger diagnostic.
        /// </summary>
        /// <param name="literal">A literal that is casted.</param>
        /// <param name="type">A type that literal is casted on.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("0_1", "int")]
        [InlineData("0_1L", "long")]
        [InlineData("0_1l", "long")]
        [InlineData("0_1UL", "ulong")]
        [InlineData("0_1Ul", "ulong")]
        [InlineData("0_1uL", "ulong")]
        [InlineData("0_1ul", "ulong")]
        [InlineData("0_1LU", "ulong")]
        [InlineData("0_1Lu", "ulong")]
        [InlineData("0_1lU", "ulong")]
        [InlineData("0_1lu", "ulong")]
        [InlineData("0_1U", "uint")]
        [InlineData("0_1u", "uint")]
        [InlineData("0_1F", "float")]
        [InlineData("0_1f", "float")]
        [InlineData("0_1D", "double")]
        [InlineData("0_1d", "double")]
        [InlineData("0_1M", "decimal")]
        [InlineData("0_1m", "decimal")]
        public async Task TestDoNotReportDiagnosticOnRedundantCastWithSeparatorsAsync(string literal, string type)
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that other types of casts should not produces diagnostics.
        /// </summary>
        /// <param name="correctCastExpression">A legal cast that should not trigger diagnostic.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("(long)~0_1")]
        [InlineData("(long)~+0_1")]
        public async Task TestOtherTypesOfCastsWithSeparatorsShouldNotTriggerDiagnosticAsync(string correctCastExpression)
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics is not produced when error CS0221 is reported.
        /// </summary>
        /// <param name="type">A type that a literal is casted on.</param>
        /// <param name="castedLiteral">A literal that is casted.</param>
        /// <param name="literalValue">The value of the literal reported in the compiler error.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("ulong", "-0_1", "-1")]
        public async Task TestCodeTriggeringCs0221WithSeparatorsShouldNotTriggerDiagnosticAsync(string type, string castedLiteral, string literalValue)
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
                DiagnosticResult.CompilerError("CS0221")
                    .WithMessage($"Constant value '{literalValue}' cannot be converted to a '{type}' (use 'unchecked' syntax to override)")
                    .WithLocation(6, 17),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expectedDiagnosticResult, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that casts in unchecked environment do not get replaced with incorrect values.
        /// </summary>
        /// <param name="castExpression">A cast which can be performed in unchecked environment.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("(ulong)-0_1L")]
        [InlineData("(int)1_000_000_000_000_000_000L")]
        [InlineData("(int)0xFFFF_FFFF_FFFF_FFFFL")]
        [InlineData("(uint)0xFFFF_FFFF_FFFF_FFFFL")]
        [InlineData("(int)0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111L")]
        [InlineData("(uint)0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111L")]
        public async Task TestCastsWithSeparatorsInUncheckedEnviromentShouldPreserveValueAsync(string castExpression)
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
