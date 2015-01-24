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
    public class SA1642ConstructorSummaryDocumentationMustBeginWithStandardText : DiagnosticAnalyzer
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

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <summary>
        /// Describes the result of matching a summary element to a specific desired wording.
        /// </summary>
        public enum MatchResult
        {
            /// <summary>
            /// The analysis could not be completed due to errors in the syntax tree or a comment structure which was
            /// not accounted for.
            /// </summary>
            Unknown = -1,

            /// <summary>
            /// No complete or partial match was found.
            /// </summary>
            None,

            /// <summary>
            /// A match to the expected text was found.
            /// </summary>
            FoundMatch,
        }

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
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclarationSyntax = context.Node as ConstructorDeclarationSyntax;

            if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                HandleConstructorDeclaration(context, StaticConstructorStandardText[0], StaticConstructorStandardText[1]);
            }
            else if (constructorDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                HandleConstructorDeclaration(context, PrivateConstructorStandardText[0], PrivateConstructorStandardText[1]);
            }
            else
            {
                HandleConstructorDeclaration(context, NonPrivateConstructorStandardText[0], NonPrivateConstructorStandardText[1]);
            }
        }

        private MatchResult HandleConstructorDeclaration(SyntaxNodeAnalysisContext context, string firstTextPart, string secondTextPart)
        {
            var constructorDeclarationSyntax = context.Node as ConstructorDeclarationSyntax;
            if (constructorDeclarationSyntax == null)
                return MatchResult.Unknown;

            var documentationStructure = XmlCommentHelper.GetDocumentationStructure(constructorDeclarationSyntax);
            if (documentationStructure == null)
                return MatchResult.Unknown;

            var summaryElement = XmlCommentHelper.GetTopLevelElement(documentationStructure, XmlCommentHelper.SummaryXmlTag) as XmlElementSyntax;
            if (summaryElement == null)
                return MatchResult.Unknown;

            // Check if the summary content could be a correct standard text
            if (summaryElement.Content.Count >= 3)
            {
                // Standard text has the form <part1><see><part2>
                var firstTextPartSyntax = summaryElement.Content[0] as XmlTextSyntax;
                var classReferencePart = summaryElement.Content[1] as XmlEmptyElementSyntax;
                var secondTextParSyntaxt = summaryElement.Content[2] as XmlTextSyntax;

                if (firstTextPartSyntax != null && classReferencePart != null && secondTextParSyntaxt != null)
                {
                    // Check text parts
                    var firstText = XmlCommentHelper.GetText(firstTextPartSyntax);
                    var secondText = XmlCommentHelper.GetText(secondTextParSyntaxt);

                    if (TextPartsMatch(firstTextPart, secondTextPart, firstTextPartSyntax, secondTextParSyntaxt)
                        && SeeTagIsCorrect(classReferencePart, constructorDeclarationSyntax))
                    {
                        // We found a correct standard text
                        return MatchResult.FoundMatch;
                    }
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, summaryElement.GetLocation()));

            // TODO: be more specific about the type of error when possible
            return MatchResult.None;
        }

        private bool SeeTagIsCorrect(XmlEmptyElementSyntax classReferencePart, ConstructorDeclarationSyntax constructorDeclarationSyntax)
        {
            if (classReferencePart.Name.ToString() == XmlCommentHelper.SeeXmlTag)
            {
                XmlCrefAttributeSyntax crefAttribute = classReferencePart.Attributes.OfType<XmlCrefAttributeSyntax>().FirstOrDefault();

                if (crefAttribute != null)
                {
                    NameMemberCrefSyntax nameMember = crefAttribute.Cref as NameMemberCrefSyntax;

                    if (nameMember != null && nameMember.Parameters == null)
                    {
                        ClassDeclarationSyntax classDeclarationSyntax = constructorDeclarationSyntax.FirstAncestorOrSelf<ClassDeclarationSyntax>();

                        if (classDeclarationSyntax != null
                            && classDeclarationSyntax.Identifier.ToString() == GetName(nameMember.Name))
                        {
                            // Check if type parameters are called the same
                            if (TypeParameterNamesMatch(classDeclarationSyntax, nameMember.Name))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private string GetName(TypeSyntax name)
        {
            return (name as SimpleNameSyntax).Identifier.ToString() ?? name.ToString();
        }

        private static bool TextPartsMatch(string firstText, string secondText, XmlTextSyntax firstTextPart, XmlTextSyntax secondTextPart)
        {
            return firstText == XmlCommentHelper.GetText(firstTextPart).TrimStart() && XmlCommentHelper.GetText(secondTextPart).StartsWith(secondText);
        }

        private static bool TypeParameterNamesMatch(ClassDeclarationSyntax classDeclarationSyntax, TypeSyntax name)
        {
            var genericName = name as GenericNameSyntax;
            if (genericName != null)
            {
                var genericNameArgumentNames = genericName.TypeArgumentList.Arguments.Cast<SimpleNameSyntax>().Select(p => p.Identifier.ToString());
                var classParameterNames = classDeclarationSyntax.TypeParameterList?.Parameters.Select(p => p.Identifier.ToString()) ?? Enumerable.Empty<string>();
                // Make sure the names match up
                return genericNameArgumentNames.SequenceEqual(classParameterNames);
            }
            else
            {
                return classDeclarationSyntax.TypeParameterList == null
                    || !classDeclarationSyntax.TypeParameterList.Parameters.Any();
            }
        }
    }
}
