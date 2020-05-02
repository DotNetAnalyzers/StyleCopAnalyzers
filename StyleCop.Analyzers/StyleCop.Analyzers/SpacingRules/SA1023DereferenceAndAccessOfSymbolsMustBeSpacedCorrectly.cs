// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// A dereference symbol or an access-of symbol within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a dereference or access-of symbol is not
    /// correct.</para>
    ///
    /// <para>The spacing around the symbol depends upon whether the symbol is used within a type declaration. If so,
    /// the symbol should always be followed by a single space, unless it is the last character on the line, or is
    /// followed by an opening square bracket or a parenthesis. In addition, the symbol should not be preceded by
    /// whitespace, and should not be the first character on the line. An example of a properly spaced dereference
    /// symbol used within a type declaration is:</para>
    ///
    /// <code language="cs">
    /// object* x = null;
    /// </code>
    ///
    /// <para>When a dereference or access-of symbol is used outside of a type declaration, the opposite rule applies.
    /// In this case, the symbol should always be preceded by a single space, unless it is the first character on the
    /// line, or is preceded by an opening square bracket, a parenthesis or a symbol of the same type i.e. an equals.
    /// The symbol should not be followed by whitespace, and should not be the last character on the line. For
    /// example:</para>
    ///
    /// <code language="cs">
    /// y = *x;
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1023";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1023.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1023Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1023Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly LocalizableString MessageNotPreceded = new LocalizableResourceString(nameof(SpacingResources.SA1023MessageNotPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1023MessageNotFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1023MessageFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotAtBeginningOfLine = new LocalizableResourceString(nameof(SpacingResources.SA1023MessageNotAtBeginningOfLine), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotAtEndOfLine = new LocalizableResourceString(nameof(SpacingResources.SA1023MessageNotAtEndOfLine), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        public static DiagnosticDescriptor DescriptorNotPreceded { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public static DiagnosticDescriptor DescriptorNotFollowed { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public static DiagnosticDescriptor DescriptorFollowed { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public static DiagnosticDescriptor DescriptorNotAtBeginningOfLine { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotAtBeginningOfLine, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        public static DiagnosticDescriptor DescriptorNotAtEndOfLine { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotAtEndOfLine, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
                if (token.IsKind(SyntaxKind.AsteriskToken))
                {
                    HandleAsteriskToken(context, token);
                }
            }
        }

        private static void HandleAsteriskToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
        {
            if (token.IsMissing)
            {
                return;
            }

            bool allowAtLineStart;
            bool allowAtLineEnd;
            bool allowPrecedingSpace;
            bool allowTrailingSpace;
            switch (token.Parent.Kind())
            {
            case SyntaxKind.PointerType:
                allowAtLineStart = false;
                allowAtLineEnd = true;
                allowPrecedingSpace = false;
                var nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.AsteriskToken:
                    allowTrailingSpace = false;
                    break;

                default:
                    allowTrailingSpace = true;
                    break;
                }

                break;

            case SyntaxKind.PointerIndirectionExpression:
                allowAtLineStart = true;
                allowAtLineEnd = false;
                allowTrailingSpace = false;
                var prevToken = token.GetPreviousToken();
                switch (prevToken.Kind())
                {
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                    allowPrecedingSpace = false;
                    break;
                default:
                    allowPrecedingSpace = true;
                    break;
                }

                break;

            default:
                return;
            }

            bool firstInLine = token.IsFirstInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace(context.CancellationToken);
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool lastInLine = token.IsLastInLine();

            if (!allowAtLineStart && firstInLine)
            {
                // Dereference symbol '*' should {not appear at the beginning of a line}.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotAtBeginningOfLine, token.GetLocation(), properties));
            }
            else if (!allowPrecedingSpace && precededBySpace)
            {
                // Dereference symbol '*' should {not be preceded by a space}.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), properties));
            }

            if (!allowAtLineEnd && lastInLine)
            {
                // Dereference symbol '*' should {not appear at the end of a line}.
                var properties = TokenSpacingProperties.RemoveFollowing;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotAtEndOfLine, token.GetLocation(), properties));
            }
            else if (!allowTrailingSpace && followedBySpace)
            {
                // Dereference symbol '*' should {not be followed by a space}.
                var properties = TokenSpacingProperties.RemoveFollowing;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), properties));
            }

            if (!followedBySpace && allowTrailingSpace)
            {
                // Dereference symbol '*' should {be followed by a space}.
                var properties = TokenSpacingProperties.InsertFollowing;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorFollowed, token.GetLocation(), properties));
            }
        }
    }
}
