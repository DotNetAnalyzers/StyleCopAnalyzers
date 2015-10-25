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
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledAlternative, Description, HelpLink);

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

                var parameters = GetMethodParameters(context.Node);
                if (!parameters.Contains(memberName))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Node.GetLocation()));
                }
            }
        }

        private static IList<string> GetMethodParameters(SyntaxNode node)
        {
            while ((node != null) && !node.IsKind(SyntaxKind.MethodDeclaration))
            {
                node = node.Parent;
            }

            if (node == null)
            {
                return new List<string>();
            }

            var methodDeclaration = (MethodDeclarationSyntax)node;
            return methodDeclaration.ParameterList.Parameters.Select(p => p.Identifier.ValueText).ToList();
        }
    }
}
