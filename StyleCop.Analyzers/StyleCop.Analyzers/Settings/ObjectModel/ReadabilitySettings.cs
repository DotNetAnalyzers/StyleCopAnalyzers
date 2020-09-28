// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

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
        protected internal ReadabilitySettings(JsonObject readabilitySettingsObject)
            : this()
        {
            foreach (var kvp in readabilitySettingsObject)
            {
                switch (kvp.Key)
                {
                case "allowBuiltInTypeAliases":
                    this.allowBuiltInTypeAliases = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }
        }

        public bool AllowBuiltInTypeAliases =>
            this.allowBuiltInTypeAliases;
    }
}
