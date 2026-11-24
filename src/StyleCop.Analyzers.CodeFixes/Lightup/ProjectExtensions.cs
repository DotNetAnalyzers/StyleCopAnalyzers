// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal static class ProjectExtensions
    {
        private static readonly Func<Project, IEnumerable<object>?> AnalyzerConfigDocumentsAccessor;
        private static readonly Func<object, DocumentId?> IdAccessor;
        private static readonly Func<object, string?> NameAccessor;
        private static readonly Func<object, string?> FilePathAccessor;
        private static readonly Func<object, CancellationToken, Task<SourceText>?> GetTextAsyncAccessor;

        static ProjectExtensions()
        {
            var textDocumentType = typeof(Project).GetTypeInfo().Assembly.GetType("Microsoft.CodeAnalysis.TextDocument");

            AnalyzerConfigDocumentsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<Project, IEnumerable<object>?>(typeof(Project), "AnalyzerConfigDocuments");
            IdAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, DocumentId?>(textDocumentType, "Id");
            NameAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, string?>(textDocumentType, "Name");
            FilePathAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, string?>(textDocumentType, "FilePath");
            GetTextAsyncAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, CancellationToken, Task<SourceText>?>(textDocumentType, typeof(CancellationToken), "GetTextAsync");
        }

        /// <summary>
        /// Gets the analyzer config documents of a project, or an empty array when the compiler does not support them.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The analyzer config documents with their text.</returns>
        public static async Task<ImmutableArray<AnalyzerConfigDocumentInfo>> GetAnalyzerConfigDocumentsAsync(this Project project, CancellationToken cancellationToken)
        {
            var documents = AnalyzerConfigDocumentsAccessor(project);
            if (documents == null)
            {
                return ImmutableArray<AnalyzerConfigDocumentInfo>.Empty;
            }

            var result = ImmutableArray.CreateBuilder<AnalyzerConfigDocumentInfo>();
            foreach (var document in documents)
            {
                var id = IdAccessor(document);
                var text = await (GetTextAsyncAccessor(document, cancellationToken) ?? Task.FromResult<SourceText>(null!)).ConfigureAwait(false);
                if (id == null || text == null)
                {
                    continue;
                }

                result.Add(new AnalyzerConfigDocumentInfo(id, NameAccessor(document) ?? string.Empty, FilePathAccessor(document), text));
            }

            return result.ToImmutable();
        }
    }
}
