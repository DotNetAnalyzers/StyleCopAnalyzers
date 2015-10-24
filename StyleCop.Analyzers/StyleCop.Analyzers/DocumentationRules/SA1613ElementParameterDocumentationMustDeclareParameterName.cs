// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A <c>&lt;param&gt;</c> tag within a C# element's documentation header is missing a <c>name</c> attribute
    /// containing the name of the parameter.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if the documentation for an element contains a <c>&lt;param&gt;</c> tag
    /// which is missing a <c>name</c> attribute, or which contains an empty <c>name</c> attribute.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1613ElementParameterDocumentationMustDeclareParameterName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1613ElementParameterDocumentationMustDeclareParameterName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1613";
        private const string Title = "Element parameter documentation must declare parameter name";
        private const string MessageFormat = "Element parameter documentation must declare parameter name";
        private const string Description = "A <param> tag within a C# element's documentation header is missing a name attribute containing the name of the parameter.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1613.md";

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
            XmlElementSyntax emptyElement = context.Node as XmlElementSyntax;

            var name = emptyElement?.StartTag?.Name;

            HandleElement(context, emptyElement, name, emptyElement?.StartTag?.GetLocation());
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax emptyElement = context.Node as XmlEmptyElementSyntax;

            var name = emptyElement?.Name;

            HandleElement(context, emptyElement, name, emptyElement?.GetLocation());
        }

        private static void HandleElement(SyntaxNodeAnalysisContext context, XmlNodeSyntax element, XmlNameSyntax name, Location alternativeDiagnosticLocation)
        {
            if (string.Equals(name.ToString(), XmlCommentHelper.ParamXmlTag))
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
