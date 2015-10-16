// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
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
        /// Verifies that a wrong file name is correctly reported
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TypeKeywords))]
        public async Task VerifyWrongFileNameAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNameSpace
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
            await this.VerifyRenameAsync(testCode, "TestType.cs", CancellationToken.None).ConfigureAwait(false);
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
            var testCode = $@"namespace TestNameSpace
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
            var testCode = $@"namespace TestNameSpace
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
            var testCode = $@"namespace TestNameSpace
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

            var testCode = $@"namespace TestNameSpace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType`3.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType`3.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType{T1,T2,T3}.cs").ConfigureAwait(false);
            await this.VerifyRenameAsync(testCode, "TestType{T1,T2,T3}.cs", CancellationToken.None).ConfigureAwait(false);
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

            var testCode = $@"namespace TestNameSpace
{{
    public {typeKeyword} TestType<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation("TestType{T1,T2,T3}.cs", 3, 13 + typeKeyword.Length);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None, "TestType{T1,T2,T3}.cs").ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None, "TestType`3.cs").ConfigureAwait(false);
            await this.VerifyRenameAsync(testCode, "TestType`3.cs", CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that no diagnostic is generated if there is no first type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyWithoutFirstTypeAsync()
        {
            var testCode = @"namespace TestNameSpace
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

        private async Task VerifyRenameAsync(string source, string expectedFileName, CancellationToken cancellationToken)
        {
            var analyzers = this.GetCSharpDiagnosticAnalyzers().ToImmutableArray();
            var document = this.CreateDocument(source, LanguageNames.CSharp);
            var analyzerDiagnostics = await GetSortedDiagnosticsFromDocumentsAsync(analyzers, new[] { document }, cancellationToken).ConfigureAwait(false);

            Assert.Equal(1, analyzerDiagnostics.Length);

            var actions = new List<CodeAction>();
            var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, d) => actions.Add(a), cancellationToken);
            await this.GetCSharpCodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

            Assert.Equal(1, actions.Count);

            var operations = await actions[0].GetOperationsAsync(cancellationToken).ConfigureAwait(false);

            var changedSolution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;

            var solutionChanges = changedSolution.GetChanges(document.Project.Solution);
            var projectChanges = solutionChanges.GetProjectChanges().ToArray();

            Assert.Equal(1, projectChanges.Length);

            var removedDocuments = projectChanges[0].GetRemovedDocuments().ToArray();
            Assert.Equal(1, removedDocuments.Length);
            Assert.Equal(document.Id, removedDocuments[0]);

            var addedDocuments = projectChanges[0].GetAddedDocuments().ToArray();
            Assert.Equal(1, addedDocuments.Length);

            var newDocument = changedSolution.GetDocument(addedDocuments[0]);
            Assert.Equal(expectedFileName, newDocument.Name);
        }
    }
}
