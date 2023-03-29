// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Helper for dealing with member priority.
    /// </summary>
    internal struct MemberOrderHelper
    {
        private static readonly ImmutableArray<SyntaxKind> TypeMemberOrder = ImmutableArray.Create(
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.NamespaceDeclaration);

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOrderHelper"/> struct.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        /// <param name="elementOrder">The element ordering traits.</param>
        internal MemberOrderHelper(MemberDeclarationSyntax member, ImmutableArray<OrderingTrait> elementOrder)
        {
            this.Member = member;
            var modifiers = member.GetModifiers();
            var type = member.Kind();
            type = type == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : type;

            this.Priority = 0;
            foreach (OrderingTrait trait in elementOrder)
            {
                switch (trait)
                {
                case OrderingTrait.Kind:
                    // 4 bits are required to store this.
                    this.Priority <<= 4;
                    this.Priority |= TypeMemberOrder.IndexOf(type) & 0x0F;
                    break;

                case OrderingTrait.Accessibility:
                    // 3 bits are required to store this.
                    this.Priority <<= 3;
                    this.Priority |= (int)GetAccessLevelForOrdering(member, modifiers) & 0x07;
                    break;

                case OrderingTrait.Constant:
                    this.Priority <<= 1;
                    if (modifiers.Any(SyntaxKind.ConstKeyword))
                    {
                        this.Priority |= 1;
                    }

                    break;

                case OrderingTrait.Static:
                    this.Priority <<= 1;
                    if (modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        this.Priority |= 1;
                    }

                    break;

                case OrderingTrait.Readonly:
                    this.Priority <<= 1;
                    if (modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                    {
                        this.Priority |= 1;
                    }

                    break;

                default:
                    continue;
                }
            }
        }

        [Flags]
        private enum ModifierFlags
        {
            /// <summary>
            /// No modifiers.
            /// </summary>
            None = 0,

            /// <summary>
            /// <see langword="readonly"/> modifier.
            /// </summary>
            Readonly = 1,

            /// <summary>
            /// <see langword="static"/> modifier.
            /// </summary>
            Static = 1 << 2,

            /// <summary>
            /// <see langword="const"/> modifier.
            /// </summary>
            Const = 1 << 3,
        }

        /// <summary>
        /// Gets the wrapped member.
        /// </summary>
        /// <value>
        /// The wrapped member.
        /// </value>
        internal MemberDeclarationSyntax Member { get; }

        /// <summary>
        /// Gets the priority for this member.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        internal int Priority { get; }

        internal static AccessLevel GetAccessLevelForOrdering(SyntaxNode member, SyntaxTokenList modifiers)
        {
            SyntaxKind type = member.Kind();

            AccessLevel accessibility;
            if ((type == SyntaxKind.ConstructorDeclaration && modifiers.Any(SyntaxKind.StaticKeyword))
                || (type == SyntaxKind.MethodDeclaration && ((MethodDeclarationSyntax)member).ExplicitInterfaceSpecifier != null)
                || (type == SyntaxKind.PropertyDeclaration && ((PropertyDeclarationSyntax)member).ExplicitInterfaceSpecifier != null)
                || (type == SyntaxKind.IndexerDeclaration && ((IndexerDeclarationSyntax)member).ExplicitInterfaceSpecifier != null))
            {
                accessibility = AccessLevel.Public;
            }
            else
            {
                accessibility = AccessLevelHelper.GetAccessLevel(modifiers);
                if (accessibility == AccessLevel.NotSpecified)
                {
                    if (member.Parent.IsKind(SyntaxKind.CompilationUnit) || member.Parent.IsKind(SyntaxKind.NamespaceDeclaration) || member.Parent.IsKind(SyntaxKindEx.FileScopedNamespaceDeclaration))
                    {
                        accessibility = AccessLevel.Internal;
                    }
                    else
                    {
                        accessibility = AccessLevel.Private;
                    }
                }
            }

            return accessibility;
        }
    }
}
