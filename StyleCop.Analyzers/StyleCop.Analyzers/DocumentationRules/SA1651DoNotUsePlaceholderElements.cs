// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    /// The documentation for the element contains one or more &lt;placeholder&gt; elements.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the element documentation contains &lt;placeholder&gt;
    /// elements:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// This method &lt;placeholder&gt;performs some operation&lt;/placeholder&gt;.
    /// /// &lt;/summary&gt;
    /// public void SomeOperation()
    /// {
    ///     ...
    /// }
    /// </code>
    ///
    /// <para>Placeholder elements should be reviewed and removed from documentation.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1651DoNotUsePlaceholderElements : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1651DoNotUsePlaceholderElements"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1651";

        /// <summary>
        /// The key used for signalling that no codefix should be offered.
        /// </summary>
        internal const string NoCodeFixKey = "NoCodeFix";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1651.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1651Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1651MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1651Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> XmlElementAction = HandleXmlElement;
        private static readonly Action<SyntaxNodeAnalysisContext> XmlEmptyElementAction = HandleXmlEmptyElement;

        private static readonly ImmutableDictionary<string, string> NoCodeFixProperties = ImmutableDictionary.Create<string, string>().Add(NoCodeFixKey, string.Empty);

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
            XmlElementSyntax syntax = (XmlElementSyntax)context.Node;
            CheckTag(context, syntax.StartTag?.Name?.ToString());
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax syntax = (XmlEmptyElementSyntax)context.Node;
            CheckTag(context, syntax.Name?.ToString());
        }

        private static void CheckTag(SyntaxNodeAnalysisContext context, string tagName)
        {
            if (string.Equals(XmlCommentHelper.IncludeXmlTag, tagName, StringComparison.Ordinal))
            {
                if (!IncludedDocumentationContainsPlaceHolderTags(context))
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation(), NoCodeFixProperties));
            }
            else
            {
                if (!string.Equals(XmlCommentHelper.PlaceholderTag, tagName, StringComparison.Ordinal))
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation()));
            }
        }

        private static bool IncludedDocumentationContainsPlaceHolderTags(SyntaxNodeAnalysisContext context)
        {
            var memberDeclaration = context.Node.FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return false;
            }

            var declaration = context.SemanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);
            if (declaration == null)
            {
                return false;
            }

            var rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
            var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
            return completeDocumentation.DescendantNodesAndSelf().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.PlaceholderTag);
        }
    }
}
