// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Host.Mef;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
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
        protected const string MisnamedDocumentSource = "namespace TestNamespace\r\n{\r\n    public class TestType\r\n    {\r\n    }\r\n}\r\n";

        protected const string MetadataSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": ""metadata""
    }
  }
}
";

        protected const string StyleCopSettings = @"
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
        [MemberData(nameof(CommonMemberData.AllTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType", diagnosticKey: 0)}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType")}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("WrongFileName.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name is correctly reported.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.GenericTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameGenericStyleCopAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" }, diagnosticKey: 0)}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("WrongFileName.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType{T}.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name is correctly reported.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.GenericTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameGenericMetadataAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" }, diagnosticKey: 0)}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("WrongFileName.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`1.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with multiple extensions is correctly reported and fixed. This is a
        /// regression test for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.AllTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameMultipleExtensionsAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType", diagnosticKey: 0)}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType")}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("WrongFileName.svc.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.svc.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a wrong file name with no extension is correctly reported and fixed. This is a regression test
        /// for DotNetAnalyzers/StyleCopAnalyzers#1829.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.AllTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyWrongFileNameNoExtensionAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType", diagnosticKey: 0)}
}}
";

            var fixedCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType")}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("WrongFileName", testCode, StyleCopSettings, expectedDiagnostic, "TestType", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.AllTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyCaseInsensitivityAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetTypeDeclaration(typeKeyword, "TestType")}
}}
";

            await VerifyCSharpDiagnosticAsync("testtype.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.GenericTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyCaseInsensitivityGenericStyleCopAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}
}}
";

            await VerifyCSharpDiagnosticAsync("testtype{t}.cs", testCode, StyleCopSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is not case sensitive.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.GenericTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyCaseInsensitivityGenericMetadataAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}
}}
";

            await VerifyCSharpDiagnosticAsync("testtype`1.cs", testCode, MetadataSettings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetTypeDeclaration(typeKeyword, "TestType", diagnosticKey: 0)}

    {GetTypeDeclaration(typeKeyword, "TestType2")}
}}
";
            var fixedCode = $@"namespace TestNamespace
{{
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetTypeDeclaration(typeKeyword, "TestType")}

    {GetTypeDeclaration(typeKeyword, "TestType2")}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("TestType2.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyFirstTypeIsUsedGenericStyleCopAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" }, diagnosticKey: 0)}

    {GetGenericTypeDeclaration(typeKeyword, "TestType2", new[] { "T" })}
}}
";
            var fixedCode = $@"namespace TestNamespace
{{
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}

    {GetGenericTypeDeclaration(typeKeyword, "TestType2", new[] { "T" })}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("TestType2.cs", testCode, StyleCopSettings, expectedDiagnostic, "TestType{T}.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the file name is based on the first type.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use during the test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.TypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        public async Task VerifyFirstTypeIsUsedGenericMetadataAsync(string typeKeyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" }, diagnosticKey: 0)}

    {GetGenericTypeDeclaration(typeKeyword, "TestType2", new[] { "T" })}
}}
";
            var fixedCode = $@"namespace TestNamespace
{{
    public enum IgnoredEnum {{ }}
    public delegate void IgnoredDelegate();

    {GetGenericTypeDeclaration(typeKeyword, "TestType", new[] { "T" })}

    {GetGenericTypeDeclaration(typeKeyword, "TestType2", new[] { "T" })}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("TestType2.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`1.cs", fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3234, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3234")]
        public async Task VerifyMultipleEnumTypesIgnoredAsync()
        {
            var testCode = $@"namespace TestNamespace
{{
    public enum TestType
    {{
    }}

    public enum TestType2
    {{
    }}
}}
";

            // File names are not checked for 'enum' if more than one is present
            await VerifyCSharpDiagnosticAsync("TestType2.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3234, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3234")]
        public async Task VerifyMultipleDelegateTypesIgnoredAsync()
        {
            var testCode = $@"namespace TestNamespace
{{
    public delegate void TestType();
    public delegate void TestType2();
}}
";

            // File names are not checked for 'delegate' if more than one is present
            await VerifyCSharpDiagnosticAsync("TestType2.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync("WrongFileName.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public {typeKeyword} {{|#0:TestType|}}<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync("TestType.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public {typeKeyword} {{|#0:TestType|}}<T1, T2, T3>
    {{
    }}
}}
";

            var expectedDiagnostic = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync("TestType{T1,T2,T3}.cs", testCode, MetadataSettings, expectedDiagnostic, "TestType`3.cs", testCode, CancellationToken.None).ConfigureAwait(false);

            expectedDiagnostic = Diagnostic().WithLocation(0);
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
    public {typeKeyword} {{|#0:TestType|}}<T>
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

            var expectedDiagnostic = Diagnostic().WithLocation(0);
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

            await VerifyCSharpDiagnosticAsync("Test0.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that no diagnostic is generated if an appropriate SuppressMessageAttribute is provided.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyWithSuppressMessageAttributeAsync()
        {
            var testCode = @"
                            using System.Diagnostics.CodeAnalysis;
                            [SuppressMessage(""StyleCop.CSharp.DocumentationRules"", ""SA1649:FileNameMustMatchTypeName"", Justification = ""Reviewed."")]

                            public class Class2
                            {
                            }
                            ";

            await VerifyCSharpDiagnosticAsync("Class1.cs", testCode, testSettings: null, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1693, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1693")]
        [WorkItem(3866, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3866")]
        public async Task VerifyWithLinkedFileAsync()
        {
            var testCode = "public class [|Type1|] { }";

            await new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest()
            {
                TestState =
                {
                    Sources =
                    {
                        ("0/TestFile.cs", testCode),
                    },
                    AdditionalProjects =
                    {
                        ["Project2"] =
                        {
                            Sources =
                            {
                                ("0/TestFile.cs", testCode),
                            },
                        },
                    },
                },
                FixedState =
                {
                    Sources =
                    {
                        ("0/Type1.cs", testCode),
                    },
                    AdditionalProjects =
                    {
                        ["Project2"] =
                        {
                            Sources =
                            {
                                ("0/Type1.cs", testCode),
                            },
                        },
                    },
                },

                // The test framework's suppression check inserts '#pragma warning disable SA1649' only into the
                // primary project's sources and re-runs the analyzer; additional projects are left unchanged. Since
                // both projects contain a document with the same path (which is what links them), the framework
                // treats the Project2 diagnostic as belonging to a primary-project source file and expects it to be
                // suppressed, but Project2's copy has no pragma, so the diagnostic is still reported. See
                // AnalyzerTest<TVerifier>.VerifySuppressionDiagnosticsAsync and IsInSourceFile in
                // Microsoft.CodeAnalysis.Testing. The other SA1649 tests still cover the suppression check.
                TestBehaviors = TestBehaviors.SkipSuppressionCheck,
            }.RunAsync().ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2277, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2277")]
        public async Task VerifyCodeFixWhenHostCannotRenameDocumentAsync()
        {
            // When the host cannot apply document info changes, the code fix must not rename the document in place, and
            // instead falls back to removing it and adding a new document with the expected name.
            using (var workspace = new NoDocumentInfoChangesWorkspace())
            {
                var (originalId, fixedSolution) = await ApplyCodeFixToMisnamedDocumentAsync(workspace).ConfigureAwait(false);

                var fixedDocument = Assert.Single(Assert.Single(fixedSolution.Projects).Documents);
                Assert.NotEqual(originalId, fixedDocument.Id);
                Assert.Equal("TestType.cs", fixedDocument.Name);
                Assert.Equal(MisnamedDocumentSource, (await fixedDocument.GetTextAsync(CancellationToken.None).ConfigureAwait(false)).ToString());
            }
        }

        protected static async Task<(DocumentId originalId, Solution fixedSolution)> ApplyCodeFixToMisnamedDocumentAsync(Workspace workspace)
        {
            // The code fix asks the workspace of the solution whether it can apply document info changes, so the
            // solution does not need to be applied to the workspace.
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);
            var solution = workspace.CurrentSolution
                .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
                .AddDocument(documentId, "WrongFileName.cs", SourceText.From(MisnamedDocumentSource), filePath: "0/WrongFileName.cs");
            var document = solution.GetDocument(documentId);

            var compilation = await document.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
            var diagnostics = await compilation
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new SA1649FileNameMustMatchTypeName()))
                .GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                .ConfigureAwait(false);
            var diagnostic = Assert.Single(diagnostics, d => d.Id == SA1649FileNameMustMatchTypeName.DiagnosticId);

            var actions = ImmutableArray.CreateBuilder<CodeAction>();
            var context = new CodeFixContext(document, diagnostic, (action, ignored) => actions.Add(action), CancellationToken.None);
            await new SA1649CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

            var operations = await Assert.Single(actions).GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
            var applyChangesOperation = Assert.IsType<ApplyChangesOperation>(Assert.Single(operations));
            return (document.Id, applyChangesOperation.ChangedSolution);
        }

        protected static string GetTypeDeclaration(string typeKind, string typeName, int? diagnosticKey = null)
        {
            if (diagnosticKey is not null)
            {
                typeName = $"{{|#{diagnosticKey}:{typeName}|}}";
            }

            return typeKind switch
            {
                "delegate" => $"public delegate void {typeName}();",
                _ => $"public {typeKind} {typeName} {{ }}",
            };
        }

        protected static string GetGenericTypeDeclaration(string typeKind, string typeName, string[] parameters, int? diagnosticKey = null)
        {
            if (diagnosticKey is not null)
            {
                typeName = $"{{|#{diagnosticKey}:{typeName}|}}";
            }

            return typeKind switch
            {
                "delegate" => $"public delegate void {typeName}<{string.Join(", ", parameters)}>();",
                _ => $"public {typeKind} {typeName}<{string.Join(", ", parameters)}> {{ }}",
            };
        }

        protected static Task VerifyCSharpDiagnosticAsync(string fileName, string source, string testSettings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new StyleCopCodeFixVerifier<SA1649FileNameMustMatchTypeName, SA1649CodeFixProvider>.CSharpTest()
            {
                TestSources = { (fileName, source) },
            };

            if (testSettings != null)
            {
                test.Settings = testSettings;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        protected static Task VerifyCSharpFixAsync(string oldFileName, string source, string testSettings, DiagnosticResult expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(oldFileName, source, testSettings, new[] { expected }, newFileName, fixedSource, cancellationToken);

        protected static Task VerifyCSharpFixAsync(string oldFileName, string source, string testSettings, DiagnosticResult[] expected, string newFileName, string fixedSource, CancellationToken cancellationToken)
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

        /// <summary>
        /// A workspace which, like some hosts, cannot apply document info changes such as renaming a document in place.
        /// </summary>
        private sealed class NoDocumentInfoChangesWorkspace : Workspace
        {
            public NoDocumentInfoChangesWorkspace()
                : base(MefHostServices.DefaultHost, "Test")
            {
            }

            public override bool CanApplyChange(ApplyChangesKind feature)
            {
                // ApplyChangesKind.ChangeDocumentInfo is not available in every Roslyn version this test runs with
                return feature.ToString() != "ChangeDocumentInfo";
            }
        }
    }
}
