// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Settings
{
    using System;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
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
            var styleCopSettings = SettingsHelper.GetStyleCopSettingsInTests(default(SyntaxTreeAnalysisContext), CancellationToken.None);

            Assert.Equal("PlaceholderCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) PlaceholderCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
            Assert.Equal("en-US", styleCopSettings.DocumentationRules.DocumentationCulture);
            Assert.Same(CultureInfo.InvariantCulture, styleCopSettings.DocumentationRules.DocumentationCultureInfo);
            Assert.True(styleCopSettings.NamingRules.AllowCommonHungarianPrefixes);
            Assert.Empty(styleCopSettings.NamingRules.AllowedHungarianPrefixes);
            Assert.Empty(styleCopSettings.NamingRules.AllowedNamespaceComponents);

            Assert.NotNull(styleCopSettings.OrderingRules);
            Assert.Equal(UsingDirectivesPlacement.InsideNamespace, styleCopSettings.OrderingRules.UsingDirectivesPlacement);
            Assert.Equal(OptionSetting.Allow, styleCopSettings.OrderingRules.BlankLinesBetweenUsingGroups);

            Assert.NotNull(styleCopSettings.LayoutRules);
            Assert.Equal(OptionSetting.Allow, styleCopSettings.LayoutRules.NewlineAtEndOfFile);
            Assert.True(styleCopSettings.LayoutRules.AllowConsecutiveUsings);
            Assert.False(styleCopSettings.LayoutRules.AllowDoWhileOnClosingBrace);

            Assert.NotNull(styleCopSettings.SpacingRules);
            Assert.NotNull(styleCopSettings.ReadabilityRules);
            Assert.NotNull(styleCopSettings.MaintainabilityRules);
        }

        [Fact]
        [WorkItem(3402, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3402")]
        public async Task VerifyDefaultCultureIsReadCorrectlyAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""documentationCulture"": ""en-US""
    },
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("en-US", styleCopSettings.DocumentationRules.DocumentationCulture);
            Assert.Same(CultureInfo.InvariantCulture, styleCopSettings.DocumentationRules.DocumentationCultureInfo);
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
      ""copyrightText"": ""Custom copyright text."",
      ""documentationCulture"": ""ru-RU"",
      ""unrecognizedValue"": 3
    },
    ""namingRules"": {
      ""allowCommonHungarianPrefixes"": false,
      ""allowedHungarianPrefixes"": [""a"", ""ab"", ""ignoredTooLong""],
      ""allowedNamespaceComponents"": [""eBay"", ""iMac""],
      ""unrecognizedValue"": 3
    },
    ""layoutRules"": {
      ""newlineAtEndOfFile"": ""require"",
      ""unrecognizedValue"": 3
    },
    ""orderingRules"": {
      ""usingDirectivesPlacement"": ""outsideNamespace"",
      ""blankLinesBetweenUsingGroups"": ""omit"",
      ""unrecognizedValue"": 3
    },
    ""maintainabilityRules"": {
      ""unrecognizedValue"": 3
    },
    ""indentation"": {
      ""unrecognizedValue"": 3
    },
    ""readabilityRules"": { },
    ""spacingRules"": { },
    ""unrecognizedValue"": 3
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

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
            Assert.Equal(OptionSetting.Omit, styleCopSettings.OrderingRules.BlankLinesBetweenUsingGroups);
        }

        /// <summary>
        /// Verifies that the settings are properly read.
        /// </summary>
        /// <param name="value">The value for testing the settings.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task VerifyBooleanDocumentationSettingsAsync(bool value)
        {
            string valueText = value.ToString().ToLowerInvariant();
            var settings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentExposedElements"": {valueText},
      ""documentInternalElements"": {valueText},
      ""documentPrivateElements"": {valueText},
      ""documentPrivateFields"": {valueText}
    }}
  }}
}}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentExposedElements);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentInternalElements);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentPrivateElements);
            Assert.Equal(value, styleCopSettings.DocumentationRules.DocumentPrivateFields);
        }

        /// <summary>
        /// Verifies that the settings are properly read.
        /// </summary>
        /// <param name="value">The value for testing the settings.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("\"all\"")]
        [InlineData("\"none\"")]
        [InlineData("\"exposed\"")]
        public async Task VerifyInheritanceDocumentationSettingsAsync(string value)
        {
            var settings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""documentInterfaces"": {value}
    }}
  }}
}}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            var expected = value switch
            {
                "true" => InterfaceDocumentationMode.All,
                "false" => InterfaceDocumentationMode.None,
                "\"all\"" => InterfaceDocumentationMode.All,
                "\"none\"" => InterfaceDocumentationMode.None,
                "\"exposed\"" => InterfaceDocumentationMode.Exposed,
                _ => throw new InvalidOperationException($"Unexpected test value: {value}"),
            };

            Assert.Equal(expected, styleCopSettings.DocumentationRules.DocumentInterfaces);
        }

        /// <summary>
        /// Verifies that the settings are properly read.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyDocumentationVariablesAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""variables"": {
        ""var"": ""value"",
        ""no space allowed"": ""value""
      }
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Single(styleCopSettings.DocumentationRules.Variables);
            Assert.Equal("value", styleCopSettings.DocumentationRules.Variables["var"]);
            Assert.False(styleCopSettings.DocumentationRules.Variables.ContainsKey("no space allowed"));
        }

        /// <summary>
        /// Verifies that the settings will use the read company name in the default copyright text.
        /// </summary>
        /// <param name="companyName">The company name to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("TestCompany")]
        [InlineData("TestCompany2")]
        public async Task VerifySettingsWillUseCompanyNameInDefaultCopyrightTextAsync(string companyName)
        {
            var settings = $@"
{{
  ""settings"": {{
    ""documentationRules"": {{
      ""companyName"": ""{companyName}""
    }}
  }}
}}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal(companyName, styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal($"Copyright (c) {companyName}. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
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

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("[CircularReference]", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
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

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("[InvalidReference]", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        /// <summary>
        /// Verifies that the settings successfully parse line comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifySettingsSupportsLineCommentsAsync()
        {
            var settings = @"
{
  // Set value for company name
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany""
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) TestCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        /// <summary>
        /// Verifies that the settings successfully parse block comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifySettingsSupportsBlockCommentsAsync()
        {
            var settings = @"
{
  /*
   * Set value for company name
   */
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany""
    }
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) TestCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        /// <summary>
        /// Verifies that the settings successfully parse trailing commas.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifySettingsSupportsTrailingCommasAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany"",
    },
    ""namingRules"": {
      ""allowedHungarianPrefixes"": [ ""a"", ],
    },
  }
}
";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) TestCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));

            Assert.Single(styleCopSettings.NamingRules.AllowedHungarianPrefixes);
            Assert.Equal("a", styleCopSettings.NamingRules.AllowedHungarianPrefixes[0]);
        }

        [Fact]
        public async Task VerifySettingsFileNameSupportsDotPrefixAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""TestCompany"",
    },
  }
}
";
            var context = await CreateAnalysisContextAsync(settings, ".stylecop.json").ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
        }

        [Fact]
        public async Task VerifyInvalidJsonBehaviorAsync()
        {
            var settings = @"This is not a JSON file.";
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            // The result is the same as the default settings.
            Assert.Equal("PlaceholderCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) PlaceholderCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Fact]
        public async Task VerifyEmptyOrMissingFileAsync()
        {
            var settings = string.Empty;
            var context = await CreateAnalysisContextAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            // The result is the same as the default settings.
            Assert.Equal("PlaceholderCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Equal("Copyright (c) PlaceholderCompany. All rights reserved.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        private static async Task<SyntaxTreeAnalysisContext> CreateAnalysisContextAsync(string stylecopJSON, string settingsFileName = SettingsHelper.SettingsFileName)
        {
            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            var solution = (await GenericAnalyzerTest.CreateWorkspaceAsync().ConfigureAwait(false))
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                .AddDocument(documentId, "Test0.cs", SourceText.From(string.Empty));

            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);

            var stylecopJSONFile = new AdditionalTextHelper(settingsFileName, stylecopJSON);
            var additionalFiles = ImmutableArray.Create<AdditionalText>(stylecopJSONFile);
            var analyzerOptions = new AnalyzerOptions(additionalFiles);

            return new SyntaxTreeAnalysisContext(syntaxTree, analyzerOptions, reportDiagnostic: _ => { }, isSupportedDiagnostic: _ => true, CancellationToken.None);
        }

        private class AdditionalTextHelper : AdditionalText
        {
            private readonly SourceText sourceText;

            public AdditionalTextHelper(string path, string text)
            {
                this.Path = path;
                this.sourceText = SourceText.From(text);
            }

            public override string Path { get; }

            public override SourceText GetText(CancellationToken cancellationToken = default)
            {
                return this.sourceText;
            }
        }
    }
}
