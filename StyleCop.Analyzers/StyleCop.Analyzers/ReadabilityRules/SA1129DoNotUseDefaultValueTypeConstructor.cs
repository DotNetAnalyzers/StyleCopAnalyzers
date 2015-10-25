// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A value type was constructed using the syntax <c>new T()</c>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1129DoNotUseDefaultValueTypeConstructor : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1129DoNotUseDefaultValueTypeConstructor"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1129";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1129Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1129MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1129Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1129.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> ObjectCreationExpressionAction = HandleObjectCreationExpression;

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
            context.RegisterSyntaxNodeActionHonorExclusions(ObjectCreationExpressionAction, SyntaxKind.ObjectCreationExpression);
        }

        private static void HandleObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            ObjectCreationExpressionSyntax newExpression = (ObjectCreationExpressionSyntax)context.Node;

            var typeToCreate = context.SemanticModel.GetTypeInfo(newExpression, context.CancellationToken);
            if (typeToCreate.Type.IsReferenceType || IsReferenceTypeParameter(typeToCreate.Type))
            {
                return;
            }

            if ((newExpression.ArgumentList == null) || (newExpression.ArgumentList.Arguments.Count > 0))
            {
                return;
            }

            if (newExpression.Initializer != null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, newExpression.GetLocation()));
        }

        private static bool IsReferenceTypeParameter(ITypeSymbol type)
        {
            return (type.Kind == SymbolKind.TypeParameter) && !((ITypeParameterSymbol)type).HasValueTypeConstraint;
        }
    }
}
