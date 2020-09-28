// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public enum SA1402SettingsConfiguration
    {
        /// <summary>
        /// Provide no custom settings.
        /// </summary>
        KeepDefaultConfiguration,

        /// <summary>
        /// Provide custom settings that configure the tested type as being a top-level type.
        /// </summary>
        ConfigureAsTopLevelType,

        /// <summary>
        /// Provide custom settings that configure the tested type as not being a top-level type.
        /// </summary>
        ConfigureAsNonTopLevelType,
    }
}
