// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers.ObjectPools
{
    // This code was copied from the Roslyn code base (and slightly modified)

    /// <summary>
    /// Shared object pool for roslyn
    ///
    /// Use this shared pool if only concern is reducing object allocations.
    /// if perf of an object pool itself is also a concern, use ObjectPool directly.
    ///
    /// For example, if you want to create a million of small objects within a second,
    /// use the ObjectPool directly. it should have much less overhead than using this.
    /// </summary>
    internal static class SharedPools
    {
        /// <summary>
        /// pool that uses default constructor with 100 elements pooled
        /// </summary>
        /// <typeparam name="T">The type of the object pool.</typeparam>
        /// <returns>A default big object pool.</returns>
        public static ObjectPool<T> BigDefault<T>()
            where T : class, new()
        {
            return DefaultBigPool<T>.Instance;
        }

        /// <summary>
        /// pool that uses default constructor with 20 elements pooled
        /// </summary>
        /// <typeparam name="T">The type of the object pool.</typeparam>
        /// <returns>A default object pool.</returns>
        public static ObjectPool<T> Default<T>()
            where T : class, new()
        {
            return DefaultNormalPool<T>.Instance;
        }

        private static class DefaultBigPool<T>
            where T : class, new()
        {
            public static ObjectPool<T> Instance { get; } =
                new ObjectPool<T>(() => new T(), 100);
        }

        private static class DefaultNormalPool<T>
            where T : class, new()
        {
            public static ObjectPool<T> Instance { get; } =
                new ObjectPool<T>(() => new T(), 20);
        }
    }
}
