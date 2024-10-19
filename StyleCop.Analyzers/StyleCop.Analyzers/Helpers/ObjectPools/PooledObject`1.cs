﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers.ObjectPools
{
    // This code was copied from the Roslyn code base (and slightly modified)
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// this is RAII object to automatically release pooled object when its owning pool.
    /// </summary>
    /// <typeparam name="T">The type of the pooled object.</typeparam>
    internal struct PooledObject<T> : IDisposable
        where T : class
    {
        private readonly Action<ObjectPool<T>, T> releaser;
        private readonly ObjectPool<T> pool;
        private T pooledObject;

        public PooledObject(ObjectPool<T> pool, Func<ObjectPool<T>, T> allocator, Action<ObjectPool<T>, T> releaser)
            : this()
        {
            this.pool = pool;
            this.pooledObject = allocator(pool);
            this.releaser = releaser;
        }

        public T Object
        {
            get
            {
                return this.pooledObject;
            }
        }

        public static PooledObject<StringBuilder> Create(ObjectPool<StringBuilder> pool)
        {
            return new PooledObject<StringBuilder>(pool, Allocator, Releaser);
        }

        public static PooledObject<Stack<TItem>> Create<TItem>(ObjectPool<Stack<TItem>> pool)
        {
            return new PooledObject<Stack<TItem>>(pool, Allocator, Releaser);
        }

        public static PooledObject<Queue<TItem>> Create<TItem>(ObjectPool<Queue<TItem>> pool)
        {
            return new PooledObject<Queue<TItem>>(pool, Allocator, Releaser);
        }

        public static PooledObject<HashSet<TItem>> Create<TItem>(ObjectPool<HashSet<TItem>> pool)
        {
            return new PooledObject<HashSet<TItem>>(pool, Allocator, Releaser);
        }

        public static PooledObject<Dictionary<TKey, TValue>> Create<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            return new PooledObject<Dictionary<TKey, TValue>>(pool, Allocator, Releaser);
        }

        public static PooledObject<List<TItem>> Create<TItem>(ObjectPool<List<TItem>> pool)
        {
            return new PooledObject<List<TItem>>(pool, Allocator, Releaser);
        }

        public void Dispose()
        {
            if (this.pooledObject != null)
            {
                this.releaser(this.pool, this.pooledObject);
                this.pooledObject = null;
            }
        }

        private static StringBuilder Allocator(ObjectPool<StringBuilder> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser(ObjectPool<StringBuilder> pool, StringBuilder sb)
        {
            pool.ClearAndFree(sb);
        }

        private static Stack<TItem> Allocator<TItem>(ObjectPool<Stack<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser<TItem>(ObjectPool<Stack<TItem>> pool, Stack<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        private static Queue<TItem> Allocator<TItem>(ObjectPool<Queue<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser<TItem>(ObjectPool<Queue<TItem>> pool, Queue<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        private static HashSet<TItem> Allocator<TItem>(ObjectPool<HashSet<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser<TItem>(ObjectPool<HashSet<TItem>> pool, HashSet<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }

        private static Dictionary<TKey, TValue> Allocator<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser<TKey, TValue>(ObjectPool<Dictionary<TKey, TValue>> pool, Dictionary<TKey, TValue> obj)
        {
            pool.ClearAndFree(obj);
        }

        private static List<TItem> Allocator<TItem>(ObjectPool<List<TItem>> pool)
        {
            return pool.AllocateAndClear();
        }

        private static void Releaser<TItem>(ObjectPool<List<TItem>> pool, List<TItem> obj)
        {
            pool.ClearAndFree(obj);
        }
    }
}
