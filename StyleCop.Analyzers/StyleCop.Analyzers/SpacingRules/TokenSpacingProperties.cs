// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class TokenSpacingProperties
    {
        internal const string LocationKey = "location";
        internal const string ActionKey = "action";
        internal const string LayoutKey = "layout";
        internal const string LocationPreceding = "preceding";
        internal const string LocationFollowing = "following";
        internal const string ActionInsert = "insert";
        internal const string ActionRemove = "remove";
        internal const string ActionRemoveImmediate = "remove-immediate";
        internal const string LayoutPack = "pack";
        internal const string LayoutPreserve = "preserve";

        internal static ImmutableDictionary<string, string> InsertPreceding { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(LocationKey, LocationPreceding)
                .SetItem(ActionKey, ActionInsert);

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
                .SetItem(LocationKey, LocationPreceding)
                .SetItem(ActionKey, ActionRemoveImmediate)
                .SetItem(LayoutKey, LayoutPack);

        internal static ImmutableDictionary<string, string> RemovePreceding { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(LocationKey, LocationPreceding)
                .SetItem(ActionKey, ActionRemove)
                .SetItem(LayoutKey, LayoutPack);

        internal static ImmutableDictionary<string, string> RemovePrecedingPreserveLayout { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(LocationKey, LocationPreceding)
                .SetItem(ActionKey, ActionRemove)
                .SetItem(LayoutKey, LayoutPreserve);

        internal static ImmutableDictionary<string, string> InsertFollowing { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(LocationKey, LocationFollowing)
                .SetItem(ActionKey, ActionInsert);

        internal static ImmutableDictionary<string, string> RemoveFollowing { get; } =
            ImmutableDictionary<string, string>.Empty
                .SetItem(LocationKey, LocationFollowing)
                .SetItem(ActionKey, ActionRemove);
    }
}
