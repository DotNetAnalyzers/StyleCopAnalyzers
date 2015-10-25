// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An increment or decrement symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an increment or decrement symbol is not
    /// correct.</para>
    ///
    /// <para>There should be no whitespace between the increment or decrement symbol and the item that is being
    /// incremented or decremented.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1020IncrementDecrementSymbolsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1020";
        private const string Title = "Increment decrement symbols must be spaced correctly";
        private const string MessageFormat = "{0} symbol '{1}' must not be {2} by a space.";
        private const string Description = "An increment or decrement symbol within a C# element is not spaced correctly.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1020.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                switch (token.Kind())
                {
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                    HandleIncrementDecrementToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleIncrementDecrementToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            switch (token.Parent.Kind())
            {
            case SyntaxKind.PreIncrementExpression:
            case SyntaxKind.PreDecrementExpression:
                if (token.HasTrailingTrivia)
                {
                    string symbolName;
                    if (token.IsKind(SyntaxKind.MinusMinusToken))
                    {
                        symbolName = "Decrement";
                    }
                    else
                    {
                        symbolName = "Increment";
                    }

                    // {Increment|Decrement} symbol '{++|--}' must not be {followed} by a space.
                    var properties = TokenSpacingProperties.RemoveFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, symbolName, token.Text, "followed"));
                }

                break;

            case SyntaxKind.PostIncrementExpression:
            case SyntaxKind.PostDecrementExpression:
                SyntaxToken previousToken = token.GetPreviousToken();
                if (!previousToken.IsMissing && previousToken.HasTrailingTrivia)
                {
                    string symbolName;
                    if (token.IsKind(SyntaxKind.MinusMinusToken))
                    {
                        symbolName = "Decrement";
                    }
                    else
                    {
                        symbolName = "Increment";
                    }

                    // {Increment|Decrement} symbol '{++|--}' must not be {preceded} by a space.
                    var properties = TokenSpacingProperties.RemovePreceding;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, symbolName, token.Text, "preceded"));
                }

                break;

            default:
                return;
            }
        }
    }
}
