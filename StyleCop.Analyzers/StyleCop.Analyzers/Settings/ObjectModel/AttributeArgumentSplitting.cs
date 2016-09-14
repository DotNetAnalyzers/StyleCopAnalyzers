// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the allowed splitting of attribute arguments across lines.
    /// </summary>
    internal enum AttributeArgumentSplitting
    {
        /// <summary>
        /// Splitting conventions for attribute arguments follow the defaults for methods.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Attribute arguments may be split across lines in any manner.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// When splitting attribute arguments across lines, the positional arguments may reside
        /// on the first line even if the named arguments reside on subsequent lines.
        /// </summary>
        PositionalParametersMayShareFirstLine = 2
    }
}
