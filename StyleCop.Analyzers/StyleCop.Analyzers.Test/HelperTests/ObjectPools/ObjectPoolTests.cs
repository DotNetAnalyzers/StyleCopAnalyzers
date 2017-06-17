// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests.ObjectPools
{
    using System;
    using Analyzers.Helpers.ObjectPools;
    using Xunit;

    public class ObjectPoolTests
    {
        [Fact]
        public void TestDefaultConstructor()
        {
            Func<object> factory = () => new object();
            var pool = new ObjectPool<object>(factory);
            Assert.IsType<object>(pool.Allocate());
        }

        [Fact]
        public void TestAllocateFree()
        {
            Func<object> factory = () => new object();
            var pool = new ObjectPool<object>(factory);

            // Covers the case where no item is in the pool
            Assert.IsType<object>(pool.Allocate());

            // Covers the case where the item is the first in the pool
            var obj = new object();
            pool.Free(obj);
            Assert.Same(obj, pool.Allocate());

            // Covers the case where the item is not the first in the pool
            var obj2 = new object();
            pool.Free(obj);
            pool.Free(obj2);
            Assert.Same(obj, pool.Allocate());
            Assert.Same(obj2, pool.Allocate());

            // Covers the case (in AllocateSlow and FreeSlow) where the item is not the first or second in the pool
            var obj3 = new object();
            pool.Free(obj);
            pool.Free(obj2);
            pool.Free(obj3);
            Assert.Same(obj, pool.Allocate());
            Assert.Same(obj2, pool.Allocate());
            Assert.Same(obj3, pool.Allocate());
        }

        [Fact]
        public void TestObjectCanBeDropped()
        {
            Func<object> factory = () => new object();
            var pool = new ObjectPool<object>(factory, 1);

            var obj = new object();
            pool.Free(obj);
            var obj2 = new object();
            pool.Free(obj2);

            Assert.Same(obj, pool.Allocate());
            Assert.NotSame(obj2, pool.Allocate());
        }
    }
}
