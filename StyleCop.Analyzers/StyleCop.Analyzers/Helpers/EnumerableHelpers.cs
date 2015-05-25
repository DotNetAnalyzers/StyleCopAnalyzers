namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class EnumerableHelpers
    {
        /// <summary>
        /// Merges two sorted <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable1">The first sorted <see cref="IEnumerable{T}"/>.</param>
        /// <param name="enumerable2">The second sorted <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparison">A <see cref="Comparison{T}"/> that is used to sort elements.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> that contains all elements of <paramref name="enumerable1"/> and <paramref name="enumerable2"/> in order.</returns>
        internal static IEnumerable<T> Merge<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2, Comparison<T> comparison)
        {
            var enumerator1 = enumerable1.GetEnumerator();
            var enumerator2 = enumerable2.GetEnumerator();

            bool hasMore1 = enumerator1.MoveNext();
            bool hasMore2 = enumerator2.MoveNext();

            while (hasMore1 || hasMore2)
            {
                if (hasMore1 && hasMore2)
                {
                    if (comparison(enumerator1.Current, enumerator1.Current) <= 0)
                    {
                        yield return enumerator1.Current;
                        hasMore1 = enumerator1.MoveNext();
                    }
                    else
                    {
                        yield return enumerator2.Current;
                        hasMore2 = enumerator2.MoveNext();
                    }
                }
                else if (hasMore1)
                {
                    yield return enumerator1.Current;
                    hasMore1 = enumerator1.MoveNext();
                }
                else
                {
                    Debug.Assert(hasMore2, nameof(hasMore2));
                    yield return enumerator2.Current;
                    hasMore2 = enumerator2.MoveNext();
                }
            }
        }
    }
}
