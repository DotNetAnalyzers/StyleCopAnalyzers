// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1122.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1122MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1122Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> StringLiteralExpressionAction = HandleStringLiteralExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(StringLiteralExpressionAction, SyntaxKind.StringLiteralExpression);
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
                || outermostExpression.Parent.IsKind(SyntaxKind.CaseSwitchLabel)
                || outermostExpression.Parent.IsKind(SyntaxKindEx.ConstantPattern))
            {
                return true;
            }

            if (outermostExpression.Parent is EqualsValueClauseSyntax equalsValueClause)
            {
                if (equalsValueClause.Parent is ParameterSyntax)
                {
                    return true;
                }

                if (!(equalsValueClause.Parent is VariableDeclaratorSyntax variableDeclaratorSyntax) || !(variableDeclaratorSyntax?.Parent is VariableDeclarationSyntax variableDeclarationSyntax))
                {
                    return false;
                }

                if (variableDeclarationSyntax.Parent is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    return true;
                }

                if (variableDeclarationSyntax.Parent is LocalDeclarationStatementSyntax localDeclarationStatementSyntax
                    && localDeclarationStatementSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
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
                if (!(node.Parent is ExpressionSyntax parent))
                {
                    break;
                }

                node = parent;
            }

            return node;
        }
    }
}
