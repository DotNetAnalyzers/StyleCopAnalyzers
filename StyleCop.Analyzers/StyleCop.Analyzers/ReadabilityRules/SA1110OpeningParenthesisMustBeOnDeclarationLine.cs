using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

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
        internal const string Title = "Opening parenthesis or bracket must be on declaration line";
        internal const string MessageFormat = "Opening parenthesis or bracket must be on declaration line.";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The opening parenthesis or bracket is not placed on the same line as the method/indexer/attribute/array name.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1110.html";

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
            context.RegisterSyntaxNodeAction(this.HandleMethodDeclaration,SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(this.HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(this.HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(this.HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(this.HandleDelegateDeclaration, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(this.HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeAction(this.HandleOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(this.HandleConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
        }

        private void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperator = (ConversionOperatorDeclarationSyntax) context.Node;

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
                CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken, identifierName.Identifier);
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
                CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context, parameterListSyntax.OpenParenToken, operatorDeclaration.OperatorToken);
            }
        }

        private void HandleArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var array = (ArrayCreationExpressionSyntax) context.Node;

            if (array.Type.IsMissing ||
                array.Type.ElementType == null ||
                !array.Type.RankSpecifiers.Any())
            {
                return;
            }

            var firstSize = array.Type.RankSpecifiers.First();

            if (!firstSize.OpenBracketToken.IsMissing)
            {
                CheckIfLocationOfExpressionAndOpenTokenAreTheSame(context, firstSize.OpenBracketToken, array.Type.ElementType);
            }
        }

        private void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.DelegateKeyword.IsMissing ||
                anonymousMethod.ParameterList.IsMissing ||
                anonymousMethod.ParameterList.OpenParenToken.IsMissing)
            {
                return;
            }

            CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,anonymousMethod.ParameterList.OpenParenToken, anonymousMethod.DelegateKeyword);
        }

        private void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax) context.Node;

            if (!delegateDeclaration.Identifier.IsMissing &&
                delegateDeclaration.ParameterList != null &&
                !delegateDeclaration.ParameterList.OpenParenToken.IsMissing)
            {
                CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,
                    delegateDeclaration.ParameterList.OpenParenToken, delegateDeclaration.Identifier);
            }
        }

        private void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax) context.Node;

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
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,
                        attribute.ArgumentList.OpenParenToken, identifierNameSyntax.Identifier);
                }
            }
        }

        private void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax) context.Node;

            if (elementAccess.Expression == null ||
                elementAccess.ArgumentList.IsMissing ||
                elementAccess.ArgumentList.OpenBracketToken.IsMissing)
            {
                return;
            }

            CheckIfLocationOfExpressionAndOpenTokenAreTheSame(context,
                elementAccess.ArgumentList.OpenBracketToken, elementAccess.Expression);
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext obj)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax) obj.Node;

            if (!indexerDeclaration.ThisKeyword.IsMissing &&
                indexerDeclaration.ParameterList != null &&
                !indexerDeclaration.ParameterList.IsMissing &&
                !indexerDeclaration.ParameterList.OpenBracketToken.IsMissing)
            {
                CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(obj, indexerDeclaration.ParameterList.OpenBracketToken, indexerDeclaration.ThisKeyword);
            }
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax) context.Node;
            var qualifiedNameSyntax = objectCreation.ChildNodes().OfType<QualifiedNameSyntax>().FirstOrDefault();
            IdentifierNameSyntax identifierNameSyntax = null;
            if (qualifiedNameSyntax != null)
            {
                identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            }
            else
            {
                identifierNameSyntax = objectCreation.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            }

            if (identifierNameSyntax != null)
            {
                if (objectCreation.ArgumentList != null &&
                    !objectCreation.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,
                        objectCreation.ArgumentList.OpenParenToken, identifierNameSyntax.Identifier);
                }
            }
        }

        private void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;
            var identifierNameSyntax = invocationExpression.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
            if (identifierNameSyntax != null)
            {
                if (invocationExpression.ArgumentList != null &&
                    !invocationExpression.ArgumentList.OpenParenToken.IsMissing &&
                    !identifierNameSyntax.Identifier.IsMissing)
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,
                        invocationExpression.ArgumentList.OpenParenToken, identifierNameSyntax.Identifier);
                }
            }
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax) context.Node;
            HandleBaseMethodDeclaration(context, constructotDeclarationSyntax);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax) context.Node;
            HandleBaseMethodDeclaration(context, methodDeclaration);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context,
            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
        {
            var identifierTokens =
                baseMethodDeclarationSyntax.ChildTokens()
                    .Where(t => t.CSharpKind() == SyntaxKind.IdentifierToken)
                    .ToList();
            var parameterListSyntax = baseMethodDeclarationSyntax.ParameterList;

            if (identifierTokens.Any() && parameterListSyntax != null)
            {
                var identifierToken = identifierTokens.First();
                var openParenToken =
                    parameterListSyntax.OpenParenToken;

                if (!openParenToken.IsMissing)
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context, openParenToken, identifierToken);
                }
            }
        }

        private static void CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(SyntaxNodeAnalysisContext context,
            SyntaxToken openToken, SyntaxToken identifierToken)
        {
            var identifierLine = identifierToken.GetLocation().GetLineSpan();
            var openParenLocation = openToken.GetLocation();
            var openParenLine = openParenLocation.GetLineSpan();
            if (identifierLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != identifierLine.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openParenLocation));
            }
        }

        private static void CheckIfLocationOfExpressionAndOpenTokenAreTheSame(SyntaxNodeAnalysisContext context,
    SyntaxToken openToken, ExpressionSyntax expression)
        {
            var identifierLine = expression.GetLocation().GetLineSpan();
            var openParenLocation = openToken.GetLocation();
            var openParenLine = openParenLocation.GetLineSpan();
            if (identifierLine.IsValid &&
                openParenLine.IsValid &&
                openParenLine.StartLinePosition.Line != identifierLine.StartLinePosition.Line)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, openParenLocation));
            }
        }
    }
}
