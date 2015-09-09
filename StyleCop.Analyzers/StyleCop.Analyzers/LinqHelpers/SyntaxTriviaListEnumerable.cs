// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace System.Linq
{
    using Collections.Generic;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// This class supports a subset of LINQ operations on <see cref="SyntaxTriviaList"/> without requiring boxing of
    /// operands as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class SyntaxTriviaListEnumerable
    {
        /// <summary>
        /// Determines if a <see cref="SyntaxTriviaList"/> contains a specific <see cref="SyntaxTrivia"/>.
        /// </summary>
        /// <remarks>
        /// <para>This method allows callers to avoid boxing the <see cref="SyntaxTriviaList"/> as an
        /// <see cref="IEnumerable{T}"/>.</para>
        /// </remarks>
        /// <param name="list">The source list.</param>
        /// <param name="trivia">The element to look for in the list.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="list"/> contains <paramref name="trivia"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        internal static bool Contains(this SyntaxTriviaList list, SyntaxTrivia trivia)
        {
            return list.IndexOf(trivia) != -1;
        }

        /// <summary>
        /// Determines if a <see cref="SyntaxTriviaList"/> contains any trivia matching the conditions specified by a
        /// predicate.
        /// </summary>
        /// <remarks>
        /// <para>This method allows callers to avoid boxing the <see cref="SyntaxTriviaList"/> as an
        /// <see cref="IEnumerable{T}"/>.</para>
        /// </remarks>
        /// <param name="list">The <see cref="SyntaxTriviaList"/> to search.</param>
        /// <param name="predicate">The predicate determining the matching conditions for trivia.</param>
        /// <returns>
        /// <see langword="true"/> if the specified list contains any trivia matching the specified predicate;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool Any(this SyntaxTriviaList list, Func<SyntaxTrivia, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (SyntaxTrivia trivia in list)
            {
                if (predicate(trivia))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if all of the trivia in a <see cref="SyntaxTriviaList"/> match the conditions specified by a
        /// predicate.
        /// </summary>
        /// <remarks>
        /// <para>This method allows callers to avoid boxing the <see cref="SyntaxTriviaList"/> as an
        /// <see cref="IEnumerable{T}"/>.</para>
        /// </remarks>
        /// <param name="list">The <see cref="SyntaxTriviaList"/> to search.</param>
        /// <param name="predicate">The predicate determining the matching conditions for trivia.</param>
        /// <returns>
        /// <see langword="true"/> if all trivia in the specified list match the specified predicate; otherwise,
        /// <see langword="false"/>. If the list is empty, this method returns <see langword="true"/>.
        /// </returns>
        internal static bool All(this SyntaxTriviaList list, Func<SyntaxTrivia, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (SyntaxTrivia trivia in list)
            {
                if (!predicate(trivia))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
