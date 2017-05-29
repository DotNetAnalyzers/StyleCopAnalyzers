// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class ReadabilitySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class during JSON deserialization.
        /// </summary>
        protected internal ReadabilitySettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class.
        /// </summary>
        /// <param name="readabilitySettingsObject">The JSON object containing the settings.</param>
        protected internal ReadabilitySettings(JsonObject readabilitySettingsObject)
            : this()
        {
        }
    }
}
