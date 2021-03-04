// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// A closing generic bracket within a C# element is not spaced correctly.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the spacing around a closing generic bracket is not correct.</para>
    ///
    /// <para>A closing generic bracket should never be preceded by whitespace, unless the bracket is the first
    /// character on the line. A closing generic bracket should be followed by an open parenthesis, a close parenthesis,
    /// a closing generic bracket, a nullable symbol, an end of line or a single whitespace (but not whitespace and an
    /// open parenthesis).</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1015ClosingGenericBracketsMustBeSpacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1015ClosingGenericBracketsMustBeSpacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1015";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1015.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1015Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1015Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly LocalizableString MessageNotPreceded = new LocalizableResourceString(nameof(SpacingResources.SA1015MessageNotPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageNotFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1015MessageNotFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFollowed = new LocalizableResourceString(nameof(SpacingResources.SA1015MessageFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly DiagnosticDescriptor DescriptorNotPreceded =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotPreceded, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorNotFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageNotFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorFollowed =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFollowed, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);
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
                if (token.IsKind(SyntaxKind.GreaterThanToken))
                {
                    HandleGreaterThanToken(context, token);
                }
            }
        }

        private static void HandleGreaterThanToken(SyntaxTreeAnalysisContext context, SyntaxToken token)
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
            bool lastInLine = token.IsLastInLine();
            bool precededBySpace = firstInLine || token.IsPrecededByWhitespace(context.CancellationToken);
            bool followedBySpace = token.IsFollowedByWhitespace();
            bool allowTrailingNoSpace;
            bool allowTrailingSpace;

            if (!lastInLine)
            {
                SyntaxToken nextToken = token.GetNextToken();
                switch (nextToken.Kind())
                {
                case SyntaxKind.OpenParenToken:
                // DotToken isn't listed above, but it's required for reasonable member access formatting
                case SyntaxKind.DotToken:
                // CommaToken isn't listed above, but it's required for reasonable nested generic type arguments formatting
                case SyntaxKind.CommaToken:
                // OpenBracketToken isn't listed above, but it's required for reasonable array type formatting
                case SyntaxKind.OpenBracketToken:
                // SemicolonToken isn't listed above, but it's required for reasonable using alias declaration formatting
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.ColonToken when nextToken.Parent.IsKind(SyntaxKindEx.CasePatternSwitchLabel):
                    allowTrailingNoSpace = true;
                    allowTrailingSpace = false;
                    break;

                case SyntaxKind.CloseParenToken:
                case SyntaxKind.GreaterThanToken:
                    allowTrailingNoSpace = true;
                    allowTrailingSpace = true;
                    break;

                case SyntaxKind.QuestionToken:
                    allowTrailingNoSpace = nextToken.Parent.IsKind(SyntaxKind.NullableType);
                    allowTrailingSpace = true;
                    break;

                // values[x as T<int>]
                //                  ^^
                case SyntaxKind.CloseBracketToken when nextToken.Parent.IsKind(SyntaxKind.BracketedArgumentList):
                    allowTrailingNoSpace = true;
                    allowTrailingSpace = false;
                    break;

                default:
                    allowTrailingNoSpace = false;
                    allowTrailingSpace = true;
                    break;
                }
            }
            else
            {
                allowTrailingNoSpace = true;
                allowTrailingSpace = true;
            }

            if (!firstInLine && precededBySpace)
            {
                // Closing generic bracket should{ not} be {preceded} by a space.
                var properties = TokenSpacingProperties.RemovePreceding;
                context.ReportDiagnostic(Diagnostic.Create(DescriptorNotPreceded, token.GetLocation(), properties));
            }

            if (!lastInLine)
            {
                if (!allowTrailingNoSpace && !followedBySpace)
                {
                    // Closing generic bracket should{} be {followed} by a space.
                    var properties = TokenSpacingProperties.InsertFollowing;
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorFollowed, token.GetLocation(), properties));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
                }
                else if (!allowTrailingSpace && followedBySpace)
                {
                    // Closing generic bracket should{ not} be {followed} by a space.
                    var properties = TokenSpacingProperties.RemoveFollowing;
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                    context.ReportDiagnostic(Diagnostic.Create(DescriptorNotFollowed, token.GetLocation(), properties));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
                }
            }
        }
    }
}
