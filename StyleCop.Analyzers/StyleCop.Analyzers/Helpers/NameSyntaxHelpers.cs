namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Class containing the extension methods for the <see cref="NameSyntax"/> class.
    /// </summary>
    internal static class NameSyntaxHelpers
    {
        private const string AliasSeparator = "::";

        /// <summary>
        /// Gets the name contained in the <see cref="NameSyntax"/>, without an alias prefix.
        /// </summary>
        /// <param name="nameSyntax">The <see cref="NameSyntax"/> to get.</param>
        /// <returns>The name contained in the <see cref="NameSyntax"/>, with its alias removed (if any).</returns>
        internal static string ToUnaliasedString(this NameSyntax nameSyntax)
        {
            var name = nameSyntax.ToString();
            var aliasSepatorIndex = name.IndexOf(AliasSeparator);
            if (aliasSepatorIndex != -1)
            {
                name = name.Substring(aliasSepatorIndex + AliasSeparator.Length);
            }

            return name;
        }
    }
}
