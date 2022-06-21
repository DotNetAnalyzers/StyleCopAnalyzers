// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the possible values for an option.
    /// </summary>
    internal enum OptionSetting
    {
        /// <summary>
        /// The option is allowed, but not required.
        /// </summary>
        Allow,

        /// <summary>
        /// The option is required.
        /// </summary>
        Require,

        /// <summary>
        /// The option is not allowed.
        /// </summary>
        Omit,
    }
}
