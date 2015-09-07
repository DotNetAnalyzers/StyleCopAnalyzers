// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

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
        private readonly AccessLevel accessibilty;
        private readonly int elementPriority;
        private readonly bool prioritizeAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOrderHelper"/> struct.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        /// <param name="prioritizeType">Indicates whether to prioritize element type.</param>
        /// <param name="prioritizeAccess">Indicates whether to prioritize access level.</param>
        /// <param name="prioritizeConst">Indicates whether to prioritize constants.</param>
        /// <param name="prioritizeStatic">Indicates whether to prioritize static elements.</param>
        /// <param name="prioritizeReadonly">Indicates whether to prioritize readonly elements.</param>
        public MemberOrderHelper(MemberDeclarationSyntax member, bool prioritizeType = true, bool prioritizeAccess = true, bool prioritizeConst = true, bool prioritizeStatic = true, bool prioritizeReadonly = true)
        {
            this.Member = member;
            this.prioritizeAccess = prioritizeAccess;
            var modifiers = member.GetModifiers();
            var type = member.Kind();
            type = type == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : type;

            this.elementPriority = prioritizeType ? TypeMemberOrder.IndexOf(type) : 0;
            this.modifierFlags = GetModifierFlags(modifiers, prioritizeConst, prioritizeStatic, prioritizeReadonly);
            if (prioritizeAccess)
            {
                if ((type == SyntaxKind.ConstructorDeclaration && this.modifierFlags.HasFlag(ModifierFlags.Static))
                    || (type == SyntaxKind.MethodDeclaration && (member as MethodDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (type == SyntaxKind.PropertyDeclaration && (member as PropertyDeclarationSyntax)?.ExplicitInterfaceSpecifier != null)
                    || (type == SyntaxKind.IndexerDeclaration && (member as IndexerDeclarationSyntax)?.ExplicitInterfaceSpecifier != null))
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
        public MemberDeclarationSyntax Member { get; }

        /// <summary>
        /// The priority for this member.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        public int Priority
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
        public int AccessibilityPriority => (int)this.accessibilty;

        /// <summary>
        /// The priority for this member only from modifiers.
        /// </summary>
        /// <value>
        /// The priority for this member.
        /// </value>
        public int ModifierPriority => (int)this.modifierFlags;

        private static ModifierFlags GetModifierFlags(SyntaxTokenList syntax, bool prioritizeConst, bool prioritizeStatic, bool prioritizeReadonly)
        {
            var flags = ModifierFlags.None;
            if (prioritizeConst && syntax.Any(SyntaxKind.ConstKeyword))
            {
                flags |= ModifierFlags.Const;
            }
            else
            {
                if (prioritizeStatic && syntax.Any(SyntaxKind.StaticKeyword))
                {
                    flags |= ModifierFlags.Static;
                }

                if (prioritizeReadonly && syntax.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    flags |= ModifierFlags.Readonly;
                }
            }

            return flags;
        }
    }
}
