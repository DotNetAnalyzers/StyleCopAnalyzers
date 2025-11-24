// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp8.Settings
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.CSharp7.Settings;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public partial class SettingsCSharp8UnitTests : SettingsCSharp7UnitTests
    {
        private const string TestProjectName = "TestProject";

        [Fact]
        public async Task VerifyEditorConfigSettingsAreReadCorrectlyAsync()
        {
            var settings = @"root = true

[*]
file_header_template = Custom copyright text.
insert_final_newline = true
csharp_using_directive_placement = outside_namespace:none
dotnet_separate_import_directive_groups = false

stylecop.documentation.companyName = TestCompany
stylecop.documentation.documentationCulture = ru-RU
stylecop.documentation.unrecognizedValue = 3

stylecop.naming.allowCommonHungarianPrefixes = false
stylecop.naming.allowedHungarianPrefixes = a, ab, ignoredTooLong
stylecop.naming.allowedNamespaceComponents = eBay, iMac
stylecop.naming.unrecognizedValue = 3

stylecop.layout.unrecognizedValue = 3

stylecop.ordering.unrecognizedValue = 3

stylecop.maintainability.unrecognizedValue = 3

stylecop.indentation.unrecognizedValue = 3

stylecop.unrecognizedValue = 3
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Custom copyright text.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
            Assert.Equal("ru-RU", styleCopSettings.DocumentationRules.DocumentationCulture);
            Assert.False(styleCopSettings.NamingRules.AllowCommonHungarianPrefixes);
            Assert.Equal(new[] { "a", "ab" }, styleCopSettings.NamingRules.AllowedHungarianPrefixes);
            Assert.Equal(new[] { "eBay", "iMac" }, styleCopSettings.NamingRules.AllowedNamespaceComponents);

            Assert.NotNull(styleCopSettings.LayoutRules);
            Assert.Equal(OptionSetting.Require, styleCopSettings.LayoutRules.NewlineAtEndOfFile);

            Assert.NotNull(styleCopSettings.OrderingRules);
            Assert.Equal(UsingDirectivesPlacement.OutsideNamespace, styleCopSettings.OrderingRules.UsingDirectivesPlacement);
            Assert.Equal(OptionSetting.Allow, styleCopSettings.OrderingRules.BlankLinesBetweenUsingGroups);
        }

        [Fact]
        public async Task VerifyFileHeaderTemplateFromEditorConfigAsync()
        {
            var settings = @"root = true

[*]
file_header_template = Line 1\nLine 2.
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("Line 1\nLine 2.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Fact]
        public async Task VerifyStyleCopDocumentationCopyrightTextFromEditorConfigAsync()
        {
            var settings = @"root = true

[*]
stylecop.documentation.copyrightText = Line 1\nLine 2.
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("Line 1\nLine 2.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Theory]
        [CombinatorialData]
        public async Task VerifyBooleanDocumentationSettingsFromEditorConfigAsync(bool value)
        {
            string valueText = value.ToString().ToLowerInvariant();
            var settings = $@"root = true

[*]
stylecop.documentation.documentExposedElements = {valueText}
stylecop.documentation.documentInternalElements = {valueText}
stylecop.documentation.documentPrivateElements = {valueText}
stylecop.documentation.documentInterfaces = {valueText}
stylecop.documentation.documentPrivateFields = {valueText}
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentExposedElements);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentInternalElements);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentPrivateElements);
            Assert.Equal(value ? InterfaceDocumentationMode.All : InterfaceDocumentationMode.None, styleCopSettings.DocumentationRules.DocumentInterfaces);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentPrivateFields);
        }

        [Theory]
        [InlineData("TestCompany")]
        [InlineData("TestCompany2")]
        public async Task VerifySettingsWillUseCompanyNameInDefaultCopyrightTextFromEditorConfigAsync(string companyName)
        {
            var settings = $@"root = true

[*]
stylecop.documentation.companyName = {companyName}
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal(companyName, styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal($"Copyright (c) {companyName}. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Fact]
        public async Task VerifyCircularReferenceBehaviorFromEditorConfigAsync()
        {
            var settings = @"root = true

[*]
file_header_template = {copyrightText}
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("[CircularReference]", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Fact]
        public async Task VerifyInvalidReferenceBehaviorFromEditorConfigAsync()
        {
            var settings = @"root = true

[*]
file_header_template = {variable}
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("[InvalidReference]", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Theory]
        [InlineData("outside_namespace")]
        [InlineData("inside_namespace")]
        public async Task VerifyEditorConfigSettingsReadCorrectlyDirectivePlacementWithoutSeverityLevelAsync(string placement)
        {
            var expected = placement switch
            {
                "outside_namespace" => UsingDirectivesPlacement.OutsideNamespace,
                "inside_namespace" => UsingDirectivesPlacement.InsideNamespace,
                _ => throw new InvalidOperationException(),
            };
            var settings = $@"root = true

[*]
csharp_using_directive_placement = {placement}
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.NotNull(styleCopSettings.OrderingRules);
            Assert.Equal(expected, styleCopSettings.OrderingRules.UsingDirectivesPlacement);
        }

        protected virtual AnalyzerConfigOptionsProvider CreateAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            => new TestAnalyzerConfigOptionsProvider(analyzerConfigSet);

        private async Task<SyntaxTreeAnalysisContext> CreateAnalysisContextFromEditorConfigAsync(string editorConfig)
        {
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);
            var analyzerConfigDocumentId = DocumentId.CreateNewId(projectId);

            var solution = GenericAnalyzerTest.CreateWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .AddDocument(documentId, "/0/Test0.cs", SourceText.From(string.Empty))
                .AddAnalyzerConfigDocument(analyzerConfigDocumentId, "/.editorconfig", SourceText.From(editorConfig), filePath: "/.editorconfig");

            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync(CancellationToken.None).ConfigureAwait(false);

            var analyzerConfigSet = AnalyzerConfigSet.Create(new[] { AnalyzerConfig.Parse(SourceText.From(editorConfig), "/.editorconfig") });
            var additionalFiles = ImmutableArray<AdditionalText>.Empty;
            var optionsProvider = this.CreateAnalyzerConfigOptionsProvider(analyzerConfigSet);
            var analyzerOptions = new AnalyzerOptions(additionalFiles, optionsProvider);

            return new SyntaxTreeAnalysisContext(syntaxTree, analyzerOptions, reportDiagnostic: _ => { }, isSupportedDiagnostic: _ => true, CancellationToken.None);
        }

        protected class TestAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            private readonly AnalyzerConfigOptionsResult analyzerConfigOptionsResult;

            public TestAnalyzerConfigOptions(AnalyzerConfigOptionsResult analyzerConfigOptionsResult)
            {
                this.analyzerConfigOptionsResult = analyzerConfigOptionsResult;
            }

            public override bool TryGetValue(string key, out string value)
            {
                return this.analyzerConfigOptionsResult.AnalyzerOptions.TryGetValue(key, out value);
            }
        }

        private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigSet analyzerConfigSet;

            public TestAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            {
                this.analyzerConfigSet = analyzerConfigSet;
            }

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            {
                return new TestAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(tree.FilePath));
            }

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            {
                return new TestAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(textFile.Path));
            }
        }
    }
}
