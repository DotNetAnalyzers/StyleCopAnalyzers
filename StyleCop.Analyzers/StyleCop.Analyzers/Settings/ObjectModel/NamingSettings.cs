// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using System.Linq;
    using LightJson;
    using StyleCop.Analyzers.Lightup;

    internal class NamingSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamingSettings"/> class.
        /// </summary>
        protected internal NamingSettings()
        {
            this.AllowCommonHungarianPrefixes = true;
            this.AllowedHungarianPrefixes = ImmutableArray<string>.Empty;
            this.AllowedNamespaceComponents = ImmutableArray<string>.Empty;

            this.IncludeInferredTupleElementNames = false;
            this.TupleElementNameCasing = TupleElementNameCase.PascalCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamingSettings"/> class.
        /// </summary>
        /// <param name="namingSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal NamingSettings(JsonObject namingSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var reader = new SettingsReader(namingSettingsObject, analyzerConfigOptions);

            this.AllowCommonHungarianPrefixes = reader.GetBoolean("allowCommonHungarianPrefixes", "stylecop.naming.allowCommonHungarianPrefixes").GetValueOrDefault(true);
            this.AllowedHungarianPrefixes = reader.GetStringList("allowedHungarianPrefixes", "stylecop.naming.allowedHungarianPrefixes", static value => IsValidHungarianPrefix(value)) ?? ImmutableArray<string>.Empty;
            this.AllowedNamespaceComponents = reader.GetStringList("allowedNamespaceComponents", "stylecop.naming.allowedNamespaceComponents") ?? ImmutableArray<string>.Empty;

            this.IncludeInferredTupleElementNames = reader.GetBoolean("includeInferredTupleElementNames", "stylecop.naming.includeInferredTupleElementNames").GetValueOrDefault(false);
            this.TupleElementNameCasing = reader.GetMapped<TupleElementNameCase>(
                "tupleElementNameCasing",
                kvp => kvp.ToEnumValue<TupleElementNameCase>(),
                "stylecop.naming.tupleElementNameCasing",
                value => value switch
                {
                    "camelCase" => TupleElementNameCase.CamelCase,
                    "pascalCase" => TupleElementNameCase.PascalCase,
                    _ => null,
                }).GetValueOrDefault(TupleElementNameCase.PascalCase);
        }

        public bool AllowCommonHungarianPrefixes { get; }

        public ImmutableArray<string> AllowedHungarianPrefixes { get; }

        public ImmutableArray<string> AllowedNamespaceComponents { get; }

        public bool IncludeInferredTupleElementNames { get; }

        public TupleElementNameCase TupleElementNameCasing { get; }

        private static bool IsValidHungarianPrefix(string prefix)
        {
            // Equivalent to Regex.IsMatch(prefix, "^[a-z]{1,2}$")
            for (var i = 0; i < prefix.Length; i++)
            {
                if (prefix[i] is not (>= 'a' and <= 'z'))
                {
                    return false;
                }
            }

            return prefix.Length is (>= 1 and <= 2);
        }
    }
}
