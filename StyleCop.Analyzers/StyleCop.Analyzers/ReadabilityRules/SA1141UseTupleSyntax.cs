// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Text;
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

        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConversionOperatorAction = HandleConversionOperator;
        private static readonly Action<SyntaxNodeAnalysisContext> PropertyDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleBasePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> InvocationExpressionAction = HandleInvocationExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DefaultExpressionAction = HandleDefaultExpression;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> CastExpressionAction = HandleCastExpression;

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly SymbolDisplayFormat DisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(ConversionOperatorAction, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(PropertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);

            context.RegisterSyntaxNodeAction(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(InvocationExpressionAction, SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(DefaultExpressionAction, SyntaxKind.DefaultExpression);

            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);

            context.RegisterSyntaxNodeAction(CastExpressionAction, SyntaxKind.CastExpression);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            CheckType(context, methodDeclaration.ReturnType);
            CheckParameterList(context, methodDeclaration.ParameterList);
        }

        private static void HandleConversionOperator(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            CheckType(context, conversionOperatorDeclaration.Type);
            CheckParameterList(context, conversionOperatorDeclaration.ParameterList);
        }

        private static void HandleBasePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var propertyDeclaration = (BasePropertyDeclarationSyntax)context.Node;
            CheckType(context, propertyDeclaration.Type);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;
            CheckType(context, objectCreationExpression.Type, objectCreationExpression.GetLocation());
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

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
            if ((symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol)
                && (namedTypeSymbol.ToDisplayString(DisplayFormat) == "System.ValueTuple"))
            {
                if (memberAccessExpression.Name.Identifier.ValueText == "Create")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccessExpression.GetLocation()));
                }
            }
        }

        private static void HandleDefaultExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var defaultExpression = (DefaultExpressionSyntax)context.Node;
            CheckType(context, defaultExpression.Type);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;
            CheckType(context, delegateDeclaration.ReturnType);
            CheckParameterList(context, delegateDeclaration.ParameterList);
        }

        private static void HandleCastExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var castExpression = (CastExpressionSyntax)context.Node;
            CheckType(context, castExpression.Type);
        }

        private static void CheckParameterList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            foreach (var parameter in parameterList.Parameters)
            {
                CheckType(context, parameter.Type);
            }
        }

        private static void CheckType(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax, Location reportLocation = null)
        {
            switch (typeSyntax.Kind())
            {
            case SyntaxKindEx.TupleType:
                CheckTupleType(context, (TupleTypeSyntaxWrapper)typeSyntax, reportLocation);
                break;

            case SyntaxKind.QualifiedName:
                CheckType(context, ((QualifiedNameSyntax)typeSyntax).Right, reportLocation ?? typeSyntax.GetLocation());
                break;

            case SyntaxKind.GenericName:
                CheckGenericName(context, (GenericNameSyntax)typeSyntax, reportLocation);
                break;
            }
        }

        private static void CheckTupleType(SyntaxNodeAnalysisContext context, TupleTypeSyntaxWrapper tupleTypeSyntax, Location reportLocation)
        {
            foreach (var tupleElementSyntax in tupleTypeSyntax.Elements)
            {
                CheckType(context, tupleElementSyntax.Type, reportLocation);
            }
        }

        private static void CheckGenericName(SyntaxNodeAnalysisContext context, GenericNameSyntax genericNameSyntax, Location reportLocation)
        {
            if (IsValueTupleWithLanguageRepresentation(context, genericNameSyntax))
            {
                var location = reportLocation ?? genericNameSyntax.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));

                // only report a single diagnostic for a type.
                return;
            }

            foreach (var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
            {
                CheckType(context, typeArgument);
            }
        }

        private static bool IsValueTupleWithLanguageRepresentation(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(syntax, context.CancellationToken);
            return symbolInfo.Symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.IsTupleType()
                && typeSymbol.TupleElements().Length > 1;
        }
    }
}
