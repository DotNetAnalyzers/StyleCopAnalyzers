// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
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

        /// <summary>
        /// Verifies that a wrong file name is correctly reported.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameAsync(string typeKeyword)
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
            await VerifyCSharpFixAsync("WrongFileName.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameMultipleExtensionsAsync(string typeKeyword)
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
            await VerifyCSharpFixAsync("WrongFileName.svc.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.svc.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with no extension is correctly reported and fixed. This is a regression test
        /// for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameNoExtensionAsync(string typeKeyword)
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
            await VerifyCSharpFixAsync("WrongFileName", testCode, StyleCopSettings, expectedDiagnostic, "TestType", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyCaseInsensitivityAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync("testtype.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyFirstTypeIsUsedAsync(string typeKeyword)
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

            await VerifyCSharpDiagnosticAsync("TestType.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that partial types are ignored.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyThatPartialTypesAreIgnoredAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public partial {typeKeyword} TestType
    {{
    }}
}}
";

            await VerifyCSharpDiagnosticAsync("WrongFileName.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the StyleCop file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyStyleCopNamingConventionForGenericTypeAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("TestType`3.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpDiagnosticAsync("TestType.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpFixAsync("TestType`3.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType{T1,T2,T3}.cs", testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the metadata file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyMetadataNamingConventionForGenericTypeAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation("TestType{T1,T2,T3}.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync("TestType{T1,T2,T3}.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`3.cs", testCode, CancellationToken.None).ConfigureAwait(false);

            expectedDiagnostic = Diagnostic().WithLocation("TestType.cs", 3, 13 + typeKeyword.Length);
            await VerifyCSharpFixAsync("TestType.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`3.cs", testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong metadata file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyMetadataNamingConventionForGenericTypeMultipleExtensionsAsync(string typeKeyword)
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
            await VerifyCSharpFixAsync("TestType.svc.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`1.svc.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync("Test0.cs", testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string fileName, string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { (fileName, source) },
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private static Task VerifyCSharpFixAsync(string oldFileName, string source, string testSettings, DiagnosticResult expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(oldFileName, source, testSettings, new[] { expected }, newFileName, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string oldFileName, string source, string testSettings, DiagnosticResult[] expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest()
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
