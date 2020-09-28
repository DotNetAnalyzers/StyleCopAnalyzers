// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    internal enum TopLevelType
    {
        /// <summary>
        /// Classes are considered top-level types.
        /// </summary>
        Class,

        /// <summary>
        /// Interfaces are considered top-level types.
        /// </summary>
        Interface,

        /// <summary>
        /// Structs are considered top-level types.
        /// </summary>
        Struct,

        /// <summary>
        /// Delegates are considered top-level types.
        /// </summary>
        Delegate,

        /// <summary>
        /// Enums are considered top-level types.
        /// </summary>
        Enum,
    }
}
