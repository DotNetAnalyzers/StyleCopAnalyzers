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

    public class SX1309SUnitTests : CodeFixVerifier
    {
        public static IEnumerable<object[]> CheckedModifiersData
        {
            get
            {
                yield return new[] { "static" };
                yield return new[] { "private static" };
            }
        }

        public static IEnumerable<object[]> UncheckedModifiersData
        {
            get
            {
                yield return new[] { "public static" };
                yield return new[] { "protected static" };
                yield return new[] { "internal static" };
                yield return new[] { "protected internal static" };
                yield return new[] { "static readonly" };
                yield return new[] { "public static readonly" };
                yield return new[] { "protected static readonly" };
                yield return new[] { "internal static readonly" };
                yield return new[] { "protected internal static readonly" };
                yield return new[] { "private static readonly" };
                yield return new[] { "const" };
                yield return new[] { "public const" };
                yield return new[] { "protected const" };
                yield return new[] { "internal const" };
                yield return new[] { "protected internal const" };
                yield return new[] { "private const" };
                yield return new[] { string.Empty };
                yield return new[] { "public" };
                yield return new[] { "protected" };
                yield return new[] { "internal" };
                yield return new[] { "protected internal" };
                yield return new[] { "private" };
                yield return new[] { "readonly" };
                yield return new[] { "public readonly" };
                yield return new[] { "protected readonly" };
                yield return new[] { "internal readonly" };
                yield return new[] { "protected internal readonly" };
                yield return new[] { "private readonly" };
            }
        }

        [Theory]
        [MemberData(nameof(CheckedModifiersData))]
        public async Task TestCheckedFieldNotStartingWithAnUnderscoreAsync(string modifiers)
        {
            var testCode = $@"public class ClassName
{{
    {modifiers} string bar = ""bar"";
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("bar").WithLocation(3, 13 + modifiers.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = $@"public class ClassName
{{
    {modifiers} string _bar = ""bar"";
}}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestUncheckedFieldNotStartingWithAnUnderscoreAsync(string modifiers)
        {
            var testCode = $@"public class ClassName
{{
    {modifiers} string bar = ""bar"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CheckedModifiersData))]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestFieldNotStartingWithAnUnderscorePlacedInsideNativeMethodsClassAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethods
{{
    {modifiers} string bar = ""bar"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CheckedModifiersData))]
        public async Task TestCheckedFieldNotStartingWithAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string bar = ""bar"";
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments("bar").WithLocation(3, 13 + modifiers.Length);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string _bar = ""bar"";
}}";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestUncheckedFieldNotStartingWithAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string bar = ""bar"";
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CheckedModifiersData))]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestFieldNotStartingWithAnUnderscorePlacedInsideOuterNativeMethodsClassAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethods
{{
    public class ClassName
    {{
        {modifiers} string bar = ""bar"";
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SX1309SStaticFieldNamesMustBeginWithUnderscore();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SX1309CodeFixProvider();
        }
    }
}
