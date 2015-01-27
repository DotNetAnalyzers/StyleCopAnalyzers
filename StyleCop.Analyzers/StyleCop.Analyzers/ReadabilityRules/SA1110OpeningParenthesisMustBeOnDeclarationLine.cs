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
        public const string DiagnosticId = "SA1110";
        internal const string Title = "Opening parenthesis must be on declaration line";
        internal const string MessageFormat = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1110.html";

        private static readonly DiagnosticDescriptor Descriptor =
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
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration,SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
        }

        private void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(((ElementAccessExpressionSyntax)context.Node).Expression).Symbol;
            var parameterSymbol = symbol as IParameterSymbol;

            if (parameterSymbol != null && parameterSymbol.IsThis)
            {
                var elementAccess = (ElementAccessExpressionSyntax)context.Node;

                var thisExpression = elementAccess.Expression as ThisExpressionSyntax;

                if (thisExpression != null  && !elementAccess.ArgumentList.OpenBracketToken.IsMissing)
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context, elementAccess.ArgumentList.OpenBracketToken, thisExpression.Token);
                }
            }
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext obj)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax) obj.Node;

            if (indexerDeclaration.ThisKeyword.IsMissing == false &&
                indexerDeclaration.ParameterList.IsMissing == false &&
                indexerDeclaration.ParameterList.OpenBracketToken.IsMissing == false)
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
                identifierNameSyntax = qualifiedNameSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            }
            else
            {
                identifierNameSyntax = objectCreation.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            }

            if (identifierNameSyntax != null)
            {
                if (objectCreation.ArgumentList.OpenParenToken.IsMissing == false &&
                    identifierNameSyntax.Identifier.IsMissing == false)
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context,
                        objectCreation.ArgumentList.OpenParenToken, identifierNameSyntax.Identifier);
                }
            }
        }

        private void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax) context.Node;
            var identifierNameSyntax = invocationExpression.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (identifierNameSyntax != null)
            {
                if (invocationExpression.ArgumentList.OpenParenToken.IsMissing == false &&
                    identifierNameSyntax.Identifier.IsMissing == false)
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
            var parameterListSyntax = baseMethodDeclarationSyntax.ChildNodes().OfType<ParameterListSyntax>().SingleOrDefault();

            if (identifierTokens.Any() && parameterListSyntax != null)
            {
                var identifierToken = identifierTokens.First();
                var openParenTokens =
                    parameterListSyntax.ChildTokens()
                        .Where(t => t.CSharpKind() == SyntaxKind.OpenParenToken)
                        .ToList();

                if (openParenTokens.Any())
                {
                    CheckIfLocationOfIdentifierNameAndOpenTokenAreTheSame(context, openParenTokens.First(), identifierToken);
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
    }
}
