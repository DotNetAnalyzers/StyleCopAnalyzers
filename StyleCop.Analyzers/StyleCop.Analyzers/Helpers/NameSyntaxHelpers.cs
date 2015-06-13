namespace StyleCop.Analyzers.Helpers
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Class containing the extension methods for the <see cref="NameSyntax"/> class.
    /// </summary>
    internal static class NameSyntaxHelpers
    {
        private const string DotChar = ".";

        /// <summary>
        /// Gets the name contained in the <see cref="NameSyntax"/>, without an alias prefix.
        /// </summary>
        /// <param name="nameSyntax">The <see cref="NameSyntax"/> from which the name will be extracted.</param>
        /// <returns>The name contained in the <see cref="NameSyntax"/>, with its alias removed (if any).</returns>
        internal static string ToUnaliasedString(this NameSyntax nameSyntax)
        {
            var sb = new StringBuilder();

            BuildName(nameSyntax, sb);

            return sb.ToString();
        }

        private static void BuildName(NameSyntax nameSyntax, StringBuilder builder)
        {
            if (nameSyntax.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = (IdentifierNameSyntax)nameSyntax;
                builder.Append(identifierNameSyntax.Identifier.ValueText);
            }
            else if (nameSyntax.IsKind(SyntaxKind.QualifiedName))
            {
                var qualifiedNameSyntax = (QualifiedNameSyntax)nameSyntax;
                BuildName(qualifiedNameSyntax.Left, builder);
                builder.Append(DotChar);
                BuildName(qualifiedNameSyntax.Right, builder);
            }
            else if (nameSyntax.IsKind(SyntaxKind.GenericName))
            {
                var genericNameSyntax = (GenericNameSyntax)nameSyntax;
                builder.AppendFormat("{0}{1}", genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList.ToString());
            }
            else if (nameSyntax.IsKind(SyntaxKind.AliasQualifiedName))
            {
                var aliasQualifiedNameSyntax = (AliasQualifiedNameSyntax)nameSyntax;
                builder.Append(aliasQualifiedNameSyntax.Name.Identifier.ValueText);
            }
        }
    }
}
