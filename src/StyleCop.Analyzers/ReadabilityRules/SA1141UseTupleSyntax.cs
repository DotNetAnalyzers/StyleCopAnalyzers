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

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1141UseTupleSyntax : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1141UseTupleSyntax"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1141";

        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
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

            var valueTupleType = context.Compilation.GetTypeByMetadataName("System.ValueTuple");
            context.RegisterSyntaxNodeAction(context => HandleObjectCreationExpression(context, expressionType), SyntaxKind.ObjectCreationExpression);
            context.RegisterSyntaxNodeAction(context => HandleInvocationExpression(context, valueTupleType, expressionType), SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(context => HandleDefaultExpression(context, expressionType), SyntaxKind.DefaultExpression);
            context.RegisterSyntaxNodeAction(context => HandleCastExpression(context, expressionType), SyntaxKind.CastExpression);
        }

        private static void CheckExpressionType(SyntaxNodeAnalysisContext context, INamedTypeSymbol? expressionType, TypeSyntax syntax, Location? reportLocation = null)
        {
            if (!LightupHelpers.SupportsIOperation)
            {
                CheckType(context, expressionType, syntax, reportLocation);
                return;
            }

            if (!ContainsValueTuple(syntax))
            {
                return;
            }

            var operation = context.SemanticModel.GetOperation(context.Node, context.CancellationToken);
            if (operation != null)
            {
                CheckOperationType(context, syntax, operation.Type, IsInExpressionTree(operation, expressionType), reportLocation);
            }
        }

        private static bool ContainsValueTuple(TypeSyntax syntax)
        {
            switch (syntax.Kind())
            {
            case SyntaxKind.QualifiedName:
                return ContainsValueTuple(((QualifiedNameSyntax)syntax).Right);

            case SyntaxKindEx.TupleType:
                foreach (var element in ((TupleTypeSyntaxWrapper)syntax).Elements)
                {
                    if (ContainsValueTuple(element.Type))
                    {
                        return true;
                    }
                }

                break;

            case SyntaxKind.GenericName:
                var genericName = (GenericNameSyntax)syntax;
                if (genericName.Identifier.ValueText == "ValueTuple")
                {
                    return true;
                }

                foreach (var argument in genericName.TypeArgumentList.Arguments)
                {
                    if (ContainsValueTuple(argument))
                    {
                        return true;
                    }
                }

                break;
            }

            return false;
        }

        private static void CheckOperationType(SyntaxNodeAnalysisContext context, TypeSyntax syntax, ITypeSymbol? type, bool isInExpressionTree, Location? reportLocation = null)
        {
            if (type is not INamedTypeSymbol namedType)
            {
                return;
            }

            switch (syntax.Kind())
            {
            case SyntaxKind.QualifiedName:
                CheckOperationType(context, ((QualifiedNameSyntax)syntax).Right, type, isInExpressionTree, reportLocation ?? syntax.GetLocation());
                break;

            case SyntaxKindEx.TupleType:
                var elements = ((TupleTypeSyntaxWrapper)syntax).Elements;
                var fields = namedType.TupleElements();
                for (var i = 0; i < elements.Count && i < fields.Length; i++)
                {
                    CheckOperationType(context, elements[i].Type, fields[i].Type, isInExpressionTree, reportLocation);
                }

                break;

            case SyntaxKind.GenericName:
                var genericName = (GenericNameSyntax)syntax;
                if (genericName.Identifier.ValueText == "ValueTuple"
                    && namedType.IsTupleType()
                    && namedType.TupleElements().Length > 1
                    && !isInExpressionTree)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, reportLocation ?? syntax.GetLocation()));
                    return;
                }

                var arguments = genericName.TypeArgumentList.Arguments;

                // Older compilers expose tuple type arguments only on the underlying ValueTuple type.
                var typeArguments = (namedType.TupleUnderlyingType() ?? namedType).TypeArguments;
                for (var i = 0; i < arguments.Count && i < typeArguments.Length; i++)
                {
                    CheckOperationType(context, arguments[i], typeArguments[i], isInExpressionTree);
                }

                break;
            }
        }

        private static bool IsInExpressionTree(IOperation operation, INamedTypeSymbol? expressionType)
        {
            if (expressionType is null)
            {
                return false;
            }

            for (var current = IOperationWrapper.FromOperation(operation); current.WrappedOperation != null; current = IOperationWrapper.FromOperation(current.Parent))
            {
                if (current.Kind == OperationKindEx.Conversion
                    && Equals(current.Type?.OriginalDefinition, expressionType)
                    && IConversionOperationWrapper.FromOperation(current.WrappedOperation).Operand.Kind == OperationKindEx.AnonymousFunction)
                {
                    return true;
                }
            }

            return false;
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
                var returnType = parenthesizedLambdaExpression.ReturnType();
                CheckType(context, expressionType: null, returnType);
                CheckParameterList(context, parenthesizedLambdaExpression.ParameterList);
            }
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;
            CheckExpressionType(context, expressionType, objectCreationExpression.Type, objectCreationExpression.GetLocation());
        }

        private static void HandleInvocationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol valueTupleType, INamedTypeSymbol expressionType)
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
            if (memberAccessExpression.Name.Identifier.ValueText == "Create")
            {
                if (LightupHelpers.SupportsIOperation)
                {
                    var operation = context.SemanticModel.GetOperation(invocationExpression, context.CancellationToken);
                    if (IInvocationOperationWrapper.IsInstance(operation)
                        && Equals(IInvocationOperationWrapper.FromOperation(operation).TargetMethod.ContainingType, valueTupleType)
                        && !IsInExpressionTree(operation, expressionType))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccessExpression.GetLocation()));
                    }

                    return;
                }

                var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Expression);
                if (symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol
                    && namedTypeSymbol.ToDisplayString(DisplayFormat) == "System.ValueTuple"
                    && !context.Node.IsInExpressionTree(context.SemanticModel, expressionType, context.CancellationToken))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccessExpression.GetLocation()));
                }
            }
        }

        private static void HandleDefaultExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionType)
        {
            var defaultExpression = (DefaultExpressionSyntax)context.Node;
            CheckExpressionType(context, expressionType, defaultExpression.Type);
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
            CheckExpressionType(context, expressionType, castExpression.Type);
        }

        private static void CheckParameterList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            foreach (var parameter in parameterList.Parameters)
            {
                CheckType(context, expressionType: null, parameter.Type);
            }
        }

        private static void CheckType(SyntaxNodeAnalysisContext context, INamedTypeSymbol? expressionType, TypeSyntax? typeSyntax, Location? reportLocation = null)
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

        private static void CheckTupleType(SyntaxNodeAnalysisContext context, INamedTypeSymbol? expressionType, TupleTypeSyntaxWrapper tupleTypeSyntax, Location? reportLocation)
        {
            foreach (var tupleElementSyntax in tupleTypeSyntax.Elements)
            {
                CheckType(context, expressionType, tupleElementSyntax.Type, reportLocation);
            }
        }

        private static void CheckGenericName(SyntaxNodeAnalysisContext context, INamedTypeSymbol? expressionType, GenericNameSyntax genericNameSyntax, Location? reportLocation)
        {
            if (genericNameSyntax.Identifier.ValueText == "ValueTuple"
                && IsValueTupleWithLanguageRepresentation(context, expressionType, genericNameSyntax))
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

        private static bool IsValueTupleWithLanguageRepresentation(SyntaxNodeAnalysisContext context, INamedTypeSymbol? expressionType, ExpressionSyntax syntax)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(syntax, context.CancellationToken);
            return symbolInfo.Symbol is INamedTypeSymbol typeSymbol
                && typeSymbol.IsTupleType()
                && typeSymbol.TupleElements().Length > 1
                && !syntax.IsInExpressionTree(context.SemanticModel, expressionType, context.CancellationToken);
        }
    }
}
