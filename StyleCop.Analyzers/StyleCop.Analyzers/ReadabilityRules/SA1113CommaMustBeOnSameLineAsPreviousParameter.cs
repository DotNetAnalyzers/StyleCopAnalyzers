namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Collections.Generic;
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

        private const string Title = "Comma must be on same line as previous parameter";
        private const string MessageFormat = "Comma must be on same line as previous parameter.";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "A comma between two parameters in a call to a C# method or indexer, or in the declaration of a method or indexer, is not placed on the same line as the previous parameter.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1113.html";

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
            context.RegisterSyntaxNodeAction(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(HandleInvoationExpression, SyntaxKind.InvocationExpression);
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

                if (elementAccess.ArgumentList.IsMissing || elementAccess.ArgumentList.Arguments.Count  < 2)
                {
                    return;
                }

                var commas = elementAccess.ArgumentList
               .ChildTokens()
               .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
               .ToList();

                var argumentListSyntax = elementAccess.ArgumentList;
                CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, argumentListSyntax);
            }
        }

        private void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (constructorDeclaration.ParameterList.IsMissing || constructorDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = constructorDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
                .ToList();

            var parameterListSyntax = constructorDeclaration.ParameterList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, parameterListSyntax);
        }

        private void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (ObjectCreationExpressionSyntax)context.Node;

            if (invocationEpression.ArgumentList.IsMissing || invocationEpression.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var commas = invocationEpression.ArgumentList
                .ChildTokens()
                .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
                .ToList();

            var argumentListSyntax = invocationEpression.ArgumentList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, argumentListSyntax);
        }

        private void HandleInvoationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (InvocationExpressionSyntax)context.Node;

            if (invocationEpression.ArgumentList.IsMissing || invocationEpression.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var commas = invocationEpression.ArgumentList
                .ChildTokens()
                .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
                .ToList();

            var argumentListSyntax = invocationEpression.ArgumentList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, argumentListSyntax);
        }

        private void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (constructorDeclaration.ParameterList.IsMissing || constructorDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = constructorDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
                .ToList();

            var parameterListSyntax = constructorDeclaration.ParameterList;
            CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, commas, parameterListSyntax);
        }

        private void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.ParameterList.IsMissing || methodDeclaration.ParameterList.Parameters.Count < 2)
            {
                return;
            }

            var commas = methodDeclaration.ParameterList
                .ChildTokens()
                .Where(t => t.CSharpKind() == SyntaxKind.CommaToken)
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
                    previousParameter.GetLocation().GetLineSpan().StartLinePosition.Line)
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
                    previousParameter.GetLocation().GetLineSpan().StartLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, commaLocation));
                }
            }
        }
    }
}
