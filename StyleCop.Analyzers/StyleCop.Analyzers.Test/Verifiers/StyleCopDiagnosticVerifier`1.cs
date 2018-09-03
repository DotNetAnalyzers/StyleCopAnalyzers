// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::LightJson;
    using global::LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using TestHelper;

    internal static class StyleCopDiagnosticVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        internal static DiagnosticResult[] EmptyDiagnosticResults
            => DiagnosticVerifier<TAnalyzer>.EmptyDiagnosticResults;

        internal static DiagnosticResult Diagnostic(string diagnosticId = null)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => DiagnosticVerifier<TAnalyzer>.Diagnostic(descriptor);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal class CSharpTest : DiagnosticVerifier<TAnalyzer>.CSharpTest
        {
            private const int DefaultIndentationSize = 4;
            private const int DefaultTabSize = 4;
            private const bool DefaultUseTabs = false;

            public CSharpTest()
            {
                this.OptionsTransforms.Add(options =>
                    options
                    .WithChangedOption(FormattingOptions.IndentationSize, this.Language, this.IndentationSize)
                    .WithChangedOption(FormattingOptions.TabSize, this.Language, this.TabSize)
                    .WithChangedOption(FormattingOptions.UseTabs, this.Language, this.UseTabs));

                this.AdditionalFilesFactories.Add(GenerateSettingsFile);

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
