﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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

        private static SourceTextValueProvider<Lazy<JsonValue>> JsonValueProvider { get; } =
            new SourceTextValueProvider<Lazy<JsonValue>>(
                text => ParseJson(text));

        /// <summary>
        /// Gets the StyleCop settings from the JSON file, i.e. without adding information frmo the .editorconfig.
        /// </summary>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="SettingsFile"/> instance which contains information about the the StyleCop settings file for the given context.
        /// Null if no settings file was found.</returns>
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
        internal static SettingsFile GetStyleCopSettingsFile(this CompilationStartAnalysisContext context, CancellationToken cancellationToken)
        {
            return GetSettingsFile(context.Options, GetJsonValue, cancellationToken);

            Lazy<JsonValue> GetJsonValue(SourceText settingsText)
            {
                if (context.TryGetValue(settingsText, JsonValueProvider, out var settingsFile))
                {
                    return settingsFile;
                }
                else
                {
                    // This should never happen, since the Lazy<> instance can always be created, but parse it normally if it does
                    Debug.Assert(false, "Could not get settings through cache");
                    return ParseJson(settingsText);
                }
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
        /// <param name="settingsFile">Information about the StyleCop settings file.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context, SettingsFile settingsFile)
        {
            return GetSettings(context.Options, context.Tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// <para>Note that this method does not use the cache.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context, CancellationToken cancellationToken)
        {
            var settingsFile = GetSettingsFile(context.Options, ParseJson, cancellationToken);
            return GetSettings(context.Options, context.Tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="settingsFile">The contents of the StyleCop settnigs file.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxNodeAnalysisContext context, SettingsFile settingsFile)
        {
            return GetSettings(context.Options, context.Node.SyntaxTree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// <para>Note that this method does not use the cache.</para>
        /// </remarks>
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="tree">The syntax tree.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, SyntaxTree tree, CancellationToken cancellationToken)
        {
            var settingsFile = GetSettingsFile(options, ParseJson, cancellationToken);
            return GetSettings(options, tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="tree">The syntax tree.</param>
        /// <param name="failureBehavior">The behavior of the method when a <see cref="JsonParseException"/> or
        /// <see cref="InvalidSettingsException"/> occurs while deserializing the settings file.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this CompilationAnalysisContext context, SyntaxTree tree, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            var settingsFile = GetSettingsFile(context.Options, GetJsonValue, cancellationToken);
            return GetSettings(context.Options, tree, settingsFile, failureBehavior);

            Lazy<JsonValue> GetJsonValue(SourceText settingsText)
            {
                if (context.TryGetValue(settingsText, JsonValueProvider, out var settingsFile))
                {
                    return settingsFile;
                }
                else
                {
                    // This should never happen, since the Lazy<> instance can always be created, but parse it normally if it does
                    Debug.Assert(false, "Could not get settings through cache");
                    return ParseJson(settingsText);
                }
            }
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

        private static StyleCopSettings GetSettings(AnalyzerOptions options, SyntaxTree tree, SettingsFile settingsFile, DeserializationFailureBehavior failureBehavior)
        {
            if (settingsFile != null)
            {
                return CreateSettingsObjectFromFile(options, tree, settingsFile, failureBehavior);
            }

            // TODO: Can this really be null? Review when nullable references has been enabled
            if (tree != null)
            {
                var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider().GetOptions(tree);
                return new StyleCopSettings(new JsonObject(), analyzerConfigOptions);
            }

            return new StyleCopSettings();
        }

        private static StyleCopSettings CreateSettingsObjectFromFile(AnalyzerOptions options, SyntaxTree tree, SettingsFile settingsFile, DeserializationFailureBehavior failureBehavior)
        {
            var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider().GetOptions(tree);

            try
            {
                // If the file was accessed through the cache, this statement will re-throw any exceptions from when parsing the file
                var rootValue = settingsFile.Content.Value;

                if (!rootValue.IsJsonObject)
                {
                    throw new JsonParseException(
                        $"Settings file at '{settingsFile.FilePath}' was missing or empty.",
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

        private static SettingsFile GetSettingsFile(AnalyzerOptions options, Func<SourceText, Lazy<JsonValue>> getJsonValue, CancellationToken cancellationToken)
        {
            var additionalFiles = options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>();
            foreach (var additionalFile in additionalFiles)
            {
                if (IsStyleCopSettingsFile(additionalFile.Path))
                {
                    SourceText additionalTextContent = additionalFile.GetText(cancellationToken);
                    var content = getJsonValue(additionalTextContent);
                    return new SettingsFile(additionalFile.Path, content);
                }
            }

            return null;
        }

        private static Lazy<JsonValue> ParseJson(SourceText text)
        {
            return new Lazy<JsonValue>(() => JsonReader.Parse(text.ToString()));
        }

        public class SettingsFile
        {
            public SettingsFile(string filePath, Lazy<JsonValue> content)
            {
                this.FilePath = filePath;
                this.Content = content;
            }

            public string FilePath { get; }

            public Lazy<JsonValue> Content { get; }
        }
    }
}
