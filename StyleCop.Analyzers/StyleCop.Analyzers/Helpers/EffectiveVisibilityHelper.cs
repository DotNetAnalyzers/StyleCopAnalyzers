namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Contains helper methods for resolving the effective visibility of an element.
    /// </summary>
    public static class EffectiveVisibilityHelper
    {
        /// <summary>
        /// Resolves a <see cref="SyntaxTokenList"/> to one of <see cref="SyntaxKind.PrivateKeyword"/>,
        /// <see cref="SyntaxKind.InternalKeyword"/> or <see cref="SyntaxKind.PublicKeyword"/>.
        /// </summary>
        /// <param name="modifiers">The token list.</param>
        /// <param name="defaultModifier">The default modifier if none are present in <paramref name="modifiers"/>.</param>
        /// <returns>One of <see cref="SyntaxKind.PrivateKeyword"/>,
        /// <see cref="SyntaxKind.InternalKeyword"/> or <see cref="SyntaxKind.PublicKeyword"/>.</returns>
        /// <remarks>
        /// <para>
        /// Resolves a list of modifiers to the effective visibility that they represent from the standpoint
        /// of the StyleCop documentation rules.
        /// </para>
        /// </remarks>
        public static SyntaxKind EffectiveVisibility(SyntaxTokenList modifiers, SyntaxKind defaultModifier)
        {
            if (modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                return SyntaxKind.PrivateKeyword;
            }

            if (modifiers.Any(SyntaxKind.ProtectedKeyword) || modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return SyntaxKind.PublicKeyword;
            }

            if (modifiers.Any(SyntaxKind.InternalKeyword))
            {
                return SyntaxKind.InternalKeyword;
            }

            return defaultModifier;
        }

        /// <summary>
        /// Resolves the visibility of an element in the context of its parent.
        /// </summary>
        /// <param name="startWith">The element's own visibility, which may be any of
        /// <see cref="SyntaxKind.PrivateKeyword"/>,
        /// <see cref="SyntaxKind.InternalKeyword"/> or <see cref="SyntaxKind.PublicKeyword"/>.</param>
        /// <param name="parent">The element's parent.</param>
        /// <returns>The resolved visibility.</returns>
        public static SyntaxKind ResolveVisibilityForChild(SyntaxKind startWith, BaseTypeDeclarationSyntax parent)
        {
            var effective = startWith;

            var current = parent;

            while (current != null)
            {
                var newCurrent = current.Parent as BaseTypeDeclarationSyntax;

                effective = ResolveVisibility(effective, EffectiveVisibility(current.Modifiers, newCurrent != null ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword));

                current = newCurrent;
            }

            return effective;
        }

        /// <summary>
        /// Resolves the more restrictive of two <see cref="SyntaxKind"/> values that may be any of
        /// <see cref="SyntaxKind.PrivateKeyword"/>,
        /// <see cref="SyntaxKind.InternalKeyword"/> or <see cref="SyntaxKind.PublicKeyword"/>.
        /// </summary>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        /// <returns>The more restrictive of <paramref name="left"/> and <paramref name="right"/>.</returns>
        private static SyntaxKind ResolveVisibility(SyntaxKind left, SyntaxKind right)
        {
            if (left != SyntaxKind.PrivateKeyword && left != SyntaxKind.InternalKeyword && left != SyntaxKind.PublicKeyword)
            {
                throw new ArgumentOutOfRangeException("left");
            }

            if (right != SyntaxKind.PrivateKeyword && right != SyntaxKind.InternalKeyword && right != SyntaxKind.PublicKeyword)
            {
                throw new ArgumentOutOfRangeException("right");
            }

            if (left == SyntaxKind.PrivateKeyword || right == SyntaxKind.PrivateKeyword)
            {
                return SyntaxKind.PrivateKeyword;
            }

            if (left == SyntaxKind.InternalKeyword || right == SyntaxKind.InternalKeyword)
            {
                return SyntaxKind.InternalKeyword;
            }

            return SyntaxKind.PublicKeyword;
        }
    }
}
