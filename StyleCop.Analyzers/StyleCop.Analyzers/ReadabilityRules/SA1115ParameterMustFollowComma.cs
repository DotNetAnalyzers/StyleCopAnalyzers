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
    /// <para>The parameter must begin on the same line as the previous comma, or on the next line. For example:</para>
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
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1115MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1115Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1115.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration);

        private static readonly ImmutableArray<SyntaxKind> ConstructorInitializerKinds =
            ImmutableArray.Create(SyntaxKind.BaseConstructorInitializer, SyntaxKind.ThisConstructorInitializer);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
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
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(BaseMethodDeclarationAction, BaseMethodDeclarationKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeActionHonorExclusions(AttributeListAction, SyntaxKind.AttributeList);
            context.RegisterSyntaxNodeActionHonorExclusions(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeActionHonorExclusions(ConstructorInitializerAction, ConstructorInitializerKinds);
            context.RegisterSyntaxNodeActionHonorExclusions(ElementBindingExpressionAction, SyntaxKind.ElementBindingExpression);
            context.RegisterSyntaxNodeActionHonorExclusions(ImplicitElementAccessAction, SyntaxKind.ImplicitElementAccess);
        }

        private static void HandleImplicitElementAccess(SyntaxNodeAnalysisContext context)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;

            AnalyzeArgumentList(context, implicitElementAccess.ArgumentList);
        }

        private static void HandleElementBindingExpression(SyntaxNodeAnalysisContext context)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, elementBinding.ArgumentList);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;

            AnalyzeArgumentList(context, constructorInitializer.ArgumentList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            AnalyzeParameterList(context, lambda.ParameterList);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            AnalyzeParameterList(context, anonymousMethod.ParameterList);
        }

        private static void HandleAttributeList(SyntaxNodeAnalysisContext context)
        {
            var attributeList = (AttributeListSyntax)context.Node;

            if (attributeList.Attributes.Count < 2)
            {
                return;
            }

            var previousLine = attributeList.Attributes[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < attributeList.Attributes.Count; i++)
            {
                var currentAttribute = attributeList.Attributes[i];
                var lineSpan = currentAttribute.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentAttribute.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            if (attribute.ArgumentList == null
                || attribute.ArgumentList.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = attribute.ArgumentList.Arguments[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < attribute.ArgumentList.Arguments.Count; i++)
            {
                var currentArgument = attribute.ArgumentList.Arguments[i];
                var lineSpan = currentArgument.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
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
                if (rankSpecifier.Sizes.Count < 2)
                {
                    continue;
                }

                var previousLine = rankSpecifier.Sizes[0].GetLineSpan().EndLinePosition.Line;
                for (int i = 1; i < rankSpecifier.Sizes.Count; i++)
                {
                    var currentSize = rankSpecifier.Sizes[i];
                    var lineSpan = currentSize.GetLineSpan();
                    var currentLine = lineSpan.StartLinePosition.Line;
                    if (currentLine - previousLine > 1)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentSize.GetLocation()));
                    }

                    previousLine = lineSpan.EndLinePosition.Line;
                }
            }
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, elementAccess.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, indexerDeclaration.ParameterList);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, objectCreation.ArgumentList);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            AnalyzeArgumentList(context, invocation.ArgumentList);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (BaseMethodDeclarationSyntax)context.Node;

            AnalyzeParameterList(context, constructorDeclaration.ParameterList);
        }

        private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context, BaseArgumentListSyntax argumentListSyntax)
        {
            if (argumentListSyntax == null
                || argumentListSyntax.Arguments.Count < 2)
            {
                return;
            }

            var previousLine = argumentListSyntax.Arguments[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < argumentListSyntax.Arguments.Count; i++)
            {
                var currentArgument = argumentListSyntax.Arguments[i];
                var lineSpan = currentArgument.GetLineSpan();
                var currentLine = lineSpan.StartLinePosition.Line;
                if (currentLine - previousLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentArgument.GetLocation()));
                }

                previousLine = lineSpan.EndLinePosition.Line;
            }
        }

        private static void AnalyzeParameterList(SyntaxNodeAnalysisContext context, BaseParameterListSyntax parameterListSyntax)
        {
            if (parameterListSyntax == null
                || parameterListSyntax.Parameters.Count < 2)
            {
                return;
            }

            var previousParameterLine = parameterListSyntax.Parameters[0].GetLineSpan().EndLinePosition.Line;
            for (int i = 1; i < parameterListSyntax.Parameters.Count; i++)
            {
                var currentParameter = parameterListSyntax.Parameters[i];
                int currentParameterLine;

                if (currentParameter.HasLeadingTrivia && currentParameter.GetLeadingTrivia().All(trivia => IsValidTrivia(trivia)))
                {
                    currentParameterLine = currentParameter.SyntaxTree.GetLineSpan(currentParameter.FullSpan).StartLinePosition.Line;
                }
                else
                {
                    currentParameterLine = currentParameter.GetLineSpan().StartLinePosition.Line;
                }

                if (currentParameterLine - previousParameterLine > 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.GetLocation()));
                }

                previousParameterLine = currentParameterLine;
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
