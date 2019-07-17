// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An opening generic bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an opening generic bracket is not correct.</para>
    ///
    /// <para>An opening generic bracket should never be preceded or followed by whitespace, unless the bracket is the
    /// first or last character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1014OpeningGenericBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1014OpeningGenericBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1014";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1014Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1014MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1014Description), SpacingResources.ResourceManager, typeof(SpacingResources));
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1014.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            foreach (var token in root.DescendantTokens())
            {
                if (token.IsKind(SyntaxKind.LessThanToken))
                {
                    HandleLessThanToken(context, token);
                }
            }
        }

        private static void HandleLessThanToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            switch (token.Parent.Kind())
            {
            case SyntaxKind.TypeArgumentList:
            case SyntaxKind.TypeParameterList:
                break;

            default:
                // not a generic bracket
                return;
            }

            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace(context.CancellationToken);
            bool followedBySpace = token.IsFollowedByWhitespace();

            if (!firstInLine && precededBySpace)
            {
                // Opening generic brackets should not be {preceded} by a space.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, "preceded"));
            }

            if (followedBySpace)
            {
                // Opening generic brackets should not be {followed} by a space.
                var properties = TokenSpacingProperties.RemoveFollowing;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, "followed"));
            }
        }
    }
}
