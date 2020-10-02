// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1649FileNameMustMatchTypeName>;

    /// <summary>
    /// Unit tests for the SA1649 diagnostic.
    /// </summary>
    public class SA1649UnitTests
    {
        private const string MetadataSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": ""metadata""
    }
  }
}
";

        private const string StyleCopSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": ""stylecop""
    }
  }
}
";

        public static IEnumerable<object[]> TypeKeywords
        {
            get
            {
                yield return new object[] { "class", LanguageVersion.CSharp6 };
                yield return new object[] { "struct", LanguageVersion.CSharp6 };
                yield return new object[] { "interface", LanguageVersion.CSharp6 };
            }
        }

        /// <summary>
        /// Verifies that a wrong file name is correctly reported.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyWrongFileNameAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("WrongFileName.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "WrongFileName.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyWrongFileNameMultipleExtensionsAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("WrongFileName.svc.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "WrongFileName.svc.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.svc.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with no extension is correctly reported and fixed. This is a regression test
        /// for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyWrongFileNameNoExtensionAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("WrongFileName", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "WrongFileName", testCode, StyleCopSettings, expectedDiagnostic, "TestType", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyCaseInsensitivityAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(languageVersion, "testtype.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyFirstTypeIsUsedAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}

    public {typeKeyword} TestType2
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(languageVersion, "TestType.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that partial types are ignored.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyThatPartialTypesAreIgnoredAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public partial {typeKeyword} TestType
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(languageVersion, "WrongFileName.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the StyleCop file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyStyleCopNamingConventionForGenericTypeAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("TestType`3.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpDiagnosticAsync(languageVersion, "TestType.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpFixAsync(languageVersion, "TestType`3.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType{T1,T2,T3}.cs", testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the metadata file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyMetadataNamingConventionForGenericTypeAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("TestType{T1,T2,T3}.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "TestType{T1,T2,T3}.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`3.cs", testCode, CancellationToken.None).ConfigureAwait(false);

            expectedDiagnostic = Diagnostic().WithLocation("TestType.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "TestType.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`3.cs", testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong metadata file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <param name="languageVersion">The language version to test with.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public virtual async Task VerifyMetadataNamingConventionForGenericTypeMultipleExtensionsAsync(string typeKeyword, LanguageVersion languageVersion)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T>
    {{
    }}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("TestType.svc.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync(languageVersion, "TestType.svc.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`1.svc.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that no diagnostic is generated if there is no first type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyWithoutFirstTypeAsync()
        {
            var testCode = @"namespace TestNamespace
{
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp6, "Test0.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion languageVersion, string fileName, string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest(languageVersion)
            {
                TestSources = { (fileName, source) },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal static Task VerifyCSharpFixAsync(LanguageVersion languageVersion, string oldFileName, string source, string testSettings, DiagnosticResult expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(languageVersion, oldFileName, source, testSettings, new[] { expected }, newFileName, fixedSource, cancellationToken);

        internal static Task VerifyCSharpFixAsync(LanguageVersion languageVersion, string oldFileName, string source, string testSettings, DiagnosticResult[] expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest(languageVersion)
            {
                TestSources = { (oldFileName, source) },
                FixedSources = { (newFileName, fixedSource) },
            };

            if (testSettings != null)
            {
                test.Settings = testSettings;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
