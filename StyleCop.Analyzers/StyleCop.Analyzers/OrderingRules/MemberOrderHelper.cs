namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Helper class for dealing with member priority.
    /// </summary>
    public class MemberOrderHelper
    {
        private readonly ModifierFlags modifierFlags;
        private readonly AccesibilityFlags accessibilty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOrderHelper"/> class.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        public MemberOrderHelper(MemberDeclarationSyntax member)
        {
            this.Member = member;
            var modifiers = member.GetModifiers();

            this.modifierFlags = GetModifierFlags(modifiers);
            this.accessibilty = GetAccessibilityFlags(modifiers);
        }

        private enum AccesibilityFlags
        {
            /// <summary>
            /// Private accessibility
            /// </summary>
            Private = 1,

            /// <summary>
            /// No accessibility specified
            /// </summary>
            None = 1 << 1,

            /// <summary>
            /// Protected accessibility
            /// </summary>
            Protected = 1 << 2,

            /// <summary>
            /// Protected Internal accessibility
            /// </summary>
            ProtectedInternal = 1 << 3,

            /// <summary>
            /// Internal accessibility
            /// </summary>
            Internal = 1 << 4,

            /// <summary>
            /// Public accessibility
            /// </summary>
            Public = 1 << 5,
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
        public int Priority => (int)this.modifierFlags + ((int)this.accessibilty * 100/*the * 100 ensures the accesibility is more important than the modifier*/);

        /// <summary>
        /// The priority for this member only from accesibility.
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

        private static AccesibilityFlags GetAccessibilityFlags(SyntaxTokenList syntax)
        {
            if (syntax.Any(SyntaxKind.PublicKeyword))
            {
                return AccesibilityFlags.Public;
            }

            if (syntax.Any(SyntaxKind.InternalKeyword))
            {
                return AccesibilityFlags.Internal;
            }

            if (syntax.Any(SyntaxKind.InternalKeyword) && syntax.Any(SyntaxKind.ProtectedKeyword))
            {
                return AccesibilityFlags.ProtectedInternal;
            }

            if (syntax.Any(SyntaxKind.ProtectedKeyword))
            {
                return AccesibilityFlags.Protected;
            }

            if (syntax.Any(SyntaxKind.PrivateKeyword))
            {
                return AccesibilityFlags.Private;
            }

            return AccesibilityFlags.None;
        }

        private static ModifierFlags GetModifierFlags(SyntaxTokenList syntax)
        {
            ModifierFlags flags = 0;
            if (syntax.Any(SyntaxKind.ConstKeyword))
            {
                flags |= ModifierFlags.Const;
            }
            else
            {
                if (syntax.Any(SyntaxKind.StaticKeyword))
                {
                    flags |= ModifierFlags.Static;
                }

                if (syntax.Any(SyntaxKind.ReadOnlyKeyword))
                {
                    flags |= ModifierFlags.Readonly;
                }
            }

            return flags == 0 ? ModifierFlags.None : flags;
        }
    }
}
