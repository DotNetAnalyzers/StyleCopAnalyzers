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
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class.
        /// </summary>
        protected internal ReadabilitySettings()
        {
            this.allowBuiltInTypeAliases = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class.
        /// </summary>
        /// <param name="readabilitySettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal ReadabilitySettings(JsonObject readabilitySettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var reader = new SettingsReader(readabilitySettingsObject, analyzerConfigOptions);

            this.allowBuiltInTypeAliases = reader.GetBoolean("allowBuiltInTypeAliases", "stylecop.readability.allowBuiltInTypeAliases").GetValueOrDefault(false);
        }

        public bool AllowBuiltInTypeAliases =>
            this.allowBuiltInTypeAliases;
    }
}
