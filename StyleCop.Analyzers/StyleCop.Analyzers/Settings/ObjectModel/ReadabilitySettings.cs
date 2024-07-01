// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;
    using StyleCop.Analyzers.Lightup;

    internal class ReadabilitySettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="AllowBuiltInTypeAliases"/> property.
        /// </summary>
        private readonly bool allowBuiltInTypeAliases;

        /// <summary>
        /// This is the backing field for the <see cref="TreatMultilineParametersAsSplit"/> property.
        /// </summary>
        private readonly bool treatMultilineParametersAsSplit;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class.
        /// </summary>
        protected internal ReadabilitySettings()
        {
            this.allowBuiltInTypeAliases = false;
            this.treatMultilineParametersAsSplit = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class.
        /// </summary>
        /// <param name="readabilitySettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal ReadabilitySettings(JsonObject readabilitySettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            bool? allowBuiltInTypeAliases = null;
            bool? treatMultilineParametersAsSplit = null;

            foreach (var kvp in readabilitySettingsObject)
            {
                switch (kvp.Key)
                {
                case "allowBuiltInTypeAliases":
                    allowBuiltInTypeAliases = kvp.ToBooleanValue();
                    break;

                case "treatMultilineParametersAsSplit":
                    treatMultilineParametersAsSplit = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }

            allowBuiltInTypeAliases ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.readability.allowBuiltInTypeAliases");
            treatMultilineParametersAsSplit ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.readability.treatMultilineParametersAsSplit");

            this.allowBuiltInTypeAliases = allowBuiltInTypeAliases.GetValueOrDefault(false);
            this.treatMultilineParametersAsSplit = treatMultilineParametersAsSplit.GetValueOrDefault(false);
        }

        public bool AllowBuiltInTypeAliases =>
            this.allowBuiltInTypeAliases;

        public bool TreatMultilineParametersAsSplit =>
            this.treatMultilineParametersAsSplit;
    }
}
