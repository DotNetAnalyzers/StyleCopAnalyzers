// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using SpacingRules;
    using StyleCop.Analyzers.Helpers;

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
    internal class SA1113CommaMustBeOnSameLineAsPreviousParameter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1113CommaMustBeOnSameLineAsPreviousParameter"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1113";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1113Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1113MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1113Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1113.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> OperatorDeclarationAction = HandleOperatorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(OperatorDeclarationAction, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
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
                var sizes = arrayRankSpecifierSyntax.Sizes;
                if (sizes.Count < 2)
                {
                    continue;
                }

                if (!arrayRankSpecifierSyntax.CloseBracketToken.IsMissing)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, sizes.GetWithSeparators());
                }
            }
        }

        private static void HandleOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, operatorDeclaration.ParameterList);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (attributeList != null && !attributeList.IsMissing)
            {
                var attributes = attributeList.Attributes;
                if (attributes.Count > 1)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, attributes.GetWithSeparators());
                }
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            var argumentList = attribute.ArgumentList;

            if (argumentList != null && !argumentList.IsMissing)
            {
                var arguments = argumentList.Arguments;
                if (arguments.Count > 1)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, arguments.GetWithSeparators());
                }
            }
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            HandleBaseParameterListSyntax(context, lambdaExpression.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, delegateDeclaration.ParameterList);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            HandleBaseParameterListSyntax(context, anonymousMethod.ParameterList);
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, elementAccess.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (IndexerDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, constructorDeclaration.ParameterList);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (ObjectCreationExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, invocationEpression.ArgumentList);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (InvocationExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, invocationEpression.ArgumentList);
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, constructorDeclaration.ParameterList);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, methodDeclaration.ParameterList);
        }

        private static void HandleBaseArgumentListSyntax(SyntaxNodeAnalysisContext context, BaseArgumentListSyntax argumentList)
        {
            if (argumentList != null && !argumentList.IsMissing)
            {
                var arguments = argumentList.Arguments;
                if (arguments.Count > 1)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, arguments.GetWithSeparators());
                }
            }
        }

        private static void HandleBaseParameterListSyntax(SyntaxNodeAnalysisContext context, BaseParameterListSyntax parameterList)
        {
            if (parameterList != null && !parameterList.IsMissing)
            {
                var parameters = parameterList.Parameters;
                if (parameters.Count > 1)
                {
                    CheckIfCommasAreAtTheSameLineAsThePreviousParameter(context, parameters.GetWithSeparators());
                }
            }
        }

        private static void CheckIfCommasAreAtTheSameLineAsThePreviousParameter(SyntaxNodeAnalysisContext context, SyntaxNodeOrTokenList nodeOrTokenList)
        {
            SyntaxNode previousNode = null;

            // If index is even we expecting parameter syntax node, otherwise we expecting comma token.
            for (int index = 0, count = nodeOrTokenList.Count; index < count; ++index)
            {
                SyntaxNodeOrToken nodeOrToken = nodeOrTokenList[index];
                if (index % 2 == 0)
                {
                    // We expecting node here
                    if (nodeOrToken.IsToken)
                    {
                        return;
                    }

                    previousNode = nodeOrToken.AsNode();
                }
                else
                {
                    // We expecting token here
                    if (nodeOrToken.IsNode)
                    {
                        return;
                    }

                    if (previousNode.GetEndLine() < nodeOrToken.GetLineSpan().StartLinePosition.Line)
                    {
                        var properties = TokenSpacingProperties.RemovePrecedingPreserveLayout;
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, nodeOrToken.GetLocation(), properties));
                    }
                }
            }
        }
    }
}
