// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class MaintainabilitySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        protected internal MaintainabilitySettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        /// <param name="maintainabilitySettingsObject">The JSON object containing the settings.</param>
        protected internal MaintainabilitySettings(JsonObject maintainabilitySettingsObject)
            : this()
        {
        }
    }
}
