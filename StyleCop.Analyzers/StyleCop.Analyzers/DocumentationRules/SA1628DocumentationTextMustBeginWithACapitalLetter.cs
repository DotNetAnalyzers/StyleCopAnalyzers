// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A section of the XML header documentation for a C# element does not begin with a capital letter.
    /// </summary>
    /// <remarks>
    /// <para>This diagnostic is not implemented in StyleCopAnalyzers.</para>
    ///
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when part of the documentation does not begin with a capital letter. For
    /// example, the summary text in the documentation below begins with a lower-case letter:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// joins a first name and a last name together into a single string.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="firstName"&gt;The first name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;The last name.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoDiagnostic("This diagnostic has an unacceptable rate of false positives.")]
    internal class SA1628DocumentationTextMustBeginWithACapitalLetter : ElementDocumentationBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1628DocumentationTextMustBeginWithACapitalLetter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1628";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1628Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1628MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1628Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1628.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly string[] SupportedTags =
            {
                XmlCommentHelper.SummaryXmlTag,
                XmlCommentHelper.ReturnsXmlTag,
                XmlCommentHelper.ParamXmlTag,
                XmlCommentHelper.RemarksXmlTag,
                XmlCommentHelper.ExampleXmlTag,
                XmlCommentHelper.PermissionXmlTag,
                XmlCommentHelper.ExceptionXmlTag,
            };

        public SA1628DocumentationTextMustBeginWithACapitalLetter()
            : base(inheritDocSuppressesWarnings: false)
        {
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        protected override void HandleXmlElement(SyntaxNodeAnalysisContext context, IEnumerable<XmlNodeSyntax> syntaxList, params Location[] diagnosticLocations)
        {
            foreach (var documentationSyntax in syntaxList)
            {
                CheckDocumentationSyntax(context, documentationSyntax);
            }
        }

        protected override void HandleCompleteDocumentation(SyntaxNodeAnalysisContext context, XElement completeDocumentation, params Location[] diagnosticLocations)
        {
            // We are working with an <include> element
            foreach (string elementName in SupportedTags)
            {
                if (CheckElement(context, completeDocumentation, diagnosticLocations, elementName))
                {
                    break;
                }
            }
        }

        private static void CheckDocumentationSyntax(SyntaxNodeAnalysisContext context, XmlNodeSyntax documentationSyntax)
        {
            XmlElementStartTagSyntax startTagSyntax = documentationSyntax.ChildNodes().FirstOrDefault() as XmlElementStartTagSyntax;
            if (startTagSyntax == null)
            {
                return;
            }

            foreach (var syntaxNode in documentationSyntax.ChildNodes().Skip(1))
            {
                var textSyntax = syntaxNode as XmlTextSyntax;
                if (textSyntax == null)
                {
                    var cdataSyntax = syntaxNode as XmlCDataSectionSyntax;

                    if (cdataSyntax != null)
                    {
                        CheckIfFirstLetterUppercase(context, startTagSyntax, cdataSyntax.TextTokens);
                    }

                    return;
                }

                if (!XmlCommentHelper.IsConsideredEmpty(textSyntax))
                {
                    CheckIfFirstLetterUppercase(context, startTagSyntax, textSyntax.TextTokens);
                    return;
                }
            }
        }

        private static void CheckIfFirstLetterUppercase(SyntaxNodeAnalysisContext context, XmlElementStartTagSyntax startTagSyntax, SyntaxTokenList textTokens)
        {
            foreach (var textToken in textTokens)
            {
                string text = textToken.Text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                int index = GetInvalidFirstLetterIndex(text);
                if (index != -1)
                {
                    // Add a diagnostic on the first character
                    TextSpan location = textToken.GetLocation().SourceSpan;
                    TextSpan character = TextSpan.FromBounds(location.Start + index, location.Start + index + 1);
                    string elementName = startTagSyntax.Name.ToString();
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(textToken.SyntaxTree, character), elementName));
                }
            }
        }

        private static bool CheckElement(SyntaxNodeAnalysisContext context, XElement completeDocumentation, Location[] diagnosticLocations, string elementName)
        {
            var includedElement = completeDocumentation.Nodes().OfType<XElement>().FirstOrDefault(element => element.Name == elementName);
            if (includedElement == null)
            {
                return false;
            }

            foreach (XNode node in includedElement.Nodes())
            {
                var textNode = node as XText;
                if (textNode == null)
                {
                    return false;
                }

                if (!XmlCommentHelper.IsConsideredEmpty(textNode))
                {
                    return CheckIfFirstLetterUppercase(context, diagnosticLocations, elementName, textNode.Value);
                }
            }

            return false;
        }

        private static bool CheckIfFirstLetterUppercase(SyntaxNodeAnalysisContext context, Location[] diagnosticLocations, string elementName, string text)
        {
            int index = GetInvalidFirstLetterIndex(text);
            if (index == -1)
            {
                return false;
            }

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("nofix", string.Empty);

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, diagnosticLocations.First(), properties.ToImmutableDictionary(), elementName));
            return true;
        }

        private static int GetInvalidFirstLetterIndex(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    continue;
                }

                if (!char.IsUpper(text[i]))
                {
                    return i;
                }

                break;
            }

            return -1;
        }
    }
}
