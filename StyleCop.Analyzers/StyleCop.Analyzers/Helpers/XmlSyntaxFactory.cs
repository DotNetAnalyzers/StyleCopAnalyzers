// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class XmlSyntaxFactory
    {
        public static DocumentationCommentTriviaSyntax DocumentationComment(string newLineText, params XmlNodeSyntax[] content)
        {
            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, List(content))
                .WithLeadingTrivia(SyntaxFactory.DocumentationCommentExterior("/// "))
                .WithTrailingTrivia(SyntaxFactory.EndOfLine(newLineText));
        }

        public static XmlElementSyntax MultiLineElement(string localName, string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement(SyntaxFactory.XmlName(localName), newLineText, content);
        }

        public static XmlElementSyntax MultiLineElement(XmlNameSyntax name, string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(name),
                content.Insert(0, NewLine(newLineText)).Add(NewLine(newLineText)),
                SyntaxFactory.XmlElementEndTag(name));
        }

        public static XmlElementSyntax Element(string localName, SyntaxList<XmlNodeSyntax> content)
        {
            return Element(SyntaxFactory.XmlName(localName), content);
        }

        public static XmlElementSyntax Element(XmlNameSyntax name, SyntaxList<XmlNodeSyntax> content)
        {
            return SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(name),
                content,
                SyntaxFactory.XmlElementEndTag(name));
        }

        public static XmlEmptyElementSyntax EmptyElement(string localName)
        {
            return SyntaxFactory.XmlEmptyElement(SyntaxFactory.XmlName(localName));
        }

        public static SyntaxList<XmlNodeSyntax> List(params XmlNodeSyntax[] nodes)
        {
            return SyntaxFactory.List(nodes);
        }

        public static XmlTextSyntax Text(string value)
        {
            return Text(TextLiteral(value));
        }

        public static XmlTextSyntax Text(params SyntaxToken[] textTokens)
        {
            return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, string value)
        {
            return TextAttribute(name, TextLiteral(value, true));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, params SyntaxToken[] textTokens)
        {
            return TextAttribute(SyntaxFactory.XmlName(name), SyntaxKind.DoubleQuoteToken, SyntaxFactory.TokenList(textTokens));
        }

        public static XmlTextAttributeSyntax TextAttribute(string name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return TextAttribute(SyntaxFactory.XmlName(name), quoteKind, textTokens);
        }

        public static XmlTextAttributeSyntax TextAttribute(XmlNameSyntax name, SyntaxKind quoteKind, SyntaxTokenList textTokens)
        {
            return SyntaxFactory.XmlTextAttribute(
                name,
                SyntaxFactory.Token(quoteKind),
                textTokens,
                SyntaxFactory.Token(quoteKind))
                .WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
        }

        public static XmlNameAttributeSyntax NameAttribute(string parameterName)
        {
            return SyntaxFactory.XmlNameAttribute(
                SyntaxFactory.XmlName(XmlCommentHelper.NameArgumentName),
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                parameterName,
                SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))
                .WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
        }

        public static XmlCrefAttributeSyntax CrefAttribute(CrefSyntax cref)
        {
            return CrefAttribute(cref, SyntaxKind.DoubleQuoteToken);
        }

        public static XmlCrefAttributeSyntax CrefAttribute(CrefSyntax cref, SyntaxKind quoteKind)
        {
            cref = cref.ReplaceTokens(cref.DescendantTokens(), ReplaceBracketTokens);
            return SyntaxFactory.XmlCrefAttribute(
                SyntaxFactory.XmlName(XmlCommentHelper.CrefArgumentName),
                SyntaxFactory.Token(quoteKind),
                cref,
                SyntaxFactory.Token(quoteKind))
                .WithLeadingTrivia(SyntaxFactory.Whitespace(" "));
        }

        public static XmlTextSyntax NewLine(string text)
        {
            return Text(TextNewLine(text));
        }

        public static SyntaxToken TextNewLine(string text)
        {
            return TextNewLine(text, true);
        }

        public static SyntaxToken TextNewLine(string text, bool continueComment)
        {
            SyntaxToken token = SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                text,
                text,
                SyntaxFactory.TriviaList());

            if (continueComment)
            {
                token = token.WithTrailingTrivia(SyntaxFactory.DocumentationCommentExterior("/// "));
            }

            return token;
        }

        public static SyntaxToken TextLiteral(string value)
        {
            return TextLiteral(value, false);
        }

        public static SyntaxToken TextLiteral(string value, bool escapeQuotes)
        {
            string encoded = new XText(value).ToString();
            if (escapeQuotes)
            {
                encoded = encoded.Replace("\"", "&quot;");
            }

            return SyntaxFactory.XmlTextLiteral(
                SyntaxFactory.TriviaList(),
                encoded,
                value,
                SyntaxFactory.TriviaList());
        }

        public static XmlElementSyntax SummaryElement(string newLineText, params XmlNodeSyntax[] content)
        {
            return SummaryElement(newLineText, List(content));
        }

        public static XmlElementSyntax SummaryElement(string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement(XmlCommentHelper.SummaryXmlTag, newLineText, content);
        }

        public static XmlElementSyntax RemarksElement(string newLineText, params XmlNodeSyntax[] content)
        {
            return RemarksElement(newLineText, List(content));
        }

        public static XmlElementSyntax RemarksElement(string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement("remarks", newLineText, content);
        }

        public static XmlElementSyntax ReturnsElement(string newLineText, params XmlNodeSyntax[] content)
        {
            return ReturnsElement(newLineText, List(content));
        }

        public static XmlElementSyntax ReturnsElement(string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement(XmlCommentHelper.ReturnsXmlTag, newLineText, content);
        }

        public static XmlElementSyntax ValueElement(string newLineText, params XmlNodeSyntax[] content)
        {
            return ValueElement(newLineText, List(content));
        }

        public static XmlElementSyntax ValueElement(string newLineText, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement(XmlCommentHelper.ValueXmlTag, newLineText, content);
        }

        public static XmlElementSyntax ExceptionElement(CrefSyntax cref, params XmlNodeSyntax[] content)
        {
            return ExceptionElement(cref, List(content));
        }

        public static XmlElementSyntax ExceptionElement(CrefSyntax cref, SyntaxList<XmlNodeSyntax> content)
        {
            XmlElementSyntax element = Element("exception", content);
            return element.WithStartTag(element.StartTag.AddAttributes(CrefAttribute(cref)));
        }

        public static XmlElementSyntax ParaElement(params XmlNodeSyntax[] content)
        {
            return ParaElement(List(content));
        }

        public static XmlElementSyntax ParaElement(SyntaxList<XmlNodeSyntax> content)
        {
            return Element("para", content);
        }

        public static XmlElementSyntax ParamElement(string parameterName, params XmlNodeSyntax[] content)
        {
            return ParamElement(parameterName, List(content));
        }

        public static XmlElementSyntax ParamElement(string parameterName, SyntaxList<XmlNodeSyntax> content)
        {
            XmlElementSyntax element = Element("param", content);
            return element.WithStartTag(element.StartTag.AddAttributes(NameAttribute(parameterName)));
        }

        public static XmlEmptyElementSyntax ParamRefElement(string parameterName)
        {
            return EmptyElement("paramref").AddAttributes(NameAttribute(parameterName));
        }

        public static XmlEmptyElementSyntax SeeElement(CrefSyntax cref)
        {
            return EmptyElement("see").AddAttributes(CrefAttribute(cref));
        }

        public static XmlEmptyElementSyntax SeeAlsoElement(CrefSyntax cref)
        {
            return EmptyElement("seealso").AddAttributes(CrefAttribute(cref));
        }

        public static XmlElementSyntax SeeAlsoElement(Uri linkAddress, SyntaxList<XmlNodeSyntax> linkText)
        {
            XmlElementSyntax element = Element("seealso", linkText);
            return element.WithStartTag(element.StartTag.AddAttributes(TextAttribute("href", linkAddress.ToString())));
        }

        public static XmlEmptyElementSyntax NullKeywordElement()
        {
            return KeywordElement("null");
        }

        public static XmlElementSyntax PlaceholderElement(params XmlNodeSyntax[] content)
        {
            return PlaceholderElement(List(content));
        }

        public static XmlElementSyntax PlaceholderElement(SyntaxList<XmlNodeSyntax> content)
        {
            return Element(XmlCommentHelper.PlaceholderTag, content);
        }

        public static XmlEmptyElementSyntax ThreadSafetyElement()
        {
            return ThreadSafetyElement(true, false);
        }

        public static XmlEmptyElementSyntax ThreadSafetyElement(bool @static, bool instance)
        {
            return EmptyElement("threadsafety").AddAttributes(
                TextAttribute("static", @static.ToString().ToLowerInvariant()),
                TextAttribute("instance", instance.ToString().ToLowerInvariant()));
        }

        public static XmlEmptyElementSyntax PreliminaryElement()
        {
            return EmptyElement("preliminary");
        }

        public static XmlElementSyntax TokenElement(string value)
        {
            return Element("token", List(Text(value)));
        }

        private static XmlEmptyElementSyntax KeywordElement(string keyword)
        {
            return EmptyElement("see").AddAttributes(
                TextAttribute("langword", keyword));
        }

        private static SyntaxToken ReplaceBracketTokens(SyntaxToken originalToken, SyntaxToken rewrittenToken)
        {
            if (rewrittenToken.IsKind(SyntaxKind.LessThanToken) && string.Equals("<", rewrittenToken.Text, StringComparison.Ordinal))
            {
                return SyntaxFactory.Token(rewrittenToken.LeadingTrivia, SyntaxKind.LessThanToken, "{", rewrittenToken.ValueText, rewrittenToken.TrailingTrivia);
            }

            if (rewrittenToken.IsKind(SyntaxKind.GreaterThanToken) && string.Equals(">", rewrittenToken.Text, StringComparison.Ordinal))
            {
                return SyntaxFactory.Token(rewrittenToken.LeadingTrivia, SyntaxKind.GreaterThanToken, "}", rewrittenToken.ValueText, rewrittenToken.TrailingTrivia);
            }

            return rewrittenToken;
        }
    }
}
