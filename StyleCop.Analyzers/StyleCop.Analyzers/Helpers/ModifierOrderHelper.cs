// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.OrderingRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    internal static class ModifierOrderHelper
    {
        /// <summary>
        /// Represents modifier type for implementing SA1206 rule.
        /// </summary>
        internal enum ModifierType
        {
            /// <summary>
            /// Represents default value.
            /// </summary>
            None,

            /// <summary>
            /// Represents any of access modifiers, i.e <see langword="public"/>, <see langword="protected"/>, <see langword="internal"/>, <see langword="private"/>, <see langword="file"/>.
            /// </summary>
            Access,

            /// <summary>
            /// Represents <see langword="static"/> modifier.
            /// </summary>
            Static,

            /// <summary>
            /// Represents other modifiers, i.e <see langword="partial"/>, <see langword="virtual"/>, <see langword="abstract"/>, <see langword="override"/>, <see langword="extern"/>, <see langword="unsafe"/>, <see langword="new"/>, <see langword="async"/>, <see langword="const"/>, <see langword="sealed"/>, <see langword="readonly"/>, <see langword="volatile"/>, <see langword="fixed"/>, <see langword="ref"/>.
            /// </summary>
            Other,
        }

        internal static ModifierType GetModifierType(SyntaxToken modifier)
        {
            var result = default(ModifierType);

            switch (modifier.Kind())
            {
            case SyntaxKind.PublicKeyword:
            case SyntaxKind.ProtectedKeyword:
            case SyntaxKind.InternalKeyword:
            case SyntaxKind.PrivateKeyword:
            case SyntaxKindEx.FileKeyword:
                result = ModifierType.Access;
                break;

            case SyntaxKind.StaticKeyword:
                result = ModifierType.Static;
                break;

            case SyntaxKind.VirtualKeyword:
            case SyntaxKind.AbstractKeyword:
            case SyntaxKind.OverrideKeyword:
            case SyntaxKind.ExternKeyword:
            case SyntaxKind.UnsafeKeyword:
            case SyntaxKind.NewKeyword:
            case SyntaxKind.SealedKeyword:
            case SyntaxKind.ReadOnlyKeyword:
            case SyntaxKind.VolatileKeyword:
            case SyntaxKind.FixedKeyword:
            case SyntaxKind.ConstKeyword:
            case SyntaxKind.AsyncKeyword:
            case SyntaxKind.PartialKeyword:
            case SyntaxKind.RefKeyword:
            case SyntaxKindEx.RequiredKeyword:
                result = ModifierType.Other;
                break;
            }

            return result;
        }
    }
}
