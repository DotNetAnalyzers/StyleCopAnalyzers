// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the allowed splitting of attribute parameters across lines.
    /// </summary>
    internal enum AttributeParameterSplitting
    {
        /// <summary>
        /// Splitting conventions for attribute parameters follow the defaults for methods.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Attribute parameters may be split across lines in any manner.
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// When splitting attribute parameters across lines,
        /// any subset of the positional parameters may reside on the first line.
        /// </summary>
        PositionalParametersMayShareFirstLine = 2
    }
}
