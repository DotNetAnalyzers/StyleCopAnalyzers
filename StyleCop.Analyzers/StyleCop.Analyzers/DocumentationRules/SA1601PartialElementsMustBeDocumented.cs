namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using System;

    /// <summary>
    /// A C# partial element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if a partial element (an element with the partial attribute) is completely
    /// missing a documentation header, or if the header is empty. In C# the following types of elements can be
    /// attributed with the partial attribute: classes, methods.</para>
    ///
    /// <para>When documentation is provided on more than one part of the partial class, the documentation for the two
    /// classes may be merged together to form a single source of documentation. For example, consider the following two
    /// parts of a partial class:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Documentation for the first part of Class1.
    /// /// &lt;/summary&gt;
    /// public partial class Class1
    /// {
    /// }
    ///
    /// /// &lt;summary&gt;
    /// /// Documentation for the second part of Class1.
    /// /// &lt;/summary&gt;
    /// public partial class Class1
    /// {
    /// }
    /// </code>
    ///
    /// <para>These two different parts of the same partial class each provide different documentation for the class.
    /// When the documentation for this class is built into an SDK, the tool building the documentation will either
    /// choose to use only one part of the documentation for the class and ignore the other parts, or, in some cases, it
    /// may merge the two sources of documentation together, to form a string like: "Documentation for the first part of
    /// Class1. Documentation for the second part of Class1."</para>
    ///
    /// <para>For these reasons, it can be problematic to provide SDK documentation on more than one part of the partial
    /// class. However, it is still advisable to document each part of the class, to increase the readability and
    /// maintainability of the code, and StyleCop will require that each part of the class contain header
    /// documentation.</para>
    ///
    /// <para>This problem is solved through the use of the <c>&lt;content&gt;</c> tag, which can replace the
    /// <c>&lt;summary&gt;</c> tag for partial classes. The recommended practice for documenting partial classes is to
    /// provide the official SDK documentation for the class on the main part of the partial class. This documentation
    /// should be written using the standard <c>&lt;summary&gt;</c> tag. All other parts of the partial class should
    /// omit the <c>&lt;summary&gt;</c> tag completely, and replace it with a <c>&lt;content&gt;</c> tag. This allows
    /// the developer to document all parts of the partial class while still centralizing all of the official SDK
    /// documentation for the class onto one part of the class. The <c>&lt;content&gt;</c> tags will be ignored by the
    /// SDK documentation tools.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1601PartialElementsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1601PartialElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1601";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1601PartialElementsMustBeDocumented"/> analyzer
        /// for internal members.
        /// </summary>
        public const string DiagnosticIdInternal = "SA1601In";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1601PartialElementsMustBeDocumented"/> analyzer
        /// for private members.
        /// </summary>
        public const string DiagnosticIdPrivate = "SA1601Pr";

        private const string Title = "Partial elements must be documented";
        private const string MessageFormat = "Partial elements must be documented";
        private const string Description = "A publicly visible C# partial element is missing a documentation header.";

        private const string TitleInternal = "Partial elements must be documented (internal visibility)";
        private const string MessageFormatInternal = "Partial elements must be documented (internal visibility)";
        private const string DescriptionInternal = "An internal C# partial element is missing a documentation header.";

        private const string TitlePrivate = "Partial elements must be documented (private visibility)";
        private const string MessageFormatPrivate = "Partial elements must be documented (private visibility)";
        private const string DescriptionPrivate = "A private C# partial element is missing a documentation header.";

        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1601.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor DescriptorInternal =
            new DiagnosticDescriptor(DiagnosticIdInternal, TitleInternal, MessageFormatInternal, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionInternal, HelpLink);

        private static readonly DiagnosticDescriptor DescriptorPrivate =
            new DiagnosticDescriptor(DiagnosticIdPrivate, TitlePrivate, MessageFormatPrivate, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionPrivate, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor, DescriptorInternal, DescriptorPrivate);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static DiagnosticDescriptor DescriptorFromEffectiveVisibility(SyntaxKind visibility)
        {
            switch (visibility)
            {
            case SyntaxKind.PublicKeyword:
                return Descriptor;

            case SyntaxKind.InternalKeyword:
                return DescriptorInternal;

            case SyntaxKind.PrivateKeyword:
                return DescriptorPrivate;

            default:
                throw new ArgumentOutOfRangeException("visibility");
            }
        }

        private void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            TypeDeclarationSyntax typeDeclaration = context.Node as TypeDeclarationSyntax;
            if (typeDeclaration != null)
            {
                if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!XmlCommentHelper.HasDocumentation(typeDeclaration))
                    {
                        bool isNested = typeDeclaration.Parent is BaseTypeDeclarationSyntax;
                        var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(
                            EffectiveVisibilityHelper.EffectiveVisibility(typeDeclaration.Modifiers, isNested ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword),
                            typeDeclaration.Parent as BaseTypeDeclarationSyntax);

                        context.ReportDiagnostic(Diagnostic.Create(DescriptorFromEffectiveVisibility(effective), typeDeclaration.Identifier.GetLocation()));
                    }
                }
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            MethodDeclarationSyntax methodDeclaration = context.Node as MethodDeclarationSyntax;
            if (methodDeclaration != null)
            {
                if (methodDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    if (!XmlCommentHelper.HasDocumentation(methodDeclaration))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(DescriptorPrivate, methodDeclaration.Identifier.GetLocation()));
                    }
                }
            }
        }
    }
}
