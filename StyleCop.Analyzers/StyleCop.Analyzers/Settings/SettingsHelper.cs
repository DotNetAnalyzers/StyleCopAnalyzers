// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.CompilerServices;
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
                static text => ParseJson(text));

        private static SyntaxTreeValueProvider<StrongBox<StyleCopSettings>> SettingsStorageValueProvider { get; } =
            new SyntaxTreeValueProvider<StrongBox<StyleCopSettings>>(static _ => new StrongBox<StyleCopSettings>());

        /// <summary>
        /// Gets the StyleCop settings from the JSON file, i.e. without adding information frmo the .editorconfig.
        /// </summary>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="SettingsFile"/> instance which contains information about the the StyleCop settings file for the given context.
        /// Null if no settings file was found.</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
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

#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        internal static StrongBox<StyleCopSettings> GetOrCreateSettingsStorage(this CompilationStartAnalysisContext context, SyntaxTree tree)
        {
            if (!context.TryGetValue(tree, SettingsStorageValueProvider, out var storage))
            {
                storage = new StrongBox<StyleCopSettings>();
            }

            return storage;
        }

        internal static StrongBox<StyleCopSettings> GetOrCreateSettingsStorage(this CompilationAnalysisContext context, SyntaxTree tree)
        {
            if (!context.TryGetValue(tree, SettingsStorageValueProvider, out var storage))
            {
                storage = new StrongBox<StyleCopSettings>();
            }

            return storage;
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="settingsStorage">The storage location for caching an evaluated <see cref="StyleCopSettings"/> object.</param>
        /// <param name="settingsFile">Information about the StyleCop settings file.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context, StrongBox<StyleCopSettings> settingsStorage, SettingsFile settingsFile)
        {
            return GetSettings(context.Options, settingsStorage, context.Tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
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
        internal static StyleCopSettings GetStyleCopSettingsInTests(this SyntaxTreeAnalysisContext context, CancellationToken cancellationToken)
        {
            var settingsFile = GetSettingsFile(context.Options, ParseJson, cancellationToken);
            return GetSettings(context.Options, settingsStorage: new StrongBox<StyleCopSettings>(), context.Tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
        }

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
        /// <remarks>
        /// <para>If a <see cref="JsonParseException"/> or <see cref="InvalidSettingsException"/> occurs while
        /// deserializing the settings file, a default settings instance is returned.</para>
        /// </remarks>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <param name="settingsStorage">The storage location for caching an evaluated <see cref="StyleCopSettings"/> object.</param>
        /// <param name="settingsFile">The contents of the StyleCop settnigs file.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxNodeAnalysisContext context, StrongBox<StyleCopSettings> settingsStorage, SettingsFile settingsFile)
        {
            return GetSettings(context.Options, settingsStorage, context.Node.SyntaxTree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
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
        internal static StyleCopSettings GetStyleCopSettingsInCodeFix(this AnalyzerOptions options, SyntaxTree tree, CancellationToken cancellationToken)
        {
            var settingsFile = GetSettingsFile(options, ParseJson, cancellationToken);
            return GetSettings(options, settingsStorage: new StrongBox<StyleCopSettings>(), tree, settingsFile, DeserializationFailureBehavior.ReturnDefaultSettings);
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
            return GetSettings(context.Options, GetOrCreateSettingsStorage(context, tree), tree, settingsFile, failureBehavior);

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

        private static StyleCopSettings GetSettings(AnalyzerOptions options, StrongBox<StyleCopSettings> settingsStorage, SyntaxTree tree, SettingsFile settingsFile, DeserializationFailureBehavior failureBehavior)
        {
            if (settingsStorage.Value is { } settings)
            {
                return settings;
            }

            if (settingsFile != null)
            {
                settings = CreateSettingsObjectFromFile(options, tree, settingsFile, failureBehavior);
            }
            else
            {
                // TODO: Can this really be null? Review when nullable references has been enabled
                if (tree != null)
                {
                    var analyzerConfigOptions = options.AnalyzerConfigOptionsProvider().GetOptions(tree);
                    settings = new StyleCopSettings(new JsonObject(), analyzerConfigOptions);
                }
                else
                {
                    settings = new StyleCopSettings();
                }
            }

            return Interlocked.CompareExchange(ref settingsStorage.Value, settings, null) ?? settings;
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
                    if (additionalTextContent != null)
                    {
                        var content = getJsonValue(additionalTextContent);
                        return new SettingsFile(additionalFile.Path, content);
                    }
                    else
                    {
                        // Failed to read the file! Probably because of a broken link.
                        var content = new Lazy<JsonValue>(() => throw new InvalidSettingsException(
                            $"Settings file at '{additionalFile.Path}' could not be read"));
                        return new SettingsFile(additionalFile.Path, content);
                    }
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
