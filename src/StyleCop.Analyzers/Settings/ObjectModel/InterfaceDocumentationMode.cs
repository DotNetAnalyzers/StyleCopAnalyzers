// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the documentation requirements for interfaces.
    /// </summary>
    internal enum InterfaceDocumentationMode
    {
        /// <summary>
        /// All interfaces should be documented.
        /// </summary>
        All,

        /// <summary>
        /// Only externally visible interfaces should be documented.
        /// </summary>
        Exposed,

        /// <summary>
        /// Interface documentation is not required.
        /// </summary>
        None,
    }
}
