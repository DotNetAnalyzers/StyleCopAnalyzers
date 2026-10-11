// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class SolutionExtensions
    {
        private static readonly Func<Solution, DocumentId, string, Solution> WithDocumentNameAccessor;

        static SolutionExtensions()
        {
            WithDocumentNameAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<Solution, DocumentId, string, Solution>(typeof(Solution), typeof(DocumentId), typeof(string), nameof(WithDocumentName));
        }

        public static Solution WithDocumentName(this Solution solution, DocumentId documentId, string name)
        {
            return WithDocumentNameAccessor(solution, documentId, name);
        }
    }
}
