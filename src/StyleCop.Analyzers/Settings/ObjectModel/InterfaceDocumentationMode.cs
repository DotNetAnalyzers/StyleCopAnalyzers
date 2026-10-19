// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies additional documentation requirements for interfaces.
    /// </summary>
    internal enum InterfaceDocumentationMode
    {
        /// <summary>
        /// All interfaces should be documented.
        /// </summary>
        All,

        /// <summary>
        /// Externally visible interfaces require documentation in addition to the general settings.
        /// </summary>
        Exposed,

        /// <summary>
        /// No additional documentation requirements are imposed on interfaces.
        /// </summary>
        None,
    }
}
