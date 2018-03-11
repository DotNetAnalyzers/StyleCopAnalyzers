// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class that manages the settings files for StyleCopAnalyzers.
    /// </summary>
    internal static class SettingsHelper
    {
        internal const string SettingsFileName = "stylecop.json";

        private static readonly SourceTextValueProvider<StyleCopSettings> SettingsValueProvider =
            new SourceTextValueProvider<StyleCopSettings>(
                text => GetStyleCopSettings(SettingsFileName, text, DeserializationFailureBehavior.ReturnDefaultSettings));

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context, CancellationToken cancellationToken)
        {
            return context.Options.GetStyleCopSettings(cancellationToken);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="failureBehavior">The behavior of the method when a <see cref="JsonParseException"/> or
        /// <see cref="InvalidSettingsException"/> occurs while deserializing the settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>(), failureBehavior, cancellationToken);
        }

        /// <summary>
        /// Gets a value indicating whether the specified path points to a StyleCop settings file (stylecop.json or .stylecop.json).
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns><c>true</c> if <paramref name="path"/> points to a StyleCop settings file; otherwise, <c>false</c>.</returns>
        internal static bool IsStyleCopSettingsFile(string path)
        {
            var fileName = Path.GetFileName(path).ToLowerInvariant();

            if (fileName.StartsWith("."))
            {
                fileName = fileName.Substring(1);
            }

            return fileName == SettingsFileName;
        }

        internal static StyleCopSettings GetStyleCopSettings(this AnalysisContext context, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context, options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        internal static StyleCopSettings GetStyleCopSettings(this AnalysisContext context, AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            string settingsFileName;
            SourceText text = TryGetStyleCopSettingsText(options, cancellationToken, out settingsFileName);
            if (text == null)
            {
                return new StyleCopSettings();
            }

            if (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                StyleCopSettings settings;
                if (!context.TryGetValue(text, SettingsValueProvider, out settings))
                {
                    return new StyleCopSettings();
                }

                return settings;
            }

            return GetStyleCopSettings(settingsFileName, text, failureBehavior);
        }

        internal static StyleCopSettings GetStyleCopSettings(this CompilationStartAnalysisContext context, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context, options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

#pragma warning disable RS1012 // Start action has no registered actions.
        internal static StyleCopSettings GetStyleCopSettings(this CompilationStartAnalysisContext context, AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
#pragma warning restore RS1012 // Start action has no registered actions.
        {
            string settingsFileName;
            SourceText text = TryGetStyleCopSettingsText(options, cancellationToken, out settingsFileName);
            if (text == null)
            {
                return new StyleCopSettings();
            }

            if (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                StyleCopSettings settings;
                if (!context.TryGetValue(text, SettingsValueProvider, out settings))
                {
                    return new StyleCopSettings();
                }

                return settings;
            }

            return GetStyleCopSettings(settingsFileName, text, failureBehavior);
        }

        private static StyleCopSettings GetStyleCopSettings(string path, SourceText text, DeserializationFailureBehavior failureBehavior)
        {
            try
            {
                var rootValue = JsonReader.Parse(text.ToString());
                if (!rootValue.IsJsonObject)
                {
                    throw new JsonParseException(
                        $"Settings file at '{path}' was missing or empty.",
                        JsonParseException.ErrorType.InvalidOrUnexpectedCharacter,
                        default(TextPosition));
                }

                var settingsObject = rootValue.AsJsonObject["settings"];
                if (settingsObject.IsJsonObject)
                {
                    return new StyleCopSettings(settingsObject.AsJsonObject);
                }
                else if (settingsObject.IsNull)
                {
                    throw new InvalidSettingsException("\"settings\" must be a JSON object.");
                }

                return new StyleCopSettings();
            }
            catch (InvalidSettingsException) when (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                // The settings file is invalid -> return the default settings.
            }
            catch (JsonParseException) when (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                // The settings file is invalid -> return the default settings.
            }

            return new StyleCopSettings();
        }

        private static SourceText TryGetStyleCopSettingsText(this AnalyzerOptions options, CancellationToken cancellationToken, out string settingsFileName)
        {
            foreach (var additionalFile in options.AdditionalFiles)
            {
                if (IsStyleCopSettingsFile(additionalFile.Path))
                {
                    settingsFileName = additionalFile.Path;

                    return additionalFile.GetText(cancellationToken);
                }
            }

            settingsFileName = null;

            return null;
        }

        private static StyleCopSettings GetStyleCopSettings(ImmutableArray<AdditionalText> additionalFiles, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            foreach (var additionalFile in additionalFiles)
            {
                if (IsStyleCopSettingsFile(additionalFile.Path))
                {
                    SourceText additionalTextContent = additionalFile.GetText(cancellationToken);
                    return GetStyleCopSettings(additionalFile.Path, additionalTextContent, failureBehavior);
                }
            }

            return new StyleCopSettings();
        }
    }
}
