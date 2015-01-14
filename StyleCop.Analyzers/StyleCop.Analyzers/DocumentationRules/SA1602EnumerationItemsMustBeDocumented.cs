namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Linq;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;


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
        public const string DiagnosticId = "SA1602";
        internal const string Title = "Enumeration items must be documented";
        internal const string MessageFormat = "Enumeration items must have a non empty documentation";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "An item within a C# enumeration is missing an Xml documentation header.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1602.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleEnumMember, SyntaxKind.EnumMemberDeclaration);
        }

        private void HandleEnumMember(SyntaxNodeAnalysisContext context)
        {
            EnumMemberDeclarationSyntax enumMemberDeclaration = context.Node as EnumMemberDeclarationSyntax;
            if (enumMemberDeclaration != null)
            {
                    var leadingTrivia = enumMemberDeclaration.GetLeadingTrivia();
                    var commentTrivia = leadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
                    if (XmlCommentHelper.IsMissingOrEmpty(commentTrivia))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, enumMemberDeclaration.Identifier.GetLocation()));
                    }
            }
        }
    }
}