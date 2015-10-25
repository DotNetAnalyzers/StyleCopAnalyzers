// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A nullable type symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a nullable type symbol is not correct.</para>
    ///
    /// <para>A nullable type symbol should never be preceded by whitespace.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1018NullableTypeSymbolsMustNotBePrecededBySpace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1018";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1018Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1018MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1018Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1018.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> NullableTypeAction = HandleNullableType;

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
            context.RegisterSyntaxNodeActionHonorExclusions(NullableTypeAction, SyntaxKind.NullableType);
        }

        private static void HandleNullableType(SyntaxNodeAnalysisContext context)
        {
            var nullableType = (NullableTypeSyntax)context.Node;
            var questionToken = nullableType.QuestionToken;

            if (questionToken.IsMissing)
            {
                return;
            }

            if (nullableType.ElementType.IsMissing)
            {
                return;
            }

            /* Do not test for the first character on the line!
             * The StyleCop documentation is wrong there, the actual StyleCop code does not accept it.
             */

            SyntaxToken precedingToken = questionToken.GetPreviousToken();
            var triviaList = TriviaHelper.MergeTriviaLists(precedingToken.TrailingTrivia, questionToken.LeadingTrivia);
            if (triviaList.Any(t => t.IsKind(SyntaxKind.WhitespaceTrivia) || t.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, questionToken.GetLocation()));
            }
        }
    }
}
