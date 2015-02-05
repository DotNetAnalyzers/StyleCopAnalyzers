namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The XML documentation header for a C# finalizer does not contain the appropriate summary text.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the summary tag within the documentation header for a finalizer does
    /// not begin with the proper text.</para>
    ///
    /// <para>The rule is intended to standardize the summary text for a finalizer. The summary for a finalizer must
    /// begin with "Finalizes an instance of the {class name} class." For example, the following shows the finalizer for
    /// the <c>Customer</c> class.</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Finalizes an instance of the Customer class.
    /// /// &lt;/summary&gt;
    /// ~Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>It is possible to embed other tags into the summary text. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Finalizes an instance of the &lt;see cref="Customer"/&gt; class.
    /// /// &lt;/summary&gt;
    /// ~Customer()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1643DestructorSummaryDocumentationMustBeginWithStandardText : StandardTextDiagnosticBase
    {
        public const string DiagnosticId = "SA1643";
        private const string Title = "Destructor summary documentation must begin with standard text";
        private const string MessageFormat = "Destructor summary documentation must begin with standard text";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The XML documentation header for a C# finalizer does not contain the appropriate summary text.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1643.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        public static ImmutableArray<string> DestructorStandardText { get; } = ImmutableArray.Create("Finalizes an instance of the ", " class.");
        
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        protected override DiagnosticDescriptor DiagnosticDescriptor
        {
            get
            {
                return Descriptor;
            }
        }


        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleDestructor, SyntaxKind.DestructorDeclaration);
        }

        private void HandleDestructor(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = context.Node as DestructorDeclarationSyntax;

            if (destructorDeclaration != null)
            {
                HandleDeclaration(context, DestructorStandardText[0], DestructorStandardText[1], true);
            }
        }
    }
}
