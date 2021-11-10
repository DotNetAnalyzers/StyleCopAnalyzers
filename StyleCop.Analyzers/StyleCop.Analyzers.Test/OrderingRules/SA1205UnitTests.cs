// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1205PartialElementsMustDeclareAccess,
        StyleCop.Analyzers.OrderingRules.SA1205CodeFixProvider>;

    /// <summary>
    /// Unit tests for the <see cref="SA1205PartialElementsMustDeclareAccess"/> class.
    /// </summary>
    public class SA1205UnitTests
    {
        private const string TestCodeTemplate = @"$$ Foo
{
}
";

        private const string FixedTestCodeTemplate = @"## $$ Foo
{
}
";

        public static IEnumerable<object[]> ValidDeclarations
        {
            get
            {
                yield return new object[] { "public partial class" };
                yield return new object[] { "internal partial class" };
                yield return new object[] { "public static partial class" };
                yield return new object[] { "internal static partial class" };
                yield return new object[] { "public sealed partial class" };
                yield return new object[] { "internal sealed partial class" };
                yield return new object[] { "public partial struct" };
                yield return new object[] { "internal partial struct" };
                yield return new object[] { "public partial interface" };
                yield return new object[] { "internal partial interface" };
                yield return new object[] { "class" };
                yield return new object[] { "struct" };
                yield return new object[] { "interface" };
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new object[] { "public partial record" };
                    yield return new object[] { "internal partial record" };
                    yield return new object[] { "public sealed partial record" };
                    yield return new object[] { "internal sealed partial record" };
                    yield return new object[] { "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new object[] { "public partial record class" };
                    yield return new object[] { "internal partial record class" };
                    yield return new object[] { "public sealed partial record class" };
                    yield return new object[] { "internal sealed partial record class" };
                    yield return new object[] { "record class" };

                    yield return new object[] { "public partial record struct" };
                    yield return new object[] { "internal partial record struct" };
                    yield return new object[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> InvalidDeclarations
        {
            get
            {
                yield return new object[] { "partial class" };
                yield return new object[] { "sealed partial class" };
                yield return new object[] { "static partial class" };
                yield return new object[] { "partial struct" };
                yield return new object[] { "partial interface" };
                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new object[] { "partial record" };
                    yield return new object[] { "sealed partial record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new object[] { "partial record class" };
                    yield return new object[] { "sealed partial record class" };

                    yield return new object[] { "partial record struct" };
                }
            }
        }

        public static IEnumerable<object[]> ValidNestedDeclarations
        {
            get
            {
                yield return new object[] { "public", "class" };
                yield return new object[] { "protected", "class" };
                yield return new object[] { "internal", "class" };
                yield return new object[] { "protected internal", "class" };
                yield return new object[] { "private", "class" };

                yield return new object[] { "public", "struct" };
                yield return new object[] { "protected", "struct" };
                yield return new object[] { "internal", "struct" };
                yield return new object[] { "protected internal", "struct" };
                yield return new object[] { "private", "struct" };

                yield return new object[] { "public", "interface" };
                yield return new object[] { "protected", "interface" };
                yield return new object[] { "internal", "interface" };
                yield return new object[] { "protected internal", "interface" };
                yield return new object[] { "private", "interface" };

                if (LightupHelpers.SupportsCSharp72)
                {
                    yield return new object[] { "private protected", "class" };
                    yield return new object[] { "private protected", "struct" };
                    yield return new object[] { "private protected", "interface" };
                }

                if (LightupHelpers.SupportsCSharp9)
                {
                    yield return new object[] { "public", "record" };
                    yield return new object[] { "protected", "record" };
                    yield return new object[] { "internal", "record" };
                    yield return new object[] { "protected internal", "record" };
                    yield return new object[] { "private", "record" };
                    yield return new object[] { "private protected", "record" };
                }

                if (LightupHelpers.SupportsCSharp10)
                {
                    yield return new object[] { "public", "record class" };
                    yield return new object[] { "protected", "record class" };
                    yield return new object[] { "internal", "record class" };
                    yield return new object[] { "protected internal", "record class" };
                    yield return new object[] { "private", "record class" };
                    yield return new object[] { "private protected", "record class" };

                    yield return new object[] { "public", "record struct" };
                    yield return new object[] { "protected", "record struct" };
                    yield return new object[] { "internal", "record struct" };
                    yield return new object[] { "protected internal", "record struct" };
                    yield return new object[] { "private", "record struct" };
                    yield return new object[] { "private protected", "record struct" };
                }
            }
        }

        /// <summary>
        /// Verifies that a valid declaration (with an access modifier or not a partial type) will not produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(ValidDeclarations))]
        public async Task TestValidDeclarationAsync(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid type declaration will produce a diagnostic.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(InvalidDeclarations))]
        public async Task TestInvalidDeclarationAsync(string declaration)
        {
            var testCode = TestCodeTemplate.Replace("$$", declaration);
            var fixedTestCode = FixedTestCodeTemplate.Replace("##", "internal").Replace("$$", declaration);

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(1, 2 + declaration.Length), fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly copy over the access modifier defined in another fragment of the partial element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestProperAccessModifierPropagationAsync()
        {
            var testCode = @"public partial class Foo
{
    private int field1;
}

partial class Foo
{
    private int field2;
}
";

            var fixedTestCode = @"public partial class Foo
{
    private int field1;
}

public partial class Foo
{
    private int field2;
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(6, 15), fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly copy over the access modifier defined in another fragment of the partial element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixWithXmlDocumentationAsync()
        {
            var testCode = @"public partial class Foo
{
    private int field1;
}

/// <summary>
/// This is a summary
/// </summary>
partial class Foo
{
    private int field2;
}
";

            var fixedTestCode = @"public partial class Foo
{
    private int field1;
}

/// <summary>
/// This is a summary
/// </summary>
public partial class Foo
{
    private int field2;
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(9, 15), fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that all 5 access modifiers are accepted for nested types.
        /// This is a regression test for issue #2040.
        /// </summary>
        /// <param name="accessModifier">The access modifier to use for the nested type.</param>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(ValidNestedDeclarations))]
        public async Task TestNestedTypeAccessModifiersAsync(string accessModifier, string typeKeyword)
        {
            var testCode = $@"
internal static partial class TestPartial
{{
    {accessModifier} partial {typeKeyword} PartialInner
    {{
    }}
}}
";

            var languageVersion = (LightupHelpers.SupportsCSharp8, LightupHelpers.SupportsCSharp72) switch
            {
                // Make sure to use C# 7.2 if supported, unless we are going to default to something greater
                (false, true) => LanguageVersionEx.CSharp7_2,
                _ => (LanguageVersion?)null,
            };

            await VerifyCSharpDiagnosticAsync(languageVersion, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a nested type without access modifiers will produce a diagnostic and can be fixed correctly.
        /// </summary>
        /// <param name="declaration">The declaration to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(InvalidDeclarations))]
        public async Task TestNestedTypeWithoutAccessModifierAsync(string declaration)
        {
            var testCode = $@"
public class Foo
{{
    {declaration} Bar
    {{
    }}
}}
";

            var fixedTestCode = $@"
public class Foo
{{
    private {declaration} Bar
    {{
    }}
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(4, 6 + declaration.Length), fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly copy over the access modifier defined in another fragment of the nested partial element.
        /// </summary>
        /// <param name="accessModifier">The access modifier to use for the nested type.</param>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(ValidNestedDeclarations))]
        public async Task TestProperNestedAccessModifierPropagationAsync(string accessModifier, string typeKeyword)
        {
            var testCode = $@"
public class Foo
{{
    {accessModifier} partial {typeKeyword} Bar
    {{
    }}

    partial {typeKeyword} Bar
    {{
    }}
}}
";

            var fixedTestCode = $@"
public class Foo
{{
    {accessModifier} partial {typeKeyword} Bar
    {{
    }}

    {accessModifier} partial {typeKeyword} Bar
    {{
    }}
}}
";

            var languageVersion = (LightupHelpers.SupportsCSharp8, LightupHelpers.SupportsCSharp72) switch
            {
                // Make sure to use C# 7.2 if supported, unless we are going to default to something greater
                (false, true) => LanguageVersionEx.CSharp7_2,
                _ => (LanguageVersion?)null,
            };

            await VerifyCSharpFixAsync(languageVersion, testCode, Diagnostic().WithLocation(8, 14 + typeKeyword.Length), fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
