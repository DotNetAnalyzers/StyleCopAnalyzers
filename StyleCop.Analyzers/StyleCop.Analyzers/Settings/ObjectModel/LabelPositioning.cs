// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    internal enum LabelPositioning
    {
        /// <summary>
        /// Placed in the Zeroth column of the text editor.
        /// </summary>
        LeftMost,

        /// <summary>
        /// Placed at one less indent to the current context.
        /// </summary>
        OneLess,

        /// <summary>
        /// Placed at the same indent as the current context.
        /// </summary>
        NoIndent,
    }
}
