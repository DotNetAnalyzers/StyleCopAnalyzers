namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1114Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1114MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1114Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1114.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttributesList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
        }

        private static void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;
            AnalyzeParametersList(context, operatorDeclaration.ParameterList);
        }

        private static void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;
            AnalyzeParametersList(context, conversionOperatorDeclaration.ParameterList);
        }

        private static void HandleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            AnalyzeParametersList(context, lambdaExpression.ParameterList);
        }

        private static void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            AnalyzeParametersList(context, anonymousMethod.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            AnalyzeParametersList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleAttributesList(SyntaxNodeAnalysisContext context)
        {
            var attributesList = (AttributeListSyntax)context.Node;

            AnalyzeAttributeList(context, attributesList);
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            AnalyzeArgumentList(context, attribute.ArgumentList);
        }

        private static void HandleElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private static void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;
            if (arrayCreation.Type == null)
            {
                return;
            }

            AnalyzeRankSpecifiers(context, arrayCreation);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            AnalyzeBracketParametersList(context, indexerDeclaration.ParameterList);
        }

        private static void HandleObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            if (objectCreation?.ArgumentList != null)
            {
                AnalyzeArgumentList(context, objectCreation.ArgumentList);
            }
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;
            AnalyzeParametersList(context, constructorDeclaration.ParameterList);
        }

        private static void HandleMethodInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            AnalyzeArgumentList(context, invocationExpression.ArgumentList);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
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

                var firstSizeLineSpan = firstSize.GetLineSpan();
                if (!firstSizeLineSpan.IsValid)
                {
                    return;
                }

                var openBracketLineSpan = openBracketToken.GetLineSpan();
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

            var firstAttributeLineSpan = firstAttribute.GetLineSpan();
            if (!firstAttributeLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
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
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
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
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = argumentListSyntax.OpenParenToken.GetLineSpan();
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
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = openParenToken.GetLineSpan();
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

            var firstParameterLineSpan = firstParameter.GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
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
            if (parameterListSyntax == null ||
                parameterListSyntax.OpenParenToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];

            var firstParameterLineSpan = firstParameter.GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = parameterListSyntax.OpenParenToken.GetLineSpan();
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
