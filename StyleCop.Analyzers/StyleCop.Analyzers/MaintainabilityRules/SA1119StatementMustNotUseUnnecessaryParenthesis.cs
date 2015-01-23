namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A C# statement contains parenthesis which are unnecessary and should be removed.
    /// </summary>
    /// <remarks>
    /// <para>It is possible in C# to insert parenthesis around virtually any type of expression, statement, or clause,
    /// and in many situations use of parenthesis can greatly improve the readability of the code. However, excessive
    /// use of parenthesis can have the opposite effect, making it more difficult to read and maintain the code.</para>
    ///
    /// <para>A violation of this rule occurs when parenthesis are used in situations where they provide no practical
    /// value. Typically, this happens anytime the parenthesis surround an expression which does not strictly require
    /// the use of parenthesis, and the parenthesis expression is located at the root of a statement. For example, the
    /// following lines of code all contain unnecessary parenthesis which will result in violations of this rule:</para>
    ///
    /// <code language="csharp">
    /// int x = (5 + b);
    /// string y = (this.Method()).ToString();
    /// return (x.Value);
    /// </code>
    ///
    /// <para>In each of these statements, the extra parenthesis can be removed without sacrificing the readability of
    /// the code:</para>
    ///
    /// <code language="csharp">
    /// int x = 5 + b;
    /// string y = this.Method().ToString();
    /// return x.Value;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1119StatementMustNotUseUnnecessaryParenthesis : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1119";
        internal const string Title = "Statement must not use unnecessary parenthesis";
        internal const string MessageFormat = "Statement must not use unnecessary parenthesis";
        internal const string Category = "StyleCop.CSharp.MaintainabilityRules";
        internal const string Description = "A C# statement contains parenthesis which are unnecessary and should be removed.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1119.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description, HelpLink);
        private static readonly DiagnosticDescriptor ParenthesisDescriptor =
            new DiagnosticDescriptor(DiagnosticId + "_p", Title, MessageFormat, Category, DiagnosticSeverity.Hidden, true, Description, HelpLink, customTags: new[] { WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.NotConfigurable });

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(HandleParenthesizedExpressionn, SyntaxKind.ParenthesizedExpression);
        }

        private void HandleParenthesizedExpressionn(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ParenthesizedExpressionSyntax;

            if (node != null && node.Expression != null)
            {
                if (!(node.Expression is BinaryExpressionSyntax)
                    && !(node.Expression is AssignmentExpressionSyntax)
                    && !(node.Expression is PrefixUnaryExpressionSyntax)
                    && !(node.Expression is PostfixUnaryExpressionSyntax)
                    && !node.Expression.IsKind(SyntaxKind.CastExpression)
                    && !node.Expression.IsKind(SyntaxKind.ConditionalExpression)
                    && !node.Expression.IsKind(SyntaxKind.IsExpression)
                    && !node.Expression.IsKind(SyntaxKind.SimpleLambdaExpression)
                    && !node.Expression.IsKind(SyntaxKind.ParenthesizedLambdaExpression)
                    && !node.Expression.IsKind(SyntaxKind.ArrayCreationExpression)
                    && !node.Expression.IsKind(SyntaxKind.CoalesceExpression)
                    && !node.Expression.IsKind(SyntaxKind.QueryExpression)
                    && !node.Expression.IsKind(SyntaxKind.AwaitExpression)
                    && !node.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    ReportDiagnostic(context, node);
                }
                else
                {
                    if (!(node.Parent is ExpressionSyntax) 
                        || node.Parent is CheckedExpressionSyntax 
                        || node.Parent is MemberAccessExpressionSyntax)
                    {
                        var memberAccess = node.Parent as MemberAccessExpressionSyntax;
                        if (memberAccess != null)
                        {
                            if (memberAccess.Expression != node)
                            {
                                ReportDiagnostic(context, node);
                            }
                        }
                        else
                        {
                            ReportDiagnostic(context, node);
                        }
                    }
                    else
                    {
                        EqualsValueClauseSyntax equalsValue = node.Parent as EqualsValueClauseSyntax;
                        if (equalsValue != null && equalsValue.Value == node)
                        {
                            ReportDiagnostic(context, node);
                        }
                        else
                        {
                            AssignmentExpressionSyntax assignValue = node.Parent as AssignmentExpressionSyntax;
                            if (assignValue != null)
                            {
                                ReportDiagnostic(context, node);
                            }
                            }
                        }
                    }
                }
            }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ParenthesizedExpressionSyntax node)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, node.GetLocation()));
            context.ReportDiagnostic(Diagnostic.Create(ParenthesisDescriptor, node.OpenParenToken.GetLocation()));
            context.ReportDiagnostic(Diagnostic.Create(ParenthesisDescriptor, node.CloseParenToken.GetLocation()));
        }
    }
}
