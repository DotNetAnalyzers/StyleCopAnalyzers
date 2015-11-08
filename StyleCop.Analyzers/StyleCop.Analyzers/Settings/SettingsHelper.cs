// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Newtonsoft.Json;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class that manages the settings files for StyleCopAnalyzers.
    /// </summary>
    internal static class SettingsHelper
    {
        private const string SettingsFileName = "stylecop.json";

        private static Func<string, bool> fileExists;
        private static Func<string, string> fileReadAllText;

        /// <summary>
        /// Gets the StyleCop settings.
        /// </summary>
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
        /// <param name="options">The analyzer options that will be used to determine the StyleCop settings.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this AnalyzerOptions options, CancellationToken cancellationToken)
        {
            return GetStyleCopSettings(options != null ? options.AdditionalFiles : ImmutableArray.Create<AdditionalText>(), cancellationToken);
        }

        private static StyleCopSettings GetStyleCopSettings(ImmutableArray<AdditionalText> additionalFiles, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var additionalFile in additionalFiles)
                {
                    if (Path.GetFileName(additionalFile.Path).ToLowerInvariant() == SettingsFileName)
                    {
                        string additionalTextContent = ReadAdditionalText(additionalFile, cancellationToken);
                        var root = JsonConvert.DeserializeObject<SettingsFile>(additionalTextContent);
                        return root.Settings;
                    }
                }
            }
            catch (JsonException)
            {
                // The settings file is invalid -> return the default settings.
            }

            return new StyleCopSettings();
        }

        /// <summary>
        /// This code works around dotnet/roslyn#6596 by using the file system APIs instead of the Roslyn APIs to read
        /// the additional text. If the file system APIs are not available, the code falls back to the previous
        /// behavior.
        /// </summary>
        /// <param name="additionalText">The additional text to read.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>The content of the additional text as a string.</returns>
        private static string ReadAdditionalText(AdditionalText additionalText, CancellationToken cancellationToken)
        {
            if (fileExists == null)
            {
                Type fileClass = typeof(string).GetTypeInfo().Assembly.GetType("System.IO.File");
                if (fileClass != null)
                {
                    MethodInfo readAllText = fileClass.GetRuntimeMethod("ReadAllText", new[] { typeof(string) });
                    MethodInfo exists = fileClass.GetRuntimeMethod("Exists", new[] { typeof(string) });
                    if (readAllText != null && exists != null)
                    {
                        Interlocked.CompareExchange(ref fileReadAllText, (Func<string, string>)readAllText.CreateDelegate(typeof(Func<string, string>)), null);
                        Interlocked.CompareExchange(ref fileExists, (Func<string, bool>)exists.CreateDelegate(typeof(Func<string, bool>)), null);
                    }
                }

                if (fileExists == null)
                {
                    // this special case allows for a clean fall back to AdditionalText.GetText()
                    fileExists = _ => false;
                }
            }

            try
            {
                if (fileExists(additionalText.Path))
                {
                    return fileReadAllText(additionalText.Path);
                }
            }
            catch (IOException)
            {
                // fall back to AdditionalFile.GetText()
            }

            return additionalText.GetText(cancellationToken).ToString();
        }
    }
}
