// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

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
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        protected internal LayoutSettings()
        {
            this.newlineAtEndOfFile = OptionSetting.Allow;
            this.allowConsecutiveUsings = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        /// <param name="layoutSettingsObject">The JSON object containing the settings.</param>
        protected internal LayoutSettings(JsonObject layoutSettingsObject)
            : this()
        {
            foreach (var kvp in layoutSettingsObject)
            {
                switch (kvp.Key)
                {
                case "newlineAtEndOfFile":
                    this.newlineAtEndOfFile = kvp.ToEnumValue<OptionSetting>();
                    break;

                case "allowConsecutiveUsings":
                    this.allowConsecutiveUsings = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }
        }

        public OptionSetting NewlineAtEndOfFile =>
            this.newlineAtEndOfFile;

        public bool AllowConsecutiveUsings =>
            this.allowConsecutiveUsings;
    }
}
