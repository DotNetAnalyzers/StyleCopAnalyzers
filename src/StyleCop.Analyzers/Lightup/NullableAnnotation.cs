// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    internal enum NullableAnnotation : byte
    {
        /// <summary>
        /// The value is not available or the expression has not been analyzed.
        /// </summary>
        None = 0,

        /// <summary>
        /// The expression is not annotated as nullable.
        /// </summary>
        NotAnnotated = 1,

        /// <summary>
        /// The expression is annotated as nullable.
        /// </summary>
        Annotated = 2,
    }
}
