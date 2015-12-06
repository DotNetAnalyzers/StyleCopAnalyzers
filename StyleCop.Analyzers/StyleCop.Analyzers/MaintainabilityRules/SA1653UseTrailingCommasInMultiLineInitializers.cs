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
        private static readonly Action<SyntaxNodeAnalysisContext> HandleAnonymousObjectInitializerAction = HandleAnonymousObjectInitializer;

        private static readonly ImmutableArray<SyntaxKind> ObjectInitializerKinds =
            ImmutableArray.Create(SyntaxKind.ObjectInitializerExpression, SyntaxKind.ArrayInitializerExpression, SyntaxKind.CollectionInitializerExpression);

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectInitializerAction, ObjectInitializerKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAnonymousObjectInitializerAction, SyntaxKind.AnonymousObjectCreationExpression);
        }

        private static void HandleObjectInitializer(SyntaxNodeAnalysisContext context)
        {
            var initializer = (InitializerExpressionSyntax)context.Node;
            if (initializer == null || !initializer.SpansMultipleLines())
            {
                return;
            }

            if (initializer.Expressions.SeparatorCount < initializer.Expressions.Count)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.Expressions.Last().GetLocation()));
            }
        }

        private static void HandleAnonymousObjectInitializer(SyntaxNodeAnalysisContext context)
        {
            var initializer = (AnonymousObjectCreationExpressionSyntax)context.Node;
            if (initializer == null || !initializer.SpansMultipleLines())
            {
                return;
            }

            if (initializer.Initializers.SeparatorCount < initializer.Initializers.Count)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.Initializers.Last().GetLocation()));
            }
        }
    }
}
