// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An opening square bracket within a C# statement is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when an opening square bracket within a statement is preceded or followed
    /// by whitespace.</para>
    ///
    /// <para>An opening square bracket should never be preceded by whitespace, unless it is the first character on the
    /// line, and an opening square should never be followed by whitespace, unless it is the last character on the
    /// line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1010OpeningSquareBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1010OpeningSquareBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1010";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1010.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1010Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1010Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly LocalizableString MessageNotPreceded = new LocalizableResourceString(nameof(SpacingResources.SA1010MessageNotPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1010MessageNotFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly DiagnosticDescriptor DescriptorNotPreceded =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorNotFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
#pragma warning restore SA1202 // Elements should be ordered by access

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorNotPreceded);

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
                if (token.IsKind(SyntaxKind.OpenBracketToken) && !token.IsMissing)
                {
                    // attribute brackets are handled separately
                    if (!token.Parent.IsKind(SyntaxKind.AttributeList))
                    {
                        HandleOpenBracketToken(context, token);
                    }
                }
            }
        }

        private static void HandleOpenBracketToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = true;
            bool ignorePrecedingSpaceProblem = false;

            if (!firstInLine)
            {
                precededBySpace = token.IsPrecededByWhitespace(context.CancellationToken);

                // ignore if handled by SA1026
                if (precededBySpace)
                {
                    var previousToken = token.GetPreviousToken();
                    if (previousToken.IsKind(SyntaxKind.NewKeyword) || previousToken.IsKind(SyntaxKind.StackAllocKeyword))
                    {
                        ignorePrecedingSpaceProblem = true;
                    }
                }
            }

            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();

            if (!firstInLine && precededBySpace && !ignorePrecedingSpaceProblem && !IsPartOfIndexInitializer(token))
            {
                // Opening square bracket should {not be preceded} by a space.
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), TokenSpacingProperties.RemovePreceding));
            }

            if (!lastInLine && followedBySpace)
            {
                // Opening square bracket should {not be followed} by a space.
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), TokenSpacingProperties.RemoveFollowing));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
            }
        }

        private static bool IsPartOfIndexInitializer(SyntaxToken token)
        {
            return token.Parent.IsKind(SyntaxKind.BracketedArgumentList)
                && token.Parent.Parent.IsKind(SyntaxKind.ImplicitElementAccess);
        }
    }
}
