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
    internal class SA1142ReferToTupleElementsByName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1142ReferToTupleElementsByName"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1142";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1142.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1142Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1142MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1142Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly Action<OperationAnalysisContext> FieldReferenceOperationAction = HandleFieldReferenceOperation;
        private static readonly Action<SyntaxNodeAnalysisContext> SimpleMemberAccessExpressionAction = HandleSimpleMemberAccessExpression;

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            if (LightupHelpers.SupportsIOperation)
            {
                context.RegisterOperationAction(FieldReferenceOperationAction, OperationKindEx.FieldReference);
            }
            else
            {
                context.RegisterSyntaxNodeAction(SimpleMemberAccessExpressionAction, SyntaxKind.SimpleMemberAccessExpression);
            }
        }

        private static void HandleFieldReferenceOperation(OperationAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var fieldReference = IFieldReferenceOperationWrapper.FromOperation(context.Operation);

            if (CheckFieldName(fieldReference.Field))
            {
                var location = fieldReference.WrappedOperation.Syntax is MemberAccessExpressionSyntax memberAccessExpression
                    ? memberAccessExpression.Name.GetLocation()
                    : fieldReference.WrappedOperation.Syntax.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }
        }

        private static void HandleSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;

            if (!(context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is IFieldSymbol fieldSymbol))
            {
                return;
            }

            if (CheckFieldName(fieldSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccessExpression.Name.GetLocation()));
            }
        }

        private static bool CheckFieldName(IFieldSymbol fieldSymbol)
        {
            if (!fieldSymbol.ContainingType.IsTupleType())
            {
                return false;
            }

            // check if this already is a proper tuple field name
            if (!Equals(fieldSymbol.CorrespondingTupleField(), fieldSymbol))
            {
                return false;
            }

            // check if there is a tuple field name declared.
            return fieldSymbol.ContainingType.GetMembers().OfType<IFieldSymbol>().Count(fs => Equals(fs.CorrespondingTupleField(), fieldSymbol)) > 1;
        }
    }
}
