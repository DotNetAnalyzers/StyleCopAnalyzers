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
        private readonly Accessibility accessibilty;

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
        /// Checks if the current field should be declared before the other.
        /// </summary>
        /// <param name="other">The field to compare against.</param>
        /// <returns>True if the field should be delcared before the other.</returns>
        public bool ShouldBeBefore(MemberOrderHelper other) => this.Priority > other.Priority;

        private static Accessibility GetAccessibilityFlags(SyntaxTokenList syntax)
        {
            if (syntax.Any(SyntaxKind.PublicKeyword))
            {
                return Accessibility.Public;
            }

            if (syntax.Any(SyntaxKind.InternalKeyword))
            {
                return Accessibility.Internal;
            }

            if (syntax.Any(SyntaxKind.InternalKeyword) && syntax.Any(SyntaxKind.ProtectedKeyword))
            {
                return Accessibility.ProtectedAndInternal;
            }

            if (syntax.Any(SyntaxKind.ProtectedKeyword))
            {
                return Accessibility.Protected;
            }

            // Do not assign private accessiblity here, since we ant to have private come after non qualified.

            return Accessibility.NotApplicable;
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