// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    internal enum FileNamingConvention
    {
        /// <summary>
        /// Files are named using the StyleCop convention (e.g. <c>TypeName{T1,T2}</c>).
        /// </summary>
        StyleCop,

        /// <summary>
        /// Files are named using the metadata convention (e.g. <c>TypeName`2</c>).
        /// </summary>
        Metadata,
    }
}
