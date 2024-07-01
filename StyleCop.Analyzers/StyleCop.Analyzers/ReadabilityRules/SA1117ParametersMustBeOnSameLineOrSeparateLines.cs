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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// The parameters to a C# method or indexer call or declaration are not all on the same line or each on a separate
    /// line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the parameters to a method or indexer are not all on the same line or
    /// each on its own line. For example:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string middle,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// <para>The parameters can all be placed on the same line:</para>
    /// <code language="csharp">
    /// public string JoinName(string first, string middle, string last)
    /// {
    /// }
    ///
    /// public string JoinName(
    ///     string first, string middle, string last)
    /// {
    /// }
    /// </code>
    /// <para>Alternatively, each parameter can be placed on its own line:</para>
    /// <code language="csharp">
    /// public string JoinName(
    ///     string first,
    ///     string middle,
    ///     string last)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1117ParametersMustBeOnSameLineOrSeparateLines : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1117ParametersMustBeOnSameLineOrSeparateLines"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1117";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1117.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1117Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1117MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1117Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> BaseMethodDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseMethodDeclarationAction = HandleBaseMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> LocalFunctionStatementAction = HandleLocalFunctionStatement;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ConstructorInitializerAction = HandleConstructorInitializer;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ImplicitObjectCreationExpressionAction = HandleImplicitObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ElementAccessExpressionAction = HandleElementAccessExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ElementBindingExpressionAction = HandleElementBindingExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(
                context =>
                {
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
                    context.RegisterSyntaxNodeAction(ArrayCreationExpressionAction, SyntaxKind.ArrayCreationExpression);
                    context.RegisterSyntaxNodeAction(AttributeAction, SyntaxKind.Attribute);
                    context.RegisterSyntaxNodeAction(AnonymousMethodExpressionAction, SyntaxKind.AnonymousMethodExpression);
                    context.RegisterSyntaxNodeAction(ParenthesizedLambdaExpressionAction, SyntaxKind.ParenthesizedLambdaExpression);
                });
        }

        private static void HandleBaseMethodDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var declaration = (BaseMethodDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, declaration.ParameterList, settings);
        }

        private static void HandleLocalFunctionStatement(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var statement = (LocalFunctionStatementSyntaxWrapper)context.Node;
            HandleParameterListSyntax(context, statement.ParameterList, settings);
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, invocation.ArgumentList, settings);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
            HandleArgumentListSyntax(context, objectCreation.ArgumentList, settings);
        }

        private static void HandleImplicitObjectCreationExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntaxWrapper)context.Node;
            HandleArgumentListSyntax(context, implicitObjectCreation.ArgumentList, settings);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;
            BracketedParameterListSyntax argumentListSyntax = indexerDeclaration.ParameterList;
            SeparatedSyntaxList<ParameterSyntax> arguments = argumentListSyntax.Parameters;
            Analyze(context, arguments, settings);
        }

        private static void HandleElementAccessExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementAccess.ArgumentList, settings);
        }

        private static void HandleArrayCreationExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

            foreach (var rankSpecifier in arrayCreation.Type.RankSpecifiers)
            {
                SeparatedSyntaxList<ExpressionSyntax> sizes = rankSpecifier.Sizes;
                Analyze(context, sizes, settings);
            }
        }

        private static void HandleAttribute(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var attribute = (AttributeSyntax)context.Node;
            AttributeArgumentListSyntax argumentListSyntax = attribute.ArgumentList;
            if (argumentListSyntax == null)
            {
                return;
            }

            SeparatedSyntaxList<AttributeArgumentSyntax> arguments = argumentListSyntax.Arguments;
            Analyze(context, arguments, settings);
        }

        private static void HandleAnonymousMethodExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, anonymousMethod.ParameterList, settings);
        }

        private static void HandleParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)context.Node;
            HandleParameterListSyntax(context, parenthesizedLambda.ParameterList, settings);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            HandleParameterListSyntax(context, delegateDeclaration.ParameterList, settings);
        }

        private static void HandleConstructorInitializer(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var constructorInitializer = (ConstructorInitializerSyntax)context.Node;
            HandleArgumentListSyntax(context, constructorInitializer.ArgumentList, settings);
        }

        private static void HandleElementBindingExpression(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementBinding = (ElementBindingExpressionSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, elementBinding.ArgumentList, settings);
        }

        private static void HandleArgumentListSyntax(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList, StyleCopSettings settings)
        {
            if (argumentList == null)
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
            Analyze(context, arguments, settings);
        }

        private static void HandleParameterListSyntax(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList, StyleCopSettings settings)
        {
            if (parameterList == null)
            {
                return;
            }

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
            Analyze(context, parameters, settings);
        }

        private static void HandleBracketedArgumentListSyntax(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax bracketedArgumentList, StyleCopSettings settings)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = bracketedArgumentList.Arguments;
            Analyze(context, arguments, settings);
        }

        private static void Analyze<T>(SyntaxNodeAnalysisContext context, SeparatedSyntaxList<T> arguments, StyleCopSettings settings)
            where T : SyntaxNode
        {
            if (arguments.Count < 2)
            {
                return;
            }

            SyntaxNode firstParameter = arguments[0];
            SyntaxNode secondParameter = arguments[1];

            Func<SyntaxNode, SyntaxNode, bool> lineCondition;
            if (settings.ReadabilityRules.TreatMultilineParametersAsSplit)
            {
                lineCondition = firstParameter.GetLine() == secondParameter.GetEndLine()
                    ? (param1, param2) => param1.GetLine() == param2.GetEndLine() // Arguments should be on same line.
                    : (param1, param2) => param1.GetEndLine() != param2.GetLine(); // Each argument should be on its own line.
            }
            else
            {
                lineCondition = firstParameter.GetLine() == secondParameter.GetLine()
                    ? (param1, param2) => param1.GetLine() == param2.GetLine() // Arguments should be on same line.
                    : (param1, param2) => param1.GetEndLine() != param2.GetLine(); // Each argument should be on its own line.
            }

            SyntaxNode previousParameter = firstParameter;
            for (int i = 1; i < arguments.Count; ++i)
            {
                SyntaxNode currentParameter = arguments[i];
                if (lineCondition(previousParameter, currentParameter))
                {
                    previousParameter = currentParameter;
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.GetLocation()));
                return;
            }
        }
    }
}
