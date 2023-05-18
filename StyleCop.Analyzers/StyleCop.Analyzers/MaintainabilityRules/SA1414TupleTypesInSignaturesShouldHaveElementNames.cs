// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [NoCodeFix("Cannot generate appropriate names.")]
    internal class SA1414TupleTypesInSignaturesShouldHaveElementNames : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1414TupleTypesInSignaturesShouldHaveElementNames"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1414";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1414.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(MaintainabilityResources.SA1414Title), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(MaintainabilityResources.SA1414MessageFormat), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(MaintainabilityResources.SA1414Description), MaintainabilityResources.ResourceManager, typeof(MaintainabilityResources));

        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = HandleMethodDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConstructorDeclarationAction = HandleConstructorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> PropertyDeclarationAction = HandlePropertyDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> IndexerDeclarationAction = HandleIndexerDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> ConversionOperatorDeclarationAction = HandleConversionOperatorDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext> DelegateDeclarationAction = HandleDelegateDeclaration;

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(ConstructorDeclarationAction, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(PropertyDeclarationAction, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(IndexerDeclarationAction, SyntaxKind.IndexerDeclaration);
            context.RegisterSyntaxNodeAction(ConversionOperatorDeclarationAction, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(DelegateDeclarationAction, SyntaxKind.DelegateDeclaration);
        }

        private static void HandleMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                return;
            }

            CheckType(context, methodDeclaration.ReturnType);
            CheckParameterList(context, methodDeclaration.ParameterList);
        }

        private static void HandleConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            CheckParameterList(context, constructorDeclaration.ParameterList);
        }

        private static void HandlePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            CheckType(context, propertyDeclaration.Type);
        }

        private static void HandleIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            CheckType(context, indexerDeclaration.Type);
        }

        private static void HandleConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var conversionOperatorDeclarion = (ConversionOperatorDeclarationSyntax)context.Node;

            CheckType(context, conversionOperatorDeclarion.Type);
            CheckParameterList(context, conversionOperatorDeclarion.ParameterList);
        }

        private static void HandleDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var delegateDeclarion = (DelegateDeclarationSyntax)context.Node;

            CheckType(context, delegateDeclarion.ReturnType);
            CheckParameterList(context, delegateDeclarion.ParameterList);
        }

        private static void CheckParameterList(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            foreach (var parameter in parameterList.Parameters)
            {
                CheckType(context, parameter.Type);
            }
        }

        private static void CheckType(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax)
        {
            switch (typeSyntax.Kind())
            {
            case SyntaxKindEx.TupleType:
                CheckTupleType(context, (TupleTypeSyntaxWrapper)typeSyntax);
                break;

            case SyntaxKind.QualifiedName:
                CheckType(context, ((QualifiedNameSyntax)typeSyntax).Right);
                break;

            case SyntaxKind.GenericName:
                CheckGenericName(context, (GenericNameSyntax)typeSyntax);
                break;
            }
        }

        private static void CheckTupleType(SyntaxNodeAnalysisContext context, TupleTypeSyntaxWrapper tupleTypeSyntax)
        {
            foreach (var tupleElementSyntax in tupleTypeSyntax.Elements)
            {
                CheckType(context, tupleElementSyntax.Type);

                if (tupleElementSyntax.Identifier.IsKind(SyntaxKind.None) && !NamedTypeHelpers.IsImplementingAnInterfaceMember(context.SemanticModel.GetDeclaredSymbol(context.Node)))
                {
                    var location = tupleElementSyntax.SyntaxNode.GetLocation();
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                }
            }
        }

        private static void CheckGenericName(SyntaxNodeAnalysisContext context, GenericNameSyntax genericNameSyntax)
        {
            foreach (var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
            {
                CheckType(context, typeArgument);
            }
        }
    }
}
