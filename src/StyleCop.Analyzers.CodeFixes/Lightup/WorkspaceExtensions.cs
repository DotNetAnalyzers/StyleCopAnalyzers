// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class WorkspaceExtensions
    {
        /// <summary>
        /// The value of <c>ApplyChangesKind.ChangeDocumentInfo</c>, or <see langword="null"/> if the current Roslyn
        /// version does not define it (it was added in Roslyn 2.x, together with <c>Solution.WithDocumentName</c>).
        /// </summary>
        private static readonly ApplyChangesKind? ChangeDocumentInfo =
            Enum.TryParse("ChangeDocumentInfo", out ApplyChangesKind kind) ? kind : (ApplyChangesKind?)null;

        /// <summary>
        /// Determines whether the workspace can apply changes to document info, such as renaming a document in place.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns><see langword="true"/> if the workspace supports <c>ApplyChangesKind.ChangeDocumentInfo</c>;
        /// otherwise, <see langword="false"/>.</returns>
        public static bool CanApplyChangeDocumentInfo(this Workspace workspace)
        {
            return ChangeDocumentInfo is { } changeDocumentInfo
                && workspace.CanApplyChange(changeDocumentInfo);
        }
    }
}
