// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using LightJson;
    using LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Class that manages the json settings files for StyleCopAnalyzers.
    /// Handles only the json file. The complete settings, based on both the json file
    /// and the analyzer options, is handled in SettingsHelper.
    /// </summary>
    internal static class SettingsJsonHelper
    {
        internal const string SettingsFileName = "stylecop.json";
        internal const string AltSettingsFileName = ".stylecop.json";

        private static readonly Lazy<JsonValue> NullJsonValue =
            new Lazy<JsonValue>(() => JsonValue.Null);

        // Used to get a pre-parsed json value from a cache, if the same settings file
        // has already been parsed before. Lazy makes sure that it can be cached
        // even if the file has errors in it. Any exceptions during parsing will be re-thrown
        // when accessing the value.
        private static SourceTextValueProvider<Lazy<JsonValue>> SettingsValueProvider { get; } =
            new SourceTextValueProvider<Lazy<JsonValue>>(
                text => new Lazy<JsonValue>(() => ParseStyleCopSettingsJson(text)));

        /// <summary>
        /// Gets the StyleCop settings as a JsonValue. Only reads from the json settings file.
        /// </summary>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="Lazy{JsonValue}"/> instance that represents the StyleCop settings for the given context.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
        internal static Lazy<JsonValue> GetStyleCopSettingsJson(this CompilationStartAnalysisContext context, CancellationToken cancellationToken)
        {
            var settingsFile = GetStyleCopSettingsFile(context.Options);
            if (settingsFile == null)
            {
                // Could not find settings file
                return NullJsonValue;
            }

            var settingsFileContent = settingsFile.GetText(cancellationToken);
            if (context.TryGetValue(settingsFileContent, SettingsValueProvider, out var json))
            {
                return json;
            }
            else
            {
                // Don't think this can ever happen, but let's parse the file ourselves instead
                return new Lazy<JsonValue>(() => ParseStyleCopSettingsJson(settingsFileContent));
            }
        }

        /// <summary>
        /// Gets the StyleCop settings as a JsonValue. Only reads from the json settings file.
        /// </summary>
        /// <param name="text">The text containing the StyleCop settings.</param>
        /// <returns>A <see cref="JsonValue"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static JsonValue ParseStyleCopSettingsJson(SourceText text)
        {
            return JsonReader.Parse(text.ToString());
        }

        /// <summary>
        /// Gets the StyleCop settings as an AdditionalText.
        /// </summary>
        /// <param name="options">The options that will be used to find the StyleCop settings file.</param>
        /// <returns>A <see cref="AdditionalText"/> instance that represents the StyleCop settings file for the given context.</returns>
        internal static AdditionalText GetStyleCopSettingsFile(AnalyzerOptions options)
        {
            var additionalFiles = options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>();
            foreach (var additionalFile in additionalFiles)
            {
                if (IsStyleCopSettingsFile(additionalFile.Path))
                {
                    return additionalFile;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the specified path points to a StyleCop settings file (stylecop.json or .stylecop.json).
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/> points to a StyleCop settings file; otherwise,
        /// <see langword="false"/>.</returns>
        internal static bool IsStyleCopSettingsFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var fileName = Path.GetFileName(path);

            return string.Equals(fileName, SettingsFileName, StringComparison.OrdinalIgnoreCase)
                || string.Equals(fileName, AltSettingsFileName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
