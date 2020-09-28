// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    /// <para>The parameter should begin on the same line as the previous comma, or on the next line. For example:</para>
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
    internal class SA1115ParameterMustFollowComma : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1115ParameterMustFollowComma"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1115";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1115.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1115MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorInitializerAction = HandleConstructorInitializer;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementBindingExpressionAction = HandleElementBindingExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ImplicitElementAccessAction = HandleImplicitElementAccess;

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
            context.RegisterSyntaxNodeAction(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeAction(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(ConstructorInitializerAction, SyntaxKinds.ConstructorInitializer);
            context.RegisterSyntaxNodeAction(ElementBindingExpressionAction, SyntaxKind.ElementBindingExpression);
            context.RegisterSyntaxNodeAction(ImplicitElementAccessAction, SyntaxKind.ImplicitElementAccess);
        }

        private static void HandleImplicitElementAccess(SyntaxNodeAnalysisContext context)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;

            if (implicitElementAccess.ArgumentList != null)
            {
                AnalyzeSyntaxList(context, implicitElementAccess.ArgumentList.Arguments);
            }
        }

        private static void HandleElementBindingExpression(SyntaxNodeAnalysisContext context)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;
            AnalyzeSyntaxList(context, elementBinding.ArgumentList.Arguments);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;
            AnalyzeSyntaxList(context, constructorInitializer.ArgumentList.Arguments);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            AnalyzeSyntaxList(context, delegateDeclaration.ParameterList.Parameters);
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;
            AnalyzeSyntaxList(context, lambda.ParameterList.Parameters);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            AnalyzeSyntaxList(context, anonymousMethod.ParameterList?.Parameters ?? default);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;
            AnalyzeSyntaxList(context, attributeList.Attributes);
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            AnalyzeSyntaxList(context, attribute.ArgumentList?.Arguments ?? default);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;
            if (arrayCreation.Type == null)
            {
                return;
            }

            foreach (var rankSpecifier in arrayCreation.Type.RankSpecifiers)
            {
                AnalyzeSyntaxList(context, rankSpecifier.Sizes);
            }
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            if (elementAccess.ArgumentList != null)
            {
                AnalyzeSyntaxList(context, elementAccess.ArgumentList.Arguments);
            }
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.ParameterList != null)
            {
                AnalyzeSyntaxList(context, indexerDeclaration.ParameterList.Parameters);
            }
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreation.ArgumentList != null)
            {
                AnalyzeSyntaxList(context, objectCreation.ArgumentList.Arguments);
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.ArgumentList != null)
            {
                AnalyzeSyntaxList(context, invocation.ArgumentList.Arguments);
            }
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            if (constructorDeclaration.ParameterList != null)
            {
                AnalyzeSyntaxList(context, constructorDeclaration.ParameterList.Parameters);
            }
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntaxWrapper)context.Node;

            if (localFunctionStatement.ParameterList != null)
            {
                AnalyzeSyntaxList(context, localFunctionStatement.ParameterList.Parameters);
            }
        }

        private static void AnalyzeSyntaxList<TNode>(SyntaxNodeAnalysisContext context, SeparatedSyntaxList<TNode> syntaxList)
            where TNode : SyntaxNode
        {
            if (syntaxList.Count < 2)
            {
                return;
            }

            var previousItemEndLine = syntaxList[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < syntaxList.Count; i++)
            {
                var currentItem = syntaxList[i];
                FileLinePositionSpan currentSpan;

                if (currentItem.HasLeadingTrivia && IsValidTrivia(currentItem.GetLeadingTrivia()))
                {
                    currentSpan = currentItem.SyntaxTree.GetLineSpan(currentItem.FullSpan);
                }
                else
                {
                    currentSpan = currentItem.GetLineSpan();
                }

                int currentItemStartLine = currentSpan.StartLinePosition.Line;
                var currentItemEndLine = currentSpan.EndLinePosition.Line;

                if (currentItemStartLine - previousItemEndLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentItem.GetLocation()));
                }

                previousItemEndLine = currentItemEndLine;
            }
        }

        private static bool IsValidTrivia(SyntaxTriviaList triviaList)
        {
            var inBlankLine = true;

            for (var i = 0; i < triviaList.Count; i++)
            {
                var trivia = triviaList[i];

                if (trivia.IsDirective)
                {
                    inBlankLine = false;
                    continue;
                }

                switch (trivia.Kind())
                {
                case SyntaxKind.WhitespaceTrivia:
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    if (inBlankLine)
                    {
                        return false;
                    }

                    inBlankLine = true;
                    break;

                case SyntaxKind.DisabledTextTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    inBlankLine = false;
                    break;

                default:
                    return false;
                }
            }

            return true;
        }
    }
}
