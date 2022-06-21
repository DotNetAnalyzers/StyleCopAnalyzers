// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    internal enum OrderingTrait
    {
        /// <summary>
        /// Elements are ordered according to their kind.
        /// </summary>
        Kind,

        /// <summary>
        /// Code elements are ordered according to their declared accessibility.
        /// </summary>
        Accessibility,

        /// <summary>
        /// Constant elements are ordered before non-constant elements.
        /// </summary>
        Constant,

        /// <summary>
        /// Static elements are ordered before non-static elements.
        /// </summary>
        Static,

        /// <summary>
        /// Readonly elements are ordered before non-readonly elements.
        /// </summary>
        Readonly,
    }
}
