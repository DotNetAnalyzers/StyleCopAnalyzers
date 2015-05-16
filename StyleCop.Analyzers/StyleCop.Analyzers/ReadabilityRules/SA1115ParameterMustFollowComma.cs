namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A parameter within a C# method or indexer call or declaration does not begin on the same line as the previous
    /// parameter, or on the next line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when there are one or more blank lines between a parameter and the
    /// previous parameter. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameter must begin on the same line as the previous comma, or on the next line. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1115ParameterMustFollowComma : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1115ParameterMustFollowComma"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1115";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1115MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string Category = "StyleCop.CSharp.ReadabilityRules";
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1115.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleLambda, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax) context.Node;

            AnalyzeParameterList(context, operatorDeclaration.ParameterList);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax) context.Node;

            AnalyzeParameterList(context, delegateDeclaration.ParameterList);
        }

        private void HandleLambda(SyntaxNodeAnalysisContext context)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax) context.Node;

            AnalyzeParameterList(context, lambda.ParameterList);
        }

        private void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax) context.Node;

            AnalyzeParameterList(context, anonymousMethod.ParameterList);
        }

        private void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (attributeList.Attributes.Count < 2)
            {
                return;
            }

            var previousLine = attributeList.Attributes[0].GetLocation().GetLineSpan().StartLinePosition.Line;
            for (int i = 1; i < attributeList.Attributes.Count; i++)
            {
                var currentAttribute = attributeList.Attributes[i];
                var currentLine = currentAttribute.GetLocation().GetLineSpan().StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentAttribute.GetLocation()));
                }

                previousLine = currentLine;
            }
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax) context.Node;

            if (attribute.ArgumentList == null
                || attribute.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = attribute.ArgumentList.Arguments[0].GetLocation().GetLineSpan().StartLinePosition.Line;
            for (int i = 1; i < attribute.ArgumentList.Arguments.Count; i++)
            {
                var currentArgument = attribute.ArgumentList.Arguments[i];
                var currentLine = currentArgument.GetLocation().GetLineSpan().StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = currentLine;
            }
        }

        private void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax) context.Node;
            if (arrayCreation.Type == null)
            {
                return;
            }

            foreach (var rankSpecifier in arrayCreation.Type.RankSpecifiers)
            {
                if (rankSpecifier.Sizes.Count < 2)
                {
                    continue;
                }

                var previousLine = rankSpecifier.Sizes[0].GetLocation().GetLineSpan().StartLinePosition.Line;
                for (int i = 1; i < rankSpecifier.Sizes.Count; i++)
                {
                    var currentSize = rankSpecifier.Sizes[i];
                    var currentLine = currentSize.GetLocation().GetLineSpan().StartLinePosition.Line;
                    if (currentLine - previousLine > 1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentSize.GetLocation()));
                    }

                    previousLine = currentLine;
                }
            }
        }

        private void HandleElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax) context.Node;

            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax) context.Node;

            AnalyzeParameterList(context, indexerDeclaration.ParameterList);
        }

        private void HandleObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax) context.Node;

            AnalyzeArgumentList(context, objectCreation.ArgumentList);
        }

        private void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax) context.Node;

            AnalyzeArgumentList(context, invocation.ArgumentList);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax) context.Node;

            AnalyzeParameterList(context, constructorDeclaration.ParameterList);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, methodDeclaration.ParameterList);
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, BaseArgumentListSyntax argumentListSyntax)
        {
            if (argumentListSyntax == null
                || argumentListSyntax.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = argumentListSyntax.Arguments[0].GetLocation().GetLineSpan().StartLinePosition.Line;
            for (int i = 1; i < argumentListSyntax.Arguments.Count; i++)
            {
                var currentArgument = argumentListSyntax.Arguments[i];
                var currentLine = currentArgument.GetLocation().GetLineSpan().StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = currentLine;
            }
        }

        private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context, BaseParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null
                || parameterListSyntax.Parameters.Count < 2)
            {
                return;
            }

            var previousLine = parameterListSyntax.Parameters[0].GetLocation().GetLineSpan().StartLinePosition.Line;
            for (int i = 1; i < parameterListSyntax.Parameters.Count; i++)
            {
                var currentParameter = parameterListSyntax.Parameters[i];
                var currentLine = currentParameter.GetLocation().GetLineSpan().StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.GetLocation()));
                }

                previousLine = currentLine;
            }
        }
    }
}
