namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Helpers;
    using System;



    /// <summary>
    /// The XML documentation header for a C# constructor does not contain the appropriate summary text.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when the summary tag within the documentation header for a constructor
    /// does not begin with the proper text.</para>
    ///
    /// <para>The rule is intended to standardize the summary text for a constructor based on the access level of the
    /// constructor. The summary for a non-private instance constructor must begin with "Initializes a new instance of
    /// the {class name} class." For example, the following shows the constructor for the <c>Customer</c> class.</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the Customer class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>It is possible to embed other tags into the summary text. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>If the class contains generic parameters, these can be annotated within the <c>cref</c> link using either
    /// of the following two formats:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer`1"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    ///
    /// /// &lt;summary&gt;
    /// /// Initializes a new instance of the &lt;see cref="Customer{T}"/&gt; class.
    /// /// &lt;/summary&gt;
    /// public Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>If the constructor is <see langword="static"/>, the summary text should begin with "Initializes static
    /// members of the {class name} class.” For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Initializes static members of the Customer class.
    /// /// &lt;/summary&gt;
    /// public static Customer()
    /// {
    /// }
    /// </code>
    ///
    /// <para>Private instance constructors must use the summary text "Prevents a default instance of the {class name}
    /// class from being created."</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Prevents a default instance of the Customer class from being created.
    /// /// &lt;/summary&gt;
    /// private Customer()
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1642ConstructorSummaryDocumentationMustBeginWithStandardText : StandardTextDiagnosticBase
    {
        public const string DiagnosticId = "SA1642";
        private const string Title = "Constructor summary documentation must begin with standard text";
        private const string MessageFormat = "Constructor summary documentation must begin with standard text";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The XML documentation header for a C# constructor does not contain the appropriate summary text.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1642.html";


        public static ImmutableArray<string> NonPrivateConstructorStandardText { get; } = ImmutableArray.Create("Initializes a new instance of the ", " class");
        public static ImmutableArray<string> PrivateConstructorStandardText { get; } = ImmutableArray.Create("Prevents a default instance of the ", " class from being created.");
        public static ImmutableArray<string> StaticConstructorStandardText { get; } = ImmutableArray.Create("Initializes static members of the ", " class.");

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);
        
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
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
            context.RegisterSyntaxNodeAction(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclarationSyntax = context.Node as ConstructorDeclarationSyntax;

            if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                this.HandleDeclaration(context, StaticConstructorStandardText[0], StaticConstructorStandardText[1], true);
            }
            else if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                if (this.HandleDeclaration(context, PrivateConstructorStandardText[0], PrivateConstructorStandardText[1], false) != MatchResult.FoundMatch)
                {
                    // also allow the non-private wording for private constructors
                    this.HandleDeclaration(context, NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
                }
            }
            else
            {
                this.HandleDeclaration(context, NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1], true);
            }
        }
    }
}
