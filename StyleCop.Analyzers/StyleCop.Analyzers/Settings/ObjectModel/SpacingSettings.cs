// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class SpacingSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpacingSettings"/> class during JSON deserialization.
        /// </summary>
        protected internal SpacingSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpacingSettings"/> class.
        /// </summary>
        /// <param name="spacingSettingsObject">The JSON object containing the settings.</param>
        protected internal SpacingSettings(JsonObject spacingSettingsObject)
            : this()
        {
            if (spacingSettingsObject.Count > 0)
            {
                throw new InvalidSettingsException($"spacingRules should not contain any child objects");
            }
        }
    }
}
