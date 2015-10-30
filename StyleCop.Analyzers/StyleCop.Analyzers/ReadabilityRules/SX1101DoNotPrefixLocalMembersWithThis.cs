// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
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
            if (context.Node.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node.Parent;
                var memberName = memberAccessExpression.Name.ToString();

                var methodDeclaration = GetContainingMethod(context.Node);
                if (methodDeclaration != null)
                {
                    if (HasMethodParameterMatch(methodDeclaration, memberName))
                    {
                        // The this expression is allowed if there is a clash with a method parameter
                        return;
                    }

                    if (HasLocalVariableMatch(methodDeclaration, memberName))
                    {
                        // The this expression is allowed if there is a clash with a local variable declaration
                        return;
                    }
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation()));
            }
        }

        private static bool HasLocalVariableMatch(BaseMethodDeclarationSyntax methodDeclaration, string memberName)
        {
            var variableDeclarators = methodDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>();
            return variableDeclarators.Any(vd => string.Equals(vd.Identifier.ValueText, memberName, StringComparison.Ordinal));
        }

        private static bool HasMethodParameterMatch(BaseMethodDeclarationSyntax methodDeclaration, string memberName)
        {
            return methodDeclaration.ParameterList.Parameters.Any(p => string.Equals(p.Identifier.ValueText, memberName, StringComparison.Ordinal));
        }

        private static BaseMethodDeclarationSyntax GetContainingMethod(SyntaxNode node)
        {
            while ((node != null) && !(node is BaseMethodDeclarationSyntax))
            {
                node = node.Parent;
            }

            return (BaseMethodDeclarationSyntax)node;
        }
    }
}
