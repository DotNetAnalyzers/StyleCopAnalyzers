namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter
    /// does not start on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the parameters to a method or indexer span across multiple lines, but
    /// the first parameter does not start on the line after the opening bracket. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameters must begin on the line after the declaration, whenever the parameter span across multiple
    /// lines:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1116SplitParametersMustStartOnLineAfterDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1116SplitParametersMustStartOnLineAfterDeclaration"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1116";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1116Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1116MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1116Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1116.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(
                HandleBaseMethodDeclaration,
                new[] { SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration });

            context.RegisterSyntaxNodeActionHonorExclusions(
                HandleConstructorInitializer,
                new[] { SyntaxKind.BaseConstructorInitializer, SyntaxKind.ThisConstructorInitializer });

            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleInvocation, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectCreation, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementAccess, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementBinding, SyntaxKind.ElementBindingExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleImpliticElementAccess, SyntaxKind.ImplicitElementAccess);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleLambda, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (BaseMethodDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, declaration.ParameterList);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, invocation.ArgumentList);
        }

        private static void HandleObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, objectCreation.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            BracketedParameterListSyntax argumentListSyntax = indexerDeclaration.ParameterList;
            SeparatedSyntaxList<ParameterSyntax> arguments = argumentListSyntax.Parameters;

            if (arguments.Count > 1)
            {
                Analyze(context, argumentListSyntax.OpenBracketToken, arguments[0], arguments[1]);
            }
        }

        private static void HandleElementAccess(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementAccess.ArgumentList);
        }

        private static void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            foreach (var rankSpecifier in arrayCreation.Type.RankSpecifiers)
            {
                SeparatedSyntaxList<ExpressionSyntax> sizes = rankSpecifier.Sizes;
                if (sizes.Count > 1)
                {
                    Analyze(context, rankSpecifier.OpenBracketToken, sizes[0], sizes[1]);
                }
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            AttributeArgumentListSyntax argumentListSyntax = attribute.ArgumentList;
            SeparatedSyntaxList<AttributeArgumentSyntax> arguments = argumentListSyntax.Arguments;

            if (arguments.Count > 1)
            {
                Analyze(context, argumentListSyntax.OpenParenToken, arguments[0], arguments[1]);
            }
        }

        private static void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, anonymousMethod.ParameterList);
        }

        private static void HandleLambda(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, parenthesizedLambda.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, delegateDeclaration.ParameterList);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;
            HandleArgumentListSyntax(context, constructorInitializer.ArgumentList);
        }

        private static void HandleElementBinding(SyntaxNodeAnalysisContext context)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementBinding.ArgumentList);
        }

        private static void HandleImpliticElementAccess(SyntaxNodeAnalysisContext context)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, implicitElementAccess.ArgumentList);
        }

        private static void HandleArgumentListSyntax(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList)
        {
            if (argumentList == null)
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> parameters = argumentList.Arguments;

            if (parameters.Count > 1)
            {
                Analyze(context, argumentList.OpenParenToken, parameters[0], parameters[1]);
            }
        }

        private static void HandleParameterListSyntax(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (parameters.Count > 1)
            {
                Analyze(context, parameterList.OpenParenToken, parameters[0], parameters[1]);
            }
        }

        private static void HandleBracketedArgumentListSyntax(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax bracketedArgumentList)
        {
            SeparatedSyntaxList<ArgumentSyntax> parameters = bracketedArgumentList.Arguments;

            if (parameters.Count > 1)
            {
                Analyze(context, bracketedArgumentList.OpenBracketToken, parameters[0], parameters[1]);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxToken openParenOrBracketToken,
            SyntaxNode firstParameter,
            SyntaxNode secondParameter)
        {
            int firstParameterLine = firstParameter.GetLineSpan().StartLinePosition.Line;
            if (openParenOrBracketToken.GetLine() == firstParameterLine)
            {
                if (firstParameterLine != secondParameter.GetLineSpan().StartLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
                }
            }
        }
    }
}
