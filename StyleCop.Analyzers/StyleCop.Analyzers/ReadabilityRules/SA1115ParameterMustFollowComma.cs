namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1115.md";

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
            context.RegisterSyntaxNodeActionHonorExclusions(HandleBaseMethodDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleLambda, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConstructorInitializer, SyntaxKind.BaseConstructorInitializer, SyntaxKind.ThisConstructorInitializer);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementBinding, SyntaxKind.ElementBindingExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleImpliticElementAccess, SyntaxKind.ImplicitElementAccess);
        }

        private static void HandleImpliticElementAccess(SyntaxNodeAnalysisContext context)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;

            AnalyzeArgumentList(context, implicitElementAccess.ArgumentList);
        }

        private static void HandleElementBinding(SyntaxNodeAnalysisContext context)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, elementBinding.ArgumentList);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;

            AnalyzeArgumentList(context, constructorInitializer.ArgumentList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleLambda(SyntaxNodeAnalysisContext context)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            AnalyzeParameterList(context, lambda.ParameterList);
        }

        private static void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            AnalyzeParameterList(context, anonymousMethod.ParameterList);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (attributeList.Attributes.Count < 2)
            {
                return;
            }

            var previousLine = attributeList.Attributes[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < attributeList.Attributes.Count; i++)
            {
                var currentAttribute = attributeList.Attributes[i];
                var lineSpan = currentAttribute.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentAttribute.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            if (attribute.ArgumentList == null
                || attribute.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = attribute.ArgumentList.Arguments[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < attribute.ArgumentList.Arguments.Count; i++)
            {
                var currentArgument = attribute.ArgumentList.Arguments[i];
                var lineSpan = currentArgument.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }

        private static void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;
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

                var previousLine = rankSpecifier.Sizes[0].GetLineSpan().EndLinePosition.Line;
                for (int i = 1; i < rankSpecifier.Sizes.Count; i++)
                {
                    var currentSize = rankSpecifier.Sizes[i];
                    var lineSpan = currentSize.GetLineSpan();
                    var currentLine = lineSpan.StartLinePosition.Line;
                    if (currentLine - previousLine > 1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentSize.GetLocation()));
                    }

                    previousLine = lineSpan.EndLinePosition.Line;
                }
            }
        }

        private static void HandleElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, indexerDeclaration.ParameterList);
        }

        private static void HandleObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, objectCreation.ArgumentList);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, invocation.ArgumentList);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, constructorDeclaration.ParameterList);
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, BaseArgumentListSyntax argumentListSyntax)
        {
            if (argumentListSyntax == null
                || argumentListSyntax.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = argumentListSyntax.Arguments[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < argumentListSyntax.Arguments.Count; i++)
            {
                var currentArgument = argumentListSyntax.Arguments[i];
                var lineSpan = currentArgument.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }

        private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context, BaseParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null
                || parameterListSyntax.Parameters.Count < 2)
            {
                return;
            }

            var previousLine = parameterListSyntax.Parameters[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < parameterListSyntax.Parameters.Count; i++)
            {
                var currentParameter = parameterListSyntax.Parameters[i];
                var lineSpan = currentParameter.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }
    }
}
