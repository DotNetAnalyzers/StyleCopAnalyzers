// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers.ObjectPools
{
    // This code was copied from the Roslyn code base (and slightly modified)
    using System.Text;

    internal static class StringBuilderPool
    {
        public static StringBuilder Allocate()
        {
            return SharedPools.Default<StringBuilder>().AllocateAndClear();
        }

        public static void Free(StringBuilder builder)
        {
            SharedPools.Default<StringBuilder>().ClearAndFree(builder);
        }

        public static string ReturnAndFree(StringBuilder builder)
        {
            SharedPools.Default<StringBuilder>();
            return builder.ToString();
        }
    }
}
