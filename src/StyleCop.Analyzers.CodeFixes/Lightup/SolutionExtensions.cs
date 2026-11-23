// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal static class SolutionExtensions
    {
        private static readonly Func<Solution, DocumentId, string, Solution> WithDocumentNameAccessor;
        private static readonly Func<Solution, DocumentId, SourceText, PreservationMode, Solution> WithAnalyzerConfigDocumentTextAccessor;

        static SolutionExtensions()
        {
            WithDocumentNameAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<Solution, DocumentId, string, Solution>(typeof(Solution), typeof(DocumentId), typeof(string), nameof(WithDocumentName));
            WithAnalyzerConfigDocumentTextAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<Solution, DocumentId, SourceText, PreservationMode, Solution>(typeof(Solution), typeof(DocumentId), typeof(SourceText), typeof(PreservationMode), nameof(WithAnalyzerConfigDocumentText));
        }

        public static Solution WithDocumentName(this Solution solution, DocumentId documentId, string name)
        {
            return WithDocumentNameAccessor(solution, documentId, name);
        }

        /// <summary>
        /// Updates the text of an analyzer config document. This is only available on compilers that support
        /// analyzer config documents; otherwise <see langword="null"/> is returned.
        /// </summary>
        /// <param name="solution">The solution to update.</param>
        /// <param name="documentId">The identifier of the analyzer config document.</param>
        /// <param name="text">The new text.</param>
        /// <param name="mode">The preservation mode.</param>
        /// <returns>The updated solution.</returns>
        public static Solution WithAnalyzerConfigDocumentText(this Solution solution, DocumentId documentId, SourceText text, PreservationMode mode = PreservationMode.PreserveValue)
        {
            return WithAnalyzerConfigDocumentTextAccessor(solution, documentId, text, mode);
        }
    }
}
