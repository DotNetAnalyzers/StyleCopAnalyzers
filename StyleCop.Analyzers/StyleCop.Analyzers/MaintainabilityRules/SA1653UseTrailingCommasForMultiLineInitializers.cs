// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A multi-line initializer must use a comma on the last item.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1653UseTrailingCommasInMultiLineInitializers : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1653UseTrailingCommasInMultiLineInitializers"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1653";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1653Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1653MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1653Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1653.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> HandleObjectInitializerAction = HandleObjectInitializer;

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectInitializerAction, SyntaxKind.ObjectInitializerExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectInitializerAction, SyntaxKind.AnonymousObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectInitializerAction, SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectInitializerAction, SyntaxKind.CollectionInitializerExpression);
        }

        private static void HandleObjectInitializer(SyntaxNodeAnalysisContext context)
        {
            var initializer = (ExpressionSyntax)context.Node;
            if (initializer == null
                || !IsMultiline(initializer))
            {
                return;
            }

            var childNodesAndTokens = initializer.ChildNodesAndTokens().ToList();
            var lastToken = childNodesAndTokens[childNodesAndTokens.Count - 2];
            if (!lastToken.IsKind(SyntaxKind.CommaToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, lastToken.GetLocation()));
            }
        }

        private static bool IsMultiline(ExpressionSyntax initializer)
        {
            var lineSpan = initializer.GetLineSpan();
            return lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line;
        }
    }
}
