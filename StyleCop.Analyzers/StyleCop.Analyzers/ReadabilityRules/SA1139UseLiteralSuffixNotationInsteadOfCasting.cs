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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A cast is performed instead of using literal of a number. Use "U" suffix to create 32-bit unsigned integer,
    /// "L" for 64-bit integer, "UL" for 64-bit unsigned integer, "F" for 32-bit floating point number, "D" for 64-bit
    /// floating point number, and "M" for a decimal number.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1139UseLiteralSuffixNotationInsteadOfCasting : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1139UseLiteralSuffixNotationInsteadOfCasting"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1139";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1139Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1139MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1139Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1139.md";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> GenericNameAction = HandleGenericName;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(GenericNameAction, SyntaxKind.CastExpression);
        }

        private static void HandleGenericName(SyntaxNodeAnalysisContext context)
        {
            var castExpressionSyntax = (CastExpressionSyntax)context.Node;

            var castingToTypeSyntax = castExpressionSyntax.Type as PredefinedTypeSyntax;
            if (castingToTypeSyntax == null)
            {
                return;
            }

            var unaryExpressionSyntax = castExpressionSyntax.Expression as PrefixUnaryExpressionSyntax;
            if (unaryExpressionSyntax != null)
            {
                if (unaryExpressionSyntax.Kind() != SyntaxKind.UnaryPlusExpression
                    && unaryExpressionSyntax.Kind() != SyntaxKind.UnaryMinusExpression)
                {
                    // don't report diagnostic if bit operations are performed and for some invalid code (eg. "(long)++1")
                    return;
                }
            }

            var castedElementTypeSyntax = unaryExpressionSyntax == null
                ? castExpressionSyntax.Expression as LiteralExpressionSyntax
                : unaryExpressionSyntax.Operand as LiteralExpressionSyntax;

            if (castedElementTypeSyntax == null)
            {
                return;
            }

            var syntaxKindKeyword = castingToTypeSyntax.Keyword.Kind();
            if (!SyntaxKinds.IntegerLiteralKeyword.Contains(syntaxKindKeyword)
                && !SyntaxKinds.RealLiteralKeyword.Contains(syntaxKindKeyword))
            {
                return;
            }

            var castedToken = castedElementTypeSyntax.Token;
            if (!castedToken.IsKind(SyntaxKind.NumericLiteralToken))
            {
                return;
            }

            if (context.SemanticModel.GetTypeInfo(castedElementTypeSyntax).Type
                == context.SemanticModel.GetTypeInfo(castExpressionSyntax).Type)
            {
                // cast is redundant which is reported by another diagnostic.
                return;
            }

            if (!context.SemanticModel.GetConstantValue(context.Node).HasValue)
            {
                // cast does not have a valid value (like "(ulong)-1") which is reported as error
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, castExpressionSyntax.GetLocation()));
        }
    }
}
