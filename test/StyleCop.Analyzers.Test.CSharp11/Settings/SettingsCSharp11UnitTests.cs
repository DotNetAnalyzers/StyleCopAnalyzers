// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.Settings
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Test.CSharp10.Settings;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public partial class SettingsCSharp11UnitTests : SettingsCSharp10UnitTests
    {
        [Fact]
        public async Task VerifyDocumentationVariablesFromEditorConfigAsync()
        {
            var settings = @"root = true

[*]
stylecop.documentation.variables.licenseName = MIT
stylecop.documentation.variables.bad-name = ignored
stylecop.documentation.copyrightText = Licensed under {licenseName}.
";
            var context = await this.CreateAnalysisContextFromEditorConfigAsync(settings).ConfigureAwait(false);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Single(styleCopSettings.DocumentationRules.Variables);
            Assert.Equal("MIT", styleCopSettings.DocumentationRules.Variables["licensename"]);
            Assert.Equal("Licensed under MIT.", styleCopSettings.DocumentationRules.GetCopyrightText("unused"));
        }

        [Fact]
        public async Task VerifyHostWithoutKeysSupportDoesNotThrowAsync()
        {
            var settings = @"root = true

[*]
stylecop.documentation.companyName = TestCompany
stylecop.documentation.variables.licenseName = MIT
";
            var syntaxTree = CSharpSyntaxTree.ParseText(string.Empty, path: "/0/Test0.cs");
            var analyzerConfigSet = AnalyzerConfigSet.Create(new[] { AnalyzerConfig.Parse(SourceText.From(settings), "/.editorconfig") });
            var analyzerOptions = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new NoKeysAnalyzerConfigOptionsProvider(analyzerConfigSet));
            var context = new SyntaxTreeAnalysisContext(syntaxTree, analyzerOptions, reportDiagnostic: _ => { }, isSupportedDiagnostic: _ => true, CancellationToken.None);

            var styleCopSettings = context.GetStyleCopSettingsInTests(CancellationToken.None);

            Assert.Equal("TestCompany", styleCopSettings.DocumentationRules.CompanyName);
            Assert.Empty(styleCopSettings.DocumentationRules.Variables);
        }

        protected override AnalyzerConfigOptionsProvider CreateAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            => new KeysAwareAnalyzerConfigOptionsProvider(analyzerConfigSet);

        private sealed class NoKeysAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            private readonly AnalyzerConfigOptionsResult result;

            public NoKeysAnalyzerConfigOptions(AnalyzerConfigOptionsResult result)
            {
                this.result = result;
            }

            public override bool TryGetValue(string key, out string value)
            {
                // Roslyn's override contract does not annotate the null value returned for missing keys.
                return this.result.AnalyzerOptions.TryGetValue(key, out value!);
            }
        }

        private sealed class NoKeysAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigSet analyzerConfigSet;

            public NoKeysAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            {
                this.analyzerConfigSet = analyzerConfigSet;
            }

            public override AnalyzerConfigOptions GlobalOptions
                => new NoKeysAnalyzerConfigOptions(this.analyzerConfigSet.GlobalConfigOptions);

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
                => new NoKeysAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(tree.FilePath));

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
                => new NoKeysAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(textFile.Path));
        }

        private sealed class KeysAwareAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            private readonly AnalyzerConfigOptionsResult result;

            public KeysAwareAnalyzerConfigOptions(AnalyzerConfigOptionsResult result)
            {
                this.result = result;
            }

            public override IEnumerable<string> Keys => this.result.AnalyzerOptions.Keys;

            public override bool TryGetValue(string key, out string value)
            {
                // Roslyn's override contract does not annotate the null value returned for missing keys.
                return this.result.AnalyzerOptions.TryGetValue(key, out value!);
            }
        }

        private sealed class KeysAwareAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigSet analyzerConfigSet;

            public KeysAwareAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            {
                this.analyzerConfigSet = analyzerConfigSet;
            }

            public override AnalyzerConfigOptions GlobalOptions
                => new KeysAwareAnalyzerConfigOptions(this.analyzerConfigSet.GlobalConfigOptions);

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
                => new KeysAwareAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(tree.FilePath));

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
                => new KeysAwareAnalyzerConfigOptions(this.analyzerConfigSet.GetOptionsForSourcePath(textFile.Path));
        }
    }
}
