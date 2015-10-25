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

    /// <summary>
    /// The C# code includes an empty string, written as <c>""</c>.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains an empty string. For example:</para>
    ///
    /// <code language="csharp">
    /// string s = "";
    /// </code>
    ///
    /// <para>This will cause the compiler to embed an empty string into the compiled code. Rather than including a
    /// hard-coded empty string, use the static <see cref="string.Empty"/> field:</para>
    ///
    /// <code language="csharp">
    /// string s = string.Empty;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1122UseStringEmptyForEmptyStrings : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1122UseStringEmptyForEmptyStrings"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1122";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1122MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1122.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> StringLiteralExpressionAction = HandleStringLiteralExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(StringLiteralExpressionAction, SyntaxKind.StringLiteralExpression);
        }

        private static void HandleStringLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            LiteralExpressionSyntax literalExpression = (LiteralExpressionSyntax)context.Node;

            var token = literalExpression.Token;
            if (token.IsKind(SyntaxKind.StringLiteralToken))
            {
                if (HasToBeConstant(literalExpression))
                {
                    return;
                }

                if (token.ValueText == string.Empty)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, literalExpression.GetLocation()));
                }
            }
        }

        private static bool HasToBeConstant(LiteralExpressionSyntax literalExpression)
        {
            ExpressionSyntax outermostExpression = FindOutermostExpression(literalExpression);

            if (outermostExpression.Parent.IsKind(SyntaxKind.AttributeArgument)
                || outermostExpression.Parent.IsKind(SyntaxKind.CaseSwitchLabel))
            {
                return true;
            }

            EqualsValueClauseSyntax equalsValueClause = outermostExpression.Parent as EqualsValueClauseSyntax;
            if (equalsValueClause != null)
            {
                ParameterSyntax parameterSyntax = equalsValueClause.Parent as ParameterSyntax;
                if (parameterSyntax != null)
                {
                    return true;
                }

                VariableDeclaratorSyntax variableDeclaratorSyntax = equalsValueClause.Parent as VariableDeclaratorSyntax;
                VariableDeclarationSyntax variableDeclarationSyntax = variableDeclaratorSyntax?.Parent as VariableDeclarationSyntax;
                if (variableDeclaratorSyntax == null || variableDeclarationSyntax == null)
                {
                    return false;
                }

                FieldDeclarationSyntax fieldDeclarationSyntax = variableDeclarationSyntax.Parent as FieldDeclarationSyntax;
                if (fieldDeclarationSyntax != null && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return true;
                }

                LocalDeclarationStatementSyntax localDeclarationStatementSyntax = variableDeclarationSyntax.Parent as LocalDeclarationStatementSyntax;
                if (localDeclarationStatementSyntax != null && localDeclarationStatementSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        private static ExpressionSyntax FindOutermostExpression(ExpressionSyntax node)
        {
            while (true)
            {
                ExpressionSyntax parent = node.Parent as ExpressionSyntax;
                if (parent == null)
                {
                    break;
                }

                node = parent;
            }

            return node;
        }
    }
}
