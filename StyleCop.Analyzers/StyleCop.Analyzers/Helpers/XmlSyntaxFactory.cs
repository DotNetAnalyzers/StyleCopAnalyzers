namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class XmlSyntaxFactory
    {
        public static XmlElementSyntax MultiLineElement(string localName, SyntaxList<XmlNodeSyntax> content)
        {
            return MultiLineElement(SyntaxFactory.XmlName(localName), content);
        }

        public static XmlElementSyntax MultiLineElement(XmlNameSyntax name, SyntaxList<XmlNodeSyntax> content)
        {
            return SyntaxFactory.XmlElement(
                SyntaxFactory.XmlElementStartTag(name),
                content.Insert(0, NewLine()).Add(NewLine()),
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

        public static SyntaxList<XmlNodeSyntax> List(params XmlNodeSyntax[] nodes)
        {
            return SyntaxFactory.List(nodes);
        }

        public static XmlTextSyntax Text(params SyntaxToken[] textTokens)
        {
            return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
        }

        public static XmlTextSyntax NewLine()
        {
            return Text(TextNewLine());
        }

        public static SyntaxToken TextNewLine()
        {
            return TextNewLine(true);
        }

        public static SyntaxToken TextNewLine(bool continueComment)
        {
            SyntaxToken token = SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                Environment.NewLine,
                Environment.NewLine,
                SyntaxFactory.TriviaList());

            if (continueComment)
                token = token.WithTrailingTrivia(SyntaxFactory.DocumentationCommentExterior("/// "));

            return token;
        }

        public static XmlElementSyntax PlaceholderElement(SyntaxList<XmlNodeSyntax> content)
        {
            return Element(XmlCommentHelper.PlaceholderTag, content);
        }
    }
}
