// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1649 diagnostic.
    /// </summary>
    public class SA1649UnitTests : CodeFixVerifier
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

        private bool useMetadataSettings;

        public static IEnumerable<object[]> TypeKeywords
        {
            get
            {
                yield return new object[] { "class" };
                yield return new object[] { "struct" };
                yield return new object[] { "interface" };
            }
        }

        /// <summary>
        /// Verifies that a wrong file name is correctly reported.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("WrongFileName.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "WrongFileName.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None, "TestType.cs").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, oldFileName: "WrongFileName.cs", newFileName: "TestType.cs", cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("WrongFileName.svc.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "WrongFileName.svc.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None, "TestType.svc.cs").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, oldFileName: "WrongFileName.svc.cs", newFileName: "TestType.svc.cs", cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with no extension is correctly reported and fixed. This is a regression test
        /// for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("WrongFileName", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "WrongFileName").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None, "TestType").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, oldFileName: "WrongFileName", newFileName: "TestType", cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyCaseInsensitivityAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType
    {{
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "testtype.cs").ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType.cs").ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that partial types are ignored.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyThatPartialTypesAreIgnoredAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public partial {typeKeyword} TestType
    {{
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "WrongFileName.cs").ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the StyleCop file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyStyleCopNamingConventionForGenericTypeAsync(string typeKeyword)
        {
            this.useMetadataSettings = false;

            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType`3.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType`3.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType{T1,T2,T3}.cs").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode, oldFileName: "TestType`3.cs", newFileName: "TestType{T1,T2,T3}.cs", cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the metadata file name convention for a generic type is handled correctly.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyMetadataNamingConventionForGenericTypeAsync(string typeKeyword)
        {
            this.useMetadataSettings = true;

            var testCode = $@"namespace TestNamespace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType{T1,T2,T3}.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType{T1,T2,T3}.cs").ConfigureAwait(false);

            expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType`3.cs").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, testCode, oldFileName: "TestType.cs", newFileName: "TestType`3.cs", cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong metadata file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyMetadataNamingConventionForGenericTypeMultipleExtensionsAsync(string typeKeyword)
        {
            this.useMetadataSettings = true;

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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType.svc.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType.svc.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None, "TestType`1.svc.cs").ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, oldFileName: "TestType.svc.cs", newFileName: "TestType`1.svc.cs", cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1649FileNameMustMatchTypeName();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1649CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return this.useMetadataSettings ? MetadataSettings : StyleCopSettings;
        }
    }
}
