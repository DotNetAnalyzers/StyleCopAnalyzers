namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not
    /// match the actual parameters on the element.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element's parameters does not match the actual
    /// parameters on the element, or if the parameter documentation is not listed in the same order as the element's parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1612ElementParameterDocumentationMustMatchElementParameters : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1612ElementParameterDocumentationMustMatchElementParameters"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1612";
        private const string Title = "Element parameter documentation must match element parameters";
        private const string MessageFormat = "Element parameter documentation must match element parameters";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The documentation describing the parameters to a C# method, constructor, delegate or indexer element does not match the actual parameters on the element.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1612.html";

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
            if (string.Equals(name.ToString(), XmlCommentHelper.ParamXmlTag))
            {
                var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);

                // Make sure we ignore violations that should be reported by SA1613 instead.
                if (!string.IsNullOrWhiteSpace(nameAttribute?.Identifier?.Identifier.ValueText) && ParentElementHasParameter(element, nameAttribute.Identifier.Identifier.ValueText) == false)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameAttribute?.Identifier?.GetLocation() ?? alternativeDiagnosticLocation));
                }
            }
        }

        /// <summary>
        /// Checks if the given <see cref="SyntaxNode"/> has a <see cref="BaseMethodDeclarationSyntax"/>, <see cref="IndexerDeclarationSyntax"/> or a <see cref="DelegateDeclarationSyntax"/>
        /// as one of its parent. If it finds one of those three with a valid parameter list it returns whether or not there is a parameter called <paramref name="parameterName"/>.
        /// </summary>
        /// <param name="node">The node the analysis should start at.</param>
        /// <param name="parameterName">The parameter name that should be checked</param>
        /// <returns>
        /// true, if one parent is a <see cref="BaseMethodDeclarationSyntax"/>, <see cref="IndexerDeclarationSyntax"/> or a <see cref="DelegateDeclarationSyntax"/> with a parameter called <paramref name="parameterName"/>.
        /// false, if one parent is a <see cref="BaseMethodDeclarationSyntax"/>, <see cref="IndexerDeclarationSyntax"/> or a <see cref="DelegateDeclarationSyntax"/> without a parameter called <paramref name="parameterName"/>.
        /// null if no parent could be found or invalid syntax is detected.
        /// </returns>
        private static bool? ParentElementHasParameter(SyntaxNode node, string parameterName)
        {
            var methodParent = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
            if (methodParent != null)
            {
                return methodParent.ParameterList?.Parameters.Any(x => x.Identifier.ValueText == parameterName);
            }

            var indexerParent = node.FirstAncestorOrSelf<IndexerDeclarationSyntax>();
            if (indexerParent != null)
            {
                return indexerParent.ParameterList?.Parameters.Any(x => x.Identifier.ValueText == parameterName);
            }

            var delegateParent = node.FirstAncestorOrSelf<DelegateDeclarationSyntax>();
            if (delegateParent != null)
            {
                return delegateParent.ParameterList?.Parameters.Any(x => x.Identifier.ValueText == parameterName);
            }

            return null;
        }
    }
}
