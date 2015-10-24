// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A <c>&lt;typeparam&gt;</c> tag within the XML header documentation for a generic C# element is missing a
    /// <c>name</c> attribute, or contains an empty <c>name</c> attribute.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the element contains a <c>&lt;typeparam&gt;</c> tag within its XML
    /// header documentation which does not declare the name of the type parameter.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1621GenericTypeParameterDocumentationMustDeclareParameterName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1621GenericTypeParameterDocumentationMustDeclareParameterName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1621";
        private const string Title = "Generic type parameter documentation must declare parameter name";
        private const string MessageFormat = "Generic type parameter documentation must declare parameter name";
        private const string Description = "A <typeparam> tag within the XML header documentation for a generic C# element is missing a name attribute, or contains an empty name attribute.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1621.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> XmlElementAction = HandleXmlElement;
        private static readonly Action<SyntaxNodeAnalysisContext> XmlEmptyElementAction = HandleXmlEmptyElement;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(XmlElementAction, SyntaxKind.XmlElement);
            context.RegisterSyntaxNodeActionHonorExclusions(XmlEmptyElementAction, SyntaxKind.XmlEmptyElement);
        }

        private static void HandleXmlElement(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax element = (XmlElementSyntax)context.Node;

            var name = element.StartTag?.Name;

            HandleElement(context, element, name, element.StartTag?.GetLocation());
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax element = (XmlEmptyElementSyntax)context.Node;

            var name = element.Name;

            HandleElement(context, element, name, element.GetLocation());
        }

        private static void HandleElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax element, XmlNameSyntax name, Location alternativeDiagnosticLocation)
        {
            if (string.Equals(name.ToString(), XmlCommentHelper.TypeParamXmlTag))
            {
                var nameAttribute = XmlCommentHelper.GetFirstAttributeOrDefault<XmlNameAttributeSyntax>(element);

                if (string.IsNullOrWhiteSpace(nameAttribute?.Identifier?.Identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, nameAttribute?.GetLocation() ?? alternativeDiagnosticLocation));
                }
            }
        }
    }
}
