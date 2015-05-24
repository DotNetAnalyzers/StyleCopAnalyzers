namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;


    /// <summary>
    /// A <c>&lt;typeparam&gt;</c> tag within the XML header documentation for a generic C# element is missing a
    /// <c>name</c> attribute, or contains an empty <c>name</c> attribute.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the element contains a <c>&lt;typeparam&gt;</c> tag within its XML
    /// header documentation which does not declare the name of the type parameter.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1621GenericTypeParameterDocumentationMustDeclareParameterName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1621GenericTypeParameterDocumentationMustDeclareParameterName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1621";
        private const string Title = "Generic type parameter documentation must declare parameter name";
        private const string MessageFormat = "Generic type parameter documentation must declare parameter name";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "A <typeparam> tag within the XML header documentation for a generic C# element is missing a name attribute, or contains an empty name attribute.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1621.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
        }

        private void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax emptyElement = context.Node as XmlElementSyntax;

            var name = emptyElement?.StartTag?.Name;

            HandleElement(context, emptyElement, name, emptyElement?.StartTag?.GetLocation());
        }

        private void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax emptyElement = context.Node as XmlEmptyElementSyntax;

            var name = emptyElement?.Name;

            HandleElement(context, emptyElement, name, emptyElement?.GetLocation());
        }

        private static void HandleElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax element, XmlNameSyntax name, Location alternativeDiagnosticLocation)
        {
            if (string.Equals(name.ToString(), XmlCommentHelper.TypeParamXmlTag))
            {
                var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);

                if (string.IsNullOrWhiteSpace(nameAttribute?.Identifier?.Identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameAttribute?.GetLocation() ?? alternativeDiagnosticLocation));
                }
            }
        }
    }
}
