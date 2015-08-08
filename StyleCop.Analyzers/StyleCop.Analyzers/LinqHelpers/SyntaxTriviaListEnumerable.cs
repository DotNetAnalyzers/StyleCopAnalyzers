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
            foreach (SyntaxTrivia item in list)
            {
                if (item.Equals(trivia))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
