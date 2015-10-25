// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The start of the parameter list for a method or indexer call or declaration does not begin on the same line as
    /// the opening bracket, or on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when there are one or more blank lines between the opening bracket and the
    /// start of the parameter list. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameter list must begin on the same line as the opening bracket, or on the next line. For
    /// example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first, string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1114ParameterListMustFollowDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1114ParameterListMustFollowDeclaration"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1114";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1114Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1114MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1114Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1114.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.OperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeListAction = HandleAttributeList;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(BaseMethodDeclarationAction, BaseMethodDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (ParenthesizedLambdaExpressionSyntax)context.Node;
            AnalyzeParametersList(context, lambdaExpression.ParameterList);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            AnalyzeParametersList(context, anonymousMethod.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            AnalyzeParametersList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributesList = (AttributeListSyntax)context.Node;

            AnalyzeAttributeList(context, attributesList);
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            AnalyzeArgumentList(context, attribute.ArgumentList);
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;
            if (arrayCreation.Type == null)
            {
                return;
            }

            AnalyzeRankSpecifiers(context, arrayCreation);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            AnalyzeBracketParametersList(context, indexerDeclaration.ParameterList);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            if (objectCreation?.ArgumentList != null)
            {
                AnalyzeArgumentList(context, objectCreation.ArgumentList);
            }
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            AnalyzeArgumentList(context, invocationExpression.ArgumentList);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            AnalyzeParametersList(context, methodDeclaration.ParameterList);
        }

        private static void AnalyzeRankSpecifiers(SyntaxNodeAnalysisContext context, ArrayCreationExpressionSyntax arrayCreation)
        {
            if (!arrayCreation.Type.RankSpecifiers.Any())
            {
                return;
            }

            foreach (var arrayRankSpecifierSyntax in arrayCreation.Type.RankSpecifiers)
            {
                var openBracketToken = arrayRankSpecifierSyntax.OpenBracketToken;
                if (openBracketToken.IsMissing ||
                    arrayRankSpecifierSyntax.IsMissing ||
                    !arrayRankSpecifierSyntax.Sizes.Any())
                {
                    return;
                }

                var firstSize = arrayRankSpecifierSyntax.Sizes[0];

                var firstSizeLineSpan = firstSize.GetLineSpan();
                if (!firstSizeLineSpan.IsValid)
                {
                    return;
                }

                var openBracketLineSpan = openBracketToken.GetLineSpan();
                if (!openBracketLineSpan.IsValid)
                {
                    return;
                }

                if (openBracketLineSpan.EndLinePosition.Line != firstSizeLineSpan.StartLinePosition.Line &&
                    openBracketLineSpan.EndLinePosition.Line != (firstSizeLineSpan.StartLinePosition.Line - 1))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstSize.GetLocation()));
                }
            }
        }

        private static void AnalyzeAttributeList(SyntaxNodeAnalysisContext context, AttributeListSyntax attributesList)
        {
            var openBracketToken = attributesList.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                attributesList.IsMissing ||
                !attributesList.Attributes.Any())
            {
                return;
            }

            var firstAttribute = attributesList.Attributes[0];

            var firstAttributeLineSpan = firstAttribute.GetLineSpan();
            if (!firstAttributeLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstAttributeLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstAttributeLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstAttribute.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax argumentListSyntax)
        {
            var openBracketToken = argumentListSyntax.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, AttributeArgumentListSyntax argumentListSyntax)
        {
            if (argumentListSyntax == null ||
                argumentListSyntax.OpenParenToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = argumentListSyntax.OpenParenToken.GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentListSyntax)
        {
            var openParenToken = argumentListSyntax.OpenParenToken;
            if (openParenToken.IsMissing ||
                argumentListSyntax.IsMissing ||
                !argumentListSyntax.Arguments.Any())
            {
                return;
            }

            var firstArgument = argumentListSyntax.Arguments[0];
            if (firstArgument.GetLeadingTrivia().Any(SyntaxKind.PragmaWarningDirectiveTrivia))
            {
                return;
            }

            var firstArgumentLineSpan = firstArgument.GetLineSpan();
            if (!firstArgumentLineSpan.IsValid)
            {
                return;
            }

            var openParenLineSpan = openParenToken.GetLineSpan();
            if (!openParenLineSpan.IsValid)
            {
                return;
            }

            if (openParenLineSpan.EndLinePosition.Line != firstArgumentLineSpan.StartLinePosition.Line &&
                openParenLineSpan.EndLinePosition.Line != (firstArgumentLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstArgument.GetLocation()));
            }
        }

        private static void AnalyzeBracketParametersList(SyntaxNodeAnalysisContext context, BracketedParameterListSyntax parameterListSyntax)
        {
            var openBracketToken = parameterListSyntax.OpenBracketToken;
            if (openBracketToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];

            var firstParameterLineSpan = firstParameter.GetLineSpan();
            if (!firstParameterLineSpan.IsValid)
            {
                return;
            }

            var openBracketLineSpan = openBracketToken.GetLineSpan();
            if (!openBracketLineSpan.IsValid)
            {
                return;
            }

            if (openBracketLineSpan.EndLinePosition.Line != firstParameterLineSpan.StartLinePosition.Line &&
                openBracketLineSpan.EndLinePosition.Line != (firstParameterLineSpan.StartLinePosition.Line - 1))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
            }
        }

        private static void AnalyzeParametersList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null ||
                parameterListSyntax.OpenParenToken.IsMissing ||
                parameterListSyntax.IsMissing ||
                !parameterListSyntax.Parameters.Any())
            {
                return;
            }

            var firstParameter = parameterListSyntax.Parameters[0];
            int firstParameterLine;

            if (firstParameter.HasLeadingTrivia && firstParameter.GetLeadingTrivia().All(trivia => IsValidTrivia(trivia)))
            {
                firstParameterLine = firstParameter.SyntaxTree.GetLineSpan(firstParameter.FullSpan).StartLinePosition.Line;
            }
            else
            {
                firstParameterLine = firstParameter.GetLineSpan().StartLinePosition.Line;
            }

            var parenLine = parameterListSyntax.OpenParenToken.GetLineSpan().EndLinePosition.Line;

            if ((firstParameterLine - parenLine) > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
            }
        }

        private static bool IsValidTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
            case SyntaxKind.IfDirectiveTrivia:
            case SyntaxKind.ElseDirectiveTrivia:
            case SyntaxKind.ElifDirectiveTrivia:
            case SyntaxKind.EndIfDirectiveTrivia:
            case SyntaxKind.DisabledTextTrivia:
            case SyntaxKind.WhitespaceTrivia:
                return true;

            default:
                return false;
            }
        }
    }
}
