namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The XML within a C# element's document header is badly formed.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation XML is badly formed and cannot be parsed. This can
    /// occur if the XML contains invalid characters, or if an XML node is missing a closing tag, for example.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Trying to fix invalid xml would most likely not produce the desired result.")]
    public class SA1603DocumentationMustContainValidXml : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1603DocumentationMustContainValidXml"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1603";
        private const string Title = "Documentation must contain valid XML";
        private const string MessageFormat = "The documentation header is composed of invalid XML: {0}";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The XML within a C# element’s document header is badly formed.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1603.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleXmlElement, SyntaxKind.XmlElement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleXmlEmptyElement, SyntaxKind.XmlEmptyElement);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleXmlCDataSection, SyntaxKind.XmlCDataSection);
        }

        private void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            var xmlElementSyntax = context.Node as XmlElementSyntax;

            if (xmlElementSyntax != null)
            {
                if (xmlElementSyntax.StartTag.LessThanToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.StartTag.GetLocation(), "XML element start tag is missing a '<'."));
                }

                if (xmlElementSyntax.StartTag.GreaterThanToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.StartTag.GetLocation(), "XML element start tag is missing a '>'."));
                }

                if (xmlElementSyntax.EndTag.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.StartTag.GetLocation(), $"The XML tag '{xmlElementSyntax.StartTag.Name}' is not closed."));
                }
                else
                {
                    if (xmlElementSyntax.EndTag.LessThanSlashToken.IsMissing)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.EndTag.GetLocation(), "XML element end tag is missing a '</'."));
                    }

                    if (xmlElementSyntax.EndTag.GreaterThanToken.IsMissing)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.EndTag.GetLocation(), "XML element end tag is missing a '>'."));
                    }

                    if (xmlElementSyntax.StartTag.Name.ToString() != xmlElementSyntax.EndTag.Name.ToString())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlElementSyntax.StartTag.GetLocation(), new[] { xmlElementSyntax.EndTag.GetLocation() }, $"The '{xmlElementSyntax.StartTag.Name}' start tag does not match the end tag of '{xmlElementSyntax.EndTag.Name}'."));
                    }
                }

                IEnumerable<SyntaxTrivia> skippedTokens = xmlElementSyntax.StartTag.DescendantTrivia()
                    .Concat(xmlElementSyntax.EndTag.DescendantTrivia())
                    .Where(trivia => trivia.IsKind(SyntaxKind.SkippedTokensTrivia));

                foreach (var item in skippedTokens)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, item.GetLocation(), "Invalid token."));
                }
            }
        }

        private void HandleXmlCDataSection(SyntaxNodeAnalysisContext context)
        {
            var xmlCDataSection = context.Node as XmlCDataSectionSyntax;

            if (xmlCDataSection != null)
            {
                if (xmlCDataSection.StartCDataToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlCDataSection.GetLocation(), "XML CDATA section is missing a start token."));
                }

                if (xmlCDataSection.EndCDataToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlCDataSection.GetLocation(), "XML CDATA section is missing an end token."));
                }

                IEnumerable<SyntaxTrivia> skippedTokens = xmlCDataSection.StartCDataToken.GetAllTrivia()
                    .Concat(xmlCDataSection.EndCDataToken.GetAllTrivia())
                    .Where(trivia => trivia.IsKind(SyntaxKind.SkippedTokensTrivia));

                foreach (var item in skippedTokens)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, item.GetLocation(), "Invalid token."));
                }
            }
        }

        private void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            var xmlEmptyElement = context.Node as XmlEmptyElementSyntax;

            if (xmlEmptyElement != null)
            {
                if (xmlEmptyElement.LessThanToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlEmptyElement.GetLocation(), "XML empty element is missing a '<'."));
                }

                if (xmlEmptyElement.SlashGreaterThanToken.IsMissing)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, xmlEmptyElement.GetLocation(), "XML empty element is missing a '/>'."));
                }

                IEnumerable<SyntaxTrivia> skippedTokens = xmlEmptyElement.DescendantTrivia()
                    .Where(trivia => trivia.IsKind(SyntaxKind.SkippedTokensTrivia));

                foreach (var item in skippedTokens)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, item.GetLocation(), "Invalid token."));
                }
            }
        }
    }
}
