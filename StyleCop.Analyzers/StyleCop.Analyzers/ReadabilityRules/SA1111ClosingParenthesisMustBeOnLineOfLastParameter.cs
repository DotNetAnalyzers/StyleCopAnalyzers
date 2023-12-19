// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or
    /// indexer, is not placed on the same line as the last parameter.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the closing bracket of a method or indexer call or declaration is not
    /// placed on the same line as the last parameter. The following examples show correct placement of the
    /// bracket:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    ///     string name = JoinStrings(
    ///         first,
    ///         last);
    /// }
    ///
    /// public int this[int x]
    /// {
    ///     get { return this.items[x]; }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1111ClosingParenthesisMustBeOnLineOfLastParameter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1111ClosingParenthesisMustBeOnLineOfLastParameter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1111";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1111.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1111Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1111MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1111Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> HandledMethodSyntaxKinds =
            ImmutableArray.Create(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BaseMethodDeclarationAction, HandledMethodSyntaxKinds);
            context.RegisterSyntaxNodeAction(LocalFunctionStatementAction, SyntaxKindEx.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            if (arrayCreation.Type == null)
            {
                return;
            }

            foreach (var arrayRankSpecifierSyntax in arrayCreation.Type.RankSpecifiers)
            {
                if (!arrayRankSpecifierSyntax.Sizes.Any())
                {
                    continue;
                }

                var lastSize = arrayRankSpecifierSyntax.Sizes
                    .Last();

                if (!arrayRankSpecifierSyntax.CloseBracketToken.IsMissing)
                {
                    CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                        context,
                        lastSize,
                        arrayRankSpecifierSyntax.CloseBracketToken);
                }
            }
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpressionSyntax = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambdaExpressionSyntax.ParameterList == null ||
                lambdaExpressionSyntax.ParameterList.IsMissing ||
                !lambdaExpressionSyntax.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = lambdaExpressionSyntax.ParameterList
                .Parameters
                .Last();

            if (!lambdaExpressionSyntax.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastParameter,
                    lambdaExpressionSyntax.ParameterList.CloseParenToken);
            }
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.ParameterList == null ||
                anonymousMethod.ParameterList.IsMissing ||
                !anonymousMethod.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = anonymousMethod.ParameterList
                .Parameters
                .Last();

            if (!anonymousMethod.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastParameter,
                    anonymousMethod.ParameterList.CloseParenToken);
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            if (attribute.ArgumentList == null ||
                attribute.ArgumentList.IsMissing ||
                !attribute.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = attribute.ArgumentList
                .Arguments
                .Last();

            if (!attribute.ArgumentList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastArgument,
                    attribute.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            if (delegateDeclaration.ParameterList == null ||
                delegateDeclaration.ParameterList.IsMissing ||
                !delegateDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = delegateDeclaration.ParameterList
                .Parameters
                .Last();

            if (!delegateDeclaration.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastParameter,
                    delegateDeclaration.ParameterList.CloseParenToken);
            }
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (elementAccess.ArgumentList == null ||
                elementAccess.ArgumentList.IsMissing ||
                !elementAccess.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = elementAccess.ArgumentList
                .Arguments
                .Last();

            if (!elementAccess.ArgumentList.CloseBracketToken.IsMissing && !lastArgument.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastArgument,
                    elementAccess.ArgumentList.CloseBracketToken);
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (invocationExpression.ArgumentList == null ||
                invocationExpression.ArgumentList.IsMissing ||
                !invocationExpression.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = invocationExpression.ArgumentList
                .Arguments
                .Last();

            if (!invocationExpression.ArgumentList.CloseParenToken.IsMissing &&
                !lastArgument.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastArgument,
                    invocationExpression.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreation.ArgumentList == null ||
                objectCreation.ArgumentList.IsMissing ||
                !objectCreation.ArgumentList.Arguments.Any())
            {
                return;
            }

            var lastArgument = objectCreation.ArgumentList
                .Arguments
                .Last();

            if (!objectCreation.ArgumentList.CloseParenToken.IsMissing &&
                !lastArgument.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
                    context,
                    lastArgument,
                    objectCreation.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.ParameterList == null ||
                indexerDeclaration.ParameterList.IsMissing ||
                !indexerDeclaration.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = indexerDeclaration.ParameterList
                         .Parameters
                         .Last();

            if (!indexerDeclaration.ParameterList.CloseBracketToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter, indexerDeclaration.ParameterList.CloseBracketToken);
            }
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclarationSyntax = (BaseMethodDeclarationSyntax)context.Node;

            if (baseMethodDeclarationSyntax.ParameterList == null ||
                baseMethodDeclarationSyntax.ParameterList.IsMissing ||
                !baseMethodDeclarationSyntax.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = baseMethodDeclarationSyntax.ParameterList
                .Parameters
                .Last();

            if (!baseMethodDeclarationSyntax.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter, baseMethodDeclarationSyntax.ParameterList.CloseParenToken);
            }
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatementSyntax = (LocalFunctionStatementSyntaxWrapper)context.Node;

            if (localFunctionStatementSyntax.ParameterList == null ||
                localFunctionStatementSyntax.ParameterList.IsMissing ||
                !localFunctionStatementSyntax.ParameterList.Parameters.Any())
            {
                return;
            }

            var lastParameter = localFunctionStatementSyntax.ParameterList
                .Parameters
                .Last();

            if (!localFunctionStatementSyntax.ParameterList.CloseParenToken.IsMissing)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter, localFunctionStatementSyntax.ParameterList.CloseParenToken);
            }
        }

        private static void CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
            SyntaxNodeAnalysisContext context,
            CSharpSyntaxNode parameterOrArgument,
            SyntaxToken closeToken)
        {
            var lastParameterLine = parameterOrArgument.GetLineSpan();
            var closeParenLine = closeToken.GetLineSpan();
            if (lastParameterLine.IsValid &&
                closeParenLine.IsValid &&
                closeParenLine.StartLinePosition.Line != lastParameterLine.EndLinePosition.Line)
            {
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeToken.GetLocation(), properties));
            }
        }
    }
}
