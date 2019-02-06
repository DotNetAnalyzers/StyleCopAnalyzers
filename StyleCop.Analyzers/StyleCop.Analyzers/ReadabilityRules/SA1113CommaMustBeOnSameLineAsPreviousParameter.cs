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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.SpacingRules;

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

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.OperatorDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorInitializerAction = HandleConstructorInitializer;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(BaseMethodDeclarationAction, BaseMethodDeclarationKinds);
            context.RegisterSyntaxNodeAction(LocalFunctionStatementAction, SyntaxKindEx.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeAction(ConstructorInitializerAction, SyntaxKinds.ConstructorInitializer);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreationExpression = (ArrayCreationExpressionSyntax)context.Node;

            if (arrayCreationExpression.Type == null)
            {
                return;
            }

            foreach (var arrayRankSpecifierSyntax in arrayCreationExpression.Type.RankSpecifiers)
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
            var parenthesizedLambdaExpression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            HandleBaseParameterListSyntax(context, parenthesizedLambdaExpression.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, delegateDeclaration.ParameterList);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethodExpression = (AnonymousMethodExpressionSyntax)context.Node;
            HandleBaseParameterListSyntax(context, anonymousMethodExpression.ParameterList);
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccessExpression = (ElementAccessExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, elementAccessExpression.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, indexerDeclaration.ParameterList);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, objectCreationExpression.ArgumentList);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationEpression = (InvocationExpressionSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, invocationEpression.ArgumentList);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var baseMethodDeclaration = (BaseMethodDeclarationSyntax)context.Node;
            HandleBaseParameterListSyntax(context, baseMethodDeclaration.ParameterList);
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntaxWrapper)context.Node;
            HandleBaseParameterListSyntax(context, localFunctionStatement.ParameterList);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;
            HandleBaseArgumentListSyntax(context, constructorInitializer.ArgumentList);
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
