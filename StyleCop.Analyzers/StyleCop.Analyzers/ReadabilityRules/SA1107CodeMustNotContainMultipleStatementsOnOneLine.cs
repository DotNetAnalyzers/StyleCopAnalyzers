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
    /// The C# code contains more than one statement on a single line.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contain more than one statement on the same line. Each
    /// statement must begin on a new line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1107CodeMustNotContainMultipleStatementsOnOneLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1107CodeMustNotContainMultipleStatementsOnOneLine"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1107";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1107Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1107MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1107Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1107.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> BlockAction = HandleBlock;

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
            context.RegisterSyntaxNodeActionHonorExclusions(BlockAction, SyntaxKind.Block);
        }

        private static void HandleBlock(SyntaxNodeAnalysisContext context)
        {
            BlockSyntax block = context.Node as BlockSyntax;

            if (block != null && block.Statements.Any())
            {
                var previousStatement = block.Statements[0];
                FileLinePositionSpan previousStatementLocation = previousStatement.GetLineSpan();
                FileLinePositionSpan currentStatementLocation;

                for (int i = 1; i < block.Statements.Count; i++)
                {
                    var currentStatement = block.Statements[i];
                    currentStatementLocation = currentStatement.GetLineSpan();

                    if (previousStatementLocation.EndLinePosition.Line
                        == currentStatementLocation.StartLinePosition.Line
                        && !IsLastTokenMissing(previousStatement))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, block.Statements[i].GetLocation()));
                    }

                    previousStatementLocation = currentStatementLocation;
                    previousStatement = currentStatement;
                }
            }
        }

        private static bool IsLastTokenMissing(StatementSyntax previousStatement)
        {
            return previousStatement.GetLastToken(includeZeroWidth: true, includeSkipped: true).IsMissing;
        }
    }
}
