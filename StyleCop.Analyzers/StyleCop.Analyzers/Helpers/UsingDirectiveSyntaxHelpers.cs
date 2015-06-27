namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Class containing the extension methods for the <see cref="UsingDirectiveSyntax"/> class.
    /// </summary>
    internal static class UsingDirectiveSyntaxHelpers
    {
        private const string SystemUsingDirectiveIdentifier = nameof(System);

        private static bool ExcludeGlobalKeyword(IdentifierNameSyntax token) => !token.Identifier.IsKind(SyntaxKind.GlobalKeyword);

        private static SyntaxToken? GetFirstIdentifierInUsingDirective(UsingDirectiveSyntax usingDirective) => usingDirective.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(ExcludeGlobalKeyword)?.Identifier;

        /// <summary>
        /// Check if <see cref="UsingDirectiveSyntax"/> is system using directive.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be checked.</param>
        /// <returns>Return true if the <see cref="UsingDirectiveSyntax"/>is system using directive, otherwise false.</returns>
        internal static bool IsSystemUsingDirective(this UsingDirectiveSyntax usingDirective) => string.Equals(SystemUsingDirectiveIdentifier, GetFirstIdentifierInUsingDirective(usingDirective)?.Text, StringComparison.Ordinal);
    }
}
