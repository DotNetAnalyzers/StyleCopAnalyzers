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
    /// A generic C# element is missing documentation for one or more of its generic type parameters.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing generic type parameters is missing documentation
    /// for one or more of its generic type parameters.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1618GenericTypeParametersMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1618GenericTypeParametersMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1618";
        private const string Title = "Generic type parameters must be documented";
        private const string MessageFormat = "The documentation for type parameter '{0}' is missing";
        private const string Description = "A generic C# element is missing documentation for one or more of its generic type parameters.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1618.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);

            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDeclaration = (TypeDeclarationSyntax)context.Node;

            if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // This is handled by SA1619
                return;
            }

            HandleMemberDeclaration(context, typeDeclaration, typeDeclaration.TypeParameterList);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)context.Node;

            HandleMemberDeclaration(context, methodDeclaration, methodDeclaration.TypeParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            DelegateDeclarationSyntax delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            HandleMemberDeclaration(context, delegateDeclaration, delegateDeclaration.TypeParameterList);
        }

        private static void HandleMemberDeclaration(SyntaxNodeAnalysisContext context, SyntaxNode node, TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList == null)
            {
                // The member does not have a type parameter list
                return;
            }

            var documentation = node.GetDocumentationCommentTriviaSyntax();

            if (documentation == null)
            {
                // Don't report if the documentation is missing
                return;
            }

            if (documentation.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Ignore nodes with an <inheritdoc/> tag.
                return;
            }

            var xmlParameterNames = documentation.Content.GetXmlElements(XmlCommentHelper.TypeParamXmlTag)
                .Select(XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                .Where(x => x != null)
                .ToImmutableArray();

            foreach (var parameter in typeParameterList.Parameters)
            {
                if (!xmlParameterNames.Any(x => x.Identifier.Identifier.ValueText == parameter.Identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Identifier.GetLocation(), parameter.Identifier.ValueText));
                }
            }
        }
    }
}
