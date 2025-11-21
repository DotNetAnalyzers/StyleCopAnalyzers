// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class XmlSyntaxFactory
    {
        public const string CrLf = "\r\n";

        public static SyntaxToken XmlCarriageReturnLineFeedWithContinuation { get; } = SyntaxFactory.XmlTextNewLine(CrLf, continueXmlDocumentationComment: true);
    }
}
