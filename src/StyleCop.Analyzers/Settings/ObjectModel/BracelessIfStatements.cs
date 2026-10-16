// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel;

/// <summary>
/// Specifies where a standalone if statement may have a braceless jump statement.
/// </summary>
internal enum BracelessIfStatements
{
    /// <summary>
    /// Braces are required.
    /// </summary>
    Off,

    /// <summary>
    /// The jump statement may appear on the same line as the closing parenthesis.
    /// </summary>
    SameLine,

    /// <summary>
    /// The jump statement may appear on the following line.
    /// </summary>
    NextLine,
}
