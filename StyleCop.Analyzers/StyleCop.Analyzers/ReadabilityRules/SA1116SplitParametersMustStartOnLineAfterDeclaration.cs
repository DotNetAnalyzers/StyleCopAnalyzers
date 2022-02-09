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

    /// <summary>
    /// The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter
    /// does not start on the line after the opening bracket.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the parameters to a method or indexer span across multiple lines, but
    /// the first parameter does not start on the line after the opening bracket. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameters should begin on the line after the declaration, whenever the parameter span across multiple
    /// lines:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1116SplitParametersMustStartOnLineAfterDeclaration : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1116SplitParametersMustStartOnLineAfterDeclaration"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1116";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1116.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1116Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1116MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1116Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.OperatorDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorInitializerAction = HandleConstructorInitializer;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ImplicitObjectCreationExpressionAction = HandleImplicitObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ElementBindingExpressionAction = HandleElementBindingExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ImplicitElementAccessAction = HandleImplicitElementAccess;
        private static readonly Action<SyntaxNodeAnalysisContext> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;

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
            context.RegisterSyntaxNodeAction(ConstructorInitializerAction, SyntaxKinds.ConstructorInitializer);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(ImplicitObjectCreationExpressionAction, SyntaxKindEx.ImplicitObjectCreationExpression);
            context.RegisterSyntaxNodeAction(ElementAccessExpressionAction, SyntaxKind.ElementAccessExpression);
            context.RegisterSyntaxNodeAction(ElementBindingExpressionAction, SyntaxKind.ElementBindingExpression);
            context.RegisterSyntaxNodeAction(ImplicitElementAccessAction, SyntaxKind.ImplicitElementAccess);
            context.RegisterSyntaxNodeAction(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
            context.RegisterSyntaxNodeAction(AttributeAction, SyntaxKind.Attribute);
            context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (BaseMethodDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, declaration.ParameterList);
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var statement = (LocalFunctionStatementSyntaxWrapper)context.Node;
            HandleParameterListSyntax(context, statement.ParameterList);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, invocation.ArgumentList);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, objectCreation.ArgumentList);
        }

        private static void HandleImplicitObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntaxWrapper)context.Node;
            HandleArgumentListSyntax(context, implicitObjectCreation.ArgumentList);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            BracketedParameterListSyntax argumentListSyntax = indexerDeclaration.ParameterList;
            SeparatedSyntaxList<ParameterSyntax> arguments = argumentListSyntax.Parameters;

            if (arguments.Count > 1)
            {
                Analyze(context, argumentListSyntax.OpenBracketToken, arguments[0], arguments[1]);
            }
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementAccess.ArgumentList);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            foreach (var rankSpecifier in arrayCreation.Type.RankSpecifiers)
            {
                SeparatedSyntaxList<ExpressionSyntax> sizes = rankSpecifier.Sizes;
                if (sizes.Count > 1)
                {
                    Analyze(context, rankSpecifier.OpenBracketToken, sizes[0], sizes[1]);
                }
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            AttributeArgumentListSyntax argumentListSyntax = attribute.ArgumentList;
            if (argumentListSyntax == null)
            {
                return;
            }

            SeparatedSyntaxList<AttributeArgumentSyntax> arguments = argumentListSyntax.Arguments;
            if (arguments.Count > 1)
            {
                Analyze(context, argumentListSyntax.OpenParenToken, arguments[0], arguments[1]);
            }
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, anonymousMethod.ParameterList);
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, parenthesizedLambda.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, delegateDeclaration.ParameterList);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;
            HandleArgumentListSyntax(context, constructorInitializer.ArgumentList);
        }

        private static void HandleElementBindingExpression(SyntaxNodeAnalysisContext context)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementBinding.ArgumentList);
        }

        private static void HandleImplicitElementAccess(SyntaxNodeAnalysisContext context)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, implicitElementAccess.ArgumentList);
        }

        private static void HandleArgumentListSyntax(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList)
        {
            if (argumentList == null)
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> parameters = argumentList.Arguments;

            if (parameters.Count > 1)
            {
                Analyze(context, argumentList.OpenParenToken, parameters[0], parameters[1]);
            }
        }

        private static void HandleParameterListSyntax(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            if (parameterList == null)
            {
                return;
            }

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
            if (parameters.Count > 1)
            {
                Analyze(context, parameterList.OpenParenToken, parameters[0], parameters[1]);
            }
        }

        private static void HandleBracketedArgumentListSyntax(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax bracketedArgumentList)
        {
            SeparatedSyntaxList<ArgumentSyntax> parameters = bracketedArgumentList.Arguments;

            if (parameters.Count > 1)
            {
                Analyze(context, bracketedArgumentList.OpenBracketToken, parameters[0], parameters[1]);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxToken openParenOrBracketToken,
            SyntaxNode firstParameter,
            SyntaxNode secondParameter)
        {
            int firstParameterLine = firstParameter.GetLineSpan().StartLinePosition.Line;
            if (openParenOrBracketToken.GetLine() == firstParameterLine)
            {
                if (firstParameterLine != secondParameter.GetLineSpan().StartLinePosition.Line)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
                }
            }
        }
    }
}
