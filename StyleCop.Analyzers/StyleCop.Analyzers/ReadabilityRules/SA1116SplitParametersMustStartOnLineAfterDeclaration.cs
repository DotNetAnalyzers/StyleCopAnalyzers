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
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ImplicitElementAccessAction = HandleImplicitElementAccess;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ArrayCreationExpressionAction = HandleArrayCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> AttributeAction = HandleAttribute;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> AnonymousMethodExpressionAction = HandleAnonymousMethodExpression;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> ParenthesizedLambdaExpressionAction = HandleParenthesizedLambdaExpression;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

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
                    context.RegisterSyntaxNodeAction(ImplicitElementAccessAction, SyntaxKind.ImplicitElementAccess);
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
            Analyze(context, argumentListSyntax.OpenBracketToken, arguments, settings);
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
                Analyze(context, rankSpecifier.OpenBracketToken, sizes, settings);
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
            Analyze(context, argumentListSyntax.OpenParenToken, arguments, settings);
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

        private static void HandleImplicitElementAccess(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var implicitElementAccess = (ImplicitElementAccessSyntax)context.Node;
            HandleBracketedArgumentListSyntax(context, implicitElementAccess.ArgumentList, settings);
        }

        private static void HandleArgumentListSyntax(SyntaxNodeAnalysisContext context, ArgumentListSyntax argumentList, StyleCopSettings settings)
        {
            if (argumentList == null)
            {
                return;
            }

            SeparatedSyntaxList<ArgumentSyntax> parameters = argumentList.Arguments;
            Analyze(context, argumentList.OpenParenToken, parameters, settings);
        }

        private static void HandleParameterListSyntax(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList, StyleCopSettings settings)
        {
            if (parameterList == null)
            {
                return;
            }

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
            Analyze(context, parameterList.OpenParenToken, parameters, settings);
        }

        private static void HandleBracketedArgumentListSyntax(SyntaxNodeAnalysisContext context, BracketedArgumentListSyntax bracketedArgumentList, StyleCopSettings settings)
        {
            SeparatedSyntaxList<ArgumentSyntax> parameters = bracketedArgumentList.Arguments;
            Analyze(context, bracketedArgumentList.OpenBracketToken, parameters, settings);
        }

        private static void Analyze<T>(SyntaxNodeAnalysisContext context, SyntaxToken openParenOrBracketToken, SeparatedSyntaxList<T> arguments, StyleCopSettings settings)
            where T : SyntaxNode
        {
            var minimumArgumentCount = settings.ReadabilityRules.TreatMultilineParametersAsSplit ? 1 : 2;
            if (arguments.Count < minimumArgumentCount)
            {
                return;
            }

            SyntaxNode firstParameter = arguments.First();
            int startLine = firstParameter.GetLine();
            if (startLine == openParenOrBracketToken.GetLine())
            {
                var endLine = settings.ReadabilityRules.TreatMultilineParametersAsSplit ? arguments.Last().GetEndLine() : arguments[1].GetLine();
                if (startLine != endLine)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, firstParameter.GetLocation()));
                }
            }
        }
    }
}
