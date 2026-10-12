// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.Settings
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Test.CSharp8.Settings;

    public partial class SettingsCSharp9UnitTests : SettingsCSharp8UnitTests
    {
        protected override AnalyzerConfigOptionsProvider CreateAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
        {
            return new TestAnalyzerConfigOptionsProvider(analyzerConfigSet);
        }

        private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigSet analyzerConfigSet;

            public TestAnalyzerConfigOptionsProvider(AnalyzerConfigSet analyzerConfigSet)
            {
                this.analyzerConfigSet = analyzerConfigSet;
            }

            public override AnalyzerConfigOptions GlobalOptions
            {
                get
                {
                    return new TestAnalyzerConfigOptions(this.analyzerConfigSet.GlobalConfigOptions);
                }
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
