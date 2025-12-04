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
    /// The spacing around a member access symbol is incorrect, within a C# code file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a member access symbol is incorrect. A member
    /// access symbol should not have whitespace on either side, unless it is the first character on the line.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1019MemberAccessSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1019MemberAccessSymbolsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1019";

        internal static readonly DiagnosticDescriptor DescriptorNotPreceded =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorNotFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1019.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1019Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1019Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly LocalizableString MessageNotPreceded = new LocalizableResourceString(nameof(SpacingResources.SA1019MessageNotPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1019MessageNotFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

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
                switch (token.Kind())
                {
                case SyntaxKind.DotToken:
                    HandleDotToken(context, token);
                    break;

                case SyntaxKind.MinusGreaterThanToken:
                    HandleMemberAccessSymbol(context, token);
                    break;

                // This case handles the new ?. and ?[ operators
                case SyntaxKind.QuestionToken:
                    HandleQuestionToken(context, token);
                    break;

                default:
                    break;
                }
            }
        }

        private static void HandleDotToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            HandleMemberAccessSymbol(context, token);
        }

        private static void HandleQuestionToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            if (!token.Parent.IsKind(SyntaxKind.ConditionalAccessExpression))
            {
                return;
            }

            HandleMemberAccessSymbol(context, token);
        }

        private static void HandleMemberAccessSymbol(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace(context.CancellationToken);
            bool followedBySpace = token.IsFollowedByWhitespace();

            if (!firstInLine && precededBySpace)
            {
                // Member access symbol '{.}' should not be {preceded} by a space.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), properties, token.Text));
            }

            if (followedBySpace)
            {
                // Member access symbol '{.}' should not be {followed} by a space.
                var properties = TokenSpacingProperties.RemoveFollowing;
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), properties, token.Text));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
#pragma warning restore IDE0079 // Remove unnecessary suppression
            }
        }
    }
}
