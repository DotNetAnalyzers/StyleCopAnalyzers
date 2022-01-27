// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    internal static class ParameterDocumentationHelper
    {
        public static SyntaxNode GetParameterDocumentationPrevNode<TParameter>(
            SyntaxNode parentDeclaration,
            TParameter parameterSyntax,
            IList<TParameter> parentDeclarationParameters,
            Func<TParameter, SyntaxToken> getParamName,
            string xmlElementName)
        {
            var documentation = parentDeclaration.GetDocumentationCommentTriviaSyntax();

            var paramNodesDocumentation = documentation.Content
                .GetXmlElements(xmlElementName)
                .ToList();
            var parameterIndex = parentDeclarationParameters.IndexOf(parameterSyntax);

            SyntaxNode prevNode = null;
            if (parameterIndex != 0)
            {
                var count = 0;
                foreach (XmlNodeSyntax paramXmlNode in paramNodesDocumentation)
                {
                    var name = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(paramXmlNode);
                    if (name != null)
                    {
                        var nameValue = name.Identifier.Identifier.ValueText;
                        if (getParamName(parentDeclarationParameters[count]).ValueText == nameValue)
                        {
                            count++;
                            if (count == parameterIndex)
                            {
                                prevNode = paramXmlNode;
                                break;
                            }

                            continue;
                        }

                        prevNode = paramXmlNode;
                        break;
                    }
                }
            }

            return prevNode;
        }
    }
}
