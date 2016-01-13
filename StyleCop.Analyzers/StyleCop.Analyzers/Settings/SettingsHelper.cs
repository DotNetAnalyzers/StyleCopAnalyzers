// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
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

        private static readonly bool AvoidAdditionalTextGetText;

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, FieldInfo>> FieldInfos =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, FieldInfo>>();

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> PropertyInfos =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>>();

        static SettingsHelper()
        {
            // dotnet/roslyn#6596 was fixed for Roslyn 1.2
            AvoidAdditionalTextGetText = typeof(AdditionalText).GetTypeInfo().Assembly.GetName().Version < new Version(1, 2, 0, 0);
        }

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

        private static StyleCopSettings GetStyleCopSettings(ImmutableArray<AdditionalText> additionalFiles, DeserializationFailureBehavior failureBehavior, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var additionalFile in additionalFiles)
                {
                    if (Path.GetFileName(additionalFile.Path).ToLowerInvariant() == SettingsFileName)
                    {
                        SourceText additionalTextContent = GetText(additionalFile, cancellationToken);
                        var root = JsonConvert.DeserializeObject<SettingsFile>(additionalTextContent.ToString());
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

        /// <summary>
        /// This code works around dotnet/roslyn#6596 by using reflection APIs to bypass the problematic method while
        /// reading the content of an <see cref="AdditionalText"/> file. If the reflection approach fails, the code
        /// falls back to the previous behavior.
        /// </summary>
        /// <param name="additionalText">The additional text to read.</param>
        /// <param name="cancellationToken">The cancellation token that the operation will observe.</param>
        /// <returns>The content of the additional text file.</returns>
        private static SourceText GetText(AdditionalText additionalText, CancellationToken cancellationToken)
        {
            if (AvoidAdditionalTextGetText)
            {
                object document = GetField(additionalText, "_document");
                if (document != null)
                {
                    object textSource = GetField(document, "textSource");
                    if (textSource != null)
                    {
                        object textAndVersion = CallMethod(textSource, "GetValue", new[] { typeof(CancellationToken) }, cancellationToken);
                        if (textAndVersion != null)
                        {
                            SourceText text = GetProperty(textAndVersion, "Text") as SourceText;
                            if (text != null)
                            {
                                return text;
                            }
                        }
                    }
                }
            }

            return additionalText.GetText(cancellationToken);
        }

        private static object GetField(object obj, string name)
        {
            if (obj == null)
            {
                return null;
            }

            ConcurrentDictionary<string, FieldInfo> fieldsForType = FieldInfos.GetOrAdd(obj.GetType(), _ => new ConcurrentDictionary<string, FieldInfo>());
            FieldInfo fieldInfo;
            if (!fieldsForType.TryGetValue(name, out fieldInfo))
            {
                fieldInfo = fieldsForType.GetOrAdd(name, _ => obj.GetType().GetRuntimeFields().FirstOrDefault(i => i.Name == name));
            }

            return fieldInfo?.GetValue(obj);
        }

        private static object CallMethod(object obj, string name, Type[] parameters, params object[] arguments)
        {
            try
            {
                MethodInfo methodInfo = obj?.GetType().GetRuntimeMethod(name, parameters);
                return methodInfo?.Invoke(obj, arguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        private static object GetProperty(object obj, string name)
        {
            if (obj == null)
            {
                return null;
            }

            ConcurrentDictionary<string, PropertyInfo> propertiesForType = PropertyInfos.GetOrAdd(obj.GetType(), _ => new ConcurrentDictionary<string, PropertyInfo>());
            PropertyInfo propertyInfo;
            if (!propertiesForType.TryGetValue(name, out propertyInfo))
            {
                propertyInfo = propertiesForType.GetOrAdd(name, _ => obj.GetType().GetRuntimeProperty(name));
            }

            try
            {
                return propertyInfo?.GetValue(obj);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
