namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Class that manages the settings files for StyleCopAnalyzers.
    /// </summary>
    internal static class SettingsHelper
    {
        private const string SettingsFileName = "stylecop.json";
        private const string DefaultCompanyName = "PlaceHolderCompany";

        private const string SettingsToken = "settings";
        private const string DocumentationRulesToken = "documentationRules";
        private const string CompanyNameToken = "companyName";
        private const string CopyrightTextToken = "copyrightText";

        /// <summary>
        /// Gets the stylecop settings.
        /// </summary>
        /// <param name="context">The context that will be used to determine the StyleCop settings.</param>
        /// <returns>A <see cref="StyleCopSettings"/> instance that represents the StyleCop settings for the given context.</returns>
        internal static StyleCopSettings GetStyleCopSettings(this SyntaxTreeAnalysisContext context)
        {
            return GetStyleCopSettings(context.Options != null ? context.Options.AdditionalFiles : ImmutableArray.Create<AdditionalText>());
        }

        private static StyleCopSettings GetStyleCopSettings(ImmutableArray<AdditionalText> additionalFiles)
        {
            try
            {
                foreach (var additionalFile in additionalFiles)
                {
                    if (Path.GetFileName(additionalFile.Path).ToLowerInvariant() == SettingsFileName)
                    {
                        var root = JObject.Parse(additionalFile.GetText().ToString());

                        var companyName = (string)root.SelectToken($"{SettingsToken}.{DocumentationRulesToken}.{CompanyNameToken}") ?? DefaultCompanyName;
                        var copyrightText = (string)root.SelectToken($"{SettingsToken}.{DocumentationRulesToken}.{CopyrightTextToken}") ?? GenerateDefaultCopyrightText(companyName);

                        return new StyleCopSettings(companyName, copyrightText ?? GenerateDefaultCopyrightText(companyName));
                    }
                }
            }
            catch (JsonException)
            {
                // The settings file is invalid -> return the default settings.
            }

            return new StyleCopSettings(DefaultCompanyName, GenerateDefaultCopyrightText(DefaultCompanyName));
        }

        private static string GenerateDefaultCopyrightText(string companyName)
        {
            return $"Copyright (c) {companyName}. All rights reserved.";
        }
    }
}
