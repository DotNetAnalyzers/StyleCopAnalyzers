using System;

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the last parameter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the closing bracket of a method or indexer call or declaration is not
    /// placed on the same line as the last parameter. The following examples show correct placement of the
    /// bracket:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    ///     string name = JoinStrings(
    ///         first, 
    ///         last);
    /// }
    ///
    /// public int this[int x]
    /// {
    ///     get { return this.items[x]; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1111ClosingParenthesisMustBeOnLineOfLastParameter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1111";
        internal const string Title = "Closing parenthesis must be on line of last parameter";
        internal const string MessageFormat = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1111.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(HandleDelegateDeclarationExpression, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(HandleAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(HandleParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpressionSyntax = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambdaExpressionSyntax.ParameterList == null ||
                !lambdaExpressionSyntax.ParameterList.IsMissing &&
                !lambdaExpressionSyntax.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = lambdaExpressionSyntax.ParameterList
                .Parameters
                .Last();

            if (!lambdaExpressionSyntax.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
                    lambdaExpressionSyntax.ParameterList.CloseParenToken);
            }
        }

        private void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (!attributeList.Attributes.Any())
            {
                return;
            }

            var lastAttribute = attributeList.Attributes
                                .Last();

            if (!attributeList.CloseBracketToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastAttribute,
                    attributeList.CloseBracketToken);
            }
        }

        private void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax) context.Node;

            if (anonymousMethod.ParameterList == null ||
                !anonymousMethod.ParameterList.IsMissing &&
                !anonymousMethod.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = anonymousMethod.ParameterList
                .Parameters
                .Last();

            if (!anonymousMethod.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
                    anonymousMethod.ParameterList.CloseParenToken);
            }
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax) context.Node;

            if (attribute.ArgumentList == null ||
                !attribute.ArgumentList.IsMissing &&
                !attribute.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = attribute.ArgumentList
                .Arguments
                .Last();

            if (!attribute.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                    attribute.ArgumentList.CloseParenToken);
            }
        }

        private void HandleDelegateDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax) context.Node;

            if (delegateDeclaration.ParameterList == null ||
                !delegateDeclaration.ParameterList.IsMissing &&
                !delegateDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = delegateDeclaration.ParameterList
                .Parameters
                .Last();

            if (!delegateDeclaration.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
                    delegateDeclaration.ParameterList.CloseParenToken);
            }
        }

        private void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(((ElementAccessExpressionSyntax)context.Node).Expression);

            if (typeSymbol.Type != null && typeSymbol.Type.TypeKind != TypeKind.Array )
            {
                var elementAccess = (ElementAccessExpressionSyntax)context.Node;

                if (elementAccess.ArgumentList == null ||
                    elementAccess.ArgumentList.IsMissing || 
                    !elementAccess.ArgumentList.Arguments.Any())
                {
                    return;
                }

                var lastArgument = elementAccess.ArgumentList
                    .Arguments
                    .Last();

                if (!elementAccess.ArgumentList.CloseBracketToken.IsMissing && !lastArgument.IsMissing)
                {
                    CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                        elementAccess.ArgumentList.CloseBracketToken);
                }
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, methodDeclaration);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, constructotDeclarationSyntax);
        }

        private void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;

            if (invocationExpression.ArgumentList == null ||
                invocationExpression.ArgumentList.IsMissing ||
                !invocationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = invocationExpression.ArgumentList
                .Arguments
                .Last();

            if (!invocationExpression.ArgumentList.CloseParenToken.IsMissing &&
                !lastArgument.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                    invocationExpression.ArgumentList.CloseParenToken);
            }
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax) context.Node;

            if (objectCreation.ArgumentList == null ||
                objectCreation.ArgumentList.IsMissing ||
                !objectCreation.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = objectCreation.ArgumentList
                .Arguments
                .Last();

            if (!objectCreation.ArgumentList.CloseParenToken.IsMissing &&
                !lastArgument.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                    objectCreation.ArgumentList.CloseParenToken);
            }

        }


        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.ParameterList == null ||
                !indexerDeclaration.ParameterList.IsMissing &&
                !indexerDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = indexerDeclaration.ParameterList
                         .Parameters
                         .Last();

            if (!indexerDeclaration.ParameterList.CloseBracketToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter, indexerDeclaration.ParameterList.CloseBracketToken);
            }
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context,
            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
        {
            if (baseMethodDeclarationSyntax.ParameterList == null ||
                baseMethodDeclarationSyntax.ParameterList.IsMissing ||
                !baseMethodDeclarationSyntax.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = baseMethodDeclarationSyntax.ParameterList
                .Parameters
                .Last();

            if (!baseMethodDeclarationSyntax.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter, baseMethodDeclarationSyntax.ParameterList.CloseParenToken);
            }
        }

        private static void CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(SyntaxNodeAnalysisContext context,
       CSharpSyntaxNode parameterOrArgument, SyntaxToken closeToken)
        {
            var lastParameterLine = parameterOrArgument.GetLocation().GetLineSpan();
            var closeParenLocation = closeToken.GetLocation();
            var closeParenLine = closeParenLocation.GetLineSpan();
            if (lastParameterLine.IsValid &&
                closeParenLine.IsValid &&
                closeParenLine.StartLinePosition.Line != lastParameterLine.EndLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeParenLocation));
            }
        }
    }
}
