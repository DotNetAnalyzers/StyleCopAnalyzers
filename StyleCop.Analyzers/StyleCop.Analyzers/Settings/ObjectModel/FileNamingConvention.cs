// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    internal enum FileNamingConvention
    {
        /// <summary>
        /// Files are named using the StyleCop convention (e.g. TypeName{T1,T2})
        /// </summary>
        StyleCop,

        /// <summary>
        /// Files are named using the metadata convention (e.g. TypeName`2)
        /// </summary>
        Metadata
    }
}
