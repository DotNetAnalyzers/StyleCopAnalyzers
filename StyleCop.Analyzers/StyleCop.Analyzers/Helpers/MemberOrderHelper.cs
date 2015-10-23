// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        private readonly ModifierFlags modifierFlags;
        private readonly int elementPriority;
        private readonly AccessLevel accessibilty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOrderHelper"/> struct.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        /// <param name="checks">The element ordering checks.</param>
        internal MemberOrderHelper(MemberDeclarationSyntax member, ElementOrderingChecks checks)
        {
            this.Member = member;
            var modifiers = member.GetModifiers();
            var type = member.Kind();
            type = type == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : type;

            this.elementPriority = checks.ElementType ? TypeMemberOrder.IndexOf(type) : 0;
            this.modifierFlags = GetModifierFlags(modifiers, checks);
            if (checks.AccessLevel)
            {
                if ((type == SyntaxKind.ConstructorDeclaration && this.modifierFlags.HasFlag(ModifierFlags.Static))
                    || (type == SyntaxKind.MethodDeclaration && ((MethodDeclarationSyntax)member).ExplicitInterfaceSpecifier != null)
                    || (type == SyntaxKind.PropertyDeclaration && ((PropertyDeclarationSyntax)member).ExplicitInterfaceSpecifier != null)
                    || (type == SyntaxKind.IndexerDeclaration && ((IndexerDeclarationSyntax)member).ExplicitInterfaceSpecifier != null))
                {
                    this.accessibilty = AccessLevel.Public;
                }
                else
                {
                    this.accessibilty = AccessLevelHelper.GetAccessLevel(modifiers);
                    if (this.accessibilty == AccessLevel.NotSpecified)
                    {
                        if (member.Parent.IsKind(SyntaxKind.CompilationUnit) || member.Parent.IsKind(SyntaxKind.NamespaceDeclaration))
                        {
                            this.accessibilty = AccessLevel.Internal;
                        }
                        else
                        {
                            this.accessibilty = AccessLevel.Private;
                        }
                    }
                }
            }
            else
            {
                this.accessibilty = AccessLevel.Public;
            }
        }

        [Flags]
        private enum ModifierFlags
        {
            /// <summary>
            /// No modifiers
            /// </summary>
            None = 0,

            /// <summary>
            /// Readonly modifier
            /// </summary>
            Readonly = 1,

            /// <summary>
            /// Static modifier
            /// </summary>
            Static = 1 << 2,

            /// <summary>
            /// Const modifier
            /// </summary>
            Const = 1 << 3,
        }

        /// <summary>
        /// The wrapped member.
        /// </summary>
        /// <value>
        /// The wrapped member.
        /// </value>
        internal MemberDeclarationSyntax Member { get; }

        /// <summary>
        /// The priority for this member.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        internal int Priority
        {
            get
            {
                var priority = this.ModifierPriority;

                // accessibility is more important than the modifiers
                priority += (this.AccessibilityPriority + 1) * 100;

                // element type is more important than accessibility
                priority += (this.elementPriority + 1) * 1000;
                return priority;
            }
        }

        /// <summary>
        /// The priority for this member only from accessibility.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        internal int AccessibilityPriority => (int)this.accessibilty;

        /// <summary>
        /// The priority for this member only from modifiers.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        internal int ModifierPriority => (int)this.modifierFlags;

        private static ModifierFlags GetModifierFlags(SyntaxTokenList syntax, ElementOrderingChecks checks)
        {
            var flags = ModifierFlags.None;
            if (checks.Const && syntax.Any(SyntaxKind.ConstKeyword))
            {
                flags |= ModifierFlags.Const;
            }
            else
            {
                if (checks.Static && syntax.Any(SyntaxKind.StaticKeyword))
                {
                    flags |= ModifierFlags.Static;
                }

                if (checks.Readonly && syntax.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    flags |= ModifierFlags.Readonly;
                }
            }

            return flags;
        }
    }
}
