// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SX1101DoNotPrefixLocalMembersWithThis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SX1101DoNotPrefixLocalMembersWithThis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SX1101";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SX1101Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SX1101MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SX1101Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SX1101.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledAlternative, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ThisExpressionAction = HandleThisExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(ThisExpressionAction, SyntaxKind.ThisExpression);
        }

        private static void HandleThisExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.Node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                return;
            }

            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node.Parent;
            var originalSymbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression, context.CancellationToken);
            if (originalSymbolInfo.Symbol == null)
            {
                return;
            }

            SyntaxNode speculationRoot;
            var annotation = new SyntaxAnnotation();
            SemanticModel speculativeModel;

            StatementSyntax statement = context.Node.FirstAncestorOrSelf<StatementSyntax>();
            if (statement != null)
            {
                var modifiedStatement = statement.ReplaceNode(memberAccessExpression, memberAccessExpression.Name.WithAdditionalAnnotations(annotation));
                speculationRoot = modifiedStatement;
                if (!context.SemanticModel.TryGetSpeculativeSemanticModel(statement.SpanStart, modifiedStatement, out speculativeModel))
                {
                    return;
                }
            }
            else
            {
                ArrowExpressionClauseSyntax arrowExpressionClause = context.Node.FirstAncestorOrSelf<ArrowExpressionClauseSyntax>();
                if (arrowExpressionClause == null)
                {
                    return;
                }

                var modifiedArrowExpressionClause = arrowExpressionClause.ReplaceNode(memberAccessExpression, memberAccessExpression.Name.WithAdditionalAnnotations(annotation));
                speculationRoot = modifiedArrowExpressionClause;
                if (!context.SemanticModel.TryGetSpeculativeSemanticModel(arrowExpressionClause.SpanStart, modifiedArrowExpressionClause, out speculativeModel))
                {
                    return;
                }
            }

            SyntaxNode mappedNode = speculationRoot.GetAnnotatedNodes(annotation).Single();
            var newSymbolInfo = speculativeModel.GetSymbolInfo(mappedNode, context.CancellationToken);
            if (!Equals(originalSymbolInfo.Symbol, newSymbolInfo.Symbol))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation()));
        }
    }
}
