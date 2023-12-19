// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// An opening brace within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around an opening brace is not correct.</para>
    ///
    /// <para>An opening brace should always be preceded by a single space, unless it is the first character on the
    /// line, or unless it is preceded by an opening parenthesis, in which case there should be no space between the
    /// parenthesis and the brace.</para>
    ///
    /// <para>An opening brace should always be followed by a single space, unless it is the last character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1012OpeningBracesMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1012OpeningBracesMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1012";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1012.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1012Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1012MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1012Description), SpacingResources.ResourceManager, typeof(SpacingResources));

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
                if (token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    HandleOpenBraceToken(context, token);
                }
            }
        }

        private static void HandleOpenBraceToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool followedBySpace = token.IsFollowedByWhitespace();

            if (token.Parent is InterpolationSyntax)
            {
                if (followedBySpace)
                {
                    // Opening brace should{} be {followed} by a space.
                    var properties = TokenSpacingProperties.RemoveFollowing;
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, " not", "followed"));
                }

                return;
            }

            bool expectPrecedingSpace = true;
            if (token.Parent.IsKind(SyntaxKindEx.PropertyPatternClause))
            {
                var prevToken = token.GetPreviousToken();
                if (prevToken is { RawKind: (int)SyntaxKind.OpenParenToken, Parent: { RawKind: (int)SyntaxKindEx.PositionalPatternClause } })
                {
                    // value is ({ P: 0 }, { P: 0 })
                    expectPrecedingSpace = false;
                }
                else if (prevToken is { RawKind: (int)SyntaxKind.OpenBracketToken, Parent: { RawKind: (int)SyntaxKindEx.ListPattern } })
                {
                    // value is [{ P: 0 }, { P: 0 }]
                    expectPrecedingSpace = false;
                }
            }

            bool precededBySpace = token.IsFirstInLine() || token.IsPrecededByWhitespace(context.CancellationToken);

            if (precededBySpace != expectPrecedingSpace)
            {
                // Opening brace should{} be {preceded} by a space.
                // Opening brace should{ not} be {preceded} by a space.
                var properties = expectPrecedingSpace ? TokenSpacingProperties.InsertPreceding : TokenSpacingProperties.RemovePrecedingPreserveLayout;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, expectPrecedingSpace ? string.Empty : " not", "preceded"));
            }

            if (!token.IsLastInLine() && !followedBySpace)
            {
                // Opening brace should{} be {followed} by a space.
                var properties = TokenSpacingProperties.InsertFollowing;
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, token.GetLocation(), properties, string.Empty, "followed"));
            }
        }
    }
}
