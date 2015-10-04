// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System;
    using Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public class XmlSyntaxFactoryTests
    {
        [Fact]
        public void TestPlainTextAttribute()
        {
            Assert.Equal(" name=\"value\"", XmlSyntaxFactory.TextAttribute("name", "value").ToFullString());
        }

        [Fact]
        public void TestEscapedTextAttribute()
        {
            Assert.Equal(" name=\"&quot;value&quot;\"", XmlSyntaxFactory.TextAttribute("name", "\"value\"").ToFullString());
            Assert.Equal(" name=\"value&amp;2\"", XmlSyntaxFactory.TextAttribute("name", "value&2").ToFullString());
        }

        [Fact]
        public void TestTextAttributeQuotes()
        {
            SyntaxTokenList textTokens = SyntaxFactory.TokenList(XmlSyntaxFactory.TextLiteral("value"));
            Assert.Equal(" name=\"value\"", XmlSyntaxFactory.TextAttribute("name", SyntaxKind.DoubleQuoteToken, textTokens).ToFullString());
            Assert.Equal(" name='value'", XmlSyntaxFactory.TextAttribute("name", SyntaxKind.SingleQuoteToken, textTokens).ToFullString());
        }

        [Fact]
        public void TestNameAttribute()
        {
            Assert.Equal(" name=\"value\"", XmlSyntaxFactory.NameAttribute("value").ToFullString());
        }

        [Fact]
        public void TestTextNewLine()
        {
            Assert.Equal("\r\n/// ", XmlSyntaxFactory.TextNewLine("\r\n", true).ToFullString());
            Assert.Equal("\r\n", XmlSyntaxFactory.TextNewLine("\r\n", false).ToFullString());
            Assert.Equal("\n/// ", XmlSyntaxFactory.TextNewLine("\n", true).ToFullString());
            Assert.Equal("\n", XmlSyntaxFactory.TextNewLine("\n", false).ToFullString());
        }

        [Fact]
        public void TestSummaryElement()
        {
            string expected =
                "<summary>\r\n"
                + "/// Summary.\r\n"
                + "/// </summary>";
            Assert.Equal(expected, XmlSyntaxFactory.SummaryElement("\r\n", XmlSyntaxFactory.Text("Summary.")).ToFullString());
        }

        [Fact]
        public void TestRemarksElement()
        {
            string expected =
                "<remarks>\r\n"
                + "/// <para>Remarks.</para>\r\n"
                + "/// </remarks>";
            Assert.Equal(expected, XmlSyntaxFactory.RemarksElement("\r\n", XmlSyntaxFactory.ParaElement(XmlSyntaxFactory.Text("Remarks."))).ToFullString());
        }

        [Fact]
        public void TestReturnsElement()
        {
            string expected =
                "<returns>\r\n"
                + "/// Returns.\r\n"
                + "/// </returns>";
            Assert.Equal(expected, XmlSyntaxFactory.ReturnsElement("\r\n", XmlSyntaxFactory.Text("Returns.")).ToFullString());
        }

        [Fact]
        public void TestValueElement()
        {
            string expected =
                "<value>\r\n"
                + "/// Value.\r\n"
                + "/// </value>";
            Assert.Equal(expected, XmlSyntaxFactory.ValueElement("\r\n", XmlSyntaxFactory.Text("Value.")).ToFullString());
        }

        [Fact]
        public void TestExceptionElement()
        {
            string expected = "<exception cref=\"System.ArgumentNullException\">Condition.</exception>";
            Assert.Equal(expected, XmlSyntaxFactory.ExceptionElement(SyntaxFactory.TypeCref(SyntaxFactory.ParseTypeName(typeof(ArgumentNullException).FullName)), XmlSyntaxFactory.Text("Condition.")).ToFullString());
        }

        [Fact]
        public void TestParamElement()
        {
            string expected = "<param name=\"parameterName\">Parameter.</param>";
            Assert.Equal(expected, XmlSyntaxFactory.ParamElement("parameterName", XmlSyntaxFactory.Text("Parameter.")).ToFullString());
        }

        [Fact]
        public void TestParamRefElement()
        {
            string expected = "<paramref name=\"parameterName\"/>";
            Assert.Equal(expected, XmlSyntaxFactory.ParamRefElement("parameterName").ToFullString());
        }

        [Fact]
        public void TestSeeAlsoElementCref()
        {
            string expected = "<seealso cref=\"System.ArgumentNullException\"/>";
            Assert.Equal(expected, XmlSyntaxFactory.SeeAlsoElement(SyntaxFactory.TypeCref(SyntaxFactory.ParseTypeName(typeof(ArgumentNullException).FullName))).ToFullString());
        }

        [Fact]
        public void TestSeeAlsoElementHref()
        {
            string expected = "<seealso href=\"http://www.example.com/\">Text.</seealso>";
            Assert.Equal(expected, XmlSyntaxFactory.SeeAlsoElement(new Uri("http://www.example.com/"), XmlSyntaxFactory.List(XmlSyntaxFactory.Text("Text."))).ToFullString());
        }

        [Fact]
        public void TestNullKeywordElement()
        {
            string expected = "<see langword=\"null\"/>";
            Assert.Equal(expected, XmlSyntaxFactory.NullKeywordElement().ToFullString());
        }

        [Fact]
        public void TestPlaceholderElement()
        {
            string expected = "<placeholder>Content.</placeholder>";
            Assert.Equal(expected, XmlSyntaxFactory.PlaceholderElement(XmlSyntaxFactory.Text("Content.")).ToFullString());
        }

        [Fact]
        public void TestThreadSafetyElement()
        {
            string expected = "<threadsafety static=\"true\" instance=\"false\"/>";
            Assert.Equal(expected, XmlSyntaxFactory.ThreadSafetyElement().ToFullString());
        }

        [Fact]
        public void TestPreliminaryElement()
        {
            string expected = "<preliminary/>";
            Assert.Equal(expected, XmlSyntaxFactory.PreliminaryElement().ToFullString());
        }

        [Fact]
        public void TestTokenElement()
        {
            string expected = "<token>Identifier</token>";
            Assert.Equal(expected, XmlSyntaxFactory.TokenElement("Identifier").ToFullString());
        }
    }
}
