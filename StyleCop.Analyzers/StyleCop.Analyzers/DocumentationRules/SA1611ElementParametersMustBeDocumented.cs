namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# method, constructor, delegate or indexer element is missing documentation for one or more of its
    /// parameters.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing parameters is missing documentation for one or
    /// more of its parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1611ElementParametersMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1611ElementParametersMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1611";
        private const string Title = "Element parameters must be documented";
        private const string MessageFormat = "The documentation for parameter '{0}' is missing";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "A C# method, constructor, delegate or indexer element is missing documentation for one or more of its parameters.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1611.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSyntaxNode, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSyntaxNode, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSyntaxNode, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleSyntaxNode, SyntaxKind.IndexerDeclaration);
        }

        private void HandleSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var documentation = XmlCommentHelper.GetDocumentationStructure(node);

            if (documentation != null)
            {
                IEnumerable<ParameterSyntax> parameterList = this.GetParameters(node);

                if (parameterList == null)
                {
                    return;
                }

                if (XmlCommentHelper.GetTopLevelElement(documentation, XmlCommentHelper.InheritdocXmlTag) != null)
                {
                    // Ignore nodes with an <inheritdoc/> tag.
                    return;
                }

                var xmlParameterNames = XmlCommentHelper.GetTopLevelElements(documentation, XmlCommentHelper.ParamTag)
                    .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                    .Where(x => x != null)
                    .ToImmutableArray();

                foreach (var parameter in parameterList)
                {
                    if (!xmlParameterNames.Any(x => x.Identifier.Identifier.ValueText == parameter.Identifier.ValueText))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                    }
                }
            }
        }

        private IEnumerable<ParameterSyntax> GetParameters(SyntaxNode node)
        {
            return (node as BaseMethodDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as IndexerDeclarationSyntax)?.ParameterList?.Parameters
                ?? (node as DelegateDeclarationSyntax)?.ParameterList?.Parameters;
        }
    }
}
