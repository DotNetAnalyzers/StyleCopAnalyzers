// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;
    using StyleCop.Analyzers.Lightup;

    internal class LayoutSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="NewlineAtEndOfFile"/> property.
        /// </summary>
        private readonly OptionSetting newlineAtEndOfFile;

        /// <summary>
        /// This is the backing field for the <see cref="AllowConsecutiveUsings"/> property.
        /// </summary>
        private readonly bool allowConsecutiveUsings;

        /// <summary>
        /// This is the backing field of the <see cref="AllowDoWhileOnClosingBrace"/> property.
        /// </summary>
        private readonly bool allowDoWhileOnClosingBrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        protected internal LayoutSettings()
        {
            this.newlineAtEndOfFile = OptionSetting.Allow;
            this.allowConsecutiveUsings = true;
            this.allowDoWhileOnClosingBrace = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        /// <param name="layoutSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal LayoutSettings(JsonObject layoutSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            OptionSetting? newlineAtEndOfFile = null;
            bool? allowConsecutiveUsings = null;
            bool? allowDoWhileOnClosingBrace = null;

            foreach (var kvp in layoutSettingsObject)
            {
                switch (kvp.Key)
                {
                case "newlineAtEndOfFile":
                    newlineAtEndOfFile = kvp.ToEnumValue<OptionSetting>();
                    break;

                case "allowConsecutiveUsings":
                    allowConsecutiveUsings = kvp.ToBooleanValue();
                    break;

                case "allowDoWhileOnClosingBrace":
                    allowDoWhileOnClosingBrace = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }

            newlineAtEndOfFile ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "insert_final_newline") switch
            {
                true => OptionSetting.Require,
                false => OptionSetting.Omit,
                _ => null,
            };

            allowConsecutiveUsings ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.layout.allowConsecutiveUsings");
            allowDoWhileOnClosingBrace ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.layout.allowDoWhileOnClosingBrace");

            this.newlineAtEndOfFile = newlineAtEndOfFile.GetValueOrDefault(OptionSetting.Allow);
            this.allowConsecutiveUsings = allowConsecutiveUsings.GetValueOrDefault(true);
            this.allowDoWhileOnClosingBrace = allowDoWhileOnClosingBrace.GetValueOrDefault(false);
        }

        public OptionSetting NewlineAtEndOfFile =>
            this.newlineAtEndOfFile;

        public bool AllowConsecutiveUsings =>
            this.allowConsecutiveUsings;

        public bool AllowDoWhileOnClosingBrace =>
            this.allowDoWhileOnClosingBrace;
    }
}
