namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides helper methods for working with source file locations.
    /// </summary>
    internal static class LocationHelpers
    {
        /// <summary>
        /// Gets the line on which the given token occurs.
        /// </summary>
        /// <param name="token">The token to use.</param>
        /// <returns>The line on which the given token occurs.</returns>
        internal static int GetLine(this SyntaxToken token)
        {
            return token.GetLocation().GetLineSpan().StartLinePosition.Line;
        }
    }
}
