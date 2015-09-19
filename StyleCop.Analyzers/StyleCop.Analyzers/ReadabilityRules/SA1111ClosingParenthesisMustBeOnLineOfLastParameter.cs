// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1111Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1111MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1111Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1111.md";

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
            context.RegisterCompilationStartAction(HandleCompilationStart);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleInvocationExpression, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleIndexerDeclaration, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleElementAccessExpression, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleDelegateDeclarationExpression, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttribute, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleAttributeList, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleArrayCreation, SyntaxKind.ArrayCreationExpression);
        }

        private static void HandleArrayCreation(SyntaxNodeAnalysisContext context)
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
                    CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastSize,
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
                    lambdaExpressionSyntax.ParameterList.CloseParenToken);
            }
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (!attributeList.Attributes.Any())
            {
                return;
            }

            var lastAttribute = attributeList.Attributes
                                .Last();

            if (!attributeList.CloseBracketToken.IsMissing && lastAttribute.ArgumentList != null)
            {
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastAttribute,
                    attributeList.CloseBracketToken);
            }
        }

        private static void HandleAnonymousMethod(SyntaxNodeAnalysisContext context)
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                    attribute.ArgumentList.CloseParenToken);
            }
        }

        private static void HandleDelegateDeclarationExpression(SyntaxNodeAnalysisContext context)
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastParameter,
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
                    elementAccess.ArgumentList.CloseBracketToken);
            }
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, methodDeclaration);
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructotDeclarationSyntax = (ConstructorDeclarationSyntax)context.Node;
            HandleBaseMethodDeclaration(context, constructotDeclarationSyntax);
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
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
                CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(context, lastArgument,
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

        private static void HandleBaseMethodDeclaration(
            SyntaxNodeAnalysisContext context,
            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
        {
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

        private static void CheckIfLocationOfLastArgumentOrParameterAndCloseTokenAreTheSame(
            SyntaxNodeAnalysisContext context,
            CSharpSyntaxNode parameterOrArgument, SyntaxToken closeToken)
        {
            var lastParameterLine = parameterOrArgument.GetLineSpan();
            var closeParenLine = closeToken.GetLineSpan();
            if (lastParameterLine.IsValid &&
                closeParenLine.IsValid &&
                closeParenLine.StartLinePosition.Line != lastParameterLine.EndLinePosition.Line)
            {
                var properties = TokenSpacingCodeFixProvider.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, closeToken.GetLocation(), properties));
            }
        }
    }
}
