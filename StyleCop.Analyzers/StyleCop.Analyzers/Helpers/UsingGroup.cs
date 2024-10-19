// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    /// <summary>
    /// Using directive group type definitions.
    /// </summary>
    internal enum UsingGroup
    {
        /// <summary>
        /// The using directive group type is not known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The using directive is part of the system usings group.
        /// </summary>
        System,

        /// <summary>
        /// The using directive is part of the regular usings group.
        /// </summary>
        Regular,

        /// <summary>
        /// The using directive is part of the static usings group.
        /// </summary>
        Static,

        /// <summary>
        /// The using directive is part of the alias usings group.
        /// </summary>
        Alias,
    }
}
