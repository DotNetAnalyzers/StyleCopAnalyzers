// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;

    internal static class SortingHelper
    {
        /// <summary>
        /// Performs a insertion sort on <paramref name="data"/>.
        /// </summary>
        /// <typeparam name="T">The type of items to sort.</typeparam>
        /// <param name="data">An array of items that should be sorted.</param>
        /// <param name="comparer">A <see cref="Comparison{T}"/> that is used to determine order.</param>
        /// <param name="moveItem">A <see cref="Action{T1, T2}"/> that is called when the first parameter should be moved in front of the second parameter.</param>
        /// <returns>A <see cref="LinkedList{T}"/> that contains all items in order.</returns>
        /// <remarks>This performs a stable sort.</remarks>
        /// <remarks>The running time is in O(n^2).</remarks>
        internal static LinkedList<T> InsertionSort<T>(T[] data, Comparison<T> comparer, Action<T, T> moveItem)
        {
            LinkedList<T> result = new LinkedList<T>();
            for (int i = 0; i < data.Length; i++)
            {
                var currentItem = result.First;
                bool itemMoved = false;

                while (currentItem != null)
                {
                    if (comparer(currentItem.Value, data[i]) > 0)
                    {
                        result.AddBefore(currentItem, data[i]);
                        moveItem(data[i], currentItem.Value);
                        itemMoved = true;
                        break;
                    }

                    currentItem = currentItem.Next;
                }

                if (!itemMoved)
                {
                    result.AddLast(data[i]);
                }
            }

            return result;
        }
    }
}
