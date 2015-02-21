using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The start of the parameter list for a method or indexer call or declaration does not begin on the same line as
    /// the opening bracket, or on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when there are one or more blank lines between the opening bracket and the
    /// start of the parameter list. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameter list must begin on the same line as the opening bracket, or on the next line. For
    /// example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1114ParameterListMustFollowDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1114ParameterListMustFollowDeclaration"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1114";
        private const string Title = "Parameter list must follow declaration";
        private const string MessageFormat = "Parameter list must follow declaration.";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The start of the parameter list for a method or indexer call or declaration does not begin on the same line as the opening bracket, or on the line after the opening bracket.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1114.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleMethodInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeAction(this.HandleElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(this.HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(this.HandleAttributesList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(this.HandleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(this.HandleConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax) context.Node;
            AnalyzeParametersList(context, operatorDeclaration.ParameterList);
        }

        private void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax) context.Node;
            AnalyzeParametersList(context, conversionOperatorDeclaration.ParameterList);
        }

        private void HandleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (ParenthesizedLambdaExpressionSyntax) context.Node;
            AnalyzeParametersList(context, lambdaExpression.ParameterList);
        }

        private void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax) context.Node;
            AnalyzeParametersList(context, anonymousMethod.ParameterList);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax) context.Node;
            AnalyzeParametersList(context, delegateDeclaration.ParameterList);
        }

        private void HandleAttributesList(SyntaxNodeAnalysisContext context)
        {
            var attributesList = (AttributeListSyntax) context.Node;

            AnalyzeAttributeList(context, attributesList);
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax) context.Node;
            AnalyzeArgumentList(context, attribute.ArgumentList);
        }

        private void HandleElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax) context.Node;
            if (arrayCreation.Type == null)
            {
                return;
            }

            AnalyzeRankSpecifiers(context, arrayCreation);
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax) context.Node;
            AnalyzeBracketParametersList(context, indexerDeclaration.ParameterList);
        }

        private void HandleObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax) context.Node;
            AnalyzeArgumentList(context, objectCreation.ArgumentList);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax) context.Node;
            AnalyzeParametersList(context,constructorDeclaration.ParameterList);
        }

        private void HandleMethodInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;
            AnalyzeArgumentList(context, invocationExpression.ArgumentList);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            AnalyzeParametersList(context, methodDeclaration.ParameterList);
        }

        private static void AnalyzeRankSpecifiers(SyntaxNodeAnalysisContext context, ArrayCreationExpressionSyntax arrayCreation)
        {
            if (!arrayCreation.Type.RankSpecifiers.Any())
            {
                return;
            }

            foreach (var arrayRankSpecifierSyntax in arrayCreation.Type.RankSpecifiers)
            {
                var openBracketToken = arrayRankSpecifierSyntax.OpenBracketToken;
                if (openBracketToken.IsMissing ||
                    arrayRankSpecifierSyntax.IsMissing ||
                    !arrayRankSpecifierSyntax.Sizes.Any())
                {
                    return;
                }

                var firstSize = arrayRankSpecifierSyntax.Sizes[0];

                var firstSizeLineSpan = firstSize.GetLocation().GetLineSpan();
                if (!firstSizeLineSpan.IsValid)
                {
                    return;
                }

                var openBracketLineSpan = openBracketToken.GetLocation().GetLineSpan();
                if (!openBracketLineSpan.IsValid)
                {
                    return;
                }

                if (openBracketLineSpan.EndLinePosition.Line != firstSizeLineSpan.StartLinePosition.Line &&
                    openBracketLineSpan.EndLinePosition.Line != (firstSizeLineSpan.StartLinePosition.Line - 1))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstSize.GetLocation()));
                }
            }
        }

        private static void AnalyzeAttributeList(SyntaxNodeAnalysisContext context, AttributeListSyntax attributesList)
        {
            var openBracketToken = attributesList.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                attributesList.IsMissing ||
                !attributesList.Attributes.Any())
            {
                return;
            }

            var firstAttribute = attributesList.Attributes[0];

            var firstAttributeLineSpan = firstAttribute.GetLocation().GetLineSpan();
            if (!firstAttributeLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLocation().GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstAttributeLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstAttributeLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstAttribute.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax argumentListSyntax)
        {
            var openBracketToken = argumentListSyntax.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];

            var firstArgumentLineSpan = firstArgument.GetLocation().GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLocation().GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, AttributeArgumentListSyntax argumentListSyntax)
        {
            if (argumentListSyntax == null ||
                argumentListSyntax.OpenParenToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];

            var firstArgumentLineSpan = firstArgument.GetLocation().GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = argumentListSyntax.OpenParenToken.GetLocation().GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentListSyntax)
        {
            var openParenToken = argumentListSyntax.OpenParenToken;
            if (openParenToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];

            var firstArgumentLineSpan = firstArgument.GetLocation().GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = openParenToken.GetLocation().GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeBracketParametersList(SyntaxNodeAnalysisContext context, BracketedParameterListSyntax parameterListSyntax)
        {
            var openBracketToken = parameterListSyntax.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];

            var firstParameterLineSpan = firstParameter.GetLocation().GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLocation().GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstParameterLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstParameterLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
            }
        }

        private static void AnalyzeParametersList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterListSyntax)
        {
            var openParenToken = parameterListSyntax.OpenParenToken;
            if (openParenToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];

            var firstParameterLineSpan = firstParameter.GetLocation().GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = openParenToken.GetLocation().GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstParameterLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstParameterLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
            }
        }
    }
}
