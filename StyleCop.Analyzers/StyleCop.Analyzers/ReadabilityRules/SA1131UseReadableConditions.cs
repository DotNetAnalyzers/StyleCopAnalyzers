// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A comparison was made between a variable and a literal or constant value, and the variable appeared on the
    /// right-hand-side of the expression.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1131UseReadableConditions : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1131UseReadableConditions"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1131";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1131Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1131MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1131Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1131.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledBinaryExpressionKinds =
            ImmutableArray.Create(
                SyntaxKind.EqualsExpression,
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.LessThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.LessThanOrEqualExpression);

        private static readonly Action<SyntaxNodeAnalysisContext> BinaryExpressionAction = HandleBinaryExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BinaryExpressionAction, HandledBinaryExpressionKinds);
        }

        private static void HandleBinaryExpression(SyntaxNodeAnalysisContext context)
        {
            var binaryExpression = (BinaryExpressionSyntax)context.Node;

            var semanticModel = context.SemanticModel;

            if (IsLiteral(binaryExpression.Left, semanticModel) && !IsLiteral(binaryExpression.Right, semanticModel))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, binaryExpression.GetLocation()));
            }
        }

        private static bool IsLiteral(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            // Default expressions are most of the time constants, but not for default(MyStruct).
            if (expression.IsKind(SyntaxKind.DefaultExpression) || expression.IsKind(SyntaxKindEx.DefaultLiteralExpression))
            {
                return true;
            }

            var constantValue = semanticModel.GetConstantValue(expression);
            if (constantValue.HasValue)
            {
                return true;
            }

            if (semanticModel.GetSymbolInfo(expression).Symbol is IFieldSymbol fieldSymbol)
            {
                return fieldSymbol.IsStatic && fieldSymbol.IsReadOnly;
            }

            return false;
        }
    }
}
