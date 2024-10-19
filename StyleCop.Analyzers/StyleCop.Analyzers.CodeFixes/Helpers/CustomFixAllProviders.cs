﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Contains custom implementations of <see cref="FixAllProvider"/>.
    /// </summary>
    internal static class CustomFixAllProviders
    {
        /// <summary>
        /// Gets the default batch fix all provider.
        /// This provider batches all the individual diagnostic fixes across the scope of fix all action,
        /// computes fixes in parallel and then merges all the non-conflicting fixes into a single fix all code action.
        /// This fixer supports fixes for the following fix all scopes:
        /// <see cref="FixAllScope.Document"/>, <see cref="FixAllScope.Project"/> and <see cref="FixAllScope.Solution"/>.
        /// </summary>
        /// <remarks>
        /// <para>The batch fix all provider only batches operations (i.e. <see cref="CodeActionOperation"/>) of type
        /// <see cref="ApplyChangesOperation"/> present within the individual diagnostic fixes. Other types of
        /// operations present within these fixes are ignored.</para>
        /// </remarks>
        /// <value>
        /// The default batch fix all provider.
        /// </value>
        public static FixAllProvider BatchFixer => CustomBatchFixAllProvider.Instance;
    }
}
