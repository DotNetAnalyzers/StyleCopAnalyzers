// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the desired placement of using directives.
    /// </summary>
    internal enum UsingDirectivesPlacement
    {
        /// <summary>
        /// Place using directives inside the namespace definition.
        /// </summary>
        InsideNamespace,

        /// <summary>
        /// Place using directives outside the namespace definition.
        /// </summary>
        OutsideNamespace,

        /// <summary>
        /// Allow using directives inside or outside the namespace definition.
        /// </summary>
        Preserve,
    }
}
