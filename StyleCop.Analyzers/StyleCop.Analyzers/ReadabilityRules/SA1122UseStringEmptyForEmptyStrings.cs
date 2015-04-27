namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;

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
    public class SA1122UseStringEmptyForEmptyStrings : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1122UseStringEmptyForEmptyStrings"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1122";
        private const string Title = "Use string.Empty for empty strings";
        private const string MessageFormat = "Use string.Empty for empty strings";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The C# code includes an empty string, written as \"\".";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1122.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleStringLiteral, SyntaxKind.StringLiteralExpression);
        }

        private void HandleStringLiteral(SyntaxNodeAnalysisContext context)
        {
            LiteralExpressionSyntax literalExpression = context.Node as LiteralExpressionSyntax;

            if (literalExpression != null)
            {
                var token = literalExpression.Token;
                if (token.IsKind(SyntaxKind.StringLiteralToken))
                {
                    if (this.HasToBeConstant(literalExpression))
                    {
                        return;
                    }

                    if (token.ValueText == string.Empty)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, literalExpression.GetLocation()));
                    }
                }
            }
        }

        private bool HasToBeConstant(LiteralExpressionSyntax literalExpression)
        {
            ExpressionSyntax outermostExpression = this.FindOutermostExpression(literalExpression);

            if (outermostExpression.Parent.IsKind(SyntaxKind.AttributeArgument))
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

        private ExpressionSyntax FindOutermostExpression(ExpressionSyntax node)
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
