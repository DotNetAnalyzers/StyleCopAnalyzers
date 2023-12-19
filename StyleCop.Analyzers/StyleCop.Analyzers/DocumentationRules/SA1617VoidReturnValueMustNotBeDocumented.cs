// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    /// A C# code element does not contain a return value, or returns <c>void</c>, but the documentation header for the
    /// element contains a <c>&lt;returns&gt;</c> tag.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers.</para>
    ///
    /// <para>A violation of this rule occurs if an element which returns <c>void</c> contains a <c>&lt;returns&gt;</c>
    /// tag within its documentation header.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1617VoidReturnValueMustNotBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1617VoidReturnValueMustNotBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1617";

        /// <summary>
        /// The key used for signalling that no codefix should be offered.
        /// </summary>
        internal const string NoCodeFixKey = "NoCodeFix";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1617.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(DocumentationResources.SA1617Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1617MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(DocumentationResources.SA1617Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;

        private static readonly ImmutableDictionary<string, string> NoCodeFixProperties = ImmutableDictionary.Create<string, string>().Add(NoCodeFixKey, string.Empty);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleMember(context, methodDeclaration.ReturnType);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleMember(context, delegateDeclaration?.ReturnType);
        }

        private static void HandleMember(SyntaxNodeAnalysisContext context, TypeSyntax returnValue)
        {
            var documentation = context.Node.GetDocumentationCommentTriviaSyntax();
            if (documentation == null)
            {
                return;
            }

            // Check if the return type is void.
            if (!(returnValue is PredefinedTypeSyntax returnType) || !returnType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return;
            }

            // Check if the return value is documented
            var returnsElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag);
            if (returnsElement == null)
            {
                var includeElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.IncludeXmlTag);
                if (includeElement != null)
                {
                    string rawDocumentation;
                    var declaration = context.SemanticModel.GetDeclaredSymbol(context.Node, context.CancellationToken);
                    if (declaration == null)
                    {
                        return;
                    }

                    rawDocumentation = declaration.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                    var completeDocumentation = XElement.Parse(rawDocumentation, LoadOptions.None);
                    if (completeDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == XmlCommentHelper.InheritdocXmlTag))
                    {
                        // Ignore nodes with an <inheritdoc/> tag in the included XML.
                        return;
                    }

                    var includedReturnsElement = completeDocumentation.Nodes().OfType<XElement>().FirstOrDefault(element => element.Name == XmlCommentHelper.ReturnsXmlTag);
                    if (includedReturnsElement != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, includeElement.GetLocation(), NoCodeFixProperties));
                    }
                }
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, returnsElement.GetLocation()));
            }
        }
    }
}
