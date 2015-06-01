namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A non-private C# code element is missing a documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of Xml documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx"/>.</para>
    ///
    /// <para>A violation of this rule occurs if an element is completely missing a documentation header, or if the
    /// header is empty. In C# the following types of elements can have documentation headers: classes, constructors,
    /// delegates, enums, events, finalizers, indexers, interfaces, methods, properties, and structs.</para>
    /// <para>No violation of this rule occurs for public elements; these are covered by <see href="https://msdn.microsoft.com/en-us/library/zk18c1w9.aspx">CS1591</see>.
    /// This rule specializes <see cref="SA1600ElementsMustBeDocumented"/>, <see cref="SA1601PartialElementsMustBeDocumented"/> and <see cref="SA1602EnumerationItemsMustBeDocumented"/>
    /// for non-private elements only.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA16X0NonPrivateElementsMustBeDocumented : DocumentationAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA16X0NonPrivateElementsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA16X0";

        private const string Title = "Non-public elements must be documented";
        private const string MessageFormat = "Non-public elements must be documented";
        private const string Description = "A non-public C# code element is missing a documentation header.";

        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA16X0.md";

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

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            // Complement of to SA1600.
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleTypeDeclaration, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleFieldDeclaration, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventDeclaration, SyntaxKind.EventDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);

            // Complement of SA1601.
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePartialTypeDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePartialTypeDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePartialTypeDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandlePartialMethodDeclaration, SyntaxKind.MethodDeclaration);

            // Complement of SA1602.
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEnumMember, SyntaxKind.EnumMemberDeclaration);
        }

        /// <inheritdoc/>
        protected override DiagnosticDescriptor DescriptorFromEffectiveVisibility(SyntaxKind visibility, SyntaxNodeAnalysisContext context)
        {
            // Public is handled by CS1591 and private, by SA1600-SA1603.
            return visibility == SyntaxKind.InternalKeyword ? Descriptor : null;
        }
    }
}
