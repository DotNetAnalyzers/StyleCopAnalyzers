namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the method or indexer name.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the opening bracket of a method or indexer call or declaration is not
    /// placed on the same line as the method or indexer. The following examples show correct placement of the opening
    /// bracket:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    ///     return JoinStrings(
    ///         first, last);
    /// }
    ///
    /// public int this[int x]
    /// {
    ///     get { return this.items[x]; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1110OpeningParenthesisMustBeOnDeclarationLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1110OpeningParenthesisMustBeOnDeclarationLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1110";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1110Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1110MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1110Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "http://www.stylecop.com/docs/SA1110.html";

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
        }

        private void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperator = (ConversionOperatorDeclarationSyntax)context.Node;

            var identifierName = conversionOperator.ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .FirstOrDefault();
            if (identifierName == null || identifierName.Identifier.IsMissing)
            {
                return;
            }

            var parameterListSyntax = conversionOperator.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.OpenParenToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken);
            }
        }

        private void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            if (operatorDeclaration.OperatorToken.IsMissing)
            {
                return;
            }

            var parameterListSyntax = operatorDeclaration.ParameterList;

            if (parameterListSyntax != null && !parameterListSyntax.OpenParenToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken);
            }
        }

        private void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var array = (ArrayCreationExpressionSyntax)context.Node;

            if (array.Type.IsMissing ||
                array.Type.ElementType == null ||
                !array.Type.RankSpecifiers.Any())
            {
                return;
            }

            var firstSize = array.Type.RankSpecifiers.First();

            if (!firstSize.OpenBracketToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, firstSize.OpenBracketToken);
            }
        }

        private void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.DelegateKeyword.IsMissing ||
                anonymousMethod.ParameterList == null ||
                anonymousMethod.ParameterList.IsMissing ||
                anonymousMethod.ParameterList.OpenParenToken.IsMissing)
            {
                return;
            }

            CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, anonymousMethod.ParameterList.OpenParenToken);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            if (!delegateDeclaration.Identifier.IsMissing &&
                delegateDeclaration.ParameterList != null &&
                !delegateDeclaration.ParameterList.OpenParenToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, delegateDeclaration.ParameterList.OpenParenToken);
            }
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            var qualifiedNameSyntax = attribute.ChildNodes().OfType<QualifiedNameSyntax>().FirstOrDefault();
            IdentifierNameSyntax identifierNameSyntax = null;
            if (qualifiedNameSyntax != null)
            {
                identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            }
            else
            {
                identifierNameSyntax = attribute.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            }

            if (identifierNameSyntax != null)
            {
                if (attribute.ArgumentList != null &&
                    !attribute.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, attribute.ArgumentList.OpenParenToken);
                }
            }
        }

        private void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (elementAccess.Expression == null ||
                elementAccess.ArgumentList.IsMissing ||
                elementAccess.ArgumentList.OpenBracketToken.IsMissing)
            {
                return;
            }

            CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, elementAccess.ArgumentList.OpenBracketToken);
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext obj)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)obj.Node;

            if (!indexerDeclaration.ThisKeyword.IsMissing &&
                indexerDeclaration.ParameterList != null &&
                !indexerDeclaration.ParameterList.IsMissing &&
                !indexerDeclaration.ParameterList.OpenBracketToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(obj, indexerDeclaration.ParameterList.OpenBracketToken);
            }
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            var identifier = this.GetIdentifier(objectCreation);

            if (!identifier.HasValue
                || identifier.Value.IsMissing)
            {
                return;
            }

            if (objectCreation.ArgumentList != null
                && !objectCreation.ArgumentList.OpenParenToken.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, objectCreation.ArgumentList.OpenParenToken);
            }
        }

        private SyntaxToken? GetIdentifier(ObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            switch (objectCreationExpressionSyntax.Type.Kind())
            {
            case SyntaxKind.QualifiedName:
                var qualifiedNameSyntax = (QualifiedNameSyntax)objectCreationExpressionSyntax.Type;
                var identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                return identifierNameSyntax?.Identifier;

            case SyntaxKind.IdentifierName:
                return ((IdentifierNameSyntax)objectCreationExpressionSyntax.Type).Identifier;

            case SyntaxKind.GenericName:
                return ((GenericNameSyntax)objectCreationExpressionSyntax.Type).Identifier;

            default:
                return null;
            }
        }

        private void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var identifierNameSyntax = invocationExpression.Expression as IdentifierNameSyntax ??
                                                        invocationExpression.Expression.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();

            if (identifierNameSyntax != null)
            {
                if (invocationExpression.ArgumentList != null &&
                    !invocationExpression.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, invocationExpression.ArgumentList.OpenParenToken);
                }
            }
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            if (constructotDeclarationSyntax.ParameterList != null
                && !constructotDeclarationSyntax.ParameterList.OpenParenToken.IsMissing
                && !constructotDeclarationSyntax.Identifier.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, constructotDeclarationSyntax.ParameterList.OpenParenToken);
            }
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (methodDeclaration.ParameterList != null
                && !methodDeclaration.ParameterList.OpenParenToken.IsMissing
                && !methodDeclaration.Identifier.IsMissing)
            {
                CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(context, methodDeclaration.ParameterList.OpenParenToken);
            }
        }

        private static void CheckIfLocationOfPreviousTokenAndOpenTokenAreTheSame(SyntaxNodeAnalysisContext context, SyntaxToken openToken)
        {
            var previousToken = openToken.GetPreviousToken();
            if (previousToken.IsMissing)
            {
                return;
            }

            var prevTokenLine = previousToken.GetLineSpan();
            var openParenLine = openToken.GetLineSpan();
            if (prevTokenLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != prevTokenLine.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openToken.GetLocation()));
            }
        }
    }
}
