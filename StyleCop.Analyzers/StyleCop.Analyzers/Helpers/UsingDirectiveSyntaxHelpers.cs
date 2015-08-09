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

        /// <summary>
        /// Check if <see cref="UsingDirectiveSyntax"/> is system using directive.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be checked.</param>
        /// <returns>Return true if the <see cref="UsingDirectiveSyntax"/>is system using directive, otherwise false.</returns>
        internal static bool IsSystemUsingDirective(this UsingDirectiveSyntax usingDirective) => string.Equals(SystemUsingDirectiveIdentifier, GetFirstIdentifierInUsingDirective(usingDirective)?.Text, StringComparison.Ordinal);

        /// <summary>
        /// Check if <see cref="UsingDirectiveSyntax"/> is preceded by a preprocessor directive.
        /// </summary>
        /// <param name="usingDirective">The using directive.</param>
        /// <returns>True if the <see cref="UsingDirectiveSyntax"/> is preceded by a preprocessor directive, otherwise false.</returns>
        internal static bool IsPrecededByPreprocessorDirective(this UsingDirectiveSyntax usingDirective)
        {
            if (!usingDirective.HasLeadingTrivia)
            {
                return false;
            }

            foreach (var trivia in usingDirective.GetLeadingTrivia())
            {
                switch (trivia.Kind())
                {
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                case SyntaxKind.RegionDirectiveTrivia:
                case SyntaxKind.EndRegionDirectiveTrivia:
                case SyntaxKind.DefineDirectiveTrivia:
                case SyntaxKind.UndefDirectiveTrivia:
                case SyntaxKind.ErrorDirectiveTrivia:
                case SyntaxKind.WarningDirectiveTrivia:
                case SyntaxKind.LineDirectiveTrivia:
                case SyntaxKind.PragmaWarningDirectiveTrivia:
                case SyntaxKind.PragmaChecksumDirectiveTrivia:
                case SyntaxKind.ReferenceDirectiveTrivia:
                case SyntaxKind.BadDirectiveTrivia:
                    return true;
                }
            }

            return false;
        }

        private static bool ExcludeGlobalKeyword(IdentifierNameSyntax token) => !token.Identifier.IsKind(SyntaxKind.GlobalKeyword);

        private static SyntaxToken? GetFirstIdentifierInUsingDirective(UsingDirectiveSyntax usingDirective) => usingDirective.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault(ExcludeGlobalKeyword)?.Identifier;
    }
}
