// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpecialRules
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings;
    using StyleCop.Analyzers.SpecialRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.SpecialRules.SA0002InvalidSettingsFile>;

    /// <summary>
    /// Unit tests for <see cref="SA0002InvalidSettingsFile"/>.
    /// </summary>
    public class SA0002UnitTests
    {
        private const string TestCode = @"
namespace NamespaceName { }
";

        [Fact]
        public async Task TestMissingSettingsAsync()
        {
            await VerifyCSharpDiagnosticAsync(TestCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidSettingsAsync()
        {
            await new CSharpTest
            {
                TestCode = TestCode,
                Settings = SettingsFileCodeFixProvider.DefaultSettingsFileContent,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingsAsync()
        {
            // The settings file is missing a comma after the $schema property
            var settings = @"
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
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingStringValueAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingStringArrayElementValueAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedHungarianPrefixes"": [ 3 ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingBooleanValueAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""xmlHeader"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingIntegerValueAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""indentation"": {
      ""tabSize"": ""3""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingEnumValueNotStringAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""fileNamingConvention"": 3
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayElementEnumValueNotStringAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""maintainabilityRules"": {
      ""topLevelTypes"": [ 3 ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayElementEnumValueNotRecognizedAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""maintainabilityRules"": {
      ""topLevelTypes"": [ ""Some incorrect value"" ]
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingArrayAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""namingRules"": {
      ""allowedHungarianPrefixes"": ""ah""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingObjectAsync()
        {
            var settings = @"
{
  ""settings"": {
    ""namingRules"": true
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidSettingSyntaxAsync()
        {
            // Missing the ':' between "companyName" and "name"
            var settings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"" ""name""
    }
  }
}
";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptySettingsAsync()
        {
            // The test infrastructure will not add a settings file to the compilation if GetSettings returns null or an empty string.
            // This is why we set settings to a simple whitespace character.
            var settings = " ";

            // This diagnostic is reported without a location
            DiagnosticResult expected = Diagnostic();

            await new CSharpTest
            {
                TestCode = TestCode,
                ExpectedDiagnostics = { expected },
                Settings = settings,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
