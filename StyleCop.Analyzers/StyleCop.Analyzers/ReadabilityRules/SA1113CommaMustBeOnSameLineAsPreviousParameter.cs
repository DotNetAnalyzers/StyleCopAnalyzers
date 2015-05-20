namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A comma between two parameters in a call to a C# method or indexer, or in the declaration of a method or
    /// indexer, is not placed on the same line as the previous parameter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a comma between two parameters to a method or indexer is not placed
    /// on the same line as the previous parameter. The following examples show correct placement of the comma:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    ///     string name = JoinStrings(
    ///         first, 
    ///         last);
    /// }
    ///
    /// public int this[int x, 
    ///    int y]
    /// {
    ///     get { return this.items[x, y]; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1113CommaMustBeOnSameLineAsPreviousParameter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1113CommaMustBeOnSameLineAsPreviousParameter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1113";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1113Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1113MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1113Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1113.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInvoationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousMethodDeclaration, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleArrayDeclaration, SyntaxKind.ArrayCreationExpression);
        }

        private void HandleArrayDeclaration(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            if (arrayCreation.Type == null)
            {
                return;
            }

            foreach (var arrayRankSpecifierSyntax in arrayCreation.Type.RankSpecifiers)
            {
                if (arrayRankSpecifierSyntax.Sizes.Count < 2)
                {
                    continue;
                }

                var commas = arrayRankSpecifierSyntax
                    .ChildTokens()
                    .Where(t => t.IsKind(SyntaxKind.CommaToken))
                    .ToList();

                if (!arrayRankSpecifierSyntax.CloseBracketToken.IsMissing)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, arrayRankSpecifierSyntax);
                }
            }
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            if (operatorDeclaration.ParameterList == null ||
                operatorDeclaration.ParameterList.IsMissing ||
                operatorDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = operatorDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, operatorDeclaration.ParameterList);
        }

        private void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeListSyntax)context.Node;

            if (attribute == null ||
                attribute.Attributes.Count < 2)
            {
                return;
            }

            var commas = attribute
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, attribute);
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            if (attribute.ArgumentList == null ||
                attribute.ArgumentList.IsMissing ||
                attribute.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var commas = attribute.ArgumentList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, attribute.ArgumentList);
        }

        private void HandleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambdaExpression.ParameterList == null ||
                lambdaExpression.ParameterList.IsMissing ||
                lambdaExpression.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = lambdaExpression.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, lambdaExpression.ParameterList);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            if (delegateDeclaration.ParameterList == null ||
                delegateDeclaration.ParameterList.IsMissing ||
                delegateDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = delegateDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, delegateDeclaration.ParameterList);
        }

        private void HandleAnonymousMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.ParameterList == null ||
                anonymousMethod.ParameterList.IsMissing ||
                anonymousMethod.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = anonymousMethod.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, anonymousMethod.ParameterList);
        }

        private void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (elementAccess.ArgumentList == null ||
                elementAccess.ArgumentList.IsMissing ||
                !elementAccess.ArgumentList.Arguments.Any())
            {
                return;
            }

            var commas = elementAccess.ArgumentList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, elementAccess.ArgumentList);
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (constructorDeclaration.ParameterList == null ||
                constructorDeclaration.ParameterList.IsMissing ||
                constructorDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = constructorDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            var parameterListSyntax = constructorDeclaration.ParameterList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, parameterListSyntax);
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (ObjectCreationExpressionSyntax)context.Node;

            if (invocationEpression.ArgumentList == null ||
                invocationEpression.ArgumentList.IsMissing ||
                invocationEpression.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var commas = invocationEpression.ArgumentList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            var argumentListSyntax = invocationEpression.ArgumentList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, argumentListSyntax);
        }

        private void HandleInvoationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (InvocationExpressionSyntax)context.Node;

            if (invocationEpression.ArgumentList == null ||
                invocationEpression.ArgumentList.IsMissing ||
                invocationEpression.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var commas = invocationEpression.ArgumentList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            var argumentListSyntax = invocationEpression.ArgumentList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, argumentListSyntax);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (constructorDeclaration.ParameterList == null ||
                constructorDeclaration.ParameterList.IsMissing ||
                constructorDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = constructorDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            var parameterListSyntax = constructorDeclaration.ParameterList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, parameterListSyntax);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.ParameterList == null ||
                methodDeclaration.ParameterList.IsMissing ||
                methodDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = methodDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.CommaToken))
                .ToList();

            var parameterListSyntax = methodDeclaration.ParameterList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, parameterListSyntax);
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, List<SyntaxToken> commas,
            BaseParameterListSyntax parameterListSyntax)
        {
            for (int index = 0; index < commas.Count; index++)
            {
                var comma = commas[index];
                if (parameterListSyntax.Parameters.Count <= index)
                {
                    return;
                }

                var previousParameter = parameterListSyntax.Parameters[index];

                var commaLocation = comma.GetLocation();
                if (commaLocation.GetLineSpan().StartLinePosition.Line !=
                    previousParameter.GetLocation().GetLineSpan().EndLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, List<SyntaxToken> commas,
            BaseArgumentListSyntax parameterListSyntax)
        {
            for (int index = 0; index < commas.Count; index++)
            {
                var comma = commas[index];
                if (parameterListSyntax.Arguments.Count <= index)
                {
                    return;
                }

                var previousParameter = parameterListSyntax.Arguments[index];

                var commaLocation = comma.GetLocation();
                if (commaLocation.GetLineSpan().StartLinePosition.Line !=
                    previousParameter.GetLocation().GetLineSpan().EndLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, List<SyntaxToken> commas,
            AttributeArgumentListSyntax attributeListSyntax)
        {
            for (int index = 0; index < commas.Count; index++)
            {
                var comma = commas[index];
                if (attributeListSyntax.Arguments.Count <= index)
                {
                    return;
                }

                var previousParameter = attributeListSyntax.Arguments[index];

                var commaLocation = comma.GetLocation();
                if (commaLocation.GetLineSpan().StartLinePosition.Line !=
                    previousParameter.GetLocation().GetLineSpan().EndLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, List<SyntaxToken> commas,
            AttributeListSyntax attributeListSyntax)
        {
            for (int index = 0; index < commas.Count; index++)
            {
                var comma = commas[index];
                if (attributeListSyntax.Attributes.Count <= index)
                {
                    return;
                }

                var previousParameter = attributeListSyntax.Attributes[index];

                var commaLocation = comma.GetLocation();
                if (commaLocation.GetLineSpan().StartLinePosition.Line !=
                    previousParameter.GetLocation().GetLineSpan().EndLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, List<SyntaxToken> commas, ArrayRankSpecifierSyntax arrayRankSpecifierSyntax)
        {
            for (int index = 0; index < commas.Count; index++)
            {
                var comma = commas[index];
                if (arrayRankSpecifierSyntax.Sizes.Count <= index)
                {
                    return;
                }

                var previousParameter = arrayRankSpecifierSyntax.Sizes[index];

                var commaLocation = comma.GetLocation();
                if (commaLocation.GetLineSpan().StartLinePosition.Line !=
                    previousParameter.GetLocation().GetLineSpan().EndLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }
    }
}
