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
    /// The <c>&lt;returns&gt;</c> tag within a C# element's documentation header is empty.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if an element contains an empty <c>&lt;returns&gt;</c> tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate documentation")]
    internal class SA1616ElementReturnValueDocumentationMustHaveText : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1616ElementReturnValueDocumentationMustHaveText"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1616";
        private const string Title = "Element return value documentation must have text";
        private const string MessageFormat = "Element return value documentation must have text";
        private const string Description = "The <returns> tag within a C# element's documentation header is empty.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1616.md";

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

            if (string.Equals(name.ToString(), XmlCommentHelper.ReturnsXmlTag) && XmlCommentHelper.IsConsideredEmpty(emptyElement))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, emptyElement.GetLocation()));
            }
        }

        private static void HandleXmlEmptyElement(SyntaxNodeAnalysisContext context)
        {
            XmlEmptyElementSyntax emptyElement = context.Node as XmlEmptyElementSyntax;

            if (string.Equals(emptyElement?.Name.ToString(), XmlCommentHelper.ReturnsXmlTag))
            {
                // <returns .../> is empty.
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, emptyElement.GetLocation()));
            }
        }
    }
}
