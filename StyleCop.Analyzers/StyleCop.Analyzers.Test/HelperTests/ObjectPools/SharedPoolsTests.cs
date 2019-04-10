// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests.ObjectPools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using StyleCop.Analyzers.Helpers.ObjectPools;
    using Xunit;

    [Collection(nameof(SequentialTestCollection))]
    public class SharedPoolsTests
    {
        [Fact]
        public void TestBigDefault()
        {
            var listPool = SharedPools.BigDefault<List<int>>();
            Assert.IsAssignableFrom<ObjectPool<List<int>>>(listPool);

            var stackPool = SharedPools.BigDefault<Stack<int>>();
            Assert.IsAssignableFrom<ObjectPool<Stack<int>>>(stackPool);

            Assert.NotSame(listPool, stackPool);
        }

        [Fact]
        public void TestDefault()
        {
            var listPool = SharedPools.BigDefault<List<int>>();
            Assert.IsAssignableFrom<ObjectPool<List<int>>>(listPool);

            var stackPool = SharedPools.BigDefault<Stack<int>>();
            Assert.IsAssignableFrom<ObjectPool<Stack<int>>>(stackPool);

            Assert.NotSame(listPool, stackPool);
        }

        [Fact]
        public void TestStringBuilderPool()
        {
            StringBuilder builder = null;
            using (var obj = SharedPools.Default<StringBuilder>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Length == 0);

                obj.Object.Append("testing");

                // Keep a copy to verify its length is reset
                builder = obj.Object;
                Assert.NotEqual(0, builder.Length);
            }

            Assert.Equal(0, builder.Length);
        }

        [Fact]
        public void TestStackPool()
        {
            Stack<int> collection = null;
            using (var obj = SharedPools.Default<Stack<int>>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Count == 0);

                obj.Object.Push(1);

                // Keep a copy to verify its length is reset
                collection = obj.Object;
                Assert.NotEmpty(collection);
            }

            Assert.Empty(collection);
        }

        [Fact]
        public void TestQueuePool()
        {
            Queue<int> collection = null;
            using (var obj = SharedPools.Default<Queue<int>>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Count == 0);

                obj.Object.Enqueue(1);

                // Keep a copy to verify its length is reset
                collection = obj.Object;
                Assert.NotEmpty(collection);
            }

            Assert.Empty(collection);
        }

        [Fact]
        public void TestHashSetPool()
        {
            HashSet<int> collection = null;
            using (var obj = SharedPools.Default<HashSet<int>>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Count == 0);

                obj.Object.Add(1);

                // Keep a copy to verify its length is reset
                collection = obj.Object;
                Assert.NotEmpty(collection);
            }

            Assert.Empty(collection);
        }

        [Fact]
        public void TestDictionaryPool()
        {
            Dictionary<int, int> collection = null;
            using (var obj = SharedPools.Default<Dictionary<int, int>>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Count == 0);

                obj.Object.Add(1, 1);

                // Keep a copy to verify its length is reset
                collection = obj.Object;
                Assert.NotEmpty(collection);
            }

            Assert.Empty(collection);
        }

        [Fact]
        public void TestListPool()
        {
            List<int> collection = null;
            using (var obj = SharedPools.Default<List<int>>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
                Assert.True(obj.Object.Count == 0);

                obj.Object.Add(1);

                // Keep a copy to verify its length is reset
                collection = obj.Object;
                Assert.NotEmpty(collection);
            }

            Assert.Empty(collection);
        }

        [Fact]
        public void TestExceptionPool()
        {
            using (var obj = SharedPools.Default<Exception>().GetPooledObject())
            {
                Assert.NotNull(obj.Object);
            }
        }

        [Fact]
        public void TestClearAndFreeNull()
        {
            SharedPools.Default<StringBuilder>().ClearAndFree(null);
            SharedPools.Default<HashSet<int>>().ClearAndFree(null);
            SharedPools.Default<Stack<int>>().ClearAndFree(null);
            SharedPools.Default<Queue<int>>().ClearAndFree(null);
            SharedPools.Default<Dictionary<int, int>>().ClearAndFree(null);
            SharedPools.Default<List<int>>().ClearAndFree(null);
        }

        [Fact]
        public void TestClearAndFreeLarge()
        {
            // StringBuilder
            var builder = new StringBuilder("text", 1024);
            SharedPools.Default<StringBuilder>().ClearAndFree(builder);
            Assert.Equal(0, builder.Length);
            Assert.True(builder.Capacity < 1024);

            // HashSet<int>
            var set = new HashSet<int>(Enumerable.Range(0, 1024));
            SharedPools.Default<HashSet<int>>().ClearAndFree(set);
            Assert.Empty(set);

            // Stack<int>
            var stack = new Stack<int>(Enumerable.Range(0, 1024));
            SharedPools.Default<Stack<int>>().ClearAndFree(stack);
            Assert.Empty(stack);

            // Queue<int>
            var queue = new Queue<int>(Enumerable.Range(0, 1024));
            SharedPools.Default<Queue<int>>().ClearAndFree(queue);
            Assert.Empty(queue);

            // Dictionary<int, int> **This one doesn't go back in the pool!**
            var dictionary = Enumerable.Range(0, 1024).ToDictionary(i => i);
            SharedPools.Default<Dictionary<int, int>>().ClearAndFree(dictionary);
            Assert.Equal(1024, dictionary.Count);

            // List<int>
            var list = new List<int>(Enumerable.Range(0, 1024));
            Assert.True(list.Capacity >= 1024);
            SharedPools.Default<List<int>>().ClearAndFree(list);
            Assert.Empty(list);
            Assert.True(list.Capacity < 1024);
        }

        [Fact]
        public void TestPooledObjectHandlesNullAllocation()
        {
            object Allocator(ObjectPool<object> pool) => null;
            void Releaser(ObjectPool<object> pool, object obj)
            {
            }

            using (var obj = new PooledObject<object>(SharedPools.Default<object>(), Allocator, Releaser))
            {
                Assert.Null(obj.Object);
            }
        }
    }
}
