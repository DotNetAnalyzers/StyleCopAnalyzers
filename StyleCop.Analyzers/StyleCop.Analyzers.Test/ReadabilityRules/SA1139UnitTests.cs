// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1139UseLiteralSuffixNotationInsteadOfCasting,
        StyleCop.Analyzers.ReadabilityRules.SA1139CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for SA1139.
    /// </summary>
    /// <seealso cref="SA1139UseLiteralSuffixNotationInsteadOfCasting" />
    /// <seealso cref="SA1139CodeFixProvider" />
    public class SA1139UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        [InlineData("long", "(+1)", "(+1L)")]
        [InlineData("long", "(-1)", "(-1L)")]
        [InlineData("long", "(1)", "(1L)")]
        [InlineData("long", "(-(1))", "(-(1L))")]
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
        [InlineData("long", "0xF", "0xFL")]
        [InlineData("long", "0x1", "0x1L")]
        [InlineData("ulong", "0x1", "0x1UL")]
        [InlineData("long", "-0x1", "-0x1L")]
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
                Diagnostic().WithLocation(4, 10 + literalType.Length),
                Diagnostic().WithLocation(8, 17),
            };
            await VerifyCSharpFixAsync(testCode, expectedDiagnosticResult, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that redundant cast does not trigger diagnostic.
        /// </summary>
        /// <param name="literal">A literal that is casted.</param>
        /// <param name="type">A type that literal is casted on.</param>
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that other types of casts should not produces diagnostics.
        /// </summary>
        /// <param name="correctCastExpression">A legal cast that should not trigger diagnostic.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("(long)~1")]
        [InlineData("(long)(~1)")]
        [InlineData("(bool)true")]
        [InlineData("(bool)(false)")]
        [InlineData("(long)~+1")]
        [InlineData("(long)(~+1)")]
        [InlineData("unchecked((int)0x80000000)")]
        [InlineData("unchecked((int)0xFFFFFFFF)")]
        [InlineData("unchecked((ulong)-1)")]
        [InlineData("unchecked((byte)-1)")]
        [InlineData("(sbyte)-1")]
        [InlineData("(int)-1")]
        [InlineData("unchecked((ulong)-1L)")]
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics is not produced when error CS0221 is reported.
        /// </summary>
        /// <param name="type">A type that a literal is casted on.</param>
        /// <param name="castedLiteral">A literal that is casted.</param>
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
                CompilerError("CS0221")
                    .WithMessage($"Constant value '{castedLiteral}' cannot be converted to a '{type}' (use 'unchecked' syntax to override)")
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
        [InlineData("(ulong)-1L")]
        [InlineData("(int)1000000000000000000L")]
        [InlineData("(int)0xFFFFFFFFFFFFFFFFL")]
        [InlineData("(uint)0xFFFFFFFFFFFFFFFFL")]
        public async Task TestCastsInUncheckedEnviromentShouldPreserveValueAsync(string castExpression)
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
