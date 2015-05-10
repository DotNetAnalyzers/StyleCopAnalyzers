namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class XmlSyntaxFactory
    {
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

        public static SyntaxList<XmlNodeSyntax> List(params XmlNodeSyntax[] nodes)
        {
            return SyntaxFactory.List(nodes);
        }

        public static XmlTextSyntax Text(params SyntaxToken[] textTokens)
        {
            return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
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

        public static XmlElementSyntax PlaceholderElement(SyntaxList<XmlNodeSyntax> content)
        {
            return Element(XmlCommentHelper.PlaceholderTag, content);
        }
    }
}
