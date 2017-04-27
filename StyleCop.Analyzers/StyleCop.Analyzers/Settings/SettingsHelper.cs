﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using Newtonsoft.Json;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class that manages the settings files for StyleCopAnalyzers.
    /// </summary>
    internal static class SettingsHelper
    {
        internal const string SettingsFileName = "stylecop.json";

        private static readonly SourceTextValueProvider<StyleCopSettings> SettingsValueProvider =
            new SourceTextValueProvider<StyleCopSettings>(
                text => GetStyleCopSettings(text, DeserializationFailureBehavior.ReturnDefaultSettings));

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonException"/> occurs while deserializing the settings file, a default settings
        /// instance is returned.</para>
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
        /// <para>If a <see cref="JsonException"/> occurs while deserializing the settings file, a default settings
        /// instance is returned.</para>
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
        /// <param name="failureBehavior">The behavior of the method when a <see cref="JsonException"/> occurs while
        /// deserializing the settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>(), failureBehavior, cancellationToken);
        }

        internal static StyleCopSettings GetStyleCopSettings(this AnalysisContext context, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context, options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        internal static StyleCopSettings GetStyleCopSettings(this AnalysisContext context, AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            SourceText text = TryGetStyleCopSettingsText(options, cancellationToken);
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

            return JsonConvert.DeserializeObject<SettingsFile>(text.ToString()).Settings;
        }

        internal static StyleCopSettings GetStyleCopSettings(this CompilationStartAnalysisContext context, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context, options, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

#pragma warning disable RS1012 // Start action has no registered actions.
        internal static StyleCopSettings GetStyleCopSettings(this CompilationStartAnalysisContext context, AnalyzerOptions options, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
#pragma warning restore RS1012 // Start action has no registered actions.
        {
            SourceText text = TryGetStyleCopSettingsText(options, cancellationToken);
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

            return JsonConvert.DeserializeObject<SettingsFile>(text.ToString()).Settings;
        }

        private static StyleCopSettings GetStyleCopSettings(SourceText text, DeserializationFailureBehavior failureBehavior)
        {
            try
            {
                var root = JsonConvert.DeserializeObject<SettingsFile>(text.ToString());

                if (root == null)
                {
                    throw new JsonException($"Settings file was missing or empty.");
                }

                return root.Settings;
            }
            catch (JsonException) when (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                // The settings file is invalid -> return the default settings.
            }

            return new StyleCopSettings();
        }

        private static SourceText TryGetStyleCopSettingsText(this AnalyzerOptions options, CancellationToken cancellationToken)
        {
            foreach (var additionalFile in options.AdditionalFiles)
            {
                if (Path.GetFileName(additionalFile.Path).ToLowerInvariant() == SettingsFileName)
                {
                    return additionalFile.GetText(cancellationToken);
                }
            }

            return null;
        }

        private static StyleCopSettings GetStyleCopSettings(ImmutableArray<AdditionalText> additionalFiles, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var additionalFile in additionalFiles)
                {
                    if (Path.GetFileName(additionalFile.Path).ToLowerInvariant() == SettingsFileName)
                    {
                        SourceText additionalTextContent = additionalFile.GetText(cancellationToken);
                        var root = JsonConvert.DeserializeObject<SettingsFile>(additionalTextContent.ToString());

                        if (root == null)
                        {
                            throw new JsonException($"Settings file at '{Path.GetFileName(additionalFile.Path)}' was missing or empty.");
                        }

                        return root.Settings;
                    }
                }
            }
            catch (JsonException) when (failureBehavior == DeserializationFailureBehavior.ReturnDefaultSettings)
            {
                // The settings file is invalid -> return the default settings.
            }

            return new StyleCopSettings();
        }
    }
}
