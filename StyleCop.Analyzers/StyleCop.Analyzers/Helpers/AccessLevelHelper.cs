namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Contains helper methods for determining an element's access level.
    /// </summary>
    internal static class AccessLevelHelper
    {
        private static readonly Dictionary<AccessLevel, string> AccessLevelNames = new Dictionary<AccessLevel, string>
        {
            [AccessLevel.NotSpecified] = "unspecified access",
            [AccessLevel.Public] = "public",
            [AccessLevel.Internal] = "internal",
            [AccessLevel.ProtectedInternal] = "protected internal",
            [AccessLevel.Protected] = "protected",
            [AccessLevel.Private] = "private"
        };

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

        /// <summary>Gets the name for a given access level.</summary>
        /// <param name="accessLevel">The access level.</param>
        /// <returns>The name for a given access level.</returns>
        internal static string GetName(AccessLevel accessLevel)
        {
            return AccessLevelNames[accessLevel];
        }
    }
}
