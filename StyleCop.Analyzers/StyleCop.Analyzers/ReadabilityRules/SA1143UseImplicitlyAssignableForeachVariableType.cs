// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    using StyleCop.Analyzers.Lightup;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1143UseImplicitlyAssignableForeachVariableType : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "SA1143";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1143.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1143Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1143MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1143Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor = new(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(context =>
            {
                var wellKnownTypeProvider = WellKnownTypeProvider.GetOrCreate(context.Compilation);

                INamedTypeSymbol? genericIEnumerableType = wellKnownTypeProvider.GetOrCreateTypeByMetadataName(WellKnownTypeNames.SystemCollectionsGenericIEnumerable1);
                if (genericIEnumerableType == null)
                {
                    return;
                }

                context.RegisterOperationAction(context => AnalyzeLoop(context, genericIEnumerableType), OperationKind.LoopStatement);
            });
        }

        private void AnalyzeLoop(OperationAnalysisContext context, INamedTypeSymbol genericIEnumerableType)
        {
            if (context.Operation is not IForEachLoopOperation loop ||
                loop.Syntax is not ForEachStatementSyntax syntax)
            {
                return;
            }

            ForEachStatementInfo loopInfo = loop.SemanticModel.GetForEachStatementInfo(syntax);
            if (!loopInfo.CurrentConversion.Exists ||
                !loopInfo.CurrentConversion.IsImplicit ||
                !loopInfo.CurrentConversion.IsIdentity)
            {
                return;
            }

            ITypeSymbol collectionElementType = loopInfo.ElementType;
            if (collectionElementType.SpecialType == SpecialType.System_Object &&
                !loop.Collection.Type.DerivesFrom(genericIEnumerableType))
            {
                return;
            }

            ITypeSymbol variableType = (loop.LoopControlVariable as IVariableDeclaratorOperation)?.Symbol?.Type;
            if (variableType == null)
            {
                return;
            }

            CommonConversion conversion = context.Compilation.ClassifyCommonConversion(collectionElementType, variableType);
            if (!conversion.Exists || conversion.IsImplicit)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, syntax.ForEachKeyword.GetLocation(), collectionElementType.Name, variableType.Name));
        }
    }
}
