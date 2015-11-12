// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Settings
{
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using Xunit;

    public class SettingsUnitTests
    {
        private const string TestProjectName = "TestProject";

        /// <summary>
        /// Verifies that proper defaults are returned when there are no additional files.
        /// </summary>
        [Fact]
        public void VerifySettingsDefaults()
        {
            var styleCopSettings = SettingsHelper.GetStyleCopSettings(default(SyntaxTreeAnalysisContext));

            Assert.Equal("PlaceholderCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) PlaceholderCompany. All rights reserved.", styleCopSettings.DocumentationRules.CopyrightText);
            Assert.True(styleCopSettings.NamingRules.AllowCommonHungarianPrefixes);
            Assert.Equal(0, styleCopSettings.NamingRules.AllowedHungarianPrefixes.Length);

            Assert.NotNull(styleCopSettings.SpacingRules);
            Assert.NotNull(styleCopSettings.ReadabilityRules);
            Assert.NotNull(styleCopSettings.OrderingRules);
            Assert.NotNull(styleCopSettings.MaintainabilityRules);
        }

        /// <summary>
        /// Verifies that the settings are properly read.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifySettingsAreReadCorrectlyAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany"",
      ""copyrightText"": ""Custom copyright text.""
    },
    ""namingRules"": {
        ""allowCommonHungarianPrefixes"": false,
        ""allowedHungarianPrefixes"": [""a"", ""ab""]
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettings();

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Custom copyright text.", styleCopSettings.DocumentationRules.CopyrightText);
            Assert.False(styleCopSettings.NamingRules.AllowCommonHungarianPrefixes);
            Assert.Equal(new[] { "a", "ab" }, styleCopSettings.NamingRules.AllowedHungarianPrefixes);
        }

        /// <summary>
        /// Verifies that the settings will use the read company name in the default copyright text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifySettingsWillUseCompanyNameInDefaultCopyrightTextAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany""
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettings();

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) TestCompany. All rights reserved.", styleCopSettings.DocumentationRules.CopyrightText);
        }

        [Fact]
        public async Task VerifyCircularReferenceBehaviorAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""copyrightText"": ""{copyrightText}""
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettings();

            Assert.Equal("[CircularReference]", styleCopSettings.DocumentationRules.CopyrightText);
        }

        [Fact]
        public async Task VerifyInvalidReferenceBehaviorAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""copyrightText"": ""{variable}""
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettings();

            Assert.Equal("[InvalidReference]", styleCopSettings.DocumentationRules.CopyrightText);
        }

        [Fact]
        public async Task VerifyInvalidJsonBehaviorAsync()
        {
            var settings = @"This is not a JSON file.";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettings();

            // The result is the same as the default settings.
            Assert.Equal("PlaceholderCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) PlaceholderCompany. All rights reserved.", styleCopSettings.DocumentationRules.CopyrightText);
        }

        private static async Task<SyntaxTreeAnalysisContext> CreateAnalysisContextAsync(string stylecopJSON)
        {
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .AddDocument(documentId, "Test0.cs", SourceText.From(string.Empty));

            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);

            var stylecopJSONFile = new AdditionalTextHelper("stylecop.json", stylecopJSON);
            var additionalFiles = ImmutableArray.Create<AdditionalText>(stylecopJSONFile);
            var analyzerOptions = new AnalyzerOptions(additionalFiles);

            return new SyntaxTreeAnalysisContext(syntaxTree, analyzerOptions, rd => { }, isd => { return true; }, CancellationToken.None);
        }

        private class AdditionalTextHelper : AdditionalText
        {
            private SourceText sourceText;

            public AdditionalTextHelper(string path, string text)
            {
                this.Path = path;
                this.sourceText = SourceText.From(text);
            }

            public override string Path { get; }

            public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
            {
                return this.sourceText;
            }
        }
    }
}
