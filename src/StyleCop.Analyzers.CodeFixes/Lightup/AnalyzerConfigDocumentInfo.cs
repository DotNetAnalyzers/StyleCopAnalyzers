// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Describes an analyzer config document (<strong>.editorconfig</strong> or <strong>.globalconfig</strong>) of a
    /// project.
    /// </summary>
    internal readonly struct AnalyzerConfigDocumentInfo
    {
        public AnalyzerConfigDocumentInfo(DocumentId id, string name, string? filePath, SourceText text)
        {
            this.Id = id;
            this.Name = name;
            this.FilePath = filePath;
            this.Text = text;
        }

        public DocumentId Id { get; }

        public string Name { get; }

        public string? FilePath { get; }

        public SourceText Text { get; }
    }
}
