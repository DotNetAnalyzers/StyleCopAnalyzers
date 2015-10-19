// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers.ObjectPools
{
    // This code was copied from the Roslyn code base (and slightly modified)
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Generic implementation of object pooling pattern with predefined pool size limit. The main
    /// purpose is that limited number of frequently used objects can be kept in the pool for
    /// further recycling.
    ///
    /// Notes:
    /// 1) it is not the goal to keep all returned objects. Pool is not meant for storage. If there
    ///    is no space in the pool, extra returned objects will be dropped.
    ///
    /// 2) it is implied that if object was obtained from a pool, the caller will return it back in
    ///    a relatively short time. Keeping checked out objects for long durations is ok, but
    ///    reduces usefulness of pooling. Just new up your own.
    ///
    /// Not returning objects to the pool in not detrimental to the pool's work, but is a bad practice.
    /// Rationale:
    ///    If there is no intent for reusing the object, do not use pool - just use "new".
    /// </summary>
    /// <typeparam name="T">The type of the objects in this cache.</typeparam>
    internal class ObjectPool<T>
            where T : class
    {
        private readonly Element[] items;

        // factory is stored for the lifetime of the pool. We will call this only when pool needs to
        // expand. compared to "new T()", Func gives more flexibility to implementers and faster
        // than "new T()".
        private readonly Func<T> factory;

        // Storage for the pool objects. The first item is stored in a dedicated field because we
        // expect to be able to satisfy most requests from it.
        private T firstItem;

        internal ObjectPool(Func<T> factory)
            : this(factory, Environment.ProcessorCount * 2)
        {
        }

        internal ObjectPool(Func<T> factory, int size)
        {
            Debug.Assert(size >= 1, "The object pool can't be empty");
            this.factory = factory;
            this.items = new Element[size - 1];
        }

        /// <summary>
        /// Produces an instance.
        /// </summary>
        /// <remarks>
        /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
        /// Note that Free will try to store recycled objects close to the start thus statistically
        /// reducing how far we will typically search.
        /// </remarks>
        /// <returns>A (possibly) cached instance of type <typeparamref name="T"/>.</returns>
        internal T Allocate()
        {
            // PERF: Examine the first element. If that fails, AllocateSlow will look at the remaining elements.
            // Note that the initial read is optimistically not synchronized. That is intentional.
            // We will interlock only when we have a candidate. in a worst case we may miss some
            // recently returned objects. Not a big deal.
            T inst = this.firstItem;
            if (inst == null || inst != Interlocked.CompareExchange(ref this.firstItem, null, inst))
            {
                inst = this.AllocateSlow();
            }

            return inst;
        }

        /// <summary>
        /// Returns objects to the pool.
        /// </summary>
        /// <remarks>
        /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
        /// Note that Free will try to store recycled objects close to the start thus statistically
        /// reducing how far we will typically search in Allocate.
        /// </remarks>
        /// <param name="obj">The object to free.</param>
        internal void Free(T obj)
        {
            if (this.firstItem == null)
            {
                // Intentionally not using interlocked here.
                // In a worst case scenario two objects may be stored into same slot.
                // It is very unlikely to happen and will only mean that one of the objects will get collected.
                this.firstItem = obj;
            }
            else
            {
                this.FreeSlow(obj);
            }
        }

        private T CreateInstance()
        {
            var inst = this.factory();
            return inst;
        }

        private T AllocateSlow()
        {
            var items = this.items;

            for (int i = 0; i < items.Length; i++)
            {
                // Note that the initial read is optimistically not synchronized. That is intentional.
                // We will interlock only when we have a candidate. in a worst case we may miss some
                // recently returned objects. Not a big deal.
                T inst = items[i].Value;
                if (inst != null)
                {
                    if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
                    {
                        return inst;
                    }
                }
            }

            return this.CreateInstance();
        }

        private void FreeSlow(T obj)
        {
            var items = this.items;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Value == null)
                {
                    // Intentionally not using interlocked here.
                    // In a worst case scenario two objects may be stored into same slot.
                    // It is very unlikely to happen and will only mean that one of the objects will get collected.
                    items[i].Value = obj;
                    break;
                }
            }
        }

        [DebuggerDisplay("{Value,nq}")]
        private struct Element
        {
            internal T Value;
        }
    }
}
