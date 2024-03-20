// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Field names within a tuple declaration should have the correct casing.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1316TupleElementNamesShouldUseCorrectCasing : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1316TupleElementNamesShouldUseCorrectCasing"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1316";

        /// <summary>
        /// The key used to signal the fixed tuple element name to the code fix.
        /// </summary>
        internal const string ExpectedTupleElementNameKey = "ExpectedTupleElementName";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1316.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1316Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1316MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1316Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TupleTypeAction = HandleTupleTypeAction;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TupleExpressionAction = HandleTupleExpressionAction;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartActionWithSettings(context =>
            {
                context.RegisterSyntaxNodeAction(TupleTypeAction, SyntaxKindEx.TupleType);
                context.RegisterSyntaxNodeAction(TupleExpressionAction, SyntaxKindEx.TupleExpression);
            });
        }

        private static void HandleTupleTypeAction(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!context.SupportsTuples())
            {
                return;
            }

            var tupleType = (TupleTypeSyntaxWrapper)context.Node;

            foreach (var tupleElement in tupleType.Elements)
            {
                CheckTupleElement(context, settings, tupleElement);
            }
        }

        private static void HandleTupleExpressionAction(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (!context.SupportsInferredTupleElementNames())
            {
                return;
            }

            if (!settings.NamingRules.IncludeInferredTupleElementNames)
            {
                return;
            }

            var tupleExpression = (TupleExpressionSyntaxWrapper)context.Node;
            foreach (var argument in tupleExpression.Arguments)
            {
                var inferredMemberName = SyntaxFactsEx.TryGetInferredMemberName(argument.NameColon?.Name ?? argument.Expression);
                if (inferredMemberName != null)
                {
                    CheckName(context, settings, inferredMemberName, argument.Expression.GetLocation(), false);
                }
            }
        }

        private static void CheckTupleElement(SyntaxNodeAnalysisContext context, StyleCopSettings settings, TupleElementSyntaxWrapper tupleElement)
        {
            if (tupleElement.Identifier == default)
            {
                return;
            }

            CheckName(context, settings, tupleElement.Identifier.ValueText, tupleElement.Identifier.GetLocation(), true);
        }

        private static void CheckName(SyntaxNodeAnalysisContext context, StyleCopSettings settings, string tupleElementName, Location location, bool prepareCodeFix)
        {
            if (tupleElementName == "_")
            {
                return;
            }

            var firstCharacterIsLower = char.IsLower(tupleElementName[0]);

            bool reportDiagnostic;
            string fixedName;

            switch (settings.NamingRules.TupleElementNameCasing)
            {
            case TupleElementNameCase.PascalCase:
                reportDiagnostic = firstCharacterIsLower;
                fixedName = char.ToUpper(tupleElementName[0]) + tupleElementName.Substring(1);
                break;

            default:
                reportDiagnostic = !firstCharacterIsLower;
                fixedName = char.ToLower(tupleElementName[0]) + tupleElementName.Substring(1);
                break;
            }

            if (reportDiagnostic)
            {
                var diagnosticProperties = ImmutableDictionary.CreateBuilder<string, string>();

                if (prepareCodeFix)
                {
                    diagnosticProperties.Add(ExpectedTupleElementNameKey, fixedName);
                }

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, diagnosticProperties.ToImmutableDictionary()));
            }
        }
    }
}
