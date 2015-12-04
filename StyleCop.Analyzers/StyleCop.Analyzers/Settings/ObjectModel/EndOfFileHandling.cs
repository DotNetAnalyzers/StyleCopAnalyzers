// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the handling for newline characters which appear at the end of a file.
    /// </summary>
    internal enum EndOfFileHandling
    {
        /// <summary>
        /// Files are allowed to end with a single newline character, but it is not required.
        /// </summary>
        Allow,

        /// <summary>
        /// Files are required to end with a single newline character.
        /// </summary>
        Require,

        /// <summary>
        /// Files may not end with a newline character.
        /// </summary>
        Omit
    }
}
