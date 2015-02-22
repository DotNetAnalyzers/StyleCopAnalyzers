namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a method or indexer does not take any parameters and the closing
    /// bracket of a call or declaration for the method or indexer is not placed on the same line as the opening
    /// bracket. The following example shows correct placement of the closing parenthesis:</para>
    /// <code language="csharp">
    /// public string GetName()
    /// {
    ///     return this.name.Trim();
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1112";
        private const string Title = "Closing parenthesis must be on line of opening parenthesis";
        private const string MessageFormat = "Closing parenthesis must be on line of opening parenthesis";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1112.html";

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
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(this.HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            if (objectCreation.ArgumentList == null ||
                objectCreation.ArgumentList.IsMissing ||
                objectCreation.ArgumentList.Arguments.Count > 0)
            {
                return;
            }

            if (!objectCreation.ArgumentList.OpenParenToken.IsMissing &&
                    !objectCreation.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfOpenAndCloseTokensAreTheSame(context,
                    objectCreation.ArgumentList.OpenParenToken, objectCreation.ArgumentList.CloseParenToken);
            }
        }

        private void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            if (invocationExpression.ArgumentList == null ||
                invocationExpression.ArgumentList.IsMissing ||
                invocationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            if (!invocationExpression.ArgumentList.OpenParenToken.IsMissing &&
                !invocationExpression.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfOpenAndCloseTokensAreTheSame(context,
                    invocationExpression.ArgumentList.OpenParenToken, invocationExpression.ArgumentList.CloseParenToken);
            }

        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, constructotDeclarationSyntax);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, methodDeclaration);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context,
            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
        {
            var parameterListSyntax =
                baseMethodDeclarationSyntax.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.Parameters.Any())
            {

                if (!parameterListSyntax.OpenParenToken.IsMissing &&
                    !parameterListSyntax.CloseParenToken.IsMissing)
                {
                    CheckIfLocationOfOpenAndCloseTokensAreTheSame(context, parameterListSyntax.OpenParenToken, parameterListSyntax.CloseParenToken);
                }
            }
        }

        private static void CheckIfLocationOfOpenAndCloseTokensAreTheSame(SyntaxNodeAnalysisContext context,
            SyntaxToken openToken, SyntaxToken closeToken)
        {
            var closeParenLocation = closeToken.GetLocation();
            var closeParenLine = closeParenLocation.GetLineSpan();
            var openParenLine = openToken.GetLocation().GetLineSpan();
            if (closeParenLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != closeParenLine.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeParenLocation));
            }
        }

    }
}
