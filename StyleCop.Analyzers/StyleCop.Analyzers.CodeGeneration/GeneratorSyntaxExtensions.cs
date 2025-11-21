// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class GeneratorSyntaxExtensions
    {
        public static TSyntax WithLeadingBlankLine<TSyntax>(this TSyntax syntax)
            where TSyntax : SyntaxNode
        {
            return syntax.WithLeadingTrivia(SyntaxFactory.TriviaList(
                SyntaxFactory.PreprocessingMessage(XmlSyntaxFactory.CrLf)));
        }
    }
}
