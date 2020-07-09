// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The XML header documentation for a C# code element contains an empty tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs when the documentation header for an element contains an empty tag. For
    /// example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Joins a first name and a last name together into a single string.
    /// /// &lt;/summary&gt;
    /// /// &lt;remarks&gt;&lt;/remarks&gt;
    /// /// &lt;param name="firstName"&gt;Other part of name.&lt;/param&gt;
    /// /// &lt;param name="lastName"&gt;Part of the name.&lt;/param&gt;
    /// /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    /// public string JoinNames(string firstName, string lastName)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1627DocumentationTextMustNotBeEmpty : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1627DocumentationTextMustNotBeEmpty"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1627";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1627.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1627Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1627MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1627Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> XmlElementAction = HandleXmlElement;
        private static readonly Action<SyntaxNodeAnalysisContext> XmlEmptyElementAction = HandleXmlEmptyElement;

        private static readonly ImmutableArray<string> ElementsToCheck =
            ImmutableArray.Create(
                XmlCommentHelper.RemarksXmlTag,
                XmlCommentHelper.PermissionXmlTag,
                XmlCommentHelper.ExceptionXmlTag,
                XmlCommentHelper.ExampleXmlTag);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(XmlElementAction, SyntaxKind.XmlElement);
            context.RegisterSyntaxNodeAction(XmlEmptyElementAction, SyntaxKind.XmlEmptyElement);
        }

        private static void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            var element = (XmlElementSyntax)context.Node;

            var name = element.StartTag.Name.ToString();
            if (ElementsToCheck.Contains(name) && XmlCommentHelper.IsConsideredEmpty(element))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, element.GetLocation(), name.ToString()));
            }
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            var elementSyntax = (XmlEmptyElementSyntax)context.Node;
            var elementName = elementSyntax.Name.ToString();
            var elementLocation = elementSyntax.GetLocation();

            if (string.Equals(elementName, XmlCommentHelper.IncludeXmlTag, StringComparison.Ordinal))
            {
                HandleIncludedDocumentation(context, elementSyntax, elementLocation);
                return;
            }

            if (ElementsToCheck.Contains(elementName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, elementLocation, elementSyntax.Name.ToString()));
            }
        }

        private static void HandleIncludedDocumentation(SyntaxNodeAnalysisContext context, XmlEmptyElementSyntax elementSyntax, Location elementLocation)
        {
            var memberDeclaration = elementSyntax.FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return;
            }

            var declaration = context.SemanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);
            if (declaration == null)
            {
                return;
            }

            var rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
            var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);

            // This documentation rule is excluded via the <exclude /> tag
            if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.ExcludeXmlTag))
            {
                return;
            }

            if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
            {
                // Ignore nodes with an <inheritdoc/> tag in the included XML.
                return;
            }

            var emptyElements = completeDocumentation.Nodes()
                .OfType<XElement>()
                .Where(element => ElementsToCheck.Contains(element.Name.ToString()))
                .Where(x => XmlCommentHelper.IsConsideredEmpty(x));

            foreach (var emptyElement in emptyElements)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, elementLocation, emptyElement.Name.ToString()));
            }
        }
    }
}
