// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class ModifierOrderHelper
    {
        /// <summary>
        /// Represents modifier type for implementing SA1206 rule.
        /// </summary>
        internal enum ModifierType
        {
            /// <summary>
            /// Represents default value
            /// </summary>
            None,

            /// <summary>
            /// Represents any of access modifiers i.e public, protected, internal, private
            /// </summary>
            Access,

            /// <summary>
            /// Represents static modifier
            /// </summary>
            Static,

            /// <summary>
            /// Represents other modifiers i.e partial, virtual, abstract, override, extern, unsafe, new, async, const, sealed, readonly, volatile, fixed, ref
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
                result = ModifierType.Other;
                break;
            }

            return result;
        }
    }
}
