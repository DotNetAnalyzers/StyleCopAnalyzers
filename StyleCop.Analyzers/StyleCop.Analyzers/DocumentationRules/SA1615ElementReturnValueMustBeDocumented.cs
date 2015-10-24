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
    /// A C# element is missing documentation for its return value.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs if an element containing a return value is missing a
    /// <c>&lt;returns&gt;</c> tag.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1615ElementReturnValueMustBeDocumented : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1615ElementReturnValueMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1615";
        private const string Title = "Element return value must be documented";
        private const string MessageFormat = "Element return value must be documented";
        private const string Description = "A C# element is missing documentation for its return value.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1615.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;

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
            context.RegisterSyntaxNodeActionHonorExclusions(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (MethodDeclarationSyntax)context.Node;

            HandleDeclaration(context, node.ReturnType);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (DelegateDeclarationSyntax)context.Node;

            HandleDeclaration(context, node.ReturnType);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context, TypeSyntax returnType)
        {
            var predefinedType = returnType as PredefinedTypeSyntax;

            if (predefinedType != null && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                // There is no return value
                return;
            }

            var documentationStructure = context.Node.GetDocumentationCommentTriviaSyntax();

            if (documentationStructure == null)
            {
                return;
            }

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.InheritdocXmlTag) != null)
            {
                // Don't report if the documentation is inherited.
                return;
            }

            if (documentationStructure.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag) == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, returnType.GetLocation()));
            }
        }
    }
}
