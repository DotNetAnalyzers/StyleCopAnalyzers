// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;
    using System.Threading;
    using LightJson;
    using LightJson.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class that manages the settings for StyleCopAnalyzers, both from the json
    /// settings file and from analyzer options.
    /// </summary>
    internal static class SettingsHelper
    {
        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the analyzer options.</param>
        /// <param name="settingsJson">The pre-parsed settings from the json settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context, Lazy<JsonValue> settingsJson, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context.Options, context.Tree, DeserializationFailureBehavior.ReturnDefaultSettings, GetSettingsJson, cancellationToken);

            JsonValue GetSettingsJson(SourceText text)
            {
                // This will make sure that we re-throw any exception from when the file was parsed
                return settingsJson.Value;
            }
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
        /// <param name="settingsJson">The pre-parsed settings from the json settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxNodeAnalysisContext context, Lazy<JsonValue> settingsJson, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(context.Options, context.Node.SyntaxTree, DeserializationFailureBehavior.ReturnDefaultSettings, GetSettingsJson, cancellationToken);

            JsonValue GetSettingsJson(SourceText text)
            {
                // This will make sure that we re-throw any exception from when the file was parsed
                return settingsJson.Value;
            }
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
            return GetStyleCopSettings(options, tree, failureBehavior, GetSettingsJson, cancellationToken);

            JsonValue GetSettingsJson(SourceText text)
            {
                return SettingsJsonHelper.ParseStyleCopSettingsJson(text);
            }
        }

        private static StyleCopSettings GetStyleCopSettings(AnalyzerOptions options, SyntaxTree tree, string path, SourceText text, DeserializationFailureBehavior failureBehavior, Func<SourceText, JsonValue> getSettingsJson)
        {
            var optionsProvider = options.AnalyzerConfigOptionsProvider().GetOptions(tree);

            try
            {
                var rootValue = getSettingsJson(text);
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
                    return new StyleCopSettings(settingsObject.AsJsonObject, optionsProvider);
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

        private static StyleCopSettings GetStyleCopSettings(AnalyzerOptions options, SyntaxTree tree, DeserializationFailureBehavior failureBehavior, Func<SourceText, JsonValue> getSettingsJson, CancellationToken cancellationToken)
        {
            var settingsFile = SettingsJsonHelper.GetStyleCopSettingsFile(options);
            if (settingsFile != null)
            {
                SourceText additionalTextContent = settingsFile.GetText(cancellationToken);
                return GetStyleCopSettings(options, tree, settingsFile.Path, additionalTextContent, failureBehavior, getSettingsJson);
            }

            if (tree != null)
            {
                var optionsProvider = options.AnalyzerConfigOptionsProvider().GetOptions(tree);
                return new StyleCopSettings(new JsonObject(), optionsProvider);
            }

            return new StyleCopSettings();
        }
    }
}
