// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup;

internal enum NullableFlowState : byte
{
    /// <summary>
    /// The value is not available or the expression has not been analyzed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The expression is known not to be null.
    /// </summary>
    NotNull = 1,

    /// <summary>
    /// The expression may be null.
    /// </summary>
    MaybeNull = 2,
}
