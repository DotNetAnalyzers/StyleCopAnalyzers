namespace StyleCop.Analyzers.OrderingRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Contains helper methods for determining an element's access level.
    /// </summary>
    internal static class AccessLevelHelper
    {
        /// <summary>Determines the access level for the given <paramref name="modifiers"/>.</summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>A <see cref="AccessLevel"/> value representing the access level.</returns>
        internal static AccessLevel GetAccessLevel(SyntaxTokenList modifiers)
        {
            bool isProtected = false;
            bool isInternal = false;
            foreach (var modifier in modifiers)
            {
                switch (modifier.Kind())
                {
                case SyntaxKind.PublicKeyword:
                    return AccessLevel.Public;
                case SyntaxKind.PrivateKeyword:
                    return AccessLevel.Private;
                case SyntaxKind.InternalKeyword:
                    if (isProtected)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isInternal = true;
                    }

                    break;
                case SyntaxKind.ProtectedKeyword:
                    if (isInternal)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isProtected = true;
                    }

                    break;
                }
            }

            if (isProtected)
            {
                return AccessLevel.Protected;
            }
            else if (isInternal)
            {
                return AccessLevel.Internal;
            }

            return AccessLevel.NotSpecified;
        }
    }
}
