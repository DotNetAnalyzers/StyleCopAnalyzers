namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Linq;
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
        private readonly AccesibilityFlags accessibiltyFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberOrderHelper"/> class.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        public MemberOrderHelper(MemberDeclarationSyntax member)
        {
            this.Member = member;
            var modifiers = member.GetModifiers();

            this.modifierFlags = GetModifierFlags(modifiers);
            this.accessibiltyFlags = GetAccessibilityFlags(modifiers);
        }

        [Flags]
        private enum ModifierFlags
        {
            /// <summary>
            /// Const modifier
            /// </summary>
            Const = 1,
            /// <summary>
            /// Static modifier
            /// </summary>
            Static = 1 << 2,
            /// <summary>
            /// Readonly modifier
            /// </summary>
            Readonly = 1 << 3,
            /// <summary>
            /// No modifiers
            /// </summary>
            None = 1 << 4
        }

        private enum AccesibilityFlags
        {
            /// <summary>
            /// Public accessibility
            /// </summary>
            Public = 1,
            /// <summary>
            /// Internal accessibility
            /// </summary>
            Internal = 1 << 1,
            /// <summary>
            /// Protected Internal accessibility
            /// </summary>
            ProtectedInternal = 1 << 2,
            /// <summary>
            /// Protected accessibility
            /// </summary>
            Protected = 1 << 3,
            /// <summary>
            /// No accessibility specified
            /// </summary>
            None = 1 << 4,
            /// <summary>
            /// Private accessibility
            /// </summary>
            Private = 1 << 5,
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
        public int Priority => (int)this.modifierFlags + (int)this.accessibiltyFlags;

        private static AccesibilityFlags GetAccessibilityFlags(SyntaxTokenList syntax)
        {
            if (Has(syntax, SyntaxKind.PublicKeyword))
            {
                return AccesibilityFlags.Public;
            }

            if (Has(syntax, SyntaxKind.InternalKeyword))
            {
                return AccesibilityFlags.Internal;
            }

            if (Has(syntax, SyntaxKind.InternalKeyword) && Has(syntax, SyntaxKind.ProtectedKeyword))
            {
                return AccesibilityFlags.ProtectedInternal;
            }

            if (Has(syntax, SyntaxKind.ProtectedKeyword))
            {
                return AccesibilityFlags.Protected;
            }

            if (Has(syntax, SyntaxKind.PrivateKeyword))
            {
                return AccesibilityFlags.Private;
            }

            return AccesibilityFlags.None;
        }

        private static ModifierFlags GetModifierFlags(SyntaxTokenList syntax)
        {
            ModifierFlags flags = 0;
            if (Has(syntax, SyntaxKind.ConstKeyword))
            {
                flags |= ModifierFlags.Const;
            }
            else
            {
                if (Has(syntax, SyntaxKind.StaticKeyword))
                {
                    flags |= ModifierFlags.Static;
                }

                if (Has(syntax, SyntaxKind.ReadOnlyKeyword))
                {
                    flags |= ModifierFlags.Readonly;
                }
            }

            return flags == 0 ? ModifierFlags.None : flags;
        }

        private static bool Has(SyntaxTokenList list, SyntaxKind modifier)
        {
            return list.Any(x => x.IsKind(modifier));
        }
    }
}