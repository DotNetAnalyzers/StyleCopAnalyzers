﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.NamingRules.SX1309FieldNamesMustBeginWithUnderscore,
        Analyzers.NamingRules.SX1309CodeFixProvider>;

    public class SX1309UnitTests
    {
        public static IEnumerable<object[]> CheckedModifiersData
        {
            get
            {
                yield return new[] { string.Empty };
                yield return new[] { "private" };
                yield return new[] { "readonly" };
                yield return new[] { "private readonly" };
            }
        }

        public static IEnumerable<object[]> UncheckedModifiersData
        {
            get
            {
                yield return new[] { "static" };
                yield return new[] { "public static" };
                yield return new[] { "protected static" };
                yield return new[] { "internal static" };
                yield return new[] { "protected internal static" };
                yield return new[] { "private static" };
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
                yield return new[] { "public" };
                yield return new[] { "protected" };
                yield return new[] { "internal" };
                yield return new[] { "protected internal" };
                yield return new[] { "public readonly" };
                yield return new[] { "protected readonly" };
                yield return new[] { "internal readonly" };
                yield return new[] { "protected internal readonly" };
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

            DiagnosticResult expected = Diagnostic().WithArguments("bar").WithLocation(3, 13 + modifiers.Length);

            var fixedCode = $@"public class ClassName
{{
    {modifiers} string _bar = ""bar"";
}}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestUncheckedFieldNotStartingWithAnUnderscoreAsync(string modifiers)
        {
            var testCode = $@"public class ClassName
{{
    {modifiers} string bar = ""bar"";
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CheckedModifiersData))]
        public async Task TestCheckedFieldNotStartingWithAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string bar = ""bar"";
}}";

            DiagnosticResult expected = Diagnostic().WithArguments("bar").WithLocation(3, 13 + modifiers.Length);

            var fixedCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string _bar = ""bar"";
}}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(UncheckedModifiersData))]
        public async Task TestUncheckedFieldNotStartingWithAnUnderscorePlacedInsideNativeMethodsClassWithIncorrectNameAsync(string modifiers)
        {
            var testCode = $@"public class ClassNameNativeMethodsClass
{{
    {modifiers} string bar = ""bar"";
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
