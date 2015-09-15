// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class SettingsFile
    {
        /// <summary>
        /// This is the backing field for the <see cref="Settings"/> property.
        /// </summary>
        [JsonProperty("settings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private StyleCopSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFile"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SettingsFile()
        {
            this.settings = new StyleCopSettings();
        }

        public StyleCopSettings Settings
        {
            get
            {
                return this.settings;
            }
        }
    }
}
