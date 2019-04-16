// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using global::LightJson;
    using global::LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Testing.Verifiers;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using Xunit;

    internal static class StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        internal static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic();

        internal static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => new DiagnosticResult(descriptor);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => StyleCopDiagnosticVerifier<TAnalyzer>.VerifyCSharpDiagnosticAsync(source, expected, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => StyleCopDiagnosticVerifier<TAnalyzer>.VerifyCSharpDiagnosticAsync(source, expected, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => StyleCopDiagnosticVerifier<TAnalyzer>.VerifyCSharpDiagnosticAsync(languageVersion, source, expected, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => StyleCopDiagnosticVerifier<TAnalyzer>.VerifyCSharpDiagnosticAsync(languageVersion, source, expected, cancellationToken);

        internal static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        internal static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            if (source == fixedSource)
            {
                test.FixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.FixedState.MarkupHandling = MarkupMode.Allow;
                test.BatchFixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.BatchFixedState.MarkupHandling = MarkupMode.Allow;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal static Task VerifyCSharpFixAsync(LanguageVersion? languageVersion, string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(languageVersion, source, new[] { expected }, fixedSource, cancellationToken);

        internal static Task VerifyCSharpFixAsync(LanguageVersion? languageVersion, string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(languageVersion)
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            if (source == fixedSource)
            {
                test.FixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.FixedState.MarkupHandling = MarkupMode.Allow;
                test.BatchFixedState.InheritanceMode = StateInheritanceMode.AutoInheritAll;
                test.BatchFixedState.MarkupHandling = MarkupMode.Allow;
            }

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal class CSharpTest : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
        {
            private const int DefaultIndentationSize = 4;
            private const int DefaultTabSize = 4;
            private const bool DefaultUseTabs = false;

            public CSharpTest()
                : this(languageVersion: null)
            {
            }

            public CSharpTest(LanguageVersion? languageVersion)
            {
                this.OptionsTransforms.Add(options =>
                    options
                    .WithChangedOption(FormattingOptions.IndentationSize, this.Language, this.IndentationSize)
                    .WithChangedOption(FormattingOptions.TabSize, this.Language, this.TabSize)
                    .WithChangedOption(FormattingOptions.UseTabs, this.Language, this.UseTabs));

                this.TestState.AdditionalReferences.Add(GenericAnalyzerTest.CSharpSymbolsReference);
                this.TestState.AdditionalReferences.Add(Netstandard20Reference);
                this.TestState.AdditionalFilesFactories.Add(GenerateSettingsFile);
                this.CodeFixValidationMode = CodeFixValidationMode.None;

                if (languageVersion != null)
                {
                    this.SolutionTransforms.Add((solution, projectId) =>
                    {
                        var parseOptions = (CSharpParseOptions)solution.GetProject(projectId).ParseOptions;
                        return solution.WithProjectParseOptions(projectId, parseOptions.WithLanguageVersion(languageVersion.Value));
                    });
                }

                return;

                // Local function
                IEnumerable<(string filename, SourceText content)> GenerateSettingsFile()
                {
                    var settings = this.Settings;

                    StyleCopSettings defaultSettings = new StyleCopSettings();
                    if (this.IndentationSize != defaultSettings.Indentation.IndentationSize
                        || this.UseTabs != defaultSettings.Indentation.UseTabs
                        || this.TabSize != defaultSettings.Indentation.TabSize)
                    {
                        var indentationSettings = $@"
{{
  ""settings"": {{
    ""indentation"": {{
      ""indentationSize"": {this.IndentationSize},
      ""useTabs"": {this.UseTabs.ToString().ToLowerInvariant()},
      ""tabSize"": {this.TabSize}
    }}
  }}
}}
";

                        if (string.IsNullOrEmpty(settings))
                        {
                            settings = indentationSettings;
                        }
                        else
                        {
                            JsonObject indentationObject = JsonReader.Parse(indentationSettings).AsJsonObject;
                            JsonObject settingsObject = JsonReader.Parse(settings).AsJsonObject;
                            JsonObject mergedSettings = MergeJsonObjects(settingsObject, indentationObject);
                            using (var writer = new JsonWriter(pretty: true))
                            {
                                settings = writer.Serialize(mergedSettings);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(settings))
                    {
                        yield return (this.SettingsFileName, SourceText.From(settings));
                    }
                }
            }

            public SourceFileList TestSources => this.TestState.Sources;

            public SourceFileList FixedSources => this.FixedState.Sources;

            public SourceFileCollection FixedAdditionalFiles => this.FixedState.AdditionalFiles;

            public List<DiagnosticResult> RemainingDiagnostics => this.FixedState.ExpectedDiagnostics;

            /// <summary>
            /// Gets or sets the value of the <see cref="FormattingOptions.IndentationSize"/> to apply to the test
            /// workspace.
            /// </summary>
            /// <value>
            /// The value of the <see cref="FormattingOptions.IndentationSize"/> to apply to the test workspace.
            /// </value>
            public int IndentationSize { get; set; } = DefaultIndentationSize;

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="FormattingOptions.UseTabs"/> option is applied to the
            /// test workspace.
            /// </summary>
            /// <value>
            /// The value of the <see cref="FormattingOptions.UseTabs"/> to apply to the test workspace.
            /// </value>
            public bool UseTabs { get; set; } = DefaultUseTabs;

            /// <summary>
            /// Gets or sets the value of the <see cref="FormattingOptions.TabSize"/> to apply to the test workspace.
            /// </summary>
            /// <value>
            /// The value of the <see cref="FormattingOptions.TabSize"/> to apply to the test workspace.
            /// </value>
            public int TabSize { get; set; } = DefaultTabSize;

            /// <summary>
            /// Gets or sets the content of the settings file to use.
            /// </summary>
            /// <value>
            /// The content of the settings file to use.
            /// </value>
            public string Settings { get; set; } = null;

            /// <summary>
            /// Gets or sets the name of the settings file to use.
            /// </summary>
            /// <value>
            /// The name of the settings file to use.
            /// </value>
            public string SettingsFileName { get; set; } = SettingsHelper.SettingsFileName;

            /// <summary>
            /// Gets the list of diagnostic identifier that will be explicitly enabled in the compilation options.
            /// </summary>
            /// <value>
            /// The list of explicitly enabled diagnostic identifiers.
            /// </value>
            public List<string> ExplicitlyEnabledDiagnostics { get; } = new List<string>();

            private static Assembly Netstandard20Reference
                => Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51");

            protected override CompilationOptions CreateCompilationOptions()
            {
                var compilationOptions = base.CreateCompilationOptions();
                var specificDiagnosticOptions = compilationOptions.SpecificDiagnosticOptions;

                foreach (var id in this.ExplicitlyEnabledDiagnostics)
                {
                    specificDiagnosticOptions = specificDiagnosticOptions.SetItem(id, ReportDiagnostic.Warn);
                }

                return compilationOptions.WithSpecificDiagnosticOptions(specificDiagnosticOptions);
            }

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
            {
                var codeFixProvider = new TCodeFix();
                Assert.NotSame(WellKnownFixAllProviders.BatchFixer, codeFixProvider.GetFixAllProvider());
                return new[] { codeFixProvider };
            }

            private static JsonObject MergeJsonObjects(JsonObject priority, JsonObject fallback)
            {
                foreach (var pair in priority)
                {
                    if (pair.Value.IsJsonObject)
                    {
                        switch (fallback[pair.Key].Type)
                        {
                        case JsonValueType.Null:
                            fallback[pair.Key] = pair.Value;
                            break;

                        case JsonValueType.Object:
                            fallback[pair.Key] = MergeJsonObjects(pair.Value.AsJsonObject, fallback[pair.Key].AsJsonObject);
                            break;

                        default:
                            throw new InvalidOperationException($"Cannot merge objects of type '{pair.Value.Type}' and '{fallback[pair.Key].Type}'.");
                        }
                    }
                    else
                    {
                        fallback[pair.Key] = pair.Value;
                    }
                }

                return fallback;
            }
        }
    }
}
