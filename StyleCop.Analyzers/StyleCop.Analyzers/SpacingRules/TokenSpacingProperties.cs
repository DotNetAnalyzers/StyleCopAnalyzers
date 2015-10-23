// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class TokenSpacingProperties
    {
        internal static ImmutableDictionary<string, string> InsertPreceding { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationPreceding)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionInsert);

        /// <summary>
        /// Gets a property collection indicating that the code fix should remove any
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> trivia which <em>immediately</em> precedes the token identified by
        /// the diagnostic span.
        /// </summary>
        /// <value>
        /// A property collection indicating that the code fix should remove any
        /// <see cref="SyntaxKind.WhitespaceTrivia"/> trivia which <em>immediately</em> precedes the token identified by
        /// the diagnostic span.
        /// </value>
        internal static ImmutableDictionary<string, string> RemoveImmediatePreceding { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationPreceding)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionRemoveImmediate)
                .SetItem(TokenSpacingCodeFixProvider.LayoutKey, TokenSpacingCodeFixProvider.LayoutPack);

        internal static ImmutableDictionary<string, string> RemovePreceding { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationPreceding)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionRemove)
                .SetItem(TokenSpacingCodeFixProvider.LayoutKey, TokenSpacingCodeFixProvider.LayoutPack);

        internal static ImmutableDictionary<string, string> RemovePrecedingPreserveLayout { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationPreceding)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionRemove)
                .SetItem(TokenSpacingCodeFixProvider.LayoutKey, TokenSpacingCodeFixProvider.LayoutPreserve);

        internal static ImmutableDictionary<string, string> InsertFollowing { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationFollowing)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionInsert);

        internal static ImmutableDictionary<string, string> RemoveFollowing { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(TokenSpacingCodeFixProvider.LocationKey, TokenSpacingCodeFixProvider.LocationFollowing)
                .SetItem(TokenSpacingCodeFixProvider.ActionKey, TokenSpacingCodeFixProvider.ActionRemove);
    }
}
