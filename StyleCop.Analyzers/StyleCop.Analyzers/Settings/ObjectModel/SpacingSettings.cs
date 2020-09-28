// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class SpacingSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpacingSettings"/> class.
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
            // Currently unused
            _ = spacingSettingsObject;
        }
    }
}
