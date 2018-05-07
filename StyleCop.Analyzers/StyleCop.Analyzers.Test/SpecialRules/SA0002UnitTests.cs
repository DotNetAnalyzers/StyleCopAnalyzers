// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpecialRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.Settings;
    using Analyzers.SpecialRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA0002InvalidSettingsFile"/>.
    /// </summary>
    public class SA0002UnitTests : DiagnosticVerifier
    {
        private const string TestCode = @"
namespace NamespaceName { }
";

        private string settings;

        [Fact]
        public async Task TestMissingSettingsAsync()
        {
           await this.VerifyCSharpDiagnosticAsync(TestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidSettingsAsync()
        {
            this.settings = SettingsFileCodeFixProvider.DefaultSettingsFileContent;

            await this.VerifyCSharpDiagnosticAsync(TestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingsAsync()
        {
            // The settings file is missing a comma after the $schema property
            this.settings = @"
{
  ""$schema"": ""https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json""
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""ACME, Inc"",
      ""copyrightText"": ""Copyright 2015 {companyName}. All rights reserved.""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingStringValueAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingStringArrayElementValueAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedHungarianPrefixes"": [ 3 ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingBooleanValueAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""xmlHeader"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingIntegerValueAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""indentation"": {
      ""tabSize"": ""3""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingEnumValueNotStringAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayElementEnumValueNotStringAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""maintainabilityRules"": {
      ""topLevelTypes"": [ 3 ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayElementEnumValueNotRecognizedAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""maintainabilityRules"": {
      ""topLevelTypes"": [ ""Some incorrect value"" ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedHungarianPrefixes"": ""ah""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingObjectAsync()
        {
            this.settings = @"
{
  ""settings"": {
    ""namingRules"": true
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingSyntaxAsync()
        {
            // Missing the ':' between "companyName" and "name"
            this.settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"" ""name""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptySettingsAsync()
        {
            // The test infrastructure will not add a settings file to the compilation if GetSettings returns null or an empty string.
            // This is why we set settings to a simple whitespace character.
            this.settings = " ";

            // This diagnostic is reported without a location
            DiagnosticResult expected = this.CSharpDiagnostic();

            await this.VerifyCSharpDiagnosticAsync(TestCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public void TestUnexpectedExceptionNotCaught()
        {
            var analysisContext = new AnalysisContextMissingOptions();
            var analyzer = new SA0002InvalidSettingsFile();
            analyzer.Initialize(analysisContext);
            Assert.NotNull(analysisContext.CompilationAction);

            var additionalFiles = ImmutableArray.Create<AdditionalText>(new InvalidAdditionalText());
            Assert.Null(additionalFiles[0].Path);
            Assert.Null(additionalFiles[0].GetText(CancellationToken.None));
            var context = new CompilationAnalysisContext(compilation: null, options: new AnalyzerOptions(additionalFiles), reportDiagnostic: null, isSupportedDiagnostic: null, cancellationToken: CancellationToken.None);
            Assert.Throws<ArgumentNullException>(() => analysisContext.CompilationAction(context));
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            return this.settings;
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA0002InvalidSettingsFile();
        }

        private class InvalidAdditionalText : AdditionalText
        {
            public override string Path => null;

            public override SourceText GetText(CancellationToken cancellationToken) => null;
        }

        /// <summary>
        /// This analysis context is used for testing the specific case where an exception occurs while evaluating the
        /// stylecop.json settings file, but it is not one of the JSON deserialization exceptions caused by this
        /// library's code.
        /// </summary>
        private class AnalysisContextMissingOptions : AnalysisContext
        {
            public Action<CompilationAnalysisContext> CompilationAction
            {
                get;
                private set;
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
            {
                throw new NotImplementedException();
            }

            public override void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
            {
                this.CompilationAction = action;
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
            {
                throw new NotImplementedException();
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
            {
                throw new NotImplementedException();
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            {
                throw new NotImplementedException();
            }

            [ExcludeFromCodeCoverage]
            public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
            {
                throw new NotImplementedException();
            }
        }
    }
}
