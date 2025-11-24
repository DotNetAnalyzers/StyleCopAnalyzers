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
            bool? allowCommonHungarianPrefixes = null;
            ImmutableArray<string>.Builder allowedHungarianPrefixes = null;
            ImmutableArray<string>.Builder allowedNamespaceComponents = null;
            bool? includeInferredTupleElementNames = null;
            TupleElementNameCase? tupleElementNameCasing = null;

            foreach (var kvp in namingSettingsObject)
            {
                switch (kvp.Key)
                {
                case "allowCommonHungarianPrefixes":
                    allowCommonHungarianPrefixes = kvp.ToBooleanValue();
                    break;

                case "allowedHungarianPrefixes":
                    kvp.AssertIsArray();
                    allowedHungarianPrefixes = ImmutableArray.CreateBuilder<string>();
                    foreach (var prefixJsonValue in kvp.Value.AsJsonArray)
                    {
                        var prefix = prefixJsonValue.ToStringValue(kvp.Key);

                        if (!IsValidHungarianPrefix(prefix))
                        {
                            continue;
                        }

                        allowedHungarianPrefixes.Add(prefix);
                    }

                    break;

                case "allowedNamespaceComponents":
                    kvp.AssertIsArray();
                    allowedNamespaceComponents = ImmutableArray.CreateBuilder<string>();
                    allowedNamespaceComponents.AddRange(kvp.Value.AsJsonArray.Select(static x => x.ToStringValue("allowedNamespaceComponents")));
                    break;

                case "includeInferredTupleElementNames":
                    includeInferredTupleElementNames = kvp.ToBooleanValue();
                    break;

                case "tupleElementNameCasing":
                    tupleElementNameCasing = kvp.ToEnumValue<TupleElementNameCase>();
                    break;

                default:
                    break;
                }
            }

            allowCommonHungarianPrefixes ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.naming.allowCommonHungarianPrefixes");
            allowedHungarianPrefixes ??= AnalyzerConfigHelper.TryGetStringListValue(analyzerConfigOptions, "stylecop.naming.allowedHungarianPrefixes")
                ?.Where(static value => IsValidHungarianPrefix(value))
                .ToImmutableArray()
                .ToBuilder();
            allowedNamespaceComponents ??= AnalyzerConfigHelper.TryGetStringListValue(analyzerConfigOptions, "stylecop.naming.allowedNamespaceComponents")?.ToBuilder();
            includeInferredTupleElementNames ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.naming.includeInferredTupleElementNames");
            tupleElementNameCasing ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.naming.tupleElementNameCasing") switch
            {
                "camelCase" => TupleElementNameCase.CamelCase,
                "pascalCase" => TupleElementNameCase.PascalCase,
                _ => null,
            };

            this.AllowCommonHungarianPrefixes = allowCommonHungarianPrefixes.GetValueOrDefault(true);
            this.AllowedHungarianPrefixes = allowedHungarianPrefixes?.ToImmutable() ?? ImmutableArray<string>.Empty;
            this.AllowedNamespaceComponents = allowedNamespaceComponents?.ToImmutable() ?? ImmutableArray<string>.Empty;

            this.IncludeInferredTupleElementNames = includeInferredTupleElementNames.GetValueOrDefault(false);
            this.TupleElementNameCasing = tupleElementNameCasing.GetValueOrDefault(TupleElementNameCase.PascalCase);
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
