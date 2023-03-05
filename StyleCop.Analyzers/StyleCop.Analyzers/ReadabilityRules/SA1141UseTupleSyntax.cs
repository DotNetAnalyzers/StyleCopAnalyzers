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

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1141UseTupleSyntax : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1141UseTupleSyntax"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1141";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1141.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1141Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1141MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1141Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConversionOperatorAction = HandleConversionOperator;
        private static readonly Action<SyntaxNodeAnalysisContext> PropertyDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> FieldDeclarationAction = HandleFieldDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> LambdaExpressionAction = HandleLambdaExpression;

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly SymbolDisplayFormat DisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(ConversionOperatorAction, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(PropertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(FieldDeclarationAction, SyntaxKind.FieldDeclaration);

            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(LambdaExpressionAction, SyntaxKinds.LambdaExpression);

            var expressionType = context.Compilation.GetTypeByMetadataName("System.Linq.Expressions.Expression`1");

            context.RegisterSyntaxNodeAction(context => HandleObjectCreationExpression(context, expressionType), SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(context => HandleInvocationExpression(context, expressionType), SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(context => HandleDefaultExpression(context, expressionType), SyntaxKind.DefaultExpression);

            context.RegisterSyntaxNodeAction(context => HandleCastExpression(context, expressionType), SyntaxKind.CastExpression);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            CheckType(context, expressionType: null, methodDeclaration.ReturnType);
            CheckParameterList(context, methodDeclaration.ParameterList);
        }

        private static void HandleConversionOperator(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            CheckType(context, expressionType: null, conversionOperatorDeclaration.Type);
            CheckParameterList(context, conversionOperatorDeclaration.ParameterList);
        }

        private static void HandleBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;
            CheckType(context, expressionType: null, propertyDeclaration.Type);
        }

        private static void HandleFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (BaseFieldDeclarationSyntax)context.Node;
            CheckType(context, expressionType: null, fieldDeclaration.Declaration.Type);
        }

        private static void HandleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambdaExpression = (LambdaExpressionSyntax)context.Node;
            if (lambdaExpression is ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpression)
            {
                CheckParameterList(context, parenthesizedLambdaExpression.ParameterList);
            }
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;
            CheckType(context, expressionType, objectCreationExpression.Type, objectCreationExpression.GetLocation());
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            if (invocationExpression.ArgumentList.Arguments.Count < 2)
            {
                // Tuple creation with less than two elements cannot use the language syntax
                return;
            }

            if (!invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                return;
            }

            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression);
            if (symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.ToDisplayString(DisplayFormat) == "System.ValueTuple"
                && !context.Node.IsInExpressionTree(context.SemanticModel, expressionType, context.CancellationToken))
            {
                if (memberAccessExpression.Name.Identifier.ValueText == "Create")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccessExpression.GetLocation()));
                }
            }
        }

        private static void HandleDefaultExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var defaultExpression = (DefaultExpressionSyntax)context.Node;
            CheckType(context, expressionType, defaultExpression.Type);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            CheckType(context, expressionType: null, delegateDeclaration.ReturnType);
            CheckParameterList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleCastExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var castExpression = (CastExpressionSyntax)context.Node;
            CheckType(context, expressionType, castExpression.Type);
        }

        private static void CheckParameterList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            foreach (var parameter in parameterList.Parameters)
            {
                CheckType(context, expressionType: null, parameter.Type);
            }
        }

        private static void CheckType(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType, TypeSyntax typeSyntax, Location reportLocation = null)
        {
            if (typeSyntax is null)
            {
                return;
            }

            switch (typeSyntax.Kind())
            {
            case SyntaxKindEx.TupleType:
                CheckTupleType(context, expressionType, (TupleTypeSyntaxWrapper)typeSyntax, reportLocation);
                break;

            case SyntaxKind.QualifiedName:
                CheckType(context, expressionType, ((QualifiedNameSyntax)typeSyntax).Right, reportLocation ?? typeSyntax.GetLocation());
                break;

            case SyntaxKind.GenericName:
                CheckGenericName(context, expressionType, (GenericNameSyntax)typeSyntax, reportLocation);
                break;
            }
        }

        private static void CheckTupleType(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType, TupleTypeSyntaxWrapper tupleTypeSyntax, Location reportLocation)
        {
            foreach (var tupleElementSyntax in tupleTypeSyntax.Elements)
            {
                CheckType(context, expressionType, tupleElementSyntax.Type, reportLocation);
            }
        }

        private static void CheckGenericName(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType, GenericNameSyntax genericNameSyntax, Location reportLocation)
        {
            if (IsValueTupleWithLanguageRepresentation(context, expressionType, genericNameSyntax))
            {
                var location = reportLocation ?? genericNameSyntax.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));

                // only report a single diagnostic for a type.
                return;
            }

            foreach (var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
            {
                CheckType(context, expressionType, typeArgument);
            }
        }

        private static bool IsValueTupleWithLanguageRepresentation(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType, ExpressionSyntax syntax)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(syntax, context.CancellationToken);
            return symbolInfo.Symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.IsTupleType()
                && typeSymbol.TupleElements().Length > 1
                && !syntax.IsInExpressionTree(context.SemanticModel, expressionType, context.CancellationToken);
        }
    }
}
