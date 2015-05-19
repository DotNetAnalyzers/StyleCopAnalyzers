namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;
    using System;

    /// <summary>
    /// An item within a C# enumeration is missing an XML documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when an item within an enumeration is missing a header. For
    /// example:</para>
    ///
    /// <code>
    /// /// &lt;summary&gt;
    /// /// Types of animals.
    /// /// &lt;/summary&gt;
    /// public enum Animals
    /// {
    ///     Dog,
    ///     Cat,
    ///     Horse
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1602EnumerationItemsMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1602EnumerationItemsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1602";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1602EnumerationItemsMustBeDocumented"/> analyzer
        /// for internal members.
        /// </summary>
        public const string DiagnosticIdInternal = "SA1602In";

        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1602EnumerationItemsMustBeDocumented"/> analyzer
        /// for private members.
        /// </summary>
        public const string DiagnosticIdPrivate = "SA1602Pr";

        private const string Title = "Enumeration items must be documented";
        private const string MessageFormat = "Enumeration items must be documented";
        private const string Description = "An item within a publicly visible C# enumeration is missing an Xml documentation header.";

        private const string TitleInternal = "Enumeration items must be documented (internal visibility)";
        private const string MessageFormatInternal = "Enumeration items must be documented (internal visibility)";
        private const string DescriptionInternal = "An item within an internal C# enumeration is missing an Xml documentation header.";

        private const string TitlePrivate = "Enumeration items must be documented (private visibility)";
        private const string MessageFormatPrivate = "Enumeration items must be documented (private visibility)";
        private const string DescriptionPrivate = "An item within a private C# enumeration is missing an Xml documentation header.";

        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1602.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEnumMember, SyntaxKind.EnumMemberDeclaration);
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

        private void HandleEnumMember(SyntaxNodeAnalysisContext context)
        {
            EnumMemberDeclarationSyntax enumMemberDeclaration = context.Node as EnumMemberDeclarationSyntax;
            if (enumMemberDeclaration != null)
            {
                if (!XmlCommentHelper.HasDocumentation(enumMemberDeclaration))
                {
                    var effective = EffectiveVisibilityHelper.ResolveVisibilityForChild(SyntaxKind.PublicKeyword, enumMemberDeclaration.Parent as BaseTypeDeclarationSyntax);

                    context.ReportDiagnostic(Diagnostic.Create(DescriptorFromEffectiveVisibility(effective), enumMemberDeclaration.Identifier.GetLocation()));
                }
            }
        }
    }
}