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
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class that manages the settings files for StyleCopAnalyzers.
    /// </summary>
    internal static class SettingsHelper
    {
        internal const string SettingsFileName = "stylecop.json";
        internal const string AltSettingsFileName = ".stylecop.json";

        private static SourceTextValueProvider<StyleCopSettings> SettingsValueProvider { get; } =
            new SourceTextValueProvider<StyleCopSettings>(
                text => GetStyleCopSettings(options: null, tree: null, SettingsFileName, text, DeserializationFailureBehavior.ReturnDefaultSettings));

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
            return GetStyleCopSettings(context.Options, context.Tree, cancellationToken);
        }

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
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxNodeAnalysisContext context, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context.Options, context.Node.SyntaxTree, cancellationToken);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="tree">The syntax tree.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, SyntaxTree tree, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options, tree, DeserializationFailureBehavior.ReturnDefaultSettings, cancellationToken);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="tree">The syntax tree.</param>
        /// <param name="failureBehavior">The behavior of the method when a <see cref="JsonParseException"/> or
        /// <see cref="InvalidSettingsException"/> occurs while deserializing the settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, SyntaxTree tree, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options, tree, options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>(), failureBehavior, cancellationToken);
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

        private static StyleCopSettings GetStyleCopSettings(AnalyzerOptions options, SyntaxTree tree, ImmutableArray<AdditionalText> additionalFiles, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            foreach (var additionalFile in additionalFiles)
            {
                if (IsStyleCopSettingsFile(additionalFile.Path))
                {
                    SourceText additionalTextContent = additionalFile.GetText(cancellationToken);
                    return GetStyleCopSettings(options, tree, additionalFile.Path, additionalTextContent, failureBehavior);
                }
            }

            if (tree != null)
            {
                var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider().GetOptions(tree);
                return new StyleCopSettings(new JsonObject(), analyzerConfigOptions);
            }

            return new StyleCopSettings();
        }

        private static StyleCopSettings GetStyleCopSettings(AnalyzerOptions options, SyntaxTree tree, string path, SourceText text, DeserializationFailureBehavior failureBehavior)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider().GetOptions(tree);

            try
            {
                var rootValue = JsonReader.Parse(text.ToString());
                if (!rootValue.IsJsonObject)
                {
                    throw new JsonParseException(
                        $"Settings file at '{path}' was missing or empty.",
                        JsonParseException.ErrorType.InvalidOrUnexpectedCharacter,
                        default);
                }

                var settingsObject = rootValue.AsJsonObject["settings"];
                if (settingsObject.IsJsonObject)
                {
                    return new StyleCopSettings(settingsObject.AsJsonObject, analyzerConfigOptions);
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
    }
}
