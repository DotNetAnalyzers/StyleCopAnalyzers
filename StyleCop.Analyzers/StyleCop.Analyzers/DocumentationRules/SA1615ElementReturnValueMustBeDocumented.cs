namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A C# element is missing documentation for its return value.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing a return value is missing a
    /// <c>&lt;returns&gt;</c> tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1615ElementReturnValueMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1615ElementReturnValueMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1615";
        private const string Title = "Element return value must be documented";
        private const string MessageFormat = "Element return value must be documented";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "A C# element is missing documentation for its return value.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1615.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;

            this.HandleDeclaration(context, node.ReturnType);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as DelegateDeclarationSyntax;

            this.HandleDeclaration(context, node.ReturnType);
        }

        private void HandleDeclaration(SyntaxNodeAnalysisContext context, TypeSyntax returnType)
        {
            var predefinedType = returnType as PredefinedTypeSyntax;

            if (predefinedType != null && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                // There is no return value
                return;
            }

            var documentationStructure = context.Node.GetDocumentationCommentTriviaSyntax();

            if (documentationStructure == null)
            {
                return;
            }

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Don't report if the documentation is inherited.
                return;
            }

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag) == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, returnType.GetLocation()));
            }
        }
    }
}
